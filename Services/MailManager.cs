﻿using Microsoft.Extensions.Options;
using test.Data;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace test.Services
{
    public class MailManager : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailManager(IOptions<MailSettings> settings)
        {
            _mailSettings = settings.Value;
        }
        public async Task SendEmailAsync(RequestEmail mailRequest)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailSettings.Mail)
                };
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder
                {
                    HtmlBody = mailRequest.Body
                };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                await smtp.SendAsync(email);

                smtp.Disconnect(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}
