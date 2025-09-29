using Microsoft.AspNetCore.Components;
using Services.Resultados;
using Services.Common;
using Peers.Moderno.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Components.Pages
{
    public partial class Resultado : ComponentBase
    {
        [Inject] public IResultadoService ResultadoService { get; set; }
        [Inject] public ComboHelper ComboHelper { get; set; }
        [Inject] public IMessageBoxService MessageBoxService { get; set; }
        [Inject] public ITelemetryService TelemetryService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected List<ComboItem> PeriodosCombo { get; set; } = new();
        protected List<ComboItem> ProjetosCombo { get; set; } = new();
        protected List<ComboItem> AssociadosCombo { get; set; } = new();
        protected ResultadoFiltroModel Filtro { get; set; } = new();
        protected List<ResultadoListItemModel> Resultados { get; set; } = new();

        protected string PeriodoExportDesempenho { get; set; } = "";
        protected string PeriodoExportMentoria { get; set; } = "";

        protected bool PodeLiberarLideranca { get; set; } = false;
        protected bool PodeLiberarMentoria { get; set; } = false;
        protected string TextoBotaoLideranca { get; set; } = string.Empty;
        protected string TextoBotaoMentoria { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await CarregarCombosAsync();
            await AtualizarEstadoBotoesLiberacaoAsync();
        }

        protected async Task CarregarCombosAsync()
        {
            PeriodosCombo = await ComboHelper.GetPeriodosResultadoComboAsync(ResultadoService.GetDbContext(), ResultadoService.GetEmpresaId());
            ProjetosCombo = await ComboHelper.GetProjetosResultadoComboAsync(ResultadoService.GetDbContext());
            AssociadosCombo = await ComboHelper.GetAssociadosResultadoComboAsync(ResultadoService.GetDbContext(), true);
            PeriodoExportDesempenho = PeriodosCombo.FirstOrDefault()?.Value ?? "";
            PeriodoExportMentoria = PeriodosCombo.FirstOrDefault()?.Value ?? "";
        }

        protected async Task BuscarResultados()
        {
            try
            {
                var filtro = new ResultadoFiltroModel
                {
                    PeriodoId = Filtro.PeriodoId,
                    ProjetoId = Filtro.ProjetoId,
                    AssociadoId = Filtro.AssociadoId
                };
                Resultados = await ResultadoService.ListarResultadosAsync(filtro);
                TelemetryService.TrackEvent("BuscarResultados", new Dictionary<string, string>
                {
                    { "PeriodoId", filtro.PeriodoId ?? "" },
                    { "ProjetoId", filtro.ProjetoId ?? "" },
                    { "AssociadoId", filtro.AssociadoId ?? "" },
                    { "Count", Resultados.Count.ToString() }
                });
            }
            catch (System.Exception ex)
            {
                MessageBoxService.ShowError("Erro ao buscar resultados de avaliações.");
                TelemetryService.TrackException(ex, new Dictionary<string, string> { { "Action", "BuscarResultados" } });
            }
        }

        protected async Task ExportarLideranca()
        {
            try
            {
                var exportResult = await ResultadoService.ExportarResultadosLiderancaAsync();
                if (exportResult.Sucesso)
                {
                    MessageBoxService.ShowSuccess("Arquivo exportado com sucesso.");
                    NavigationManager.NavigateTo(exportResult.UrlDownload, true);
                }
                else
                {
                    MessageBoxService.ShowWarning(exportResult.Mensagem ?? "Falha ao exportar resultados de liderança.");
                }
                TelemetryService.TrackEvent("ExportarResultadosLideranca");
            }
            catch (System.Exception ex)
            {
                MessageBoxService.ShowError("Erro ao exportar resultados de liderança.");
                TelemetryService.TrackException(ex, new Dictionary<string, string> { { "Action", "ExportarLideranca" } });
            }
        }

        protected async Task ExportarDesempenho()
        {
            if (string.IsNullOrEmpty(PeriodoExportDesempenho))
            {
                MessageBoxService.ShowWarning("Selecione o período.");
                return;
            }
            try
            {
                var exportResult = await ResultadoService.ExportarResultadosDesempenhoAsync(PeriodoExportDesempenho);
                if (exportResult.Sucesso)
                {
                    MessageBoxService.ShowSuccess("Arquivo exportado com sucesso.");
                    NavigationManager.NavigateTo(exportResult.UrlDownload, true);
                }
                else
                {
                    MessageBoxService.ShowWarning(exportResult.Mensagem ?? "Falha ao exportar resultados de desempenho.");
                }
                TelemetryService.TrackEvent("ExportarResultadosDesempenho", new Dictionary<string, string> { { "PeriodoId", PeriodoExportDesempenho } });
            }
            catch (System.Exception ex)
            {
                MessageBoxService.ShowError("Erro ao exportar resultados de desempenho.");
                TelemetryService.TrackException(ex, new Dictionary<string, string> { { "Action", "ExportarDesempenho" } });
            }
        }

        protected async Task ExportarMentoria()
        {
            if (string.IsNullOrEmpty(PeriodoExportMentoria))
            {
                MessageBoxService.ShowWarning("Selecione o período.");
                return;
            }
            try
            {
                var exportResult = await ResultadoService.ExportarResultadosMentoriaAsync(PeriodoExportMentoria);
                if (exportResult.Sucesso)
                {
                    MessageBoxService.ShowSuccess("Relatório exportado com sucesso.");
                    NavigationManager.NavigateTo(exportResult.UrlDownload, true);
                }
                else
                {
                    MessageBoxService.ShowWarning(exportResult.Mensagem ?? "Falha ao exportar resultados de mentorado.");
                }
                TelemetryService.TrackEvent("ExportarResultadosMentoria", new Dictionary<string, string> { { "PeriodoId", PeriodoExportMentoria } });
            }
            catch (System.Exception ex)
            {
                MessageBoxService.ShowError("Erro ao exportar resultados de mentorado.");
                TelemetryService.TrackException(ex, new Dictionary<string, string> { { "Action", "ExportarMentoria" } });
            }
        }

        protected async Task AtualizarEstadoBotoesLiberacaoAsync()
        {
            var lideranca = await ResultadoService.ObterLiberacaoLiderancaAsync();
            PodeLiberarLideranca = lideranca.PodeLiberar;
            TextoBotaoLideranca = lideranca.TextoBotao;
            var mentoria = await ResultadoService.ObterLiberacaoMentoriaAsync();
            PodeLiberarMentoria = mentoria.PodeLiberar;
            TextoBotaoMentoria = mentoria.TextoBotao;
        }

        protected async Task LiberarLideranca()
        {
            try
            {
                var result = await ResultadoService.LiberarLiderancaAsync();
                if (result.Sucesso)
                {
                    MessageBoxService.ShowSuccess("Liderança liberada com sucesso.");
                    PodeLiberarLideranca = false;
                }
                else
                {
                    MessageBoxService.ShowWarning(result.Mensagem ?? "Falha ao liberar liderança.");
                }
                TelemetryService.TrackEvent("LiberarLideranca");
            }
            catch (System.Exception ex)
            {
                MessageBoxService.ShowError("Erro ao liberar liderança.");
                TelemetryService.TrackException(ex, new Dictionary<string, string> { { "Action", "LiberarLideranca" } });
            }
        }

        protected async Task LiberarMentoria()
        {
            try
            {
                var result = await ResultadoService.LiberarMentoriaAsync();
                if (result.Sucesso)
                {
                    MessageBoxService.ShowSuccess("Mentoria liberada com sucesso.");
                    PodeLiberarMentoria = false;
                }
                else
                {
                    MessageBoxService.ShowWarning(result.Mensagem ?? "Falha ao liberar mentoria.");
                }
                TelemetryService.TrackEvent("LiberarMentoria");
            }
            catch (System.Exception ex)
            {
                MessageBoxService.ShowError("Erro ao liberar mentoria.");
                TelemetryService.TrackException(ex, new Dictionary<string, string> { { "Action", "LiberarMentoria" } });
            }
        }
    }

    public class ResultadoFiltroModel
    {
        public string? PeriodoId { get; set; }
        public string? ProjetoId { get; set; }
        public string? AssociadoId { get; set; }
    }

    public class ResultadoListItemModel
    {
        public string Periodo { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string Mentor { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public int QtdProjetos { get; set; }
        public int IdAssociado { get; set; }
        public int IdPeriodo { get; set; }
    }
}
