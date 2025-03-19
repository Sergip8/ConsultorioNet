
using Consultorio.Function.Models;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioTratamientoService
{
    Task<ResponseResult>  CreateTratamiento(TratamientoRequest consultorioRequest);
    Task<ResponseResult>  DeleteTratamiento(int id);
    Task<List<ConsultorioResponse>> GetTratamientospaginated(TratamientoParamsRequest consultorioFilter);
    Task<ResponseResult>  UpdateTratamiento(TratamientoRequest consultorioRequest);
    Task<List<TratamientoResponse>> GetTratamientoByCitasId(long citasId);
}