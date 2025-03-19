using System.Security.Claims;
using System.Text;
using Api.FunctionApp.DataContext;
using Api.FunctionApp.Repositories;
using Consultorio.Function.Repositories.Interfaces;
using EventManagementSystem.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Configura la carga de configuración
        if (context.HostingEnvironment.IsDevelopment())
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
        }
        else
        {
            config.AddEnvironmentVariables();
        }
    })
    
    .ConfigureFunctionsWebApplication(app =>{
        app.UseWhen<JwtMiddleware>((context) =>
                    {
                        return context.FunctionDefinition.InputBindings.Values
                                      .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
                    });
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        // Obtén la configuración
        var configuration = context.Configuration;

        // Configura JwtSettings
        JwtSettings jwtSettings;
        if (context.HostingEnvironment.IsDevelopment())
        {
            jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        }
        else
        {
            jwtSettings = new JwtSettings
            {
                JwtTokenName = Environment.GetEnvironmentVariable("JwtTokenName") ?? string.Empty,
                SecretKey = Environment.GetEnvironmentVariable("SecretKey") ?? string.Empty,
                Issuer = Environment.GetEnvironmentVariable("Issuer") ?? string.Empty,
                ValidateIssuer = Convert.ToBoolean(Environment.GetEnvironmentVariable("ValidateIssuer")),
                Audience = Environment.GetEnvironmentVariable("Audience") ?? string.Empty,
                ValidateAudience = Convert.ToBoolean(Environment.GetEnvironmentVariable("ValidateAudience")),
                TokenExpirationInMinutes = Convert.ToInt32(Environment.GetEnvironmentVariable("TokenExpirationInMinutes")),
                ValidateLifetime = Convert.ToBoolean(Environment.GetEnvironmentVariable("ValidateLifetime"))
            };
        }

        if (jwtSettings is null)
        {
            throw new InvalidOperationException("JwtSettings configuration is missing.");
        }

        // Registra JwtSettings como un servicio singleton
        services.AddSingleton(jwtSettings);

        // Configura la autenticación JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey))
                };
            });

        // Configura la autorización
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "ADMIN"));
            options.AddPolicy("PatientOnly", policy => policy.RequireClaim("role", "PATIENT"));
        });

        // Registra tus servicios personalizados
        services.AddScoped<IConsultorioManagementService, ConsultorioManagementService>();
        services.AddScoped<IConsultorioUsersService, ConsultorioUserService>();
        services.AddScoped<IConsultorioPersonalInfoService, ConsultorioPersonalInfoService>();
        services.AddScoped<IConsultorioMedicalInfoService, ConsultorioMedicalInfoService>();
        services.AddScoped<IConsultorioCitasService, ConsultorioCitasService>();
        services.AddScoped<IConsultorioConsulService, ConsultorioConsulService>();
        services.AddScoped<IConsultorioDoctorService, ConsultorioDoctorService>();
        services.AddScoped<IConsultorioTratamientoService, ConsultorioTratamientoService>();





        services.AddSingleton<DapperContext>();
    })
   
    .Build();
// Authentication and Authorization are configured in ConfigureServices
// Habilita la autenticación y autorización
host.Run();
