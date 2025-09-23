using Peers.Moderno.Models;
using System.Net.Mail;
using System.Net;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public interface IEmailUtils
{
    Task<bool> EnviarEmailAsync(ConfigEmail config, Destinatario destinatario, Mensagem mensagem);
    string ConfigurarCorpoEmail(string template, Projeto projeto, Associado associado, string prazoFinal, string etapa, string destinatarioNome, Associado avaliador, string descricao = "Uma nova avaliação está disponível para você.", bool removeDatas = false);
    string CarregarMascara(string template);
}

public class EmailUtils : IEmailUtils
{
    public async Task<bool> EnviarEmailAsync(ConfigEmail config, Destinatario destinatario, Mensagem mensagem)
    {
        try
        {
            using var client = new SmtpClient(config.SmtpServer, config.Porta)
            {
                Credentials = new NetworkCredential(config.From, config.Senha),
                EnableSsl = config.UsarSSL
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(config.From, config.Remetente),
                Subject = mensagem.Titulo,
                Body = mensagem.Corpo,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(destinatario.Email, destinatario.Nome));

            await client.SendMailAsync(mailMessage);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string ConfigurarCorpoEmail(string template, Projeto projeto, Associado associado, string prazoFinal, string etapa, string destinatarioNome, Associado avaliador, string descricao = "Uma nova avaliação está disponível para você.", bool removeDatas = false)
    {
        var corpo = template;
        
        corpo = corpo.Replace("[NOME]", destinatarioNome);
        corpo = corpo.Replace("[CARGO]", associado.Cargo?.Nome ?? "");
        corpo = corpo.Replace("[MENTOR]", associado.Mentor?.Nome ?? "");
        corpo = corpo.Replace("[PROJETO]", projeto.Projeto);
        corpo = corpo.Replace("[GESTOR]", projeto.AssociadoGestor?.Nome ?? "");
        corpo = corpo.Replace("[AVALIADOR]", avaliador.Nome);
        corpo = corpo.Replace("[PRAZO_FINAL]", prazoFinal);
        corpo = corpo.Replace("[ETAPA_AVALIACAO]", etapa);
        corpo = corpo.Replace("[NOME_AVALIADO]", associado.Nome);
        corpo = corpo.Replace("[DESCRICAO]", descricao);

        if (removeDatas)
        {
            corpo = corpo.Replace("<p><strong>Data de Início:</strong> [DATA_INICIO]</p>", "");
            corpo = corpo.Replace("<p><strong>Data de Término:</strong> [DATA_FINAL]</p>", "");
        }
        else
        {
            corpo = corpo.Replace("[DATA_INICIO]", projeto.DataInicio.ToString("dd/MM/yyyy"));
            corpo = corpo.Replace("[DATA_FINAL]", projeto.DataFim?.ToString("dd/MM/yyyy") ?? "");
        }

        return corpo;
    }

    public string CarregarMascara(string template)
    {
        return template;
    }
}

public class ConfigEmail
{
    public string From { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int Porta { get; set; }
    public string Dominio { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public bool UsarSSL { get; set; }
    public string Remetente { get; set; } = string.Empty;
}

public class Destinatario
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Mensagem
{
    public string Titulo { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
}