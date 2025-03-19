

using Consultorio.Function.Models;
using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioMedicalInfoService
{

    Task<ResponseResult> UpdateMedicalInfo(PatientMedicalInfoRequest personalInfo);
}