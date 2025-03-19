namespace Consultorio.Function.Models.Request;

public class UserSearchParams
{
    public string SearchTerm { get; set; }
    public string OrderCriteria  { get; set; }
    public int Page { get; set; } = 0;
    public int Size { get; set; } = 10;
    public string OrderDirection { get; set; }  = "ASC";
}