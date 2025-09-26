namespace Peers.Moderno.Services.Feedback.Common;

using System;
using System.Collections.Generic;

public class ProjetoFeedbackModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? DataInicio { get; set; }
    public string? DataTermino { get; set; }
    public AssociadoFeedbackModel? Gestor { get; set; }
    public AssociadoFeedbackModel? Responsavel { get; set; }
    public StatusFeedbackModel? Status { get; set; }
    public ClienteFeedbackModel? Cliente { get; set; }
    public List<ProjetosAssociadosFeedbackModel> Associados { get; set; } = new();
}

public class ClienteFeedbackModel
{
    public int IdCliente { get; set; }
    public string Cliente { get; set; } = string.Empty;
}

public class PeriodoFeedbackModel
{
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class StatusFeedbackModel
{
    public int IdStatus { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ProjetosAssociadosFeedbackModel
{
    public int Id { get; set; }
    public ProjetoFeedbackModel? Projeto { get; set; }
    public AssociadoFeedbackModel? Associado { get; set; }
    public string? DataInicio { get; set; }
    public string? DataTermino { get; set; }
    public PeriodoFeedbackModel? Periodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public FotoAssociadoFeedbackModel? FotoAssociado { get; set; }
    public AssociadoFeedbackModel? Gestor { get; set; }
    public AssociadoFeedbackModel? Avaliador { get; set; }
    public string? Etapa { get; set; }
    public StatusFeedbackModel? Status { get; set; }
    public string RotuloBotao { get; set; } = string.Empty;
    public bool ExibirBotaoFinalizar { get; set; }
    public string ExibirFeedback { get; set; } = string.Empty;
    public string ExibirRotuloEtapa { get; set; } = string.Empty;
    public bool AvaliacaoLiberada { get; set; }
    public string? IdEmail { get; set; }
}

public class FotoAssociadoFeedbackModel
{
    public string? Imagem { get; set; }
}

public class FeedbackFiltroModel
{
    public int? ProjetoId { get; set; }
    public int? ClienteId { get; set; }
    public int? PeriodoId { get; set; }
    public int? StatusId { get; set; }
}

public class FeedbackAvaliacaoEmailModel
{
    public int idAvaliacao { get; set; }
    public int idAssociado { get; set; }
    public int idProjeto { get; set; }
    public int idPeriodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public bool Liberado { get; set; }
    public int PosicaoAtualFluxoAvaliacao { get; set; }
}

public class AvaliacaoPerformanceModel
{
    public int IdAvaliacaoPerformance { get; set; }
    public int IdNotaNivel1Feedback { get; set; }
    public DateTime? DataHoraFimFeedback { get; set; }
}

public class AvaliacaoCompetenciaModel
{
    public int IdAvaliacaoCompetencia { get; set; }
    public int IdNotaNivel1Feedback { get; set; }
    public int IdNotaNivel2Feedback { get; set; }
    public int? IdModo { get; set; }
    public CompetenciaModel? COMPETENCIAS { get; set; }
    public DateTime? DataHoraFimFeedback { get; set; }
}

public class CompetenciaModel
{
    public int IdModo { get; set; }
    public int? IdNotaPadraoNivel1 { get; set; }
    public int? IdNotaPadraoNivel2 { get; set; }
}

public class AssociadoFeedbackModel
{
    public int IdAssociado { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? FotoNome { get; set; }
    public CargoFeedbackModel? CARGOS { get; set; }
    public AssociadoFeedbackModel? ASSOCIADOS3 { get; set; }
    public bool Ativo { get; set; }
}

public class CargoFeedbackModel
{
    public int IdCargo { get; set; }
    public string Cargo { get; set; } = string.Empty;
}
