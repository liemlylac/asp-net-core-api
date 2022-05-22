namespace ASPNetCoreAPI.Models
{
    public class UserUpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Icon { get; set; }
        public string? Phone { get; set; }
    }
}