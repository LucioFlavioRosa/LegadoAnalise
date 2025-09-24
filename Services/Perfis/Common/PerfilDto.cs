namespace Peers.Moderno.Services.Perfis.Common;

public class PerfilDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime? DHC { get; set; }
    public string StatusTexto => StatusHelper.GetStatusText(Ativo);
}

public class PerfilListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string StatusTexto { get; set; } = string.Empty;
    public bool PodeInativar { get; set; }
}