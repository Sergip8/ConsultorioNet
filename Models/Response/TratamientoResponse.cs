
namespace ConsultorioNet.Models.Request
{
    public class TratamientoResponse
    {
    public int Diagnostico_id { get; set; }
    public string Diagnostico_descripcion { get; set; }
    public DateTime Diagnostico_fecha_creacion { get; set; }
    public int Diagnostico_medicamentoId { get; set; }
    public string Observaciones { get; set; }
    public int Cantidad { get; set; }
    public string Medicament_name { get; set; }
    public string Concentracion { get; set; }
    public string Dosificacion { get; set; }
    }
}