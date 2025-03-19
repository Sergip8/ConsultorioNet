using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Consultorio.Function
{
public class ManageConsultorios
    {
        private readonly ILogger<ManageConsultorios> _logger;
        private readonly IConsultorioConsulService _consultorioConsultService;

        public ManageConsultorios(ILogger<ManageConsultorios> logger, IConsultorioConsulService consultorioConsultService)
        {
            _logger = logger;
            _consultorioConsultService = consultorioConsultService;
        }

        [Function("GetConsultoriospaginated")]
        public async Task<IActionResult> GetConsultoriospaginated([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, FunctionContext context)
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

            var paramsRequest = await FunctionsHelpers.DeserializeRequestBody<ConsultorioFilterParamsRequest>(req);
            if (paramsRequest == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var patients = await _consultorioConsultService.GetConsultoriospaginated(paramsRequest);
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
    [Function("CreateConsultorio")]
    public async Task<IActionResult> CreateConsultorio([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, FunctionContext context)
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

        var consultorioRequest = await FunctionsHelpers.DeserializeRequestBody<ConsultorioRequest>(req);
        if (consultorioRequest == null)
        {
            return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
        }

        var consultorio = await _consultorioConsultService.CreateConsultorio(consultorioRequest);

        return new OkObjectResult(consultorio);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "An unexpected error occurred.");
        return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [Function("UpdateConsultorio")]
    public async Task<IActionResult> UpdateConsultorio([HttpTrigger(AuthorizationLevel.Anonymous, "put")] HttpRequest req, FunctionContext context)
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

        var consultorioRequest = await FunctionsHelpers.DeserializeRequestBody<ConsultorioRequest>(req);
        if (consultorioRequest == null)
        {
            return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
        }

        var consultorio = await _consultorioConsultService.UpdateConsultorio(consultorioRequest);

        return new OkObjectResult(consultorio);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "An unexpected error occurred.");
        return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [Function("DeleteConsultorio")]
    public async Task<IActionResult> DeleteConsultorio([HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequest req, FunctionContext context)
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

        var consultorioRequest = await FunctionsHelpers.DeserializeRequestBody<ConsultorioRequest>(req);
        if (consultorioRequest == null)
        {
            return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
        }

        await _consultorioConsultService.DeleteConsultorio(consultorioRequest.Id);

        return new OkObjectResult("Consultorio deleted successfully.");
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "An unexpected error occurred.");
        return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

   
}}