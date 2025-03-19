namespace ConsultorioNet.Models.Request
{
    public class ConsultorioRequest
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Number { get; set; }
        public short Type { get; set; }
        public int MedicalCenterId { get; set; }
    }
}