
using Api.FunctionApp.DataContext;
using Consultorio.Function.Models;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models;
using ConsultorioNet.Models.Request;
using Dapper;

public class ConsultorioMedicalInfoService: IConsultorioMedicalInfoService
{
    private readonly DapperContext _context;


    public ConsultorioMedicalInfoService(DapperContext context)
    {
        _context = context;
    }

    public async Task<ResponseResult> UpdateMedicalInfo(PatientMedicalInfoRequest medicallInfo)
    {
    using (var connection = _context.CreateConnection())
    {
        var query = "UPDATE informacion_medica_paciente SET blood_type = @Blood_type, height = @Height, weight = @Weight WHERE id = @Id";
        var parameters = new
        {
            Blood_type = medicallInfo.Blood_type,
            Height = medicallInfo.Height,
            Weight = medicallInfo.Weight,
            Id = medicallInfo.Id
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