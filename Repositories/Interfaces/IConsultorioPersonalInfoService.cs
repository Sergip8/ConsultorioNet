

using Consultorio.Function.Models;
using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioPersonalInfoService
{

    Task<ResponseResult> UpdatePersonalInfo(PersonalInfoRequest personalInfo);
}