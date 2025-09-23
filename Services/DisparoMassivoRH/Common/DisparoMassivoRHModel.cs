namespace Peers.Moderno.Services.DisparoMassivoRH.Common;

public class DisparoMassivoRHItem
{
    public int IdConsideracoesMentor { get; set; }
    public int IdAssociado { get; set; }
    public string Associado { get; set; } = string.Empty;
    public int IdMentor { get; set; }
    public string Mentor { get; set; } = string.Empty;
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class DisparoEmailRequest
{
    public int IdAssociado { get; set; }
    public int IdMentor { get; set; }
    public int IdPeriodo { get; set; }
    public string DataFinal { get; set; } = string.Empty;
}

public class DisparoEmailResult
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public int TotalDisparados { get; set; }
    public List<string> Erros { get; set; } = new();
}

public class ConfiguracaoEmail
{
    public string From { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int Porta { get; set; }
    public string Dominio { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public bool UsarSSL { get; set; }
    public string Remetente { get; set; } = string.Empty;
    public string ModeloEmail { get; set; } = string.Empty;
}