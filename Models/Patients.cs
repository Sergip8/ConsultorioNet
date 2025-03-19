
namespace Consultorio.Function.Models
{
    public class Patients
    {
        public int Id { get; set; } 
        public DocumentTypePatient Document_type { get; set; }  
        public string Firstname { get; set; }
        public string  Identity_number { get; set; }
        public string  Lastname { get; set; }
        public int Usuarios_id { get; set; }
        public int Informacion_personal_id  { get; set; }
        public int Informacion_medica_id  { get; set; }



    }
    public enum DocumentTypePatient {
        DNI,
        Pasaporte,
        Cédula,
        TI
    }
}
