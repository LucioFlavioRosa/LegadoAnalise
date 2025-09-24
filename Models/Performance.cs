using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("PERFORMANCES")]
public class Performance
{
    [Key]
    public int IdPerformance { get; set; }
    
    [Required]
    public int IdEmpresa { get; set; }
    
    [Required]
    public int IdCargo { get; set; }
    
    [Required]
    public int IdNivel { get; set; }
    
    [Required]
    [StringLength(500)]
    public string PerformanceNome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string PerformanceAbaixo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string PerformanceEsperado { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string PerformanceAcima { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Abrangencia { get; set; } = string.Empty;
    
    public bool InputAutoavaliacao { get; set; } = true;
    
    public int? NotaPadraoAutoAvaliacao { get; set; }
    
    public bool InputAvaliacaoAsCegas { get; set; } = true;
    
    public int? NotaPadraoAvaliacaoAsCegas { get; set; }
    
    public bool InputAvaliacaoGestor { get; set; } = true;
    
    public int? NotaPadraoAvaliacaoGestor { get; set; }
    
    [Required]
    public int ATV { get; set; } = 1;
    
    [Required]
    public DateTime DHC { get; set; } = DateTime.Now;
    
    [Required]
    public int USR { get; set; }
    
    // Navigation Properties
    [ForeignKey("IdCargo")]
    public virtual Cargo? Cargo { get; set; }
    
    [ForeignKey("IdNivel")]
    public virtual CargoNivel? CargoNivel { get; set; }
    
    [ForeignKey("NotaPadraoAutoAvaliacao")]
    public virtual AvaliacaoCompetenciaNota? NotaAutoAvaliacao { get; set; }
    
    [ForeignKey("NotaPadraoAvaliacaoAsCegas")]
    public virtual AvaliacaoCompetenciaNota? NotaAvaliacaoAsCegas { get; set; }
    
    [ForeignKey("NotaPadraoAvaliacaoGestor")]
    public virtual AvaliacaoCompetenciaNota? NotaAvaliacaoGestor { get; set; }
    
    // Computed Properties
    [NotMapped]
    public string StatusTexto => ATV == 1 ? "Ativo" : "Inativo";
    
    [NotMapped]
    public string AbrangenciaTexto => Abrangencia == "0" ? "Individual" : "Coletivo";
    
    [NotMapped]
    public bool IsAtivo => ATV == 1;
    
    [NotMapped]
    public bool IsInativo => ATV == 0;
}

public class PerformanceExportModel
{
    public int? IdPerformance { get; set; }
    public int IdCargo { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public string Performance { get; set; } = string.Empty;
    public string DescricaoAbaixo { get; set; } = string.Empty;
    public string DescricaoEsperado { get; set; } = string.Empty;
    public string DescricaoAcima { get; set; } = string.Empty;
    public int ATV { get; set; }
    public string Abrangencia { get; set; } = string.Empty;
    public int InputAutoAvaliacao { get; set; }
    public int? IdNotaPadraoAutoAvaliacao { get; set; }
    public string NotaPadraoAutoAvaliacao { get; set; } = string.Empty;
    public int InputAvaliacaoAsCegas { get; set; }
    public int? IdNotaPadraoAvaliacaoAsCegas { get; set; }
    public string NotaPadraoAvaliacaoAsCegas { get; set; } = string.Empty;
    public int InputAvaliacaoGestor { get; set; }
    public int? IdNotaPadraoAvaliacaoGestor { get; set; }
    public string NotaPadraoAvaliacaoGestor { get; set; } = string.Empty;
}

public class PerformanceImportResult
{
    public int LinhasInseridas { get; set; }
    public int LinhasAlteradas { get; set; }
    public int LinhasDesconsideradas { get; set; }
    public List<string> Erros { get; set; } = new List<string>();
    public bool Sucesso => !Erros.Any();
    
    public string MensagemResumo => 
        $"Performances importadas com sucesso<br>Inseridas: {LinhasInseridas}<br>Alteradas: {LinhasAlteradas}<br>Desconsideradas: {LinhasDesconsideradas}";
}

public class PerformanceValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public string ErrorMessage => string.Join(", ", Errors);
    
    public static PerformanceValidationResult Success()
    {
        return new PerformanceValidationResult { IsValid = true };
    }
    
    public static PerformanceValidationResult Failure(params string[] errors)
    {
        return new PerformanceValidationResult 
        { 
            IsValid = false, 
            Errors = errors.ToList() 
        };
    }
}