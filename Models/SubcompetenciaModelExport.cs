using System.ComponentModel;

namespace Peers.Moderno.Models;

public class SubcompetenciaModelExport
{
    [DisplayName("Código")]
    public int IdSubcompetencia { get; set; }

    [DisplayName("Sub Competência")]
    public string Subcompetencia { get; set; } = string.Empty;

    [DisplayName("Tipo Avaliação")]
    public string? TipoAvaliacao { get; set; }

    [DisplayName("Status")]
    public int ATV { get; set; }

    [DisplayName("Status Texto")]
    public string StatusTexto => ATV == 1 ? "Ativo" : "Inativo";
}