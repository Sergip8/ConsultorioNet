namespace ConsultorioNet.Models.Response
{
    public class CitaResponse
    {
    public int Id { get; set; }
    public string Slot { get; set; }
    public DateTime Appointment_start_time { get; set; }
    public string Response { get; set; }
    public string State { get; set; }
    public string Type { get; set; }
    }
}