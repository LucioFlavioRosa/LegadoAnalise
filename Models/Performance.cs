namespace Peers.Moderno.Models;

public class Performance
{
    public int IdPerformance { get; set; }
    public int IdEmpresa { get; set; }
    public int IdCargo { get; set; }
    public int IdNivel { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string PerformanceAbaixo { get; set; } = string.Empty;
    public string PerformanceEsperado { get; set; } = string.Empty;
    public string PerformanceAcima { get; set; } = string.Empty;
    public string Abrangencia { get; set; } = string.Empty;
    public bool InputAutoavaliacao { get; set; } = true;
    public int? NotaPadraoAutoAvaliacao { get; set; }
    public bool InputAvaliacaoAsCegas { get; set; } = true;
    public int? NotaPadraoAvaliacaoAsCegas { get; set; }
    public bool InputAvaliacaoGestor { get; set; } = true;
    public int? NotaPadraoAvaliacaoGestor { get; set; }
    public int ATV { get; set; } = 1;
    public DateTime DHC { get; set; } = DateTime.Now;
    public int USR { get; set; }

    public virtual Cargo? Cargo { get; set; }
    public virtual CargoNivel? CargoNivel { get; set; }
}

public class PerformanceExportModel
{
    public int IdPerformance { get; set; }
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
}

public class PerformanceValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public string ErrorMessage => string.Join("; ", Errors);
}