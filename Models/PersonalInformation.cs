using System;

namespace ConsultorioNet.Models
{
    public class PersonalInformation
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string ECivil { get; set; }
        public string Gender { get; set; }
    }
}