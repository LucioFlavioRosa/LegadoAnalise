using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Tria.Framework.Domain.Interface;

namespace Tria.Framework.Domain.Service
{
    public class ConfigEmail
    {
        public string SmtpServer { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Porta { get; set; }
        public string Dominio { get; set; }
        public string Senha { get; set; }
        public bool UsarSSL { get; set; }
        public string Remetente { get; set; }
    }

    public class Mensagem
    {
        public string Titulo { get; set; }
        public string Corpo { get; set; }
    }

    public class Destinatario
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }

    public class EmailUtils : IEmailUtils
    {
        public string CarregaMascara(string caminhoMascaraNewsletter) 
        {
            StreamReader objStreamReader;
            try { 
                objStreamReader = File.OpenText(caminhoMascaraNewsletter);
            }
            catch(DirectoryNotFoundException) {
                String[] file = caminhoMascaraNewsletter.Split('\\');
                objStreamReader = File.OpenText("C:\\inetpub\\wwwroot\\Emails\\" + file[file.Length - 1]);
            }
            string strConteudoMascara = objStreamReader.ReadToEnd();
            objStreamReader.Close();
            return strConteudoMascara;
        }
    }
}
