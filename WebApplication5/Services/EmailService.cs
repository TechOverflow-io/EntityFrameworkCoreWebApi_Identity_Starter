using MailKit.Security;
using MailKit;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid.Helpers.Mail;
using WebApplication5.Dtos.Email;
using MailKit.Net.Smtp;
using WebApplication5.Services.Abstract;
using WebApplication5.Model;

namespace WebApplication5.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _mailSettings;

        public EmailService(IOptions<EmailConfiguration> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        #region SendEmail
        public async Task<GenericResponse<string>> SendEmailAsync(MailRequest mailRequest)
        {   var response = new GenericResponse<string>();
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            response.Data = null;
            response.Success = true;
            response.Message = "Email sending was successful !";

            return response;
        }
        #endregion
    }
}
