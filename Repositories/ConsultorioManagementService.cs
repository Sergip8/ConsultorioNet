using System.Data;
using Api.FunctionApp.DataContext;
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models;
using ConsultorioNet.Models.Request;
using Dapper;
using Newtonsoft.Json;

namespace Api.FunctionApp.Repositories;

public class ConsultorioManagementService : IConsultorioManagementService
{
    private readonly DapperContext _context;

    public ConsultorioManagementService(DapperContext context)
    {
        _context = context;
    }

    public Task<PatientAllInfo> GetPatientAllInfo(long userId)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
            var sql = @"
            SELECT p.*, ip.*, im.*
            FROM pacientes p
            LEFT JOIN informacion_personal ip ON p.informacion_personal_id = ip.id
            LEFT JOIN informacion_medica_paciente im ON p.informacion_medica_id = im.id
            WHERE p.user_id = @user_id";

            var result = db.Query<PatientAllInfo, PersonalInfoRequest, PatientMedicalInfoRequest, PatientAllInfo>(
                sql,
                (patient, personalInfo, medicalInfo) =>
                {
                    patient.Informacion_personal = personalInfo?.Id != null ? personalInfo : null ;
                    patient.Informacion_medica = medicalInfo?.Id != null ? medicalInfo : null;
                    return patient;
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
     public Task<PatientAllInfo> GetPatientAllInfoByPatientId(long patientId)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
            var sql = @"
            SELECT p.*, ip.*, im.*
            FROM pacientes p
            LEFT JOIN informacion_personal ip ON p.informacion_personal_id = ip.id
            LEFT JOIN informacion_medica_paciente im ON p.informacion_medica_id = im.id
            WHERE p.id = @patient_id";

            var result = db.Query<PatientAllInfo, PersonalInfoRequest, PatientMedicalInfoRequest, PatientAllInfo>(
                sql,
                (patient, personalInfo, medicalInfo) =>
                {
                    patient.Informacion_personal = personalInfo?.Id != null ? personalInfo : null ;
                    patient.Informacion_medica = medicalInfo?.Id != null ? medicalInfo : null;
                    return patient;
                },
                param: new {patient_id = patientId},
                splitOn: "Id,Id"
            ).FirstOrDefault();

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving patient information for ID: {patientId}", ex);
        }
        finally
        {
            if (db.State == ConnectionState.Open)
                db.Close();
        }
    }

      public async Task<ResponseResult> InsertPatient(PatientCreateRequest patient)
    {
        var json = JsonConvert.SerializeObject(patient, Formatting.Indented);
        Console.WriteLine(json);
        using IDbConnection db = _context.CreateConnection(); 
        try
        {
            
                var parameters = new
                {
                    p_document_type = patient.Document_type,
                    p_firstname = patient.Firstname,
                    p_identity_number = patient.Identity_number,
                    p_lastname = patient.Lastname,
                    p_tel = patient.Tel,
                    p_user_id = patient.User_id,
                    p_address = patient.Informacion_personal.Address,
                    p_birth_date = patient.Informacion_personal.Birth_date,
                    p_e_civil = patient.Informacion_personal.E_civil,
                    p_gender = patient.Informacion_personal.Gender,
                    p_blood_type = patient.Informacion_medica.Blood_type,
                    p_height = patient.Informacion_medica.Height,
                    p_weight = patient.Informacion_medica.Weight
                };
 
                var result = await db.ExecuteAsync("CALL sp_insertar_paciente(@p_document_type, @p_firstname, @p_identity_number, @p_lastname, @p_tel, @p_user_id, @p_address, @p_birth_date, @p_e_civil, @p_gender, @p_blood_type, @p_height, @p_weight)", parameters);
             return new ResponseResult
        {
            IsError = result < 0,
            Message = result > 0 ? "Datos registrados" : "Fallo al registrar los datos"
        };
        }
        catch (Exception ex)
        {
             return new ResponseResult
        {
            IsError = true,
            Message = "Error inserting patient: " + ex.Message
        };
            // Manejar la excepción (por ejemplo, registrar el error o lanzar una excepción personalizada)
        
        }
    }

    public async Task<PaginatedResult<UserBasicInfoRequest>> GetPatientByIdOrEmail(UserSearchParams userParams)
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
                "sp_get_paginated_patients",
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

    public async Task<ResponseResult> GetPatientByIdOrEmail(PatientCreateRequest patientCreate)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
            if (db.State == ConnectionState.Closed)
            db.Open();
            var parameters = new DynamicParameters();
            parameters.Add("p_document_type", patientCreate.Document_type);
            parameters.Add("p_firstname", patientCreate.Firstname);
            parameters.Add("p_identity_number", patientCreate.Identity_number);
            parameters.Add("p_lastname", patientCreate.Lastname);
            parameters.Add("p_tel", patientCreate.Tel);
            parameters.Add("p_user_id", patientCreate.User_id);
            parameters.Add("p_address", patientCreate.Informacion_personal.Address);
            parameters.Add("p_birth_date", patientCreate.Informacion_personal.Birth_date);
            parameters.Add("p_e_civil", patientCreate.Informacion_personal.E_civil);
            parameters.Add("p_gender", patientCreate.Informacion_personal.Gender);
            parameters.Add("p_blood_type", patientCreate.Informacion_medica.Blood_type);
            parameters.Add("p_height", patientCreate.Informacion_medica.Height);
            parameters.Add("p_weight", patientCreate.Informacion_medica.Weight);
            var result = await db.ExecuteAsync(
            "sp_insert_paciente",
            parameters,
            commandType: CommandType.StoredProcedure
            );
             return new ResponseResult
        {
            IsError = result < 0,
            Message = result > 0 ? "Update successful" : "Update failed"
        };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error inserting patient: ", ex);
        }
        finally
        {
            if (db.State == ConnectionState.Open)
            db.Close();
        }
    }
    public async Task<ResponseResult> UpdatePatient(PatientUpdateRequest personalInfo)
    {
    using (var connection = _context.CreateConnection())
    {
        var query = "UPDATE pacientes SET document_type = @DocumentType, firstname = @Firstname, identity_number = @IdentityNumber, lastname = @LastName, tel = @Tel WHERE id = @Id";
        var parameters = new
        {
            DocumentType = personalInfo.Document_type,
            Firstname = personalInfo.Firstname,
            IdentityNumber = personalInfo.Identity_number,
            LastName = personalInfo.Lastname,
            Tel = personalInfo.Tel,
            Id = personalInfo.Id
        };

        var result = await connection.ExecuteAsync(query, parameters);
        return new ResponseResult
        {
            IsError = result < 0,
            Message = result > 0 ? "Update successful" : "Update failed"
        };
    }
    }
}
