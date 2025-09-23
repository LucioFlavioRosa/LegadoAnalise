using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("COMPETENCIAS")]
public class Competencia
{
    [Key]
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
    
    public string? PalavrasChave { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    
    public bool Ativo { get; set; } = true;
    public int UsuarioCriacao { get; set; }
    public DateTime DataCriacao { get; set; }
    
    // Configurações de auto preenchimento
    public bool InputAutoAvaliacao { get; set; } = true;
    public bool InputAvaliacaoAsCegas { get; set; } = true;
    public bool InputAvaliacaoGestor { get; set; } = true;
    public bool InputFeedback { get; set; } = true;
    public bool InputNivel1 { get; set; } = true;
    public bool InputNivel2 { get; set; } = true;
    
    // Configurações de visibilidade
    public bool VisivelAutoAvaliacao { get; set; } = true;
    public bool VisivelAvaliacaoAsCegas { get; set; } = true;
    public bool VisivelAvaliacaoGestor { get; set; } = true;
    public bool VisivelFeedback { get; set; } = true;
    public bool VisivelNivel1 { get; set; } = true;
    public bool VisivelNivel2 { get; set; } = true;
    
    // Notas padrão
    public int? IdNotaPadraoNivel1 { get; set; }
    public int? IdNotaPadraoNivel2 { get; set; }
    
    // Modo de cálculo
    public int IdModoCalculo { get; set; }
    
    // Navigation Properties
    public virtual Cargo? Cargo { get; set; }
    public virtual Eixo? Eixo { get; set; }
    public virtual SubCompetencia? SubCompetencia { get; set; }
    public virtual Dimensao? Dimensao { get; set; }
    public virtual ModoCalculoCompetencia? ModoCalculo { get; set; }
}