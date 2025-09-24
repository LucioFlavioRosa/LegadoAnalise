namespace Peers.Moderno.Services.Common.Avaliacoes;

public interface ITotalAvaliacaoService
{
    Task<int> ObterTotalAvaliacoesAsync();
    Task<int> ObterTotalAvaliacoesPorTipoAsync(string tipoAvaliacao);
    Task<int> ObterTotalAvaliacoesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<int> ObterTotalAvaliacoesPorAssociadoAsync(int idAssociado);
    Task<Dictionary<string, int>> ObterEstatisticasAvaliacoesAsync();
    Task<bool> ExistemAvaliacoesAsync();
}