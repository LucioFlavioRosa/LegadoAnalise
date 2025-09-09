using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Tria.Framework.Domain.Service;
using System.Data.Entity.Core.Metadata.Edm;

namespace Business.Services
{
    public class EmailBulk
    {
        public List<EmailData> bulkEmailData { get; set; }
        List<Task> tasks = new List<Task>();
        public async Task Send()
        {
            EMAILPARAMETROS paramEmail = new EmailParametroService().ObterParametro(1);
            SmtpClient client = new SmtpClient(paramEmail.SMTPServer, (int)paramEmail.Porta)
            {
                Credentials = new NetworkCredential(paramEmail.RemetenteEmail, paramEmail.Password),
                EnableSsl = (bool)paramEmail.UsarSSL
            };

            foreach (var item in bulkEmailData)
            {
                string sendTo = item.emailTo;
#if DEBUG
                sendTo = "filipe.oliveira@peers.com.br";
#endif

                MailMessage message = new MailMessage(paramEmail.RemetenteEmail, sendTo, item.subject, item.body);
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;

                await client.SendMailAsync(message);
                //Console.WriteLine($"Email sent to {item}");
            }
        }
    }
    public class EmailData
    {
        public string emailTo { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
