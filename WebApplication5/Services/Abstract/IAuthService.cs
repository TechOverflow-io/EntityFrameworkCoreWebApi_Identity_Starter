using Microsoft.AspNetCore.Mvc;
using WebApplication5.Dtos.Email;
using WebApplication5.Dtos.User;
using WebApplication5.Model;

namespace WebApplication5.Services.Abstract
{
    public interface IAuthService
    {
         Task<string> ProtectedRoute();
         Task<GenericResponse<RegisterResponseDto>> RegisterUser(RegisterDto registerDto);
         Task<GenericResponse<LoginResponseDto>> LoginUser(LoginDto loginDto);
         Task<GenericResponse<string>> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<GenericResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
