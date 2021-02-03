using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using TasteStore.Utility.Interfaces;

namespace TasteStore.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string name, string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();

            message.From.AddRange(new List<MailboxAddress>()
            {
                new MailboxAddress(name: "Taste Store Ltda.", address: _configuration["EmailConfiguration:SmtpUsername"])
            });

            message.To.AddRange(new List<MailboxAddress>()
            {
                new MailboxAddress(name: name, address: email)
            });

            message.Importance = MessageImportance.High;
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 465, true);
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(_configuration["EmailConfiguration:SmtpUsername"], _configuration["EmailConfiguration:SmtpPassword"]);

                await emailClient.SendAsync(message);

                emailClient.Disconnect(true);
            }
        }
    }
}
