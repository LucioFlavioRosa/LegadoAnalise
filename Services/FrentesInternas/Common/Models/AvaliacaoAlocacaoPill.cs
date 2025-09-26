using System.Collections.Generic;

namespace Services.FrentesInternas.Common.Models;

public class AvaliacaoAlocacaoPill
{
    public int idAvaliacao { get; set; }
    public int idAvaliado { get; set; }
    public string Avaliado { get; set; } = string.Empty;
    public string FotoNome { get; set; } = string.Empty;
    public int idNota { get; set; }
    public string Nota { get; set; } = string.Empty;
    public string Comentarios { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public List<ComboItem> Notas { get; set; } = new();
    public string ValidadoMD { get; set; } = string.Empty;
}