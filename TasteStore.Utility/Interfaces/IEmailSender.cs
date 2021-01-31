using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TasteStore.Utility.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string name, string email, string subject, string htmlMessage);
    }
}
