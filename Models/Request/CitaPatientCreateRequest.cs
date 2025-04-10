namespace ConsultorioNet.Models.Request
{
    public class CitaPatientCreateRequest
    {
        public short Slot { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public string State { get; set; } = "Agendada";
        public short Type { get; set; }
        public int DoctorId { get; set; }
        public int UserId { get; set; }
    }
}