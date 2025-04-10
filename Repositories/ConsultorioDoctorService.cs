using System.Data;
using Api.FunctionApp.DataContext;
using Azure;
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models;
using ConsultorioNet.Models.Request;
using Dapper;
using Newtonsoft.Json;

namespace Api.FunctionApp.Repositories;

public class ConsultorioDoctorService : IConsultorioDoctorService
{
    private readonly DapperContext _context;

    public ConsultorioDoctorService(DapperContext context)
    {
        _context = context;
    }

  

    public async Task<PaginatedResult<UserBasicInfoRequest>> GetDoctorPaginated(UserSearchParams userParams)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
             var result = new PaginatedResult<UserBasicInfoRequest>();
            if (db.State == ConnectionState.Closed)
                db.Open();
            var parameters = new DynamicParameters();
            parameters.Add("p_search_term", userParams.SearchTerm);
            parameters.Add("p_page_number", userParams.Page);
            parameters.Add("p_page_size", userParams.Size);
            parameters.Add("p_order_by", userParams.OrderCriteria);
            parameters.Add("p_order_direction", userParams.OrderDirection);
            var multi = await db.QueryMultipleAsync(
                "sp_get_paginated_doctors",
                parameters,
                commandType: CommandType.StoredProcedure
            );
             result.Data = multi.Read<UserBasicInfoRequest>().ToList();

