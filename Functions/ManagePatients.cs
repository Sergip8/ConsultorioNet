using System.Net;
using System.Security.Claims;
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using Consultorio.Function.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consultorio.Function
{
    public class ManagePatients
    {
        private readonly ILogger<ManagePatients> _logger;
        private readonly IConsultorioManagementService _consultorioManagementService;

        public ManagePatients(ILogger<ManagePatients> logger, IConsultorioManagementService consultorioManagementService)
        {
            _logger = logger;
            _consultorioManagementService = consultorioManagementService;
        }

        [Function("GetPaginatedPatients")]
        public async Task<IActionResult> getPaginatedPatient([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, FunctionContext context)
    {
        try
        {
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }

            var userParamsRequest = await FunctionsHelpers.DeserializeRequestBody<UserSearchParams>(req);
            if (userParamsRequest == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var patients = await _consultorioManagementService.GetPatientByIdOrEmail(userParamsRequest);
            if (patients == null || !patients.Any())
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Patients not found.")) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new OkObjectResult(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
[Function("GetPatientInfo")]
        public async Task<IActionResult> getPatientInfo([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="GetPatientInfo/{id}")] FunctionContext context, long id)
    {
        try
        {
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }


            var patients = await _consultorioManagementService.GetPatientAllInfo(id);
            if (patients == null)
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Patients not found.")) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new OkObjectResult(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
    
 [Function("UpdatePatient")]
    public async Task<IActionResult> UpdatePatient(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req, FunctionContext context)
    {
        
         try
        {
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }

            var patient = await FunctionsHelpers.DeserializeRequestBody<PatientUpdateRequest>(req);
            if (patient == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var response = await _consultorioManagementService.UpdatePatient(patient);
            if (response.IsError)
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Personal info not exists")) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [Function("CreatePatient")]
    public async Task<IActionResult> CreatePatient(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, FunctionContext context)
    {
        
         try
        {
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }

            var patient = await FunctionsHelpers.DeserializeRequestBody<PatientCreateRequest>(req);
            
            if (patient == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var response = await _consultorioManagementService.InsertPatient(patient);
            if (response.IsError)
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse(response.Message)) { StatusCode = StatusCodes.Status400BadRequest };
            }

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    }
}
