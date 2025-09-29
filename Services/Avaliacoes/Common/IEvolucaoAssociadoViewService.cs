using Peers.Moderno.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public interface IEvolucaoAssociadoViewService
{
    Task<EvolucaoAssociadoViewModel> ObterEvolucaoAssociadoAsync(int associadoId, string tipoAvaliacao, string escopo);
    Task<string> MontarJsonRadarAsync(List<EvolucaoAssociadoProjeto> projetos, int idCargo);
    Task<AssociadoInfoViewModel> ObterInfoAssociadoAsync(int associadoId);
}

public class EvolucaoAssociadoViewModel
{
    public AssociadoInfoViewModel AssociadoInfo { get; set; } = new();
    public List<EvolucaoAssociadoProjeto> Projetos { get; set; } = new();
    public string JsonRadar { get; set; } = string.Empty;
    public bool HasData { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class AssociadoInfoViewModel
{
    public string Nome { get; set; } = string.Empty;
    public string Mentor { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string ProximoCargo { get; set; } = string.Empty;
}

public class EvolucaoAssociadoProjeto
{
    public string Cargo { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public decimal NotaCompetencia { get; set; }
    public decimal? NotaPerformance { get; set; }
    public string RatingPerformance { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public int IdPeriodo { get; set; }
    public List<EvolucaoCompetencia> Competencias { get; set; } = new();
    public List<EvolucaoPerformance> Performances { get; set; } = new();
    public bool ExibirPerformance { get; set; } = true;
}

public class EvolucaoCompetencia
{
    public string Eixo { get; set; } = string.Empty;
    public decimal NotaNeutra { get; set; }
    public decimal? Nota { get; set; }
}

public class EvolucaoPerformance
{
    public string Performance { get; set; } = string.Empty;
    public decimal Nota { get; set; }
}
