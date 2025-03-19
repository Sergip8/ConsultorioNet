namespace ConsultorioNet.Models.Request
{

  public class PatientMedicalInfoRequest
    {
    
        public int Id { get; set; }
        public string Blood_type { get; set; }
        public short? Height { get; set; }
        public short? Weight { get; set; }
    }
}