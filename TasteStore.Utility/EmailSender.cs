using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using TasteStore.Utility.Interfaces;

namespace TasteStore.Utility
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string name, string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();

            message.From.AddRange(new List<MailboxAddress>()
            {
                new MailboxAddress(name: "Taste Store Ltda.", address: "snacks.ltda.1@gmail.com")
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
                emailClient.Authenticate("snacks.ltda.1@gmail.com", "@Passw0rd");

                await emailClient.SendAsync(message);

                emailClient.Disconnect(true);
            }
        }
    }
}
