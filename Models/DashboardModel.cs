namespace Peers.Moderno.Models;

public class DashboardModel
{
    public int QtdAssociados { get; set; }
    public int QtdMentores { get; set; }
    public int QtdGestores { get; set; }
    public int QtdAvaliadores { get; set; }
    public int QtdAvaliacoes { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public List<DashboardItemModel> ListAndamento { get; set; } = new();
}

public class DashboardItemModel
{
    public string Status { get; set; } = string.Empty;
    public int QtdStatus { get; set; }
}

public class EvolucaoPerformanceModel
{
    public string Performance { get; set; } = string.Empty;
    public decimal? Nota { get; set; }
}

public class ResultadoProjetosModel
{
    public List<SomaCompetenciasModel> ListSomaCompetenciasN1N2 { get; set; } = new();
}

public class SomaCompetenciasModel
{
    public string Eixo { get; set; } = string.Empty;
    public decimal? PercentualSomaNotaFinalN1N2 { get; set; }
}

public class Periodo
{
    public int IdPeriodo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int IdEmpresa { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class Projeto
{
    public int IdProjeto { get; set; }
    public string Nome { get; set; } = string.Empty;
}