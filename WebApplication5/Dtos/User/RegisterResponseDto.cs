namespace WebApplication5.Dtos.User
{
    public class RegisterResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Height { get; set; }
        public string Nickname { get; set; } = string.Empty;
    }
}
