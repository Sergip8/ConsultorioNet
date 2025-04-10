
namespace ConsultorioNet.Models.Response
{
    public class CitasSchedureResponse
    {
    public int Id { get; set; }
    public string Slot { get; set; }
    public DateTime Appointment_start_time { get; set; }
    public string Response { get; set; }
    public string State { get; set; }
    public string Type { get; set; }
    public string Patient_document_type { get; set; }
    public string patient_firstname { get; set; }
    public string Patient_identity_number { get; set; }
    public string patient_lastname { get; set; }
    public string Patient_tel { get; set; }
    }
}