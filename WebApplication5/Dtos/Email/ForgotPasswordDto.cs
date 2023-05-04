using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Dtos.Email
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
