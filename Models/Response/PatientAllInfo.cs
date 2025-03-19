using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Models.Request
{
    public class PatientAllInfo
    {
        public int Id { get; set; }
        public string Document_type { get; set; }  
        public string Firstname { get; set; }
        public string  Identity_number { get; set; }
        public string  Lastname { get; set; }
        public string  Tel { get; set; }
        public int User_id { get; set; }
        public PersonalInfoRequest Informacion_personal { get; set; }
        public PatientMedicalInfoRequest Informacion_medica { get; set; }



    }
}