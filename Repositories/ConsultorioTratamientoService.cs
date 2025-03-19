using System.Data;
using Api.FunctionApp.DataContext;
using Consultorio.Function.Models;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;
using Dapper;

namespace Api.FunctionApp.Repositories
{
    public class ConsultorioTratamientoService : IConsultorioTratamientoService
    {
        private readonly DapperContext _context;

        public ConsultorioTratamientoService(DapperContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> CreateTratamiento(TratamientoRequest tratamiento)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "INSERT INTO tratamientos (description, citas_id, created_date) VALUES (@Description, @CitasId, @CreateDate)";
                var result = await connection.ExecuteAsync(query, new { tratamiento.Description, tratamiento.CitasId, DateTime.Now });
                return new ResponseResult
                {
                    IsError = result < 0,
                    Message = result > 0 ? "Update successful" : "Update failed"
                };
            }
        }

        public async Task<ResponseResult> UpdateTratamiento(TratamientoRequest tratamiento)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "UPDATE tratamientos SET description = @Description, citas_id = @CitasId WHERE id = @Id";
                var result = await connection.ExecuteAsync(query, new { tratamiento.Description, tratamiento.CitasId, Id = tratamiento.Id });
                return new ResponseResult
                {
                    IsError = result < 0,
                    Message = result > 0 ? "Update successful" : "Update failed"
                };
            }
        }

        public async Task<ResponseResult> DeleteTratamiento(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "DELETE FROM tratamientos WHERE id = @Id";
                var result = await connection.ExecuteAsync(query, new { Id = id });
                return new ResponseResult
                {
                    IsError = result < 0,
                    Message = result > 0 ? "Update successful" : "Update failed"
                };
            }
        }

        public async Task<List<ConsultorioResponse>> GetTratamientospaginated(TratamientoParamsRequest consultorioFilter)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_page", consultorioFilter.Page);
                parameters.Add("p_page_size", consultorioFilter.PageSize);
                parameters.Add("p_order_by", consultorioFilter.OrderBy);
                parameters.Add("p_order_direction", consultorioFilter.OrderDirection);

                var result = await connection.QueryAsync<ConsultorioResponse>(
                    "sp_get_consultorios",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
        }

        public async Task<List<TratamientoResponse>> GetTratamientoByCitasId(long citasId)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_cita_id", citasId);

                var query = "sp_get_datos_cita";

                var result = await connection.QueryAsync<TratamientoResponse>(
                    query,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
        }
    }
}
