using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Models;

public class ProjetosAssociadosModel
{
    public int Id { get; set; }
    
    public Projeto? Projeto { get; set; }
    
    public Associado? Associado { get; set; }
    
    public Associado? Gestor { get; set; }
    
    public Associado? Avaliador { get; set; }
    
    public PeriodoAvaliacao? Periodo { get; set; }
    
    public string DataInicio { get; set; } = string.Empty;
    
    public string DataTermino { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string TipoAvaliacao { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Escopo { get; set; } = string.Empty;
    
    public StatusAvaliacao? Status { get; set; }
    
    public FotoAssociado? FotoAssociado { get; set; }
    
    public string RotuloBotao { get; set; } = string.Empty;
    
    public bool ExibirBotaoFinalizar { get; set; }
    
    public string Etapa { get; set; } = string.Empty;
    
    public string ExibirRotuloEtapa { get; set; } = string.Empty;
    
    public bool AvaliacaoLiberada { get; set; }
    
    public string IdEmail { get; set; } = string.Empty;
    
    public string IsHidden { get; set; } = string.Empty;
    
    public string CssClass { get; set; } = string.Empty;
    
    public Dictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();
}

public class FotoAssociado
{
    public int IdFoto { get; set; }
    public int IdAssociado { get; set; }
    public string Imagem { get; set; } = string.Empty;
    public string? NomeArquivo { get; set; }
    public DateTime? DataUpload { get; set; }
    public bool Ativo { get; set; } = true;
}