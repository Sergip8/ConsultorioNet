using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Consultorio.Function
{
public class ManageTratamientos
    {
        private readonly ILogger<ManageTratamientos> _logger;
        private readonly IConsultorioTratamientoService _consultorioTratamientoService;

        public ManageTratamientos(ILogger<ManageTratamientos> logger, IConsultorioTratamientoService consultorioTratamientoService)
        {
            _logger = logger;
            _consultorioTratamientoService = consultorioTratamientoService;
        }

        [Function("GetTratamientosPaginated")]
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

            var paramsRequest = await FunctionsHelpers.DeserializeRequestBody<TratamientoParamsRequest>(req);
            if (paramsRequest == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var patients = await _consultorioTratamientoService.GetTratamientospaginated(paramsRequest);
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
    [Function("CreateTratamineto")]
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

        var consultorioRequest = await FunctionsHelpers.DeserializeRequestBody<TratamientoRequest>(req);
        if (consultorioRequest == null)
        {
            return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
        }

        var consultorio = await _consultorioTratamientoService.CreateTratamiento(consultorioRequest);

        return new OkObjectResult(consultorio);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "An unexpected error occurred.");
        return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [Function("UpdateTratamiento")]
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

        var consultorioRequest = await FunctionsHelpers.DeserializeRequestBody<TratamientoRequest>(req);
        if (consultorioRequest == null)
        {
            return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
        }

        var consultorio = await _consultorioTratamientoService.UpdateTratamiento(consultorioRequest);

        return new OkObjectResult(consultorio);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "An unexpected error occurred.");
        return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [Function("DeleteTratamiento")]
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

        await _consultorioTratamientoService.DeleteTratamiento(consultorioRequest.Id);

        return new OkObjectResult("Consultorio deleted successfully.");
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "An unexpected error occurred.");
        return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }


        [Function("GetTratamientoCita")]
        public async Task<IActionResult> GetTratamientoCita([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route ="GetTratamientoCita/{citaId}")] FunctionContext context, long citaId)
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

        
            if (citaId == null)
            {
                return new BadRequestObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid request body."));
            }

            var patients = await _consultorioTratamientoService.GetTratamientoByCitasId(citaId);
            if (patients == null || !patients.Any())
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Tratamiento not found.")) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new OkObjectResult(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}}