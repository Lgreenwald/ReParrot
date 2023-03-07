using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Services;
using Sabio.Models.Domain.Specialties;
using Sabio.Models.Requests.Specialties;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Linq.Expressions;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/specialties")]
    [ApiController]
    public class SpecialtyApiController : BaseApiController
    {
        private ISpecialtyService _service = null;
        private IAuthenticationService<int> _authService = null;

        public SpecialtyApiController(ISpecialtyService service, IAuthenticationService<int> authService, ILogger<SpecialtyApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
        }
        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(SpecialtyAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
                Logger.LogError(ex.ToString());
            }
            return result;
        }
        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(SpecialtyUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Specialty>> GetSpecialtyById(int id)
        {
            int code = 200;
            BaseResponse response = null;
            Specialty specialty = null;

            try
            {
                specialty = _service.GetById(id);

                if (specialty == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Specialty>() { Item = specialty };
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

        [HttpGet("orgId")]
        public ActionResult<ItemResponse<Paged<Specialty>>> GetByOrgId(int pageIndex, int pageSize, int orgId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Specialty> paged = _service.GetByOrgId(pageIndex, pageSize, orgId);

                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<Specialty>> { Item = paged };
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

        [HttpGet("name")]
        public ActionResult<ItemResponse<Paged<Specialty>>> GetByName(int pageIndex, int pageSize, string name)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<Specialty> paged = _service.GetByName(pageIndex, pageSize, name);

                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<Specialty>> { Item = paged };
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

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
    }
}
