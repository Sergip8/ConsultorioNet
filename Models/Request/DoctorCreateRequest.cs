using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Models.Request
{
    public class DoctorCreateRequest
    {
        public DocumentTypePatient Document_type { get; set; }  
        public string Firstname { get; set; }
        public string  Identity_number { get; set; }
        public string  Lastname { get; set; }
        public string  Tel { get; set; }
        public string  Address { get; set; }

        public int User_id { get; set; }
        public PersonalInfoRequest Informacion_personal { get; set; }
        public ProfesionalInfoRequest informacion_profesional { get; set; }



    }
}