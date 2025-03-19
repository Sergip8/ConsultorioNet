namespace ConsultorioNet.Models.Response
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
        public UserResponse User {get; set;}
    }
}