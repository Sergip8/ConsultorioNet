using System.Net;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Consultorio.Function.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YamlDotNet.Core.Events;


public class ManageChat
{
    private readonly ILogger<ManageChat> _logger;
    private readonly IConsultorioDoctorService _consultorioDoctorService;
    private readonly HttpClient _httpClient;
    private string ApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
    private string url = Environment.GetEnvironmentVariable("OPENROUTER_API_URL");
    private string model = Environment.GetEnvironmentVariable("OPENROUTER_MODEL");
    private Boolean Iscomplete = false;

    public int GetCitasByPatientId { get; private set; }
    private const string MedicalContext = @"Eres un asistente médico virtual para un consultorio. Tu objetivo es recopilar la siguiente información del usuario: especialización médica requerida, fecha deseada, horario preferido y presupuesto en COP.

Formula preguntas naturales, cortas y claras para obtener estos datos. No ofrezcas ejemplos en tus preguntas.

Extrae de cada respuesta del usuario los datos relevantes y conviértelos según estas reglas:
- Especialización: Capitalizada y con tildes correctas (ej: 'Oftalmología')
- Fecha: Formato ISO (YYYY-MM-DD). Si no se menciona año, usa el actual
- Hora: Formato ISO en rango (HH:MM-HH:MM). Si mencionan 'mañana' o 'tarde', conviértelo al rango horario correspondiente
- Costo: Solo el valor numérico en COP

Después de cada interacción, incluye AL FINAL de tu respuesta un JSON con la información obtenida hasta el momento:
{
  especialidad: [valor extraído],
  fecha: [valor extraído],
  hora: [valor extraído],
  costo: [valor extraído] opcional,
  complete: false
}

Cuando tengas los cuatro datos completos, cambia complete a true.

Mantén siempre un tono empático y profesional. Una vez recopilados todos los datos, indica al usuario que vas a buscar disponibilidad con esta información.";
    public ManageChat(ILogger<ManageChat> logger, IConsultorioDoctorService consultorioDoctorService)
    {
        _logger = logger;
        _consultorioDoctorService = consultorioDoctorService;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
       
    }



    [Function("Chat")]
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
{
    _logger.LogInformation("Procesando consulta médica...");

    _logger.LogInformation($"apikey: {ApiKey}");

     var requestData = await FunctionsHelpers.DeserializeRequestBody<MedicalChatRequest>(req);
    _logger.LogInformation(JsonConvert.SerializeObject(requestData));

    if (requestData == null || string.IsNullOrEmpty(requestData.Message))
    {
        return new BadRequestObjectResult("Por favor proporcione un mensaje válido");
    }

    requestData.ConversationHistory.Insert(0, new ChatMessageDto { content = MedicalContext, role = "system" });

    try
    {
        var openRouterRequest = new
        {
            model = model,
            messages = requestData.ConversationHistory,
            temperature = 0.3,
            max_tokens = 500
        };
        _logger.LogInformation(JsonConvert.SerializeObject(openRouterRequest));

        var response = await _httpClient.PostAsJsonAsync(new Uri(url), openRouterRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Error de OpenRouter: {errorContent}");
            return new ObjectResult("Error al procesar su consulta médica") { StatusCode = StatusCodes.Status503ServiceUnavailable };
        }

        var responseData = await response.Content.ReadFromJsonAsync<OpenRouterResponse>();
        _logger.LogInformation(JsonConvert.SerializeObject(responseData));

        var CommentSplit = responseData?.choices[0].message.content.Split("```");
        DoctorAvailabilityChatResponse? DoctorAvailability = null;
        var assistantMessage = CommentSplit[0];

        if (CommentSplit.Length > 1)
        {
            DoctorAvailability = await FormatComment(CommentSplit[1]);
        }
        else
        {
            assistantMessage = responseData?.choices[0].message.content;
        }

        if (string.IsNullOrEmpty(assistantMessage))
        {
            return new ObjectResult("No se recibió respuesta del asistente") { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var responsePayload = new MedicalChatResponse
        {
            Reply = assistantMessage,
            Complete = Iscomplete,
            DoctorAvailability = DoctorAvailability
        };

        return new OkObjectResult(responsePayload);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error inesperado");
        return new ObjectResult("Error interno del sistema") { StatusCode = StatusCodes.Status500InternalServerError };
    }
}

    private async Task<DoctorAvailabilityChatResponse?> FormatComment(string comment)
    {

        int inicio = comment.IndexOf("{"); // Encuentra el inicio del JSON
        int fin = comment.IndexOf("}", inicio) +1; // Encuentra el final del JSON

        if (inicio != -1 && fin != -1)
        {
            try{

            var data = JsonConvert.DeserializeObject<UserExtractData>(comment.Substring(inicio, fin - inicio));
            Console.WriteLine(JsonConvert.SerializeObject(data));
            if(data.complete){
                Iscomplete = true;
                Console.WriteLine("data complete");
                return await _consultorioDoctorService.GetAvailabilityAppointments(data);            
            }
            }catch{
                return null;
            }
            
        }
        return null;
    }

    private HttpResponseData CreateErrorResponse(HttpRequestData req, string message, HttpStatusCode statusCode)
    {
        var response = req.CreateResponse(statusCode);
        response.WriteAsJsonAsync(message);
        return response;
    }

}

public class UserExtractData
{
    public string? especializacion { get; set; }
    public DateTime fecha { get; set; }
    public string? hora { get; set; }
    public decimal? costo_max { get; set; } 
    public decimal? costo_min { get; set; } 
    public decimal? costo { get; set; } 
    public bool complete { get; set; }
}

public class OpenRouterResponse
{
    public Choice[] choices { get; set; }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
    }
}


