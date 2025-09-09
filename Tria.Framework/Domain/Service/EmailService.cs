using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tria.Framework.Domain.Interface;
using static iText.Svg.SvgConstants;

namespace Tria.Framework.Domain.Service
{
    public class EmailService : IEmailService
    {
        private ConfigEmail _configEmail { get; }
        private Mensagem _mensagem { get; }
        private Destinatario _destinatario { get; }
        private MailAddressCollection BCCs { get; set; }

        public EmailService(ConfigEmail configuracoes, Destinatario destinatario, Mensagem mensagem, MailAddressCollection bCCs = null)
        {
            _configEmail = configuracoes;
            _destinatario = destinatario;
            _mensagem = mensagem;
            BCCs = bCCs;
        }

        public void Enviar()
        {
            MailMessage email = new MailMessage();
            SmtpClient objSmtp = new SmtpClient();
            
            try
            {
                #if DEBUG
                    MailAddress mailTo = new MailAddress("filipe.oliveira@peers.com.br", _destinatario.Nome);
                #else
                    MailAddress mailTo = new MailAddress(_destinatario.Email, _destinatario.Nome);
                #endif

                email = new MailMessage(new MailAddress(_configEmail.From, _configEmail.Remetente, Encoding.UTF8), mailTo);
                email.ReplyToList.Add(new MailAddress(_configEmail.Dominio, _configEmail.Remetente, Encoding.UTF8));
                email.Priority = MailPriority.High;
                email.IsBodyHtml = true;
                email.Subject = _mensagem.Titulo;
                email.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
                email.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");
                email.HeadersEncoding = Encoding.UTF8;
                objSmtp.Host = _configEmail.SmtpServer;
                objSmtp.Port = _configEmail.Porta;
                objSmtp.EnableSsl = _configEmail.UsarSSL;
                objSmtp.Credentials = new NetworkCredential(_configEmail.Dominio, _configEmail.Senha);
                email.Body = _mensagem.Corpo;
                if (BCCs != null)
                {
                    email.Bcc.Add(BCCs.ToString());
                }
                objSmtp.Send(email);
            }
            catch (Exception err)
            {
                string erro = err.Message;
            }
            finally
            {
                email = null;
                objSmtp = null;
            }
        }
    }

}