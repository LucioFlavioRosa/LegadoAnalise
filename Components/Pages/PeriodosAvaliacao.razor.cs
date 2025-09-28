using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Models;
using Services.Periodos.Common;

namespace Components.Pages;

public partial class PeriodosAvaliacao : ComponentBase
{
    [Inject] public Services.Periodos.PeriodoService PeriodoService { get; set; } = default!;
    [Inject] public IAvaliacoesSinalizadasService AvaliacoesSinalizadasService { get; set; } = default!;
    [Inject] public Services.Common.ComboHelper ComboHelper { get; set; } = default!;
    [Inject] public Services.Periodos.Common.PeriodoValidator PeriodoValidator { get; set; } = default!;
    [Inject] public Services.Periodos.Common.PeriodoFormatHelper PeriodoFormatHelper { get; set; } = default!;
    [Inject] public Services.Common.IMessageBoxService MessageBoxService { get; set; } = default!;

    protected List<Peers.Moderno.Models.PERIODOSAVALIACOES> periodos = new();
    protected List<AvaliacaoSinalizadaDto> avaliacoesSinalizadas = new();
    protected List<Services.Common.ComboItem> empresas = new();
    protected List<Services.Common.ComboItem> statusList = new();
    protected Peers.Moderno.Models.PERIODOSAVALIACOES periodoModel = new Peers.Moderno.Models.PERIODOSAVALIACOES();

    protected override async Task OnInitializedAsync()
    {
        await CarregarCombosAsync();
        await CarregarPeriodosAsync();
        await CarregarAvaliacoesSinalizadasAsync();
        LimparFormulario();
    }

    private async Task CarregarCombosAsync()
    {
        empresas = await ComboHelper.GetPeriodosComboAsync(null); // Ajuste para buscar empresas se necessário
        statusList = Services.Common.ComboHelper.GetStatusItems();
    }

    private async Task CarregarPeriodosAsync()
    {
        periodos = await PeriodoService.ListarTodosAsync();
    }

    private async Task CarregarAvaliacoesSinalizadasAsync()
    {
        avaliacoesSinalizadas = await AvaliacoesSinalizadasService.ObterAvaliacoesSinalizadasAsync();
    }

    protected async Task HandleValidSubmit()
    {
        var validation = await PeriodoValidator.ValidarAsync(periodoModel);
        if (!validation.IsValid)
        {
            MessageBoxService.ShowWarning(validation.Message);
            return;
        }

        if (periodoModel.IdPeriodo == 0)
        {
            await PeriodoService.InserirAsync(periodoModel);
            var ultimoPeriodo = await PeriodoService.ObterUltimoPeriodoAsync();
            if (ultimoPeriodo != null)
            {
                await AvaliacoesSinalizadasService.AtualizarAvaliacoesParaUltimoPeriodoAsync(ultimoPeriodo.IdPeriodo);
            }
            MessageBoxService.ShowSuccess("Período inserido com sucesso!");
        }
        else
        {
            await PeriodoService.AlterarAsync(periodoModel);
            MessageBoxService.ShowSuccess("Período alterado com sucesso!");
        }

        await CarregarPeriodosAsync();
        await CarregarAvaliacoesSinalizadasAsync();
        LimparFormulario();
    }

    protected void EditarPeriodo(Peers.Moderno.Models.PERIODOSAVALIACOES periodo)
    {
        periodoModel = new Peers.Moderno.Models.PERIODOSAVALIACOES
        {
            IdPeriodo = periodo.IdPeriodo,
            IdEmpresa = periodo.IdEmpresa,
            Codigo = periodo.Codigo,
            Periodo = periodo.Periodo,
            DataInicio = periodo.DataInicio,
            DataFim = periodo.DataFim,
            ATV = periodo.ATV
        };
    }

    protected void LimparFormulario()
    {
        periodoModel = new Peers.Moderno.Models.PERIODOSAVALIACOES
        {
            ATV = 1
        };
    }
}
