using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using WebApplication5.Dtos.Email;
using WebApplication5.Dtos.User;
using WebApplication5.Model;
using WebApplication5.Services.Abstract;

namespace WebApplication5.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailSender;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IConfiguration configuration, IMapper mapper, IEmailService emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _emailSender = emailSender;
        }      

        #region Login
        public async Task<GenericResponse<LoginResponseDto>> LoginUser(LoginDto loginDto)
        {
            var response = new GenericResponse<LoginResponseDto>();
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    response.Data = null;
                    response.Success = false;
                    response.Message = "Incorrect Email";

                    return response;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded)
                {
                    response.Data = null;
                    response.Success = false;
                    response.Message = "Incorrect password";

                    return response;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                 response.Data = new LoginResponseDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expires
                };
                response.Message = "Logged in Successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region Protected Route
        public Task<string> ProtectedRoute()
        {
            string response = "I am ultron";
            return Task.FromResult(response);

        }
        #endregion

        #region Register
        public async Task<GenericResponse<RegisterResponseDto>> RegisterUser(RegisterDto registerDto)
        {
            var response = new GenericResponse<RegisterResponseDto>();
            try
            {
                
                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                  response.Data = null;
                  response.Message = "The password and confirmation password do not match.";
                  response.Success = false;

                  return response;
                }

                var user = new User
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    Height = registerDto.Height,
                    Nickname = registerDto.Nickname
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);


                if (!result.Succeeded)
                {
                    response.Data = null;
                    response.Success = false;
                    response.Message = "Error Creating User";

                    return response;
                }

                if (registerDto.Role != null)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(registerDto.Role);

                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole((registerDto.Role)));
                    }
                    await _userManager.AddToRoleAsync(user, registerDto.Role);
                }

                response.Data = new RegisterResponseDto
                {
                    Message = "User Created",
                    FullName = user.UserName,
                    Nickname = user.Nickname,
                    Height = user.Height,
                };
                response.Message = "Registered Successfully";

            }
            catch
            (Exception ex) 
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region ForgotPassword
        public async Task<GenericResponse<string>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var response = new GenericResponse<string>();
            try
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                }

                // Change the callback with a proper callback url for resetting password / Button to redirect to reset password screen
                var email = new MailRequest();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callback = token;

                email.ToEmail = user.Email;
                email.Subject = "Reset Password Url";
                email.Body = callback;

               var sentEmail = await _emailSender.SendEmailAsync(email);
                if (sentEmail == null) 
                {
                    response.Success = false;
                    response.Message = "Error sending Email";
                }

                response.Data = null;
                response.Success = true;
                response.Message = "Email sent successfully";
                
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

        #region ResetPassword
        public async Task<GenericResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = new GenericResponse<string>();
            try 
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                }
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    response.Data = null;
                    response.Success = true;
                    response.Message = "Password reset done !";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        #endregion

    }
}
