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
    public class ManageUsers
    {
    private readonly ILogger<ManageUsers> _logger;
    private readonly IConsultorioUsersService _consultorioUsersService;

    public ManageUsers(ILogger<ManageUsers> logger, IConsultorioUsersService consultorioUsersService)
    {
        _logger = logger;
        _consultorioUsersService = consultorioUsersService;
    }


    [Function("Register")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
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
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
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
    }
}
