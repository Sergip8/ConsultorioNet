using System;

namespace ConsultorioNet.Models.Request
{
    public class PersonalInfoRequest
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime? Birth_date { get; set; }
        public string E_civil { get; set; }
        public string Gender { get; set; }
    }
}