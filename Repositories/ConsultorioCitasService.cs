
using System.Data;
using Api.FunctionApp.DataContext;
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;
using Dapper;

public class ConsultorioCitasService: IConsultorioCitasService
{
    private readonly DapperContext _context;


    public ConsultorioCitasService(DapperContext context)
    {
        _context = context;
    }

    public async Task<ResponseResult> CreateCita(CitaCreateRequest citaCreate)
    {
        using var connection = _context.CreateConnection();
        var query = "CALL sp_crear_cita(@p_slot, @p_appointment_start_time, @p_state, @p_type, @p_doctor_id, @p_patient_id)";
        var parameters = new 
        {
            p_slot = citaCreate.Slot,
            p_appointment_start_time = citaCreate.AppointmentStartTime,
            p_state = citaCreate.State,
            p_type = citaCreate.Type,
            p_doctor_id = citaCreate.DoctorId,
            p_patient_id = citaCreate.PatientId
        };
        try{
        var result = await connection.QueryAsync<dynamic>(query, parameters);
        var mappedResult = result.Select(r => new ResponseResult
        {
            IsError = false,
            Message = r.response,
        }).FirstOrDefault();
        return mappedResult;

        }catch(Exception ex){
            return new ResponseResult
        {
            IsError = true,
            Message = ex.Message,
        };
        }
     
        }

         public async Task<ResponseResult> CreatePatientCita(CitaPatientCreateRequest citaCreate)
    {
        using var connection = _context.CreateConnection();
        var patientId = await connection.QueryAsync<long>(
            "SELECT p.id FROM pacientes as p WHERE p.user_id = @userId",
            new {userId = citaCreate.UserId}
        );
        var query = "CALL sp_crear_cita(@p_slot, @p_appointment_start_time, @p_state, @p_type, @p_doctor_id, @p_patient_id)";
        var parameters = new 
        {
            p_slot = citaCreate.Slot,
            p_appointment_start_time = citaCreate.AppointmentStartTime,
            p_state = citaCreate.State,
            p_type = citaCreate.Type,
            p_doctor_id = citaCreate.DoctorId,
            p_patient_id = patientId
        };
        try{
        var result = await connection.QueryAsync<dynamic>(query, parameters);
        var mappedResult = result.Select(r => new ResponseResult
        {
            IsError = false,
            Message = r.response,
        }).FirstOrDefault();
        return mappedResult;

        }catch(Exception ex){
            return new ResponseResult
        {
            IsError = true,
            Message = ex.Message,
        };
        }
     
        }


    public async Task<List<CitasDoctorResponse>> GetCitasByDoctorId(long userId)
    {
        using var connection = _context.CreateConnection();
        var userEntityQuery = "SELECT p.id FROM doctores as p WHERE p.user_id = @user_id";
        var patientId = await connection.QueryAsync<int>(userEntityQuery,  new { user_id = userId });
        if(patientId == null){
           return null;     
        }
        var query = "CALL sp_get_appointments_by_doctor_id(@p_patient_id)";
        var parameters = new { p_patient_id = patientId };
        var result = await connection.QueryAsync<CitasDoctorResponse>(query, parameters);
        return result.ToList();
    }

    public async Task<List<CitasPatientResponse>> GetCitasByPatientId(long userId)
    {
        using var connection = _context.CreateConnection();
        var userEntityQuery = "SELECT p.id FROM pacientes as p WHERE p.user_id = @user_id";
        var patientId = await connection.QueryAsync<int>(userEntityQuery,  new { user_id = userId });
        if(patientId == null){
           return null;     
        }
        var query = "CALL sp_get_appointments_by_pacient_id(@p_patient_id)";
        var parameters = new { p_patient_id = patientId };
        var result = await connection.QueryAsync<CitasPatientResponse>(query, parameters);
        return result.ToList();
    }

   
    public async Task<PaginatedResult<CitaResponse>> GetCitasPaginated(UserSearchParams userParams)
    {
       using var connection = _context.CreateConnection();
        try
        {

            var result = new PaginatedResult<CitaResponse>();
            var multi = await connection.QueryMultipleAsync(
                "sp_get_paginated_appointments",
                new
                {
                    p_search_term = userParams.SearchTerm,
                    p_page_number = userParams.Page,
                    p_page_size = userParams.Size,
                    p_order_by = userParams.OrderCriteria,
                    p_order_direction = userParams.OrderDirection
                },
                commandType: System.Data.CommandType.StoredProcedure);
            {
                result.Data = (await multi.ReadAsync<CitaResponse>()).ToList();
                result.TotalRecords = await multi.ReadSingleAsync<int>();

                return result;
              
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving guests for reservation with ID: ", ex);
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        
    }
}