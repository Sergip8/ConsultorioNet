

using System.Security.Claims;
using Consultorio.Function.Models;
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

    public static bool UserHasPatientRole(ClaimsPrincipal user)
    {
        var rolesStr = user?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        if (string.IsNullOrEmpty(rolesStr)) return false;
        var roles = rolesStr.Split(",");
        return roles.Contains("PATIENT");
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

}