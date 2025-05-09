

using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioCitasService
{

    Task<List<CitasPatientResponse>> GetCitasByPatientId(long patient_id);

    Task<List<CitasDoctorResponse>> GetCitasByDoctorId(long doctor_id);
    Task<ResponseResult> CreateCita(CitaCreateRequest citaCreate);
    Task<ResponseResult> CreatePatientCita(CitaPatientCreateRequest citaCreate);
    Task<PaginatedResult<CitaResponse>> GetCitasPaginated(UserSearchParams userParams);
}