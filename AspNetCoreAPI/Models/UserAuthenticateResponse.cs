namespace ASPNetCoreAPI.Models
{
    public class UserAuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}