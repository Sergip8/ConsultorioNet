using System;

namespace ConsultorioNet.Models.Request
{
    public class TratamientoRequest
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long CitasId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}