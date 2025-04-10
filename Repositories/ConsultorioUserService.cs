using System.Data;
using Api.FunctionApp.DataContext;
using ApiConsultorio.Models;
using Consultorio.Function.Models;
using Consultorio.Function.Models.Request;
using Consultorio.Function.Repositories.Interfaces;
using ConsultorioNet.Models.Request;
using ConsultorioNet.Models.Response;
using Dapper;
using EventManagementSystem.Helpers;
using Newtonsoft.Json;

namespace Api.FunctionApp.Repositories;

public class ConsultorioUserService : IConsultorioUsersService
{
 private readonly DapperContext _context;
 private readonly JwtSettings _jwtSettings;

    public ConsultorioUserService(DapperContext context, JwtSettings jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings;
    }

    async public Task<LoginResponse> Login(LoginRequest login)
    {
    using var connection = _context.CreateConnection();
    var parameters = new DynamicParameters();
    parameters.Add("p_email", login.Email, DbType.String);
    parameters.Add("p_password", login.Password, DbType.String);

    var result = await connection.QueryFirstOrDefaultAsync<UserResponse>(
        "sp_login_user", 
        parameters, 
        commandType: CommandType.StoredProcedure
    );

    
    string token = JwtHelper.GenerateJwt(_jwtSettings, result.Id, result.Roles, result.Email);
    return new LoginResponse {
           Token = token,
           Message = "Login successful",
           IsError = false,
           User = result
    };
    }

    async public Task<ResponseResult> Register(RegisterRequest register)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("p_email", register.Email, DbType.String);
        parameters.Add("p_password", register.Password, DbType.String);
        parameters.Add("p_rol", "PATIENT", DbType.String);

        var result = await connection.ExecuteAsync("sp_register_user", parameters, commandType: CommandType.StoredProcedure);

        return new ResponseResult
        {
            IsError = result > 0,
            Message = result > 0 ? "User registered successfully" : "User registration failed"
        };
    }
    public async Task<PaginatedResult<UserResponse>> GetPaginatedUsers(UserSearchParams userSearch)
{
    var result = new PaginatedResult<UserResponse>();

    using var connection = _context.CreateConnection();
        // Definir los parámetros del stored procedure
        var parameters = new
        {
            p_search_term = userSearch.SearchTerm,
            p_page_number = userSearch.Page,
            p_page_size = userSearch.Size,
            p_order_by = userSearch.OrderCriteria,
            p_order_direction = userSearch.OrderDirection
        };

        // Ejecutar el stored procedure
        using (var multi = await connection.QueryMultipleAsync("sp_get_paginated_users", parameters, commandType: CommandType.StoredProcedure))
        {
            // Leer el primer resultado: la lista paginada de usuarios
            result.Data = multi.Read<UserResponse>().ToList();

            // Leer el segundo resultado: el conteo total de registros
            result.TotalRecords = multi.Read<int>().Single();
        }
    

    return result;
}}