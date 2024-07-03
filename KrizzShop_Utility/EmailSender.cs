using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Text;



namespace KrizzShop_Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly MailSettings _mailSettings;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _mailSettings = _configuration.GetSection("Mail").Get<MailSettings>();
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(Encoding.UTF8, "KrizzShop", "mailsender@gmail.com"));
            message.To.Add(new MailboxAddress(Encoding.UTF8, "Customer", email));
                       
            message.Subject = subject;
            message.Body = new BodyBuilder()
            {
                HtmlBody = body
            }.ToMessageBody();
           
            using(SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate(_mailSettings.MailBox, _mailSettings.AccessPassword);
                
                await smtpClient.SendAsync(message);
                smtpClient.Disconnect(true);
            }
        }
    }
}
