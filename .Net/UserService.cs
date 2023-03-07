using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Organizations;
using Sabio.Models.Domain.Users;
using Sabio.Models.Domain.UserVehicles;
using Sabio.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sabio.Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService<int> _authenticationService;
        private IDataProvider _dataProvider;

        public UserService(IAuthenticationService<int> authService, IDataProvider dataProvider)
        {
            _authenticationService = authService;
            _dataProvider = dataProvider;
        }

        #region Registration and Confirmation

        public int Create(UserAddRequest model)
        {
            int id = 0;
            string password = model.Password;
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(password, salt);
            string procName = "[dbo].[Users_Insert]";
            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {

                    MapUserParams(model, col, hashedPassword);
                    SqlParameter idOut = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                    idOut.Direction = System.Data.ParameterDirection.Output;
                    col.Add(idOut);

                }, returnParameters: delegate (SqlParameterCollection returnCol)
                {

                    object oId = returnCol["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);

                });
            return id;
        }

        public void CreateUserRole(int userId,int roleId)
        {
            string procName = "[dbo].[UserRoles_Insert]";
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@UserId", userId);
                col.AddWithValue("@RoleId", roleId);
            }, returnParameters: null);
        }

        public string CreateNewUserToken(int userId)
        {
            string token = Guid.NewGuid().ToString();
            string procName = "[dbo].[UserTokens_Insert]";
            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    MapNewUserToken(col, token, userId);
                }, returnParameters: null);
            return token;
        }

        public string CreateResetPasswordToken(int userId)
        {
            string token = Guid.NewGuid().ToString();
            string procName = "dbo.UserTokens_Insert";
            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    MapPasswordResetToken(col, token, userId);
                });
            return token;
        }

        public int GetUserIdFromEmail(string email)
        {
            int userId = 0;
            string procName = "[dbo].[Users_SelectId_ByEmail]";
            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", email);
            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                userId = reader.GetSafeInt32(index);
            });
            return userId;
        }

        public bool CompareUserTokenById(int userId, int tokenTypeId, string userToken)
        {
            bool isCorrectToken = false;
            string procName = "dbo.UserTokens_SelectBy_UserId";
            string tokenFromdb = "";
            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@UserId", userId);
                col.AddWithValue("@TokenTypeId", tokenTypeId);
            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                tokenFromdb = reader.GetSafeString(index);
            });
            if (userToken == tokenFromdb)
            {
                isCorrectToken = true;
            }
            return isCorrectToken;
        }

        #endregion

        #region Updates and Changes

        public void UpdateUserConfirm(int id, string token)
        {
            string procName = "dbo.Users_Confirm";
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
                col.AddWithValue("@Token", token);
            }, returnParameters: null);
        }

        public void UpdateUserPassword(UserPasswordResetRequest model)
        {
            string procName = "dbo.Users_UpdatePassword";
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(model.Password, salt);
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@UserId", model.Id);
                col.AddWithValue("@Password", hashedPassword);
                col.AddWithValue("@Token", model.Token);
            }, returnParameters: null);
        }

        public void UpdateUserStatus(int status, int id)
        {
            string procName = "dbo.Users_UpdateStatus";
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@StatusTypeId", status);
                col.AddWithValue("@Id", id);
            }, returnParameters: null);
        }

        #endregion

        #region Authentication
        public async Task<bool> LogInAsync(string email, string password)
        {
            bool isSuccessful = false;
            IUserAuthData response = Get(email, password);

            if (response != null)
            {
                await _authenticationService.LogInAsync(response);
                isSuccessful = true;
            }
            return isSuccessful;
        }

        public async Task<bool> LogInTest(string email, string password, int id, string[] roles = null)
        {
            bool isSuccessful = false;
            var testRoles = new[] { "User", "Super", "Content Manager" };

            var allRoles = roles == null ? testRoles : testRoles.Concat(roles);

            IUserAuthData response = new UserBase
            {
                Id = id
                ,
                Name = email
                ,
                Roles = allRoles
                ,
                TenantId = "Acme Corp UId"
            };

            Claim fullName = new Claim("CustomClaim", "Sabio Bootcamp");
            await _authenticationService.LogInAsync(response, new Claim[] { fullName });

            return isSuccessful;
        }

        /// <summary>
        /// Gets the Data call to get a give user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        private IUserAuthData Get(string email, string password)
        {
            string procName = "[dbo].[Users_Select_AuthData_V3]";
            string passwordFromDb = null;
            int userId = 0;
            UserBase user = null;
            List<string> roles = null;
            string role = null;
            string firstName = null;
            string mi = null;
            string lastName = null;
            string avatarUrl = null;
            bool isConfirmed = false;
            int organizationId = 0;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", email);
            }, delegate (IDataReader reader, short set)
            {
                switch (set)
                {
                    case 0:
                        int startingIndex = 0;
                        passwordFromDb = reader.GetSafeString(startingIndex++);
                        userId = reader.GetSafeInt32(startingIndex++);
                        firstName = reader.GetSafeString(startingIndex++);
                        mi = reader.GetSafeString(startingIndex++);
                        lastName = reader.GetSafeString(startingIndex++);
                        avatarUrl = reader.GetSafeString(startingIndex++);
                        startingIndex++;
                        isConfirmed = reader.GetSafeBool(startingIndex++);
                        organizationId = reader.GetSafeInt32(startingIndex++);
                        break;

                    case 1:
                        int index = 0;
                        role = reader.GetSafeString(index++);
                        if (roles == null)
                        {
                            roles = new List<string>();
                        }
                        roles.Add(role);
                        break;

                    default: break;
                }

            });

            bool isValidCredentials = BCrypt.BCryptHelper.CheckPassword(password, passwordFromDb);

            if (isValidCredentials && isConfirmed)
            {
                user = new UserBase();
                user.Id = userId;
                user.Name = email;
                user.Roles = roles;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.AvatarUrl = avatarUrl;
                user.TenantId = "ReParrot01";
                user.OrganizationId = organizationId;
            }
            return user;
        }

        #endregion

        public User GetUserById(int id)
        {
            string procName = "[dbo].[Users_SelectById]";
            User user = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                user = MapSingleUser(reader, ref startingIndex);
            });
            return user;
        }

        public List<LookUp> GetAllRoles()
        {
            string procName = "[dbo].[Roles_SelectAll]";

            List<LookUp> list = null;

            _dataProvider.ExecuteCmd(procName, inputParamMapper: null
           , singleRecordMapper: delegate (IDataReader reader, short set)
           {
               int startingIndex = 0;
               LookUp role = new LookUp();
               role.Id = reader.GetSafeInt32(startingIndex++);
               role.Name = reader.GetSafeString(startingIndex++);

               if (list == null)
               {
                   list = new List<LookUp>();
               }

               list.Add(role);
           });

            return list;
        }




    private static User MapSingleUser(IDataReader reader, ref int startingIndex)
        {
            User user = new User();

            user.Id = reader.GetSafeInt32(startingIndex++);
            user.Email = reader.GetSafeString(startingIndex++);
            user.FirstName = reader.GetSafeString(startingIndex++);
            user.LastName = reader.GetSafeString(startingIndex++);
            user.Mi = reader.GetSafeString(startingIndex++);
            user.AvatarUrl = reader.GetSafeString(startingIndex++);
            user.IsConfirmed = reader.GetBoolean(startingIndex++);
            user.StatusTypeId = reader.GetSafeInt32(startingIndex++);
            user.DateCreated = reader.GetSafeDateTime(startingIndex++);
            user.DateModified = reader.GetSafeDateTime(startingIndex++);

            return user;
        }

        private static void MapUserParams(UserAddRequest model, SqlParameterCollection col, string hashedPassword)
        {
            col.AddWithValue("@Email", model.Email);
            col.AddWithValue("@FirstName", model.FirstName);
            col.AddWithValue("@LastName", model.LastName);
            col.AddWithValue("@Mi", model.Mi);
            col.AddWithValue("@AvatarUrl", model.AvatarUrl);
            col.AddWithValue("@Password", hashedPassword);
            col.AddWithValue("@StatusTypeId", model.StatusTypeId);
        }

        private static void MapNewUserToken(SqlParameterCollection col, string token, int userId)
        {
            col.AddWithValue("@Token", token);
            col.AddWithValue("@UserId", userId);
            col.AddWithValue("@TokenTypeId", (int)TokenType.NewUser);
        }

        private static void MapPasswordResetToken(SqlParameterCollection col, string token, int userId)
        {
            col.AddWithValue("@Token", token);
            col.AddWithValue("@UserId", userId);
            col.AddWithValue("@TokenTypeId", (int)TokenType.ResetPassword);
        }
    }
}