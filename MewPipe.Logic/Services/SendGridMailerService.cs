using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RazorEngine;
using RazorEngine.Templating;
using SendGrid;

namespace MewPipe.Logic.Services
{
    public interface IMailerService
    {
        Task SendMail<T>(string to, string subject, string templateName, T parameters, Stream attachementStream,
            string attachmentName);
    }

    public class SendGridMailerService : IMailerService
    {
        private readonly Web _webTransport;
        private const string SentFrom = "MewPipe <no-reply@mewpipe.com>";

        public SendGridMailerService(string sendGridUserName, string sendGridPassword)
        {
            _webTransport = new Web(new NetworkCredential(sendGridUserName, sendGridPassword));
        }

        public async Task SendMail<T>(string to, string subject, string templatePath, T parameters, Stream attachementStream = null,
            string attachmentName = null)
        {
            var mail = new SendGridMessage();
            try
            {
                

            var htmlMail = GetCompiledTemplate(GetTemplateFullPath(templatePath, true), parameters);
            var textMail = GetCompiledTemplate(GetTemplateFullPath(templatePath, false), parameters);
            mail.From = new MailAddress(SentFrom);
            mail.AddTo(to);
            mail.Subject = subject;

            mail.Html = htmlMail;
            mail.Text = textMail;

            if (attachementStream != null && attachmentName != null)
            {
                mail.AddAttachment(attachementStream, attachmentName);
            }

            await _webTransport.DeliverAsync(mail);
            }
            catch(Exception e)
            {
                Console.Out.WriteLine(e);
            }
        }

        private string GetCompiledTemplate<T>(string templatePath, T parameters)
        {
            if (!Engine.Razor.IsTemplateCached(templatePath, typeof(T)))
            {
                var templateContent = File.ReadAllText(templatePath);

                Engine.Razor.Compile(templateContent, templatePath, typeof(T));
            }

            return Engine.Razor.Run(templatePath, typeof(T), parameters);
        }

        private string GetTemplateFullPath(string templatePath, bool isHtml)
        {
            if (isHtml)
            {
                return templatePath + "_html.cshtml";
            }
            return templatePath + "_txt.cshtml";
        }
    }
}
