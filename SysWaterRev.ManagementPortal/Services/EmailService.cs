using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SysWaterRev.ManagementPortal.Properties;

namespace SysWaterRev.ManagementPortal.Services
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            var emailMessage = new MailMessage(Settings.Default.EmailAccount, message.Destination, message.Subject,
                message.Body);
            var smtpClient = new SmtpClient();
            smtpClient.Send(emailMessage);
            return Task.FromResult(0);
        }
    }
}