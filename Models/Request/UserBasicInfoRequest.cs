namespace ConsultorioNet.Models.Request
{
    public class UserBasicInfoRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Document_type { get; set; }
        public string Identity_number { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public int? Count { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class PaginatedResult<T>
{
    public List<T> Data { get; set; } // Lista paginada de pacientes
    public int TotalRecords { get; set; } // Conteo total de registros
}
}