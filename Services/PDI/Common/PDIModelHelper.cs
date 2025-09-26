using Peers.Moderno.Services.PDI.Common.Models;

namespace Peers.Moderno.Services.PDI.Common;

public static class PDIModelHelper
{
    public static PDIRespostasModel MapToPDIRespostasModel(PDI_QUESTOES questao, PDI_RESPOSTAS? resposta, PERIODOSAVALIACOES periodo, PERIODOSAVALIACOES? periodoUltimo, bool isUltimoPeriodo)
    {
        string corAzul = "#021240";
        var subtituloStyle = "style=\"align-self:center;background-color:#F2F2F2;font-weight:bolder;width:40%;min-width:80px;text-align:center;padding:2px\"";
        string styleBordas = string.Empty;
        if (questao.BordaEsquerda == false) styleBordas += "border-left:none;";
        if (questao.BordaDireita == false) styleBordas += "border-right:none;";
        if (questao.BordaCima == false) styleBordas += "border-top:none;";
        if (questao.BordaBaixo == false) styleBordas += "border-bottom:none;";
        var flexGrow = questao.ColunaTamanho.ToString().Replace(",", ".");
        if (questao.FixTamanho > -1)
            flexGrow += $";min-height:{questao.FixTamanho}%;max-height:{questao.FixTamanho}%";
        var respostaText = resposta?.Resposta ?? "";
        respostaText = respostaText.Replace("\n", "fsdfsdfs").Replace("\r", "fsdfsdfs");
        return new PDIRespostasModel
        {
            Titulo = !string.IsNullOrEmpty(questao.Titulo) ? questao.Titulo : "TITULO",
            TituloStyle = string.IsNullOrEmpty(questao.Titulo) ? "white" : corAzul,
            Subtitulo = questao.Subtitulo,
            SubtituloStyle = !string.IsNullOrEmpty(questao.Subtitulo) ? subtituloStyle : string.Empty,
            FlexGrow = flexGrow,
            Icone = !string.IsNullOrEmpty(questao.Icone) ? questao.Icone : "assets/images/icon/empty.png",
            BorderStyle = styleBordas,
            Resposta = respostaText,
            RespostaEnabled = periodo.IdPeriodo == periodoUltimo?.IdPeriodo,
            idPDIResposta = resposta?.idPDIRespostas ?? 0,
            OnInput = isUltimoPeriodo && resposta != null ? $"action_AtualizaResposta('{resposta.idPDIRespostas}', this); return false; this.focus();" : string.Empty
        };
    }
}