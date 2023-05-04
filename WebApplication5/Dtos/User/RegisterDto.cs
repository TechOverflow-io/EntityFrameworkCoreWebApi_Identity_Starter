namespace WebApplication5.Dtos.User
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public int Height { get; set; }
        public string Nickname { get; set; }
        public string? Role { get; set; }
    }
}
