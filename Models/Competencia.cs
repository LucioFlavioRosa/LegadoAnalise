namespace Peers.Moderno.Models;

public class Competencia
{
    public int IdCompetencia { get; set; }
    public int IdEmpresa { get; set; }
    public int IdCargo { get; set; }
    public int IdNivel { get; set; }
    public int IdEixo { get; set; }
    public int IdSubCompetencia { get; set; }
    public int IdDimensao { get; set; }
    public string CompetenciaJR { get; set; } = string.Empty;
    public string CompetenciaPL { get; set; } = string.Empty;
    public string CompetenciaSR { get; set; } = string.Empty;
    public string CompetenciaJRDetalhe { get; set; } = string.Empty;
    public string CompetenciaPLDetalhe { get; set; } = string.Empty;
    public string CompetenciaSRDetalhe { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int ATV { get; set; }
    public int USR { get; set; }
    public DateTime DHC { get; set; }
    
    // Configurações de auto preenchimento
    public bool InputAutoAvaliacao { get; set; }
    public bool InputAvaliacaoAsCegas { get; set; }
    public bool InputAvaliacaoGestor { get; set; }
    public bool InputFeedback { get; set; }
    public bool InputNivel1 { get; set; }
    public bool InputNivel2 { get; set; }
    public bool VisivelAutoAvaliacao { get; set; }
    public bool VisivelAvaliacaoAsCegas { get; set; }
    public bool VisivelAvaliacaoGestor { get; set; }
    public bool VisivelFeedback { get; set; }
    public bool VisivelNivel1 { get; set; }
    public bool VisivelNivel2 { get; set; }
    public int? IdNotaPadraoNivel1 { get; set; }
    public int? IdNotaPadraoNivel2 { get; set; }
    public int IdModo { get; set; }
    
    // Navegação
    public virtual Cargo? Cargo { get; set; }
    public virtual Eixo? Eixo { get; set; }
    public virtual SubCompetencia? SubCompetencia { get; set; }
    public virtual Dimensao? Dimensao { get; set; }
}