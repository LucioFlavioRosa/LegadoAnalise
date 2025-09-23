using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Peers.Moderno.Models;
using Peers.Moderno.Services;

namespace Peers.Moderno.Pages;

public partial class Competencias
{
    private Competencia competenciaModel = new();
    private List<Competencia> competencias = new();
    private List<Cargo> cargos = new();
    private List<Eixo> eixosDesempenho = new();
    private List<Eixo> eixosLideranca = new();
    private List<SubCompetencia> subCompetencias = new();
    private List<Dimensao> dimensoesDesempenho = new();
    private List<AvaliacaoCompetenciaNota> notasPadrao = new();
    private List<ModoCalculoCompetencia> modosCalculo = new();
    private string tipoAvaliacaoSelecionado = string.Empty;
    private string relacaoCargoSubcompetencia = string.Empty;
    private string mensagem = string.Empty;
    private string tipoMensagem = string.Empty;
    private InputFile? fileInputRef;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        competencias = await CompetenciasService.ObterListaCompetenciasAsync(true);
        cargos = await CargosService.ObterListaCargosAsync(true);
        eixosDesempenho = await EixosService.ObterListaEixosAsync(true, "desempenho");
        eixosLideranca = await EixosService.ObterListaEixosAsync(true, "lideranca");
        subCompetencias = await SubCompetenciasService.ObterListaSubCompetenciasAsync(true);
        dimensoesDesempenho = await DimensoesService.ObterListaDimensoesAsync(true, "desempenho");
        notasPadrao = await AvaliacoesService.ObterAvaliacaoCompetenciasNotasAsync();
        modosCalculo = await AvaliacoesService.ObterModosCalculosCompetenciasAsync();
    }

    private async Task OnTipoAvaliacaoChanged(ChangeEventArgs e)
    {
        tipoAvaliacaoSelecionado = e.Value?.ToString() ?? string.Empty;
        LimparFormulario();
        StateHasChanged();
    }

    private async Task OnCargoSubCompetenciaChanged(int idCargo, int idSubCompetencia)
    {
        if (idCargo > 0 && idSubCompetencia > 0)
        {
            var relacao = await CargosService.ObterRelacaoCargoSubcompetenciaAsync(idCargo, idSubCompetencia);
            relacaoCargoSubcompetencia = relacao?.Descricao ?? string.Empty;
            StateHasChanged();
        }
    }

    private async Task SalvarCompetencia()
    {
        try
        {
            competenciaModel.TipoAvaliacao = tipoAvaliacaoSelecionado;
            competenciaModel.DHC = DateTime.Now;
            competenciaModel.ATV = 1;

            if (tipoAvaliacaoSelecionado == "desempenho")
            {
                // Salvar relação cargo x subcompetência
                if (!string.IsNullOrEmpty(relacaoCargoSubcompetencia))
                {
                    var relacao = new RelacaoCargoSubcompetencia
                    {
                        IdCargo = competenciaModel.IdCargo,
                        IdSubcompetencia = competenciaModel.IdSubCompetencia,
                        Descricao = relacaoCargoSubcompetencia
                    };
                    await CargosService.AtualizarRelacaoCargoSubcompetenciaAsync(relacao);
                }
            }
            else if (tipoAvaliacaoSelecionado == "lideranca")
            {
                competenciaModel.IdCargo = 96;
                competenciaModel.IdDimensao = 10;
                competenciaModel.IdNivel = 1;
            }

            bool sucesso;
            if (competenciaModel.IdCompetencia == 0)
            {
                sucesso = await CompetenciasService.InserirCompetenciaAsync(competenciaModel);
                mensagem = sucesso ? "Competência inserida com sucesso!" : "Falha na inserção da competência!";
            }
            else
            {
                sucesso = await CompetenciasService.AlterarCompetenciaAsync(competenciaModel);
                mensagem = sucesso ? "Competência alterada com sucesso!" : "Falha na alteração da competência!";
            }

            tipoMensagem = sucesso ? "success" : "error";

            if (sucesso)
            {
                LimparFormulario();
                await CarregarDados();
            }
        }
        catch (Exception ex)
        {
            mensagem = $"Erro ao salvar competência: {ex.Message}";
            tipoMensagem = "error";
        }

        StateHasChanged();
    }

    private void LimparFormulario()
    {
        competenciaModel = new Competencia
        {
            InputAutoAvaliacao = true,
            InputAvaliacaoAsCegas = true,
            InputAvaliacaoGestor = true,
            InputFeedback = true,
            InputNivel1 = true,
            InputNivel2 = true,
            VisivelAutoAvaliacao = true,
            VisivelAvaliacaoAsCegas = true,
            VisivelAvaliacaoGestor = true,
            VisivelFeedback = true,
            VisivelNivel1 = true,
            VisivelNivel2 = true
        };
        relacaoCargoSubcompetencia = string.Empty;
        tipoAvaliacaoSelecionado = string.Empty;
    }

    private async Task EditarCompetencia(int id)
    {
        var competencia = await CompetenciasService.ObterCompetenciaAsync(id);
        if (competencia != null)
        {
            competenciaModel = competencia;
            tipoAvaliacaoSelecionado = competencia.TipoAvaliacao;
            
            if (competencia.TipoAvaliacao == "desempenho")
            {
                await OnCargoSubCompetenciaChanged(competencia.IdCargo, competencia.IdSubCompetencia);
            }
            
            StateHasChanged();
        }
    }

    private async Task InativarCompetencia(int id)
    {
        var sucesso = await CompetenciasService.ExcluirCompetenciaAsync(id);
        mensagem = sucesso ? "Competência inativada com sucesso!" : "Erro ao inativar competência!";
        tipoMensagem = sucesso ? "success" : "error";
        
        if (sucesso)
        {
            await CarregarDados();
        }
        
        StateHasChanged();
    }

    private async Task ExportarCompetencias()
    {
        try
        {
            var competenciasExport = await CompetenciasService.ObterCompetenciasParaExportAsync();
            var fileName = $"Competências_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var fileBytes = ExportFileService.GenerateExcelCompetencias(fileName, competenciasExport);
            
            await JSRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(fileBytes));
        }
        catch (Exception ex)
        {
            mensagem = $"Erro ao exportar: {ex.Message}";
            tipoMensagem = "error";
            StateHasChanged();
        }
    }

    private async Task ImportarCompetencias(InputFileChangeEventArgs e)
    {
        // Implementação da importação seria similar ao código original
        // mas adaptada para usar streams e o novo modelo de dados
        mensagem = "Funcionalidade de importação em desenvolvimento";
        tipoMensagem = "warning";
        StateHasChanged();
    }

    private async Task AgregarCompetencia()
    {
        // Implementação da agregação seria similar ao código original
        mensagem = "Funcionalidade de agregação em desenvolvimento";
        tipoMensagem = "warning";
        StateHasChanged();
    }
}