using System;

namespace ConsultorioNet.Models.Request
{
    public class ProfesionalInfoRequest
    {
        public int Id { get; set; }
        public DateTime? HireDate { get; set; }
        public string ProfessionalNumber { get; set; }
        public string WorkShift { get; set; }
        public short Specialization { get; set; }
        public int ConsultoriosId { get; set; }
    }
}