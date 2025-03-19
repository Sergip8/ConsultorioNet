using System;

namespace ConsultorioNet.Models.Request
{
    public class CitaCreateRequest
    {
        public short Slot { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public string State { get; set; }
        public short Type { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
    }
}