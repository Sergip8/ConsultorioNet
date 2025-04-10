
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Repositories.Interfaces;

public interface IConsultorioDoctorService
{
   
    
   
    Task<ResponseResult> CreateDoctor(DoctorCreateRequest patientCreate);

    Task<ResponseResult> UpdateDoctorProfile(UserProfileRequest personalInfo);
    Task<DoctorAllInfo> GetDoctorAllInfo(long userId);
    Task<DoctorAvailabilityResponse> GetDoctorAvailability(DoctorAvailabilityRequest doctorAvailability);

    Task<DoctorAvailabilityChatResponse> GetAvailabilityAppointments(UserExtractData extractData);
    Task<PaginatedResult<UserBasicInfoRequest>> GetDoctorPaginated(UserSearchParams userParams);
    Task<DoctorAllInfo> GetDoctorAllInfoByDoctorId(long doctorId);
}