
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Consultorio.Function
{
public class ManageCitas
    {
        private readonly ILogger<ManageCitas> _logger;
        private readonly IConsultorioCitasService _consultorioCitasService;

        public int GetCitasByPatientId { get; private set; }

        public ManageCitas(ILogger<ManageCitas> logger, IConsultorioCitasService consultorioCitasService)
        {
            _logger = logger;
            _consultorioCitasService = consultorioCitasService;
        }

      

    [Function("GetCitasByUserId")]
        public async Task<IActionResult> getCitasByUserId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="GetCitasByUserId/{userId}")] FunctionContext context, long userId)
    {
        try
        {
            IEnumerable<dynamic> citas = null;

            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }

            if (!FunctionsHelpers.UserHasPatientRole(user))
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Admin can access")) { StatusCode = StatusCodes.Status403Forbidden };
            }
            var roles = FunctionsHelpers.UserRoles(user);

            if(roles == null){
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            if(roles.Contains("PATIENT")){
                citas = await _consultorioCitasService.GetCitasByPatientId(userId);

            }
            if(roles.Contains("DOCTOR")){
                citas = await _consultorioCitasService.GetCitasByDoctorId(userId);
            }
            if (citas == null || !citas.Any())
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Patients not found.")) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new OkObjectResult(citas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    [Function("CreateCita")]
        public async Task<IActionResult> CreateCita([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, FunctionContext context)
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

            var cita = await FunctionsHelpers.DeserializeRequestBody<CitaCreateRequest>(req);
            if (cita == null )
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Patients not found.")) { StatusCode = StatusCodes.Status404NotFound };
            }
            var response = await _consultorioCitasService.CreateCita(cita);
            _logger.LogInformation(response.ToString());    
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
    

