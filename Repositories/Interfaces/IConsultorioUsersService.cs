
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioUsersService
{
   
    
    Task<ResponseResult> Register(RegisterRequest register);
    Task<LoginResponse> Login(LoginRequest login);

}