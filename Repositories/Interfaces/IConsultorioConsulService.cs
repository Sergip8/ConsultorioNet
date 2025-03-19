
using Consultorio.Function.Models;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioConsulService
{
    Task<ResponseResult>  CreateConsultorio(ConsultorioRequest consultorioRequest);
    Task<ResponseResult>  DeleteConsultorio(int id);
    Task<List<ConsultorioResponse>> GetConsultoriospaginated(ConsultorioFilterParamsRequest consultorioFilter);
    Task<ResponseResult>  UpdateConsultorio(ConsultorioRequest consultorioRequest);
}