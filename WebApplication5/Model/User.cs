using Microsoft.AspNetCore.Identity;

namespace WebApplication5.Model
{
    public class User : IdentityUser
    {
        public int Height { get; set; }
        public string Nickname { get; set; } = string.Empty;
    }
}
