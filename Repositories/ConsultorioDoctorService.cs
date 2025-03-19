using System.Data;
using Api.FunctionApp.DataContext;
using Azure;
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models;
using ConsultorioNet.Models.Request;
using Dapper;

namespace Api.FunctionApp.Repositories;

public class ConsultorioDoctorService : IConsultorioDoctorService
{
    private readonly DapperContext _context;

    public ConsultorioDoctorService(DapperContext context)
    {
        _context = context;
    }

    public Task<PatientAllInfo> GetPatientAllInfo(long patientId)
    {
        using IDbConnection db = _context.CreateConnection();
        try
        {
            var sql = @"
            SELECT p.*, ip.*, im.*
            FROM pacientes p
            INNER JOIN informacion_personal ip ON p.informacion_personal_id = ip.id
            INNER JOIN informacion_medica_paciente im ON p.informacion_medica_id = im.id";

            var result = db.Query<PatientAllInfo, PersonalInfoRequest, PatientMedicalInfoRequest, PatientAllInfo>(
                sql,
                (patient, personalInfo, medicalInfo) =>
                {
                    patient.Informacion_personal = personalInfo;
                    patient.Informacion_medica = medicalInfo;
                    return patient;
                },
                splitOn: "Id,Id"
            ).First();

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
            parameters.Add("p_hire_date", doctorCreate.informacion_profesional.HireDate);
            parameters.Add("p_professional_number", doctorCreate.informacion_profesional.ProfessionalNumber);
            parameters.Add("p_work_shift", doctorCreate.informacion_profesional.WorkShift);
            parameters.Add("p_specialization", doctorCreate.informacion_profesional.Specialization);
            parameters.Add("p_consultorios_id", doctorCreate.informacion_profesional.ConsultoriosId);

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
