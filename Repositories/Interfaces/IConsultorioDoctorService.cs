
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioDoctorService
{
   
    
   
    Task<ResponseResult> CreateDoctor(DoctorCreateRequest patientCreate);
    Task<PatientAllInfo> GetPatientAllInfo(long patientId);
    Task<ResponseResult> UpdatePatient(PatientUpdateRequest personalInfo);
}