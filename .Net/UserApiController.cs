using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using Sabio.Web.Controllers;
using System;
using Sabio.Web.Models.Responses;
using Sabio.Models;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.Authorization;
using Sabio.Models.Domain.Services;
using Microsoft.CodeAnalysis.CSharp;
using Sabio.Services.Interfaces;
using Sabio.Models.Requests.Emails;
using Microsoft.AspNetCore.Identity;
using Sabio.Models.Requests.OrganizationMember;
using Sabio.Models.Domain.Organizations;
using Sabio.Models.Domain;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserApiController : BaseApiController
    {
        #region ------CONSTRUCTORS------
        private IUserService _service = null;
        private IEmailService _emailService = null;
        private IAuthenticationService<int> _authenticationService = null;
        private IOrganizationsService _orgService = null;
        private IInviteMemberService _inviteMemberService = null;

        public UserApiController(IUserService service
            , IEmailService emailService
            , IAuthenticationService<int> authenticationService
            , ILogger<UserApiController> logger, IOrganizationsService organizationsService, IInviteMemberService inviteMemberService) : base(logger)
        {
            _service = service;
            _emailService = emailService;
            _authenticationService = authenticationService;
            _orgService = organizationsService;
            _inviteMemberService = inviteMemberService;
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ItemResponse<User>> Create(UserAddRequest model)
        {
            ObjectResult result = null;
            int roleId = 5;
            if (model.IsBlogger == false)
            {
                roleId = 4;
            }
            try
            {
                int id = _service.Create(model);
                _service.CreateUserRole(id, roleId);
                ItemResponse<int> response = new ItemResponse<int> { Item = id };
                result = Created201(response);
                string token = _service.CreateNewUserToken(id);
                Recipient newUser = new Recipient();
                newUser.Name = model.FirstName;
                newUser.Email = model.Email;
                _emailService.SendNewUserEmail(newUser, id, token);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

        [AllowAnonymous]
        [HttpPost("addorgmember")]
        public ActionResult<ItemResponse<User>> CreateOrgMember(OrganizationMemberAddRequest model)
        {
            int code = 201;
            BaseResponse response = null;
            try
            {
                int id = _service.Create(model);
                model.UserId= id;
                int orgMemberId = _orgService.AddOrgMember(model);
                _inviteMemberService.DeleteById(model.InviteId);
                response = new ItemResponse<int> { Item = orgMemberId };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<Task<bool>> Login(UserLoginRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Task<bool> isSuccessful = _service.LogInAsync(model.Email, model.Password);
                if (isSuccessful.Result == true)
                {
                    response = new SuccessResponse();
                }
                else
                {
                    code = 401;
                    response = new ErrorResponse("Invalid password or username");
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("currentuser")]
        public ActionResult<ItemResponse<IUserAuthData>> GetUserAuthData()
        {
            int code = 200;
            BaseResponse response = null;
            IUserAuthData userAuthData = null;
            try
            {
                userAuthData = _authenticationService.GetCurrentUser();
                if (userAuthData != null)
                {
                    response = new ItemResponse<IUserAuthData> { Item = userAuthData };
                }
                else
                {
                    code = 401;
                    response = new ErrorResponse("Forbidden");
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("logout")]
        public async Task<ActionResult<SuccessResponse>> LogoutAsync()
        {
            await _authenticationService.LogOutAsync();
            SuccessResponse response = new SuccessResponse();
            return Ok200(response);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "SysAdmin")]
        public ActionResult<ItemResponse<User>> GetUserById(int id)
        {
            int code = 200;
            BaseResponse response = null;
            User user = null;

            try
            {
                user = _service.GetUserById(id);

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found");
                }
                else
                {
                    response = new ItemResponse<User>() { Item = user };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [AllowAnonymous]
        [HttpPut("confirm/{id:int}")]
        public ActionResult UpdateUserConfirmation(int id, string token)
        {
            int code = 200;
            BaseResponse response = null;
            int tokenTypeId = (int)TokenType.NewUser;
            bool isAuthorized = false;
            try
            {
                isAuthorized = _service.CompareUserTokenById(id, tokenTypeId, token);
                if (isAuthorized == true)
                {
                    _service.UpdateUserConfirm(id, token);
                    response = new SuccessResponse();
                }
                else
                {
                    code = 403;
                    response = new ErrorResponse("Forbidden");
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [AllowAnonymous]
        [HttpPut("passwordreset/{id:int}")]
        public ActionResult UpdateUserPassword(UserPasswordResetRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            int tokenTypeId = (int)TokenType.ResetPassword;
            bool isAuthorized = false;
            try
            {
                isAuthorized = _service.CompareUserTokenById(model.Id, tokenTypeId, model.Token);
                if (isAuthorized)
                {
                    _service.UpdateUserPassword(model);
                    response = new SuccessResponse();
                }
                else
                {
                    code = 403;
                    response = new ErrorResponse("Forbidden");
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPut("status/{id:int}/{status:int}")]
        [Authorize(Roles ="SysAdmin, OrgAdmin")]
        public ActionResult UpdateUserStatus(int status, int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.UpdateUserStatus( status, id);
                response = new SuccessResponse();
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("roles")]
        [Authorize(Roles ="SysAdmin, OrgAdmin")]
        public ActionResult<ItemResponse<LookUp>>GetAllRoles() {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<LookUp> role = _service.GetAllRoles();

                if (role == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemResponse<List<LookUp>> { Item = role };
                }
            }
           catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

    }
}
