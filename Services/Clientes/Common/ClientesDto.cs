namespace Peers.Moderno.Services.Clientes.Common;

public class ClienteDto
{
    public int IdCliente { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string? Telefones { get; set; }
    public int IdAssociacoResponsavel { get; set; }
    public string? GestorCliente { get; set; }
    public string? Email { get; set; }
    public int IdEmpresa { get; set; }
    public int USR { get; set; }
    public DateTime DHC { get; set; }
    public int ATV { get; set; }
    public string StatusTexto => ATV == 1 ? "Ativo" : "Inativo";
    public string? NomeSocio { get; set; }
}

public class SocioDto
{
    public int IdAssociado { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ClienteFormDto
{
    public int IdCliente { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public int IdSocio { get; set; }
    public string? GestorCliente { get; set; }
    public string? Email { get; set; }
    public int Status { get; set; } = 1;
}