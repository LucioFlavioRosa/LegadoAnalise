using Microsoft.AspNetCore.Components;
using Services.PDI.Common;
using Services.PDI.Common.Models;
using Services.Common;

namespace Components.PDI;

public partial class AvaliacaoPDI : ComponentBase
{
    [Inject] public IPDIService PDIService { get; set; } = default!;
    [Inject] public IUserContextService UserContextService { get; set; } = default!;

    public List<PDIPeriodosModel> Periodos { get; set; } = new();
    public List<PDIPillsModel> Pills { get; set; } = new();
    public bool IsLoading { get; set; } = true;
    private int _idAssociado;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        var user = await UserContextService.GetUsuarioLogadoAsync();
        if (user == null)
        {
            IsLoading = false;
            return;
        }
        _idAssociado = user.Id;
        await CarregarPeriodos();
        IsLoading = false;
    }

    private async Task CarregarPeriodos()
    {
        Periodos = await PDIService.ObterPeriodosAsync(_idAssociado);
        Pills = Periodos.Select((p, i) => new PDIPillsModel
        {
            Id = $"tab-{i}-tab",
            Href = $"tab-{i}",
            Ariacontrols = $"tab-{i}",
            Ariaselected = i == Periodos.Count - 1 ? "true" : "false",
            Active = i == Periodos.Count - 1 ? "active" : string.Empty,
            Periodo = p.Periodo
        }).ToList();
        // Carregar colunas e respostas para cada período
        foreach (var periodo in Periodos)
        {
            var questoes = await PDIService.ObterQuestoesAsync(int.Parse(periodo.Id.Replace("tab-", "")));
            var colunas = questoes.GroupBy(q => q.ColunaPosicao)
                .Select(g => new PDIColunasModel
                {
                    PDIRespostas = g.Select(q => new PDIRespostasModel
                    {
                        Titulo = q.Titulo,
                        TituloStyle = string.IsNullOrWhiteSpace(q.Titulo) ? "white" : "#021240",
                        Subtitulo = q.Subtitulo,
                        SubtituloStyle = !string.IsNullOrWhiteSpace(q.Subtitulo) ? "style=\"align-self:center;background-color:#F2F2F2;font-weight:bolder;width:40%;min-width:80px;text-align:center;padding:2px\"" : string.Empty,
                        FlexGrow = q.ColunaTamanho.ToString().Replace(",", ".") + (q.FixTamanho > -1 ? $";min-height:{q.FixTamanho}%;max-height:{q.FixTamanho}%" : string.Empty),
                        Icone = string.IsNullOrWhiteSpace(q.Icone) ? "assets/images/icon/empty.png" : q.Icone,
                        BorderStyle = (q.BordaEsquerda ? string.Empty : "border-left:none;") +
                                      (q.BordaDireita ? string.Empty : "border-right:none;") +
                                      (q.BordaCima ? string.Empty : "border-top:none;") +
                                      (q.BordaBaixo ? string.Empty : "border-bottom:none;"),
                        Resposta = string.Empty,
                        RespostaEnabled = true,
                        IdPDIResposta = q.IdPDIQuestoes,
                        OnInput = string.Empty
                    }).ToList()
                }).ToList();
            periodo.PDIColunas = colunas;
        }
    }

    private async Task OnRespostaChanged(PDIRespostasModel resposta, ChangeEventArgs e)
    {
        if (resposta.RespostaEnabled)
        {
            resposta.Resposta = e.Value?.ToString() ?? string.Empty;
            await PDIService.AtualizarRespostaAsync(resposta.IdPDIResposta, resposta.Resposta);
        }
    }
}