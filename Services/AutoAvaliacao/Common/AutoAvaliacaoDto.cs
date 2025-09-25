using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public class AutoAvaliacaoDto
{
    public int IdProjeto { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public int IdAvaliacao { get; set; }
    public int IdGestor { get; set; }
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    
    // Informações do Avaliado
    public AssociadoInfoDto AssociadoInfo { get; set; } = new AssociadoInfoDto();
    public ProjetoInfoDto ProjetoInfo { get; set; } = new ProjetoInfoDto();
    public PeriodoInfoDto PeriodoInfo { get; set; } = new PeriodoInfoDto();
    
    // Cabeçalho de Descrições
    public CabecalhoDescricoesDto CabecalhoDescricoes { get; set; } = new CabecalhoDescricoesDto();
    
    // Lista de Competências para Avaliação
    public List<CompetenciaAvaliacaoDto> Competencias { get; set; } = new List<CompetenciaAvaliacaoDto>();
    
    // Status e Controle
    public string EtapaAtual { get; set; } = string.Empty;
    public string StatusAvaliacao { get; set; } = string.Empty;
    public bool PodeEditar { get; set; } = true;
    public bool PodeFinalizar { get; set; } = false;
    public bool TodosItensPreenchidos { get; set; } = false;
    public string TempoRestante { get; set; } = string.Empty;
    
    // Configurações de Exibição
    public bool ExibirDetalhamentoProximoNivel { get; set; } = true;
    public string LabelDetalhamentoProximoNivel { get; set; } = "Detalhamento Próximo Nível";
    public bool TrocaTitulosBackoffice { get; set; } = false;
    
    // Metadados
    public DateTime? DataInicioAvaliacao { get; set; }
    public DateTime? DataFimAvaliacao { get; set; }
    public DateTime DataCarregamento { get; set; } = DateTime.Now;
}

public class AssociadoInfoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
    public string TempoPeers { get; set; } = string.Empty;
    public int IdCargo { get; set; }
    public int IdNivel { get; set; }
    public int IdEmpresa { get; set; }
    public string Vertical { get; set; } = string.Empty;
}

public class ProjetoInfoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public int IdCliente { get; set; }
    public int IdEmpresa { get; set; }
}

public class PeriodoInfoDto
{
    public int Id { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}

public class CabecalhoDescricoesDto
{
    // Informações do Cargo Atual
    public string CargoAtual { get; set; } = string.Empty;
    public string FuncaoAtual { get; set; } = string.Empty;
    public string AutonomiaAtual { get; set; } = string.Empty;
    public string EscopoAtual { get; set; } = string.Empty;
    public string InterlocucaoAtual { get; set; } = string.Empty;
    
    // Informações do Próximo Cargo
    public string CargoProximo { get; set; } = string.Empty;
    public string FuncaoProximo { get; set; } = string.Empty;
    public string AutonomiaProximo { get; set; } = string.Empty;
    public string EscopoProximo { get; set; } = string.Empty;
    public string InterlocucaoProximo { get; set; } = string.Empty;
    
    // Competências Extensíveis
    public List<DescricaoCompetenciaDto> CompetenciasDescricoes { get; set; } = new List<DescricaoCompetenciaDto>();
}

public class DescricaoCompetenciaDto
{
    public string TituloExibicao { get; set; } = string.Empty;
    public string DescricaoCompetencia_Atual { get; set; } = string.Empty;
    public string DescricaoCompetencia_Proximo { get; set; } = string.Empty;
}

public class CompetenciaAvaliacaoDto
{
    public int IdCompetencia { get; set; }
    public int IdEixo { get; set; }
    public int IdSubCompetencia { get; set; }
    public int IdDimensao { get; set; }
    public int IdModo { get; set; }
    
    // Informações de Agrupamento
    public string Eixo { get; set; } = string.Empty;
    public string SubCompetencia { get; set; } = string.Empty;
    public string Dimensao { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    
    // Detalhamentos
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string DetalheProximoNivel { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public string CompetenciaProximo { get; set; } = string.Empty;
    
    // Notas e Respostas
    public int NotaNivel1 { get; set; } = 0;
    public int NotaNivel2 { get; set; } = 0;
    public string Consideracoes { get; set; } = string.Empty;
    
    // Configurações de Exibição e Interação
    public bool InputEtapaAtual { get; set; } = true;
    public bool VisivelEtapaAtual { get; set; } = true;
    public bool InputNivel1 { get; set; } = true;
    public bool InputNivel2 { get; set; } = true;
    public bool VisivelNivel1 { get; set; } = true;
    public bool VisivelNivel2 { get; set; } = true;
    
    // Estados de Controle
    public bool DisableSelectNivel1 { get; set; } = false;
    public bool DisableSelectNivel2 { get; set; } = false;
    public bool HiddenSelectNivel1 { get; set; } = false;
    public bool HiddenSelectNivel2 { get; set; } = false;
    public bool HiddenTextBoxNivel1 { get; set; } = true;
    public bool HiddenTextBoxNivel2 { get; set; } = true;
    
    // Textos para Exibição (quando não editável)
    public string TextoNotaNivel1 { get; set; } = string.Empty;
    public string TextoNotaNivel2 { get; set; } = string.Empty;
    
    // Controle de Visibilidade
    public bool IsHidden { get; set; } = false;
    public string IsHiddenDetalhamentoProximoNivel { get; set; } = string.Empty;
    
    // Lista de Opções para Combos
    public List<ComboItem> OpcoesNotaNivel1 { get; set; } = new List<ComboItem>();
    public List<ComboItem> OpcoesNotaNivel2 { get; set; } = new List<ComboItem>();
    
    // Validação
    public bool IsValid { get; set; } = true;
    public List<string> ValidationErrors { get; set; } = new List<string>();
    
    // Metadados
    public bool JaFinalizada { get; set; } = false;
    public DateTime? DataUltimaAlteracao { get; set; }
}