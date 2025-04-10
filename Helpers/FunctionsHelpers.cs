

using System.Security.Claims;
using Consultorio.Function.Models;
using ConsultorioNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;

public static class FunctionsHelpers
{
     public static ClaimsPrincipal GetUserFromContext(FunctionContext context)
    {
        if (context.Items.TryGetValue("User", out var userObj) && userObj is ClaimsPrincipal user)
        {
            return user;
        }
        return null;
    }

    public static bool UserHasPatientRole(ClaimsPrincipal user, List<string> role = null)
    {
        if (role == null){
            role = new List<string>{"PATIENT"};
        }
        var rolesStr = user?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        if (string.IsNullOrEmpty(rolesStr)) return false;
        var roles = rolesStr.Split(",");
        return roles.Any(r => role.Contains(r));
    }

    public static string[] UserRoles(ClaimsPrincipal user)
    {
        var rolesStr = user?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        if (string.IsNullOrEmpty(rolesStr)) return null;
        var roles = rolesStr.Split(",");
        return roles;
    }
    public static async Task<UserSearchParams> DeserializeRequestBody<UserSearchParams>(HttpRequest req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Console.WriteLine(requestBody);
            return JsonConvert.DeserializeObject<UserSearchParams>(requestBody);
        }
        catch (JsonException)
        {
            return default; 
        }
    }

    public static ResponseResult CreateErrorResponse(string message)
    {
        return new ResponseResult
        {
            IsError = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }
    public static List<DateTime> ConvertirFechaYHora(DateTime fechaStr, string horaStr)
    {
        // 1. Convertir la fecha a DateTime
       

        // 2. Dividir el rango de horas
        string[] horas = horaStr.Split('-');
        DateTime horaInicio = DateTime.Parse(horas[0]);
        DateTime horaFin = DateTime.Parse(horas[1]);

        // 3. Crear las fechas combinadas
        DateTime fechaInicio = fechaStr.Date + horaInicio.TimeOfDay;
        DateTime fechaFin = fechaStr.Date + horaFin.TimeOfDay;

        // 4. Devolver la lista de fechas
        return new List<DateTime> { fechaInicio, fechaFin };
    }

}