﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public EmailSenderOptions Options { get; }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, string textMessage = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(this.Options.EmailFromAddress, this.Options.EmailFromName);
            mailMessage.To.Add(toEmail);
            mailMessage.Body = textMessage;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = Encoding.UTF8;

            if (!string.IsNullOrEmpty(htmlMessage))
            {
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage);
                htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                mailMessage.AlternateViews.Add(htmlView);
            }

            using (SmtpClient client = new SmtpClient(this.Options.Host, this.Options.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(this.Options.Username, this.Options.Password);
                client.EnableSsl = this.Options.EnableSSL;
              
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
