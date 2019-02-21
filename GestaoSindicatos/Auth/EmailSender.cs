using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GestaoSindicatos.Auth
{
    public class EmailSender : IEmailSender
    {

        // Our private configuration variables
        private readonly string host;  //10.200.48.120
        private readonly int port;     //25
        private readonly bool enableSSL;// false
        private readonly bool useDefaultCredentials; // false
        private readonly string userName;
        private readonly string password;

        // Get our parameterized configuration
        public EmailSender(string host, int port, bool enableSSL, bool useDefaultCredentials, string userName = null, string password = null)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.useDefaultCredentials = useDefaultCredentials;
            this.userName = userName;
            this.password = password;
        }

        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(host, port)
            {
                UseDefaultCredentials = useDefaultCredentials,
                EnableSsl = enableSSL
            };
            if (!string.IsNullOrEmpty(password))
            {
                client.Credentials = new NetworkCredential(userName, password);
            }
            return client.SendMailAsync(
                new MailMessage(userName, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}
