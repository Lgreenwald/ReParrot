using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Organizations;
using Sabio.Models.Requests.OrganizationMember;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Organizations;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using SendGrid;
using System;
using System.Collections.Generic;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/organizations")]
	[ApiController]
	public class OrganizationApiController : BaseApiController
	{
		private IOrganizationsService _service = null;
		private ILocationsService _locationService = null;
		private IAuthenticationService<int> _authService = null;

		public OrganizationApiController(IOrganizationsService service
			, ILogger<OrganizationApiController> logger
			, IAuthenticationService<int> authService
			, ILocationsService locationService) : base(logger) 
		{
			_service = service;
			_authService = authService;
			_locationService = locationService;
		}

		[HttpGet("current")]
		public ActionResult<ItemResponse<Organization>> GetByUserId()
		{

			int iCode = 200;
			BaseResponse response = null;
			try
			{
				int userId = _authService.GetCurrentUserId();
				Organization org = _service.GetByUserId(userId);

				if (org == null)
				{

					iCode = 404;
					response = new ErrorResponse("Application Resource not found.");

				}
				else
				{
					response = new ItemResponse<Organization>() { Item = org };
				}
			}
			catch (Exception ex)
			{
				iCode = 500;
				base.Logger.LogError(ex.ToString());
				response = new ErrorResponse($"Generic Error: {ex.Message}");
			}

			return StatusCode(iCode, response);
		}


        [HttpGet("users/{userId:int}")]
        public ActionResult<ItemsResponse<Organization>> GetByUserIdV4(int userId)
        {

            int iCode = 200;
            BaseResponse response = null;
            try
            {

                List<Organization> list = _service.GetByUserIdV4(userId);

                if (list == null)
                {

                    iCode = 404;
                    response = new ErrorResponse("Application Resource not found.");

                }
                else
                {
                    response = new ItemsResponse<Organization> { Items = list };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }

            return StatusCode(iCode, response);
        }


        [HttpGet("paginate")]
		[AllowAnonymous]
		public ActionResult<ItemResponse<Paged<Organization>>> GetAllPagination(int pageIndex, int pageSize)
		{
			int code = 200;

			BaseResponse response = null;

			try
			{
				Paged<Organization> page = _service.GetAll(pageIndex, pageSize);

				if (page == null)
				{
					code = 404;
					response = new ErrorResponse("App Resource not found.");
				}
				else
				{
					response = new ItemResponse<Paged<Organization>> { Item = page };

				}
			}
			catch (Exception ex)
			{
				code = 500;
				response = new ErrorResponse(ex.Message);
				base.Logger.LogError(ex.ToString());
			}

			return StatusCode(code, response);
		
		}

        [HttpGet]
        public ActionResult<ItemsResponse<Organization>> GetAllNoPag()
        {
            int code = 200;

            BaseResponse response = null;

            try
            {
                List<Organization> list = _service.GetAllNoPag();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<Organization> { Items = list };

                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);

        }

        [HttpGet("{id:int}")]
		public ActionResult<ItemResponse<Organization>> GetById(int id)
		{

			int iCode = 200;
			BaseResponse response = null;
			try
			{
				Organization org = _service.GetById(id);

				if (org == null)
				{

					iCode = 404;
					response = new ErrorResponse("Application Resource not found.");

				}
				else
				{
					response = new ItemResponse<Organization>() { Item = org };
				}
			}
			catch (Exception ex)
			{
				iCode = 500;
				base.Logger.LogError(ex.ToString());
				response = new ErrorResponse($"Generic Error: {ex.Message}");
			}

			return StatusCode(iCode, response);

		}

		[HttpGet("search")]
		public ActionResult<ItemResponse<Paged<Organization>>> Search(int pageIndex, int pageSize, string query)
		{
			int code = 200;

			BaseResponse response = null;

			try
			{
				Paged<Organization> page = _service.Search(pageIndex, pageSize, query);

				if (page == null)
				{
					code = 404;
					response = new ErrorResponse("Records not found.");
				}
				else
				{
					response = new ItemResponse<Paged<Organization>> { Item = page };
				}
			}
			catch (Exception ex)
			{
				code = 500;
				response = new ErrorResponse(ex.Message);
				base.Logger.LogError(ex.ToString());
			}

			return StatusCode(code, response);

		}

        [HttpGet("currentorgid")]
        public ActionResult<ItemResponse<IUserAuthData>> GetUserAuthDataOrg()
        {
            int code = 200;
            BaseResponse response = null;
            int userAuthDataOrg = 0;
            try
            {
                userAuthDataOrg = _authService.GetCurrentOrgId();
                if (userAuthDataOrg != 0)
                {
                    response = new ItemResponse<int> { Item = userAuthDataOrg };
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

        [HttpGet("orgid")]
        [Authorize(Roles = "SysAdmin, OrgAdmin, OrgMember")]
        public ActionResult<ItemResponse<Paged<OrganizationMember>>> GetUsersByOrgId(int pageIndex, int pageSize, int orgId)
        {
            int code = 200;
            BaseResponse response = null;

			try
			{
				Paged<OrganizationMember> page = _service.GetUsersByOrgId(pageIndex, pageSize, orgId);

				if (page == null)
				{
					code = 404;
					response = new ErrorResponse("Records not found.");
				}
				else
				{
					response = new ItemResponse<Paged<OrganizationMember>> { Item = page };
				}
			}

			catch (Exception ex)
			{
				code = 500;
				response = new ErrorResponse(ex.Message);
				base.Logger.LogError(ex.ToString());
			}

            return StatusCode(code, response);
        }

	[HttpDelete("orgmember/{Id:int}")]
        [Authorize(Roles = "SysAdmin, OrgAdmin")]
		public ActionResult<SuccessResponse> DeleteOrgMember(int id)
		{
			int code = 200;
			BaseResponse response = null;

			try
			{
				_service.DeleteOrgMember(id);
				response = new SuccessResponse();
            }
			catch (Exception ex) {
				code = 500;
				response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
			return StatusCode(code, response);
		}

	[HttpPut("orgmember/roles")]
        [Authorize(Roles = "SysAdmin, OrgAdmin")]
		public ActionResult<SuccessResponse> UpdateOrgMemberRoles(OrgMemberRoleUpdate model)
		{
			int code = 200;
			BaseResponse response = null;
			int currentUserId = _authService.GetCurrentUserId();

            try
			{
				_service.UpdateOrgMemberRoles(model, currentUserId);
				response= new SuccessResponse();
            }
			catch(Exception ex) 
			{ code = 500;
				response = new ErrorResponse(ex.Message);
				Logger.LogError(ex.ToString());
			}

			return StatusCode(code, response);
		}
    }

}
