using System;
using System.Threading.Tasks;

namespace Numero.Email
{
    public interface IEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(string userEmail, string emailSubject, string message);
    }
}
