using Peers.Moderno.Services.AutoAvaliacao.Common;

namespace Peers.Moderno.Services.AutoAvaliacao.Common;

public interface IAutoAvaliacaoValidator
{
    Task<AutoAvaliacaoValidationResult> ValidarAvaliacaoCompletaAsync(AutoAvaliacaoDto avaliacao);
    Task<AutoAvaliacaoValidationResult> ValidarCompetenciaAsync(CompetenciaAvaliacaoDto competencia);
    Task<AutoAvaliacaoValidationResult> ValidarConsistenciaNiveisAsync(CompetenciaAvaliacaoDto competencia);
    Task<AutoAvaliacaoValidationResult> ValidarPilaresPreenchidosAsync(List<CompetenciaAvaliacaoDto> competencias);
    Task<AutoAvaliacaoValidationResult> ValidarNotasObrigatoriasAsync(List<CompetenciaAvaliacaoDto> competencias);
    Task<AutoAvaliacaoValidationResult> ValidarRegrasNegocioAsync(AutoAvaliacaoDto avaliacao);
    Task<AutoAvaliacaoValidationResult> ValidarPermissaoEdicaoAsync(AutoAvaliacaoDto avaliacao);
    Task<AutoAvaliacaoValidationResult> ValidarEtapaAvaliacaoAsync(string etapaAtual, string operacao);
    Task<bool> PodeFinalizarAvaliacaoAsync(AutoAvaliacaoDto avaliacao);
    Task<List<CompetenciaValidationError>> ValidarCompetenciasIndividualmenteAsync(List<CompetenciaAvaliacaoDto> competencias);
    Task<AutoAvaliacaoValidationResult> ValidarNotaNaoSeAplicaAsync(CompetenciaAvaliacaoDto competencia);
    Task<AutoAvaliacaoValidationResult> ValidarHieraquiaNotasAsync(CompetenciaAvaliacaoDto competencia);
}

public static class ValidationConstants
{
    public const int NOTA_NAO_SE_APLICA = 5;
    public const int NOTA_SELECIONAR = 0;
    
    public static readonly string[] ETAPAS_PERMITIDAS_EDICAO = 
    {
        "nao_iniciada",
        "auto_avaliacao", 
        "avaliacao_cegas",
        "em_paralelo"
    };
    
    public static readonly string[] OPERACOES_VALIDACAO = 
    {
        "salvar",
        "finalizar",
        "navegar"
    };
    
    public static readonly Dictionary<string, string> MENSAGENS_VALIDACAO = new Dictionary<string, string>
    {
        { "NOTAS_OBRIGATORIAS", "É obrigatório selecionar uma nota para cada nível." },
        { "INCONSISTENCIA_NAO_SE_APLICA", "Quando a nota de Competência do Nível Atual for = Não Se Aplica, o Próximo Nível deve ser Não Se Aplica." },
        { "HIERARQUIA_NOTAS", "A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual)." },
        { "PILAR_VAZIO", "Cada pilar precisa receber ao mínimo 1 nota mensurável em cada nível (diferente de não se aplica)." },
        { "COMPETENCIAS_NAO_PREENCHIDAS", "É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação." },
        { "ETAPA_NAO_PERMITIDA", "Não é possível editar a avaliação na etapa atual." },
        { "PERMISSAO_NEGADA", "Você não tem permissão para esta operação." }
    };
}