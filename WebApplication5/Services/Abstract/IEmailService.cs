using AutoMapper.Internal;
using WebApplication5.Dtos.Email;
using WebApplication5.Model;

namespace WebApplication5.Services.Abstract
{
    public interface IEmailService
    {
        Task<GenericResponse<string>> SendEmailAsync(MailRequest mailRequest);
    }
}
