using Microsoft.AspNetCore.Components;
using Services.Resultados;
using Services.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class AvalizacaoResultado : ComponentBase
{
    [Inject] public IResultadoService ResultadoService { get; set; } = default!;
    [Inject] public IMessageBoxService MessageBoxService { get; set; } = default!;
    [Inject] public IFormatHelper FormatHelper { get; set; } = default!;
    [Inject] public IComboHelper ComboHelper { get; set; } = default!;
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    // Dados do Associado
    public string FotoAssociado { get; set; } = "assets/images/users/usernophoto.jpg";
    public string AssociadoNome { get; set; } = "";
    public string PeriodoNome { get; set; } = "";
    public string CargoNome { get; set; } = "";
    public string ProximoCargoNome { get; set; } = "";
    public string TempoCargo { get; set; } = "";
    public string TempoPeers { get; set; } = "";
    public string MentorNome { get; set; } = "";
    public string VerticalNome { get; set; } = "";
    public string ElegivelPromocao { get; set; } = "";
    public string InputPromocao { get; set; } = "";
    public string ProjetosEnvolvidos { get; set; } = "";
    public string Trajetoria { get; set; } = "";
    public string PontosFortes { get; set; } = "";
    public string PontosFracos { get; set; } = "";
    public bool MentoriaRealizada { get; set; } = false;

    // Dados de Resultado
    public List<ResultadoProjetosModel> ResultadosProjetos { get; set; } = new();
    public List<ResultadoSomaProjetosModel> ResultadosSomaProjetos { get; set; } = new();

    // Timer
    private bool TimerRunning = false;
    private int hour = 0, minute = 0, second = 0, count = 0;
    public string HrString => hour < 10 ? $"0{hour}" : hour.ToString();
    public string MinString => minute < 10 ? $"0{minute}" : minute.ToString();
    public string SecString => second < 10 ? $"0{second}" : second.ToString();
    public string CountString => count < 10 ? $"0{count}" : count.ToString();

    // Tab Control
    public string ActiveTab { get; set; } = "competencia";

    protected override async Task OnInitializedAsync()
    {
        await CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        try
        {
            // Recupera parâmetros da URL
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            int idAssociado = int.TryParse(query.Get("IdAssociado"), out var tmpIdA) ? tmpIdA : 0;
            int idPeriodo = int.TryParse(query.Get("IdPeriodo"), out var tmpIdP) ? tmpIdP : 0;
            string tipoAvaliacao = query.Get("TipoAvaliacao") ?? "desempenho";
            string escopo = query.Get("Escopo") ?? "projeto";

            var resultadoAssociado = await ResultadoService.ObterResultadoAssociadoAsync(idAssociado, idPeriodo, tipoAvaliacao, escopo);
            ResultadosProjetos = resultadoAssociado;
            ResultadosSomaProjetos = await ResultadoService.ObterSomaProjetosAsync(resultadoAssociado);

            var dadosAssociado = await ResultadoService.ObterDadosAssociadoAsync(idAssociado, idPeriodo, tipoAvaliacao, escopo);
            if (dadosAssociado != null)
            {
                FotoAssociado = string.IsNullOrWhiteSpace(dadosAssociado.Foto) ? FotoAssociado : dadosAssociado.Foto;
                AssociadoNome = dadosAssociado.Nome;
                PeriodoNome = dadosAssociado.Periodo;
                CargoNome = dadosAssociado.Cargo;
                ProximoCargoNome = dadosAssociado.ProximoCargo;
                TempoCargo = dadosAssociado.TempoCargo;
                TempoPeers = dadosAssociado.TempoPeers;
                MentorNome = dadosAssociado.Mentor;
                VerticalNome = dadosAssociado.Vertical;
                ElegivelPromocao = dadosAssociado.ElegivelPromocao;
                InputPromocao = dadosAssociado.InputPromocao;
                ProjetosEnvolvidos = dadosAssociado.ProjetosEnvolvidos;
                Trajetoria = dadosAssociado.Trajetoria;
                PontosFortes = dadosAssociado.PontosFortes;
                PontosFracos = dadosAssociado.PontosFracos;
                MentoriaRealizada = dadosAssociado.MentoriaRealizada;
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"Erro ao carregar dados: {ex.Message}");
        }
    }

    public void ChangeTab(string tab)
    {
        ActiveTab = tab;
    }

    public void StartTimer()
    {
        TimerRunning = true;
        _ = RunTimerAsync();
    }

    public void StopTimer()
    {
        TimerRunning = false;
    }

    public void ResetTimer()
    {
        TimerRunning = false;
        hour = 0; minute = 0; second = 0; count = 0;
    }

    private async Task RunTimerAsync()
    {
        while (TimerRunning)
        {
            await Task.Delay(10);
            count++;
            if (count == 100)
            {
                second++;
                count = 0;
            }
            if (second == 60)
            {
                minute++;
                second = 0;
            }
            if (minute == 60)
            {
                hour++;
                minute = 0;
                second = 0;
            }
            StateHasChanged();
        }
    }
}
