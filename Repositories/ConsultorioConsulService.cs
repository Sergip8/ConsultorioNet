using System.Data;
using Api.FunctionApp.DataContext;
using Consultorio.Function.Models;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;
using Dapper;

namespace Api.FunctionApp.Repositories
{
    public class ConsultorioConsulService : IConsultorioConsulService
    {
        private readonly DapperContext _context;

        public ConsultorioConsulService(DapperContext context)
        {
            _context = context;
        }
        public async Task<ResponseResult> CreateConsultorio(ConsultorioRequest consultorioRequest)
        {
            // MySQL specific code for creating a consultorio
            using (var connection = _context.CreateConnection())
            {
                var query = @"
                    INSERT INTO Consultorios (Description, IsActive, Number, Type, MedicalCenterId)
                    VALUES (@Description, @IsActive, @Number, @Type, @MedicalCenterId);
                    SELECT LAST_INSERT_ID();";

                var parameters = new DynamicParameters();
                parameters.Add("Description", consultorioRequest.Description);
                parameters.Add("IsActive", consultorioRequest.IsActive);
                parameters.Add("Number", consultorioRequest.Number);
                parameters.Add("Type", consultorioRequest.Type);
                parameters.Add("MedicalCenterId", consultorioRequest.MedicalCenterId);

                var result = await connection.QuerySingleAsync<int>(query, parameters);

                return new ResponseResult
                {
                    IsError = result <= 0,
                    Message = result > 0 ? "Consultorio created successfully." : "Failed to create consultorio."
                };
            }
          
          
           
            
        }

        public async Task<ResponseResult> UpdateConsultorio(ConsultorioRequest consultorioRequest)
        {
             using (var connection = _context.CreateConnection())
            {
            var query = @"
                UPDATE Consultorios
                SET Description = @Description, IsActive = @IsActive, Number = @Number, Type = @Type, MedicalCenterId = @MedicalCenterId
                WHERE Id = @Id;";

            var parameters = new DynamicParameters();
            parameters.Add("Id", consultorioRequest.Id);
            parameters.Add("Description", consultorioRequest.Description);
            parameters.Add("IsActive", consultorioRequest.IsActive);
            parameters.Add("Number", consultorioRequest.Number);
            parameters.Add("Type", consultorioRequest.Type);
            parameters.Add("MedicalCenterId", consultorioRequest.MedicalCenterId);

            var result = await connection.ExecuteAsync(query, parameters);

            return new ResponseResult
            {
                IsError = result <= 0,
                Message = result > 0 ? "Consultorio updated successfully." : "Failed to update consultorio."
            };
           
            }
        }

        public async Task<ResponseResult> DeleteConsultorio(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "DELETE FROM Consultorios WHERE Id = @Id;";
                var parameters = new DynamicParameters();
                parameters.Add("Id", id);

                var result = await connection.ExecuteAsync(query, parameters);

                return new ResponseResult
                {
                    IsError = result <= 0,
                    Message = result > 0 ? "Consultorio deleted successfully." : "Failed to delete consultorio."
                };
            }
          
            
        }
     

        public async Task<List<ConsultorioResponse>> GetConsultoriospaginated(ConsultorioFilterParamsRequest consultorioFilter)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_page", consultorioFilter.Page);
                parameters.Add("p_page_size", consultorioFilter.PageSize);
                parameters.Add("p_order_by", consultorioFilter.OrderBy);
                parameters.Add("p_order_direction", consultorioFilter.OrderDirection);
                parameters.Add("p_type", consultorioFilter.Type);
                parameters.Add("p_is_active", consultorioFilter.IsActive);
                parameters.Add("p_medical_center_id", consultorioFilter.MedicalCenterId);

                var result = await connection.QueryAsync<ConsultorioResponse>(
                    "sp_get_consultorios",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
        }

       

    }
}
