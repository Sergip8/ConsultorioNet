using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
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
    public class ManageUsers
    {
    private readonly ILogger<ManageUsers> _logger;
    private readonly IConsultorioUsersService _consultorioUsersService;
    private readonly IConsultorioManagementService _consultorioManagementService;
    private readonly IConsultorioDoctorService _consultorioDoctorService;


    public ManageUsers(ILogger<ManageUsers> logger, 
    IConsultorioUsersService consultorioUsersService,  
    IConsultorioManagementService consultorioManagementService, 
    IConsultorioDoctorService consultorioDoctorService)
    {
        _logger = logger;
        _consultorioUsersService = consultorioUsersService;
        _consultorioManagementService = consultorioManagementService;
        _consultorioDoctorService = consultorioDoctorService;
    }


    [Function("Register")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        try{
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<RegisterRequest>(requestBody);
      
        
        if (string.IsNullOrEmpty(data?.Email) || string.IsNullOrEmpty(data?.Password))
        {
             return new BadRequestObjectResult(new ResponseResult
                {
                    IsError = true,
                    Message = "Please pass a username and password in the request body.",
                });
           
        }
        _logger.LogInformation(data.Email);
        await _consultorioUsersService.Register(data);

        return new OkObjectResult(
            new ResponseResult
                {
                    IsError = false,
                    Message = "usuario registrado con exito",
                }
                );
            

        }catch(Exception ex){
             return new BadRequestObjectResult(new ResponseResult
                {
                    IsError = true,
                    Message = ex.Message,
                });
           
        }


    }

    [Function("Login")]
    public async Task<IActionResult> Login(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<LoginRequest>(requestBody);

            if (string.IsNullOrEmpty(data?.Email) || string.IsNullOrEmpty(data?.Password))
            {
                return new BadRequestObjectResult(new ResponseResult
                {
                    IsError = true,
                    Message = "Please pass a username and password in the request body.",
                });
            }

            _logger.LogInformation(data.ToString());
            var user = await _consultorioUsersService.Login(data);
            if (user == null)
            {
                return new UnauthorizedResult();
            }

           return new OkObjectResult(user);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new ResponseResult
            {
                IsError = true,
                Message = ex.Message,
            });
        }
    }

    [Function("GetUserInfo")]
        public async Task<IActionResult> getPatientInfo([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="GetUserInfo/{userId}")] FunctionContext context, long userId)
    {
        try
        {
            dynamic userInfo = null;
            var user = FunctionsHelpers.GetUserFromContext(context);
            if (user == null)
            {
                return new UnauthorizedObjectResult(FunctionsHelpers.CreateErrorResponse("Invalid Token."));
            }
            var roles = FunctionsHelpers.UserRoles(user);
           

            if(roles == null){
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            if(roles.Contains("PATIENT")){
                userInfo = await _consultorioManagementService.GetPatientAllInfo(userId);

            }
            else if(roles.Contains("DOCTOR")){
                userInfo = await _consultorioDoctorService.GetDoctorAllInfo(userId);
            }
            else{
             
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Only Patient or Doctor can access")) { StatusCode = StatusCodes.Status403Forbidden };
            
            }
            if (userInfo == null)
            {
                return new ObjectResult(FunctionsHelpers.CreateErrorResponse("Patients not found.")) { StatusCode = StatusCodes.Status404NotFound };
            }

            return new OkObjectResult(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return new ObjectResult(FunctionsHelpers.CreateErrorResponse("An unexpected error occurred.")) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
     [Function("GetPaginatedUsers")]
        public async Task<IActionResult> getPaginatedPatient([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, FunctionContext context)
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

            var patients = await _consultorioUsersService.GetPaginatedUsers(userParamsRequest);
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
    }
}
