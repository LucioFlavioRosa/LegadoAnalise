using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Models;

public class ProjetoModel
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Nome { get; set; } = string.Empty;
    
    public string DataInicio { get; set; } = string.Empty;
    
    public string? DataTermino { get; set; }
    
    public Cliente? Cliente { get; set; }
    
    public Associado? Responsavel { get; set; }
    
    public Associado? Gestor { get; set; }
    
    public StatusProjeto? Status { get; set; }
    
    public List<ProjetosAssociadosModel> Associados { get; set; } = new List<ProjetosAssociadosModel>();
    
    public bool IsVisible { get; set; } = true;
    
    public string CssClass { get; set; } = string.Empty;
    
    public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
}

public class StatusProjeto
{
    public int IdStatus { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public class StatusAvaliacao
{
    public int IdStatus { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public class PeriodoAvaliacao
{
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int IdEmpresa { get; set; }
    public bool Ativo { get; set; } = true;
}

public class AvaliacaoEmail
{
    public int IdAvaliacao { get; set; }
    public int IdAssociado { get; set; }
    public int IdProjeto { get; set; }
    public int IdPeriodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public bool Liberado { get; set; }
    public string PosicaoAtualFluxoAvaliacao { get; set; } = string.Empty;
    public DateTime? DataEnvio { get; set; }
    public DateTime? DataLiberacao { get; set; }
}

public class AvaliacaoCompetencia
{
    public int IdAvaliacaoCompetencia { get; set; }
    public int IdAssociado { get; set; }
    public int IdProjeto { get; set; }
    public int IdPeriodo { get; set; }
    public int IdAvaliacao { get; set; }
    public int IdCompetencia { get; set; }
    public int IdNotaNivel1AutoAvaliacao { get; set; }
    public int IdNotaNivel2AutoAvaliacao { get; set; }
    public string? ComentariosAutoAvaliacao { get; set; }
    public DateTime? DataHoraFimAutoAvaliacao { get; set; }
    public DateTime? DataHoraFimAvaliacaoCegas { get; set; }
    public string PosicaoAtualFluxoAvaliacao { get; set; } = string.Empty;
}

public class AvaliacaoPerformance
{
    public int IdAvaliacaoPerformance { get; set; }
    public int IdAssociado { get; set; }
    public int IdProjeto { get; set; }
    public int IdPeriodo { get; set; }
    public int IdPerformance { get; set; }
    public int IdNotaNivel1AutoAvaliacao { get; set; }
    public string? ComentariosAutoAvaliacao { get; set; }
    public DateTime? DataHoraFimAutoAvaliacao { get; set; }
    public DateTime? DataHoraFimAvaliacaoCegas { get; set; }
    public string PosicaoAtualFluxoAvaliacao { get; set; } = string.Empty;
}