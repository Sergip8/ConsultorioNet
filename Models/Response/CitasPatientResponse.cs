using System;

namespace ConsultorioNet.Models.Response
{
    public class CitasPatientResponse
    {
    public int Id { get; set; }
    public string Slot { get; set; }
    public DateTime Appointment_start_time { get; set; }
    public DateTime Created_date { get; set; }
    public string Response { get; set; }
    public string State { get; set; }
    public string Type { get; set; }
    public string Doctor_document_type { get; set; }
    public string Doctor_firstname { get; set; }
    public string Doctor_identity_number { get; set; }
    public string Doctor_lastname { get; set; }
    public string Doctor_tel { get; set; }
    }
}