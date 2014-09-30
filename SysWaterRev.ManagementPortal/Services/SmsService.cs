using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SimpleRevCollection.Management.Services;
using SysWaterRev.ManagementPortal.Properties;

namespace SysWaterRev.ManagementPortal.Services
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            var gateway = new AfricasTalkingGateway(Settings.Default.AfricaTalkingUserName,
                Settings.Default.AfricaTalkingAPIKey);
            gateway.SendMessage(message.Destination, message.Body);
            return Task.FromResult(0);
        }
    }
}