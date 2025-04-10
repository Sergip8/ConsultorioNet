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
    public class ManageDoctors
    {
        private readonly ILogger<ManageDoctors> _logger;
        private readonly IConsultorioDoctorService _consultorioDoctorService;

        public ManageDoctors(ILogger<ManageDoctors> logger, IConsultorioDoctorService consultorioDoctorService)
        {
            _logger = logger;
            _consultorioDoctorService = consultorioDoctorService;
        }

        
[Function("GetDoctorInfo")]
        public async Task<IActionResult> getDoctorInfo([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="GetDoctorInfo/{id}")] FunctionContext context, long id)
    {
        try
        {
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user, ["DOCTOR"]))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }


            var patients = await _consultorioDoctorService.GetDoctorAllInfoByDoctorId(id);
            if (patients == null)
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse(" not found.")) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new OkObjectResult(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
    
      [Function("GetPaginatedDoctors")]
        public async Task<IActionResult> getPaginatedPatient([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, FunctionContext context)
    {
        try
        {
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user, ["ADMIN"]))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }

            var userParamsRequest = await FunctionsHelpers.DeserializeRequestBody<UserSearchParams>(req);
            if (userParamsRequest == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var patients = await _consultorioDoctorService.GetDoctorPaginated(userParamsRequest);
            if (patients == null || !patients.Data.Any())
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
 [Function("UpdateDoctorProfile")]
    public async Task<IActionResult> UpdatePersonalInfo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req, FunctionContext context)
    {
        
         try
        {
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user, ["DOCTOR", "ADMIN"]))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }
         

            var patient = await FunctionsHelpers.DeserializeRequestBody<UserProfileRequest>(req);
            if (patient == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var response = await _consultorioDoctorService.UpdateDoctorProfile(patient);
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

    [Function("CreateDoctor")]
    public async Task<IActionResult> CreateDoctor(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req, FunctionContext context)
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

            var patient = await FunctionsHelpers.DeserializeRequestBody<DoctorCreateRequest>(req);
            if (patient == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var response = await _consultorioDoctorService.CreateDoctor(patient);
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
    
    [Function("GetDoctorAvailability")]
    public async Task<IActionResult> DoctorAvailability(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        
         try
        {

            var doctor = await FunctionsHelpers.DeserializeRequestBody<DoctorAvailabilityRequest>(req);
            if (doctor == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var response = await _consultorioDoctorService.GetDoctorAvailability(doctor);
            if (response == null)
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


    }
}
