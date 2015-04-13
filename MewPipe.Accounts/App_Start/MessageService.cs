using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace MewPipe.Accounts
{
    class MessageServices
    {

        public class EmailService : IIdentityMessageService
        {
            public async Task SendAsync(IdentityMessage message)
            {
                var myMessage = new SendGridMessage
                {
                    From = new MailAddress("no-reply@mewpipe.com", "MewPipe Staff")
                };

                myMessage.AddTo(message.Destination);

                myMessage.Subject = message.Subject;

                myMessage.Html = message.Body;
                myMessage.Text = "Test";

                var credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailerUsername"], ConfigurationManager.AppSettings["MailerPassword"]);

                var transportWeb = new Web(credentials);

                await transportWeb.DeliverAsync(myMessage);
            }
        }

        public class SmsService : IIdentityMessageService
        {
            public Task SendAsync(IdentityMessage message)
            {
                // Plug in your SMS service here to send a text message.
                return Task.FromResult(0);
            }
        }

    }
}
