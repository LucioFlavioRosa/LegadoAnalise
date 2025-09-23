using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardModel> ObterNumerosAsync(int idPeriodo);
    Task<DashboardModel> ObterAndamentoAvaliacoesAsync(int idPeriodo);
    Task<List<DashboardModel>> ObterAvaliadosPorPeriodoAsync();
    Task<List<DashboardModel>> ObterAvaliadosPorProjetoAsync(int idPeriodo);
    Task<ResultadoProjetosModel> ObterRadarConsolidadoCompetenciaAsync(int idPeriodo);
    Task<List<EvolucaoPerformanceModel>> ObterChartConsolidadoPerformanceAsync(int idPeriodo);
    Task<List<Periodo>> ObterPeriodosAsync(int idEmpresa);
    Task<Periodo> ObterPeriodoAtualAsync();
    Task<Periodo> ObterPeriodoAsync(int idPeriodo);
}