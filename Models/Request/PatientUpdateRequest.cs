using ConsultorioNet.Models.Request;

namespace Consultorio.Function.Models.Request
{
    public class PatientUpdateRequest
    {
        public int Id { get; set; }
        public DocumentTypePatient Document_type { get; set; }  
        public string Firstname { get; set; }
        public string  Identity_number { get; set; }
        public string  Lastname { get; set; }
        public string  Tel { get; set; }


    }
}