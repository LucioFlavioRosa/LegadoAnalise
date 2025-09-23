namespace Peers.Moderno.Services.Complexidades.Common;

public class ComplexidadeDto
{
    public int IdComplexidade { get; set; }
    public string Complexidade { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int ATV { get; set; }
    public decimal Peso { get; set; }
    public decimal PesoPonderado { get; set; }
    public decimal Ponderacao { get; set; }
    public decimal FaixaInicial { get; set; }
    public decimal FaixaFinal { get; set; }
    public decimal SomaMinimaFator { get; set; }
    public DateTime DHC { get; set; }
    public string USR { get; set; } = string.Empty;
    public string StatusTexto => ATV == 1 ? "Ativo" : "Inativo";
    public bool IsAtivo => ATV == 1;
}