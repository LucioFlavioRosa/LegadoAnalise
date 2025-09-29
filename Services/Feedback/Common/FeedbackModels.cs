namespace Peers.Moderno.Services.Feedback.Common;

public class ProjetoFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ClienteFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class PeriodoFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class StatusFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class FeedbackFiltroModel
{
    public int? ProjetoId { get; set; }
    public int? StatusId { get; set; }
    public int? PeriodoId { get; set; }
    public int? ClienteId { get; set; }
}

public class FeedbackAvaliacaoEmailModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class AvaliacaoPerformanceModel
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

public class AvaliacaoCompetenciaModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class AssociadoFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
