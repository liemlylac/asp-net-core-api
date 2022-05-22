using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreAPI.Models
{
    public class UserAuthenticateRequest
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
    }
}