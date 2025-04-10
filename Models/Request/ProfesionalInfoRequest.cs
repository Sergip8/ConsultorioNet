using System;

namespace ConsultorioNet.Models.Request
{
    public class ProfesionalInfoRequest
    {
        public int Id { get; set; }
        public DateTime? Hire_date { get; set; }
        public string Professional_number { get; set; }
        public string Work_shift { get; set; }
        public short Specialization { get; set; }
        public int Consultorios_id { get; set; }
    }
}