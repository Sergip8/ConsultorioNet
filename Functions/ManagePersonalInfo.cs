using Consultorio.Function.Models;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consultorio.Function
{
    public class ManagePersonalInfo
    {
    private readonly ILogger<ManagePersonalInfo> _logger;
    private readonly IConsultorioPersonalInfoService _consultorioPersonalInfo;

    public ManagePersonalInfo(ILogger<ManagePersonalInfo> logger, IConsultorioPersonalInfoService consultorioPersonalInfo)
    {
        _logger = logger;
        _consultorioPersonalInfo = consultorioPersonalInfo;
    }


    [Function("UpdatePersonalInfo")]
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

            if (!FunctionsHelpers.UserHasPatientRole(user))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }

            var personalInfo = await FunctionsHelpers.DeserializeRequestBody<PersonalInfoRequest>(req);
            if (personalInfo == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var response = await _consultorioPersonalInfo.UpdatePersonalInfo(personalInfo);
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
    }
}