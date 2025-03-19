namespace ConsultorioNet.Models.Request
{
    public class TratamientoParamsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; } 
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; } 
    
    }
}