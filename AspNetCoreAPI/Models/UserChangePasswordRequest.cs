using System.ComponentModel.DataAnnotations;

namespace ASPNetCoreAPI.Models
{
    public class UserChangePasswordRequest
    {
        [Required] public string OldPassword { get; set; }
        [Required] public string NewPassword { get; set; }
        [Required] public string RepeatPassword { get; set; }
    }
}