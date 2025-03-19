
using Api.FunctionApp.DataContext;
using Consultorio.Function.Models;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using Dapper;

public class ConsultorioPersonalInfoService: IConsultorioPersonalInfoService
{
    private readonly DapperContext _context;


    public ConsultorioPersonalInfoService(DapperContext context)
    {
        _context = context;
    }

    public async Task<ResponseResult> UpdatePersonalInfo(PersonalInfoRequest personalInfo)
    {
    using (var connection = _context.CreateConnection())
    {
        var query = "UPDATE informacion_personal SET address = @Address, birth_date = @BirthDate, e_civil = @ECivil, gender = @Gender WHERE id = @Id";
        var parameters = new
        {
            Address = personalInfo.Address,
            BirthDate = personalInfo.Birth_date,
            ECivil = personalInfo.E_civil,
            Gender = personalInfo.Gender,
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