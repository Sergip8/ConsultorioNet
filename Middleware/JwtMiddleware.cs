


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

internal sealed class JwtMiddleware : IFunctionsWorkerMiddleware
{
       private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(ILogger<JwtMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        _logger.LogInformation("middleware bearer token");
        // Obtén el token del encabezado de la solicitud
        if (context.BindingContext.BindingData.TryGetValue("Headers", out var headersObj) &&
            headersObj is string headers)
        {
            dynamic head = JsonConvert.DeserializeObject<dynamic>(headers);
            string authHeader = head["Authorization"];
            _logger.LogInformation(authHeader);

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                _logger.LogInformation(token);
                try
                {
                    // Valida el token
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                    if (expClaim != null)
                    {
                        var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
                        if (expDate < DateTime.UtcNow)
                        {
                            // Token expirado
                            context.Items["User"] = null;
                            
                          
                        }else{
                            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
                            context.Items["User"] = claimsPrincipal; // Asegúrate de agregar el usuario al contexto
                        }
                    }

                    // Agrega los claims al contexto
                  
                }
                catch (Exception ex)
                {
                    // Maneja errores de token inválido
                    context.Items["User"] = null; // Si el token es inválido, no hay usuario
                }
            }
            else{
                 context.Items["User"] = null;
            }
        }

        await next(context);
    }
}