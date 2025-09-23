namespace Peers.Moderno.Models;

public class ProjetoModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string DataInicio { get; set; } = string.Empty;
    public string DataTermino { get; set; } = string.Empty;
    public Associado? Gestor { get; set; }
    public Associado? Responsavel { get; set; }
    public ProjetoStatus? Status { get; set; }
    public Cliente? Cliente { get; set; }
    public List<ProjetosAssociadosModel> Associados { get; set; } = new();
    public string FotoNome { get; set; } = string.Empty;
    public int IdAvaliado { get; set; }
    public string Projeto { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public int IdPeriodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int IdGestor { get; set; }
    public int IdProjeto { get; set; }
    public string Pendencia { get; set; } = string.Empty;
    public string DataLimite { get; set; } = string.Empty;
    public string Respondente { get; set; } = string.Empty;
    public string RespondenteEmail { get; set; } = string.Empty;
}

public class ProjetosAssociadosModel
{
    public int Id { get; set; }
    public string IdEmail { get; set; } = string.Empty;
    public Projeto? Projeto { get; set; }
    public Associado? Associado { get; set; }
    public string DataInicio { get; set; } = string.Empty;
    public string DataTermino { get; set; } = string.Empty;
    public PeriodoAvaliacao? Periodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public Associado? Gestor { get; set; }
    public Associado? Avaliador { get; set; }
    public List<DisparosModel> Disparos { get; set; } = new();
}

public class DisparosModel
{
    public int IdPrazo { get; set; }
    public string NomeDisparo { get; set; } = string.Empty;
}

public class PendenciaModel
{
    public string Nome { get; set; } = string.Empty;
    public string Respondente { get; set; } = string.Empty;
    public string RespondenteEmail { get; set; } = string.Empty;
    public string Projeto { get; set; } = string.Empty;
    public string Pendencia { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public string DataLimite { get; set; } = string.Empty;
    public int IdProjeto { get; set; }
    public int IdAvaliado { get; set; }
    public int IdPeriodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int IdGestor { get; set; }
}

public class EnvioAvaliacaoRequest
{
    public int IdProjeto { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int IdGestor { get; set; }
    public int IdPrazo { get; set; }
}

public class EnvioAvaliacaoResult
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class FiltrosEnvioAvaliacao
{
    public int? IdPeriodo { get; set; }
    public int? IdProjeto { get; set; }
    public int? IdCliente { get; set; }
    public int? IdStatus { get; set; }
    public int? IdAssociado { get; set; }
    public int IdDisparo { get; set; }
}

public class Projeto
{
    public int IdProjeto { get; set; }
    public string Projeto { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int IdAssociadoGestor { get; set; }
    public int IdAssociadoResponsavel { get; set; }
    public int IdCliente { get; set; }
    public int IdEmpresa { get; set; }
    public bool ATV { get; set; }
    public Associado? AssociadoGestor { get; set; }
    public Associado? AssociadoResponsavel { get; set; }
}

public class ProjetoStatus
{
    public int IdStatus { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PeriodoAvaliacao
{
    public int IdPeriodo { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool ATV { get; set; }
}

public class Prazo
{
    public int IdPrazo { get; set; }
    public string NomeDisparo { get; set; } = string.Empty;
    public int DuracaoAutoAvaliacao { get; set; }
    public int DuracaoAvaliacaoAsCegas { get; set; }
    public int DuracaoAvaliacaoGestor { get; set; }
    public int DuracaoFeedback { get; set; }
    public int DuracaoMentor { get; set; }
    public int CompensadorAutoAvaliacao { get; set; }
    public int CompensadorAvaliacaoAsCegas { get; set; }
    public int CompensadorAvaliacaoGestor { get; set; }
    public int CompensadorFeedback { get; set; }
    public int CompensadorMentor { get; set; }
}

public class Avaliacao
{
    public int IdAvaliacao { get; set; }
    public int IdEmpresa { get; set; }
    public int IdProjeto { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public bool Liberado { get; set; }
    public int IdStatus { get; set; }
    public DateTime DHC { get; set; }
    public int USR { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int IdGestor { get; set; }
    public int IdPrazo { get; set; }
    public string PosicaoAtualFluxoAvaliacao { get; set; } = string.Empty;
    public DateTime? DataLiberacao { get; set; }
    public Associado? Associado { get; set; }
    public Projeto? Projeto { get; set; }
    public PeriodoAvaliacao? PeriodoAvaliacao { get; set; }
    public Prazo? Prazo { get; set; }
}