            // Leer el segundo resultado: el conteo total de registros
            result.TotalRecords = multi.Read<int>().Single();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving guests for reservation with ID: ", ex);
        }
        finally
        {
            if (db.State == ConnectionState.Open)
                db.Close();
        }


    }


    public async Task<ResponseResult> CreateDoctor(DoctorCreateRequest doctorCreate)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
            if (db.State == ConnectionState.Closed)
            db.Open();
            var parameters = new DynamicParameters();
            parameters.Add("p_document_type", doctorCreate.Document_type);
            parameters.Add("p_firstname", doctorCreate.Firstname);
            parameters.Add("p_identity_number", doctorCreate.Identity_number);
            parameters.Add("p_lastname", doctorCreate.Lastname);
            parameters.Add("p_tel", doctorCreate.Tel);
            parameters.Add("p_user_id", doctorCreate.User_id);
            parameters.Add("p_address", doctorCreate.Informacion_personal.Address);
            parameters.Add("p_birth_date", doctorCreate.Informacion_personal.Birth_date);
            parameters.Add("p_e_civil", doctorCreate.Informacion_personal.E_civil);
            parameters.Add("p_gender", doctorCreate.Informacion_personal.Gender);
            parameters.Add("p_hire_date", doctorCreate.informacion_profesional.Hire_date);
            parameters.Add("p_professional_number", doctorCreate.informacion_profesional.Professional_number);
            parameters.Add("p_work_shift", doctorCreate.informacion_profesional.Work_shift);
            parameters.Add("p_specialization", doctorCreate.informacion_profesional.Specialization);
            parameters.Add("p_consultorios_id", doctorCreate.informacion_profesional.Consultorios_id);

            var response = await db.ExecuteAsync(
            "sp_insert_doctor",
            parameters,
            commandType: CommandType.StoredProcedure
            );
             return new ResponseResult
        {
            IsError = response < 0,
            Message = response > 0 ? "Update successful" : "Update failed"
        }; // Return an empty list or modify as needed
        }
        catch (Exception ex)
        {
             return new ResponseResult
        {
            IsError = true,
            Message = ex.Message
        };
        }
        finally
        {
            if (db.State == ConnectionState.Open)
            db.Close();
        }
    }
    public async Task<ResponseResult> UpdateDoctorProfile(UserProfileRequest personalInfo)
    {
    using IDbConnection db = _context.CreateConnection();
        try
        {
            if (db.State == ConnectionState.Closed)
            db.Open();
            var parameters = new DynamicParameters();
            parameters.Add("p_document_type", personalInfo.Document_type);
            parameters.Add("p_firstname", personalInfo.Firstname);
            parameters.Add("p_identity_number", personalInfo.Identity_number);
            parameters.Add("p_lastname", personalInfo.Lastname);
            parameters.Add("p_tel", personalInfo.Tel);
            parameters.Add("p_user_id", personalInfo.User_id);
            parameters.Add("p_address", personalInfo.Informacion_personal.Address);
            parameters.Add("p_birth_date", personalInfo.Informacion_personal.Birth_date);
            parameters.Add("p_e_civil", personalInfo.Informacion_personal.E_civil);
            parameters.Add("p_gender", personalInfo.Informacion_personal.Gender);

            var response = await db.ExecuteAsync(
            "sp_insert_doctor_profile",
            parameters,
            commandType: CommandType.StoredProcedure
            );
             return new ResponseResult
        {
            IsError = response < 0,
            Message = response > 0 ? "Update successful" : "Update failed"
        }; // Return an empty list or modify as needed
        }
        catch (Exception ex)
        {
             return new ResponseResult
        {
            IsError = true,
            Message = ex.Message
        };
        }
        finally
        {
            if (db.State == ConnectionState.Open)
            db.Close();
        }
    }
     public Task<DoctorAllInfo> GetDoctorAllInfo(long userId)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
            var sql = @"
            SELECT d.*, ip.*, im.*
            FROM doctores d
            LEFT JOIN informacion_personal ip ON d.informacion_personal_id = ip.id
            LEFT JOIN informacion_profesional im ON d.informacion_profesional_id = im.id
            WHERE d.user_id = @user_id";

            var result = db.Query<DoctorAllInfo, PersonalInfoRequest, ProfesionalInfoRequest, DoctorAllInfo>(
                sql,
                (doctor, personalInfo, profesionalInfo) =>
                {
                    doctor.Informacion_personal = personalInfo?.Id != null ? personalInfo : null ;
                    doctor.Informacion_profesional = profesionalInfo?.Id != null ? profesionalInfo : null;
                    return doctor;
                },
                param: new {user_id = userId},
                splitOn: "Id,Id"
            ).FirstOrDefault();

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving patient information for ID: {userId}", ex);
        }
        finally
        {
            if (db.State == ConnectionState.Open)
                db.Close();
        }
    }

     public Task<DoctorAllInfo> GetDoctorAllInfoByDoctorId(long doctorId)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
            var sql = @"
            SELECT d.*, ip.*, im.*
            FROM doctores d
            LEFT JOIN informacion_personal ip ON d.informacion_personal_id = ip.id
            LEFT JOIN informacion_profesional im ON d.informacion_profesional_id = im.id
            WHERE d.id = @doctor_id";

            var result = db.Query<DoctorAllInfo, PersonalInfoRequest, ProfesionalInfoRequest, DoctorAllInfo>(
                sql,
                (doctor, personalInfo, profesionalInfo) =>
                {
                    doctor.Informacion_personal = personalInfo?.Id != null ? personalInfo : null ;
                    doctor.Informacion_profesional = profesionalInfo?.Id != null ? profesionalInfo : null;
                    return doctor;
                },
                param: new {doctor_id = doctorId},
                splitOn: "Id,Id"
            ).FirstOrDefault();

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving patient information for ID: {doctorId}", ex);
        }
        finally
        {
            if (db.State == ConnectionState.Open)
                db.Close();
        }
    }


    public async Task<DoctorAvailabilityResponse> GetDoctorAvailability(DoctorAvailabilityRequest doctorAvailability)
    {
         var resultado = new DoctorAvailabilityResponse();

    
            using IDbConnection db = _context.CreateConnection();

            // Parámetros para el stored procedure
            using (var multi = db.QueryMultiple("sp_get_doctors_with_appointments_by_specializacion", new
            {
                doctorAvailability.EspecializacionId,
                doctorAvailability.Page,
                doctorAvailability.PageSize
            }, commandType: CommandType.StoredProcedure))
            {
                resultado.Doctores = multi.Read<dynamic>().Select(d => new DoctorvailabilityInfo
                {
                    Id = d.id,
                    DocumentType = d.document_type,
                    FirstName = d.firstname,
                    IdentityNumber = d.identity_number,
                    LastName = d.lastname,
                    Tel = d.tel,
                    Address = d.address,
                    SpeName = d.speName,
                    Citas = JsonConvert.DeserializeObject<List<CitaDoctor>>(
                        d.citas?.ToString() ?? "[]"
                         )
                }).ToList();

                resultado.TotalCount = multi.ReadSingle<int>();
                

                return resultado;
            }
        }

    public async Task<DoctorAvailabilityChatResponse> GetAvailabilityAppointments(UserExtractData extractData){
        var dates = FunctionsHelpers.ConvertirFechaYHora(extractData.fecha, extractData.hora);
        var resultado = new DoctorAvailabilityChatResponse();
        using IDbConnection db = _context.CreateConnection();
              var userEntityQuery = "SELECT sm.id FROM especialidades_medicas sm WHERE sm.nombre = @name";
            var speId = await db.QueryAsync<int>(userEntityQuery,  new { name = extractData.especializacion });
            Console.WriteLine(JsonConvert.SerializeObject(speId));
            // Parámetros para el stored procedure
            using (var multi = db.QueryMultiple("sp_get_doctors_with_appointments_by_date_range_and_spe", new
            {
                especializacionId = speId.First(),
                start_date = dates[0],
                end_date = dates[1]
            }, commandType: CommandType.StoredProcedure))
            {
                resultado.Doctores = multi.Read<dynamic>().Select(d => new DoctorvailabilityInfo
                {
                    Id = d.id,
                    DocumentType = d.document_type,
                    FirstName = d.firstname,
                    IdentityNumber = d.identity_number,
                    LastName = d.lastname,
                    Tel = d.tel,
                    Address = d.address,
                    SpeName = d.speName,
                    Citas = JsonConvert.DeserializeObject<List<CitaDoctor>>(d.citas?.ToString().Replace("[null]", "[]") ?? "[]")
                }).ToList();
                resultado.SpeId = speId.First();
                resultado.DateRange = dates;
                Console.WriteLine(JsonConvert.SerializeObject(resultado));
                return resultado;
            }

    }
    
}
