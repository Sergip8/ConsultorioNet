namespace ConsultorioNet.Models.Request
{
    public class ConsultorioFilterParamsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; } 
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; } 
        public short? Type { get; set; }
        public bool? IsActive { get; set; }
        public int? MedicalCenterId { get; set; } 
    }
}