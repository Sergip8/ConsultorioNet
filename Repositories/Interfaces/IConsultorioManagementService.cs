
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioManagementService
{
   
    
    Task<PaginatedResult<UserBasicInfoRequest>> GetPatientByIdOrEmail(UserSearchParams userParams);
    Task<ResponseResult> GetPatientByIdOrEmail(PatientCreateRequest patientCreate);
    Task<PatientAllInfo> GetPatientAllInfo(long patientId);
    Task<ResponseResult> UpdatePatient(PatientUpdateRequest personalInfo);
    Task<ResponseResult> InsertPatient(PatientCreateRequest patient);
     Task<PatientAllInfo> GetPatientAllInfoByPatientId(long patientId);
}