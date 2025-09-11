using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using AjaxControlToolkit;
using SistemaAvaliacao.Scripts;
using System.Linq;
using Microsoft.Ajax.Utilities;
using System.Windows.Forms.Design;

namespace SistemaAvaliacao
{
    public partial class avaliacao_PDI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var periodoUltimo = new PeriodoService().ObterPeriodoUltimo();
                ValidaPDIRespostas(WebStorage.GetUsuarioLogado().Id, periodoUltimo.IdPeriodo);
                CarregaPillsPeriodos(WebStorage.GetUsuarioLogado().Id);
                CarregaPDIRespostas(WebStorage.GetUsuarioLogado().Id);
            }
        }

        public void CarregaPDIRespostas(int idAssociado)
        {
            var pdiService = new PDIService();
            var periodoUltimo = new PeriodoService().ObterPeriodoUltimo();
            var pdiPeriodosModel = new List<PDIPeriodosModel>();
            string corAzul = "#021240";

            var pdiRespostas = pdiService.ObterPDIRespostas(idAssociado:idAssociado);
            var pdiPeriodos = pdiRespostas.Select(x => x.PERIODOSAVALIACOES).Distinct().ToList();

            for (var j = 0; j < pdiPeriodos.Count; j++)
            {
                var getPeriodo = pdiPeriodos[j];

                var addPeriodo = new PDIPeriodosModel();
                addPeriodo.id = "tab-" + j.ToString();
                addPeriodo.active = j == pdiPeriodos.Count - 1 ? "active in show" : "";
                addPeriodo.arialabelled = "tab-" + j.ToString() + "-tab";
                addPeriodo.Periodo = getPeriodo.Periodo;

                var pdiColunasModel = new List<PDIColunasModel>();
                var pdiQuestoes = pdiService.ObterPDIQuestoes();

                var pdiPeriodoRespostas = pdiRespostas.Where(x => x.idPeriodo == getPeriodo.IdPeriodo);
                if (getPeriodo.IdPeriodo != periodoUltimo.IdPeriodo)
                {
                    pdiQuestoes = pdiPeriodoRespostas.Select(x => x.PDI_QUESTOES).Distinct().ToList();
                }

                var lastColuna = pdiQuestoes.Select(x => x.ColunaPosicao).Max();
                for (var i = 1; i <= lastColuna; i++)
                {
                    var pdiColuna = pdiQuestoes.Where(x => x.ColunaPosicao == i).ToList();

                    var addColuna = new PDIColunasModel();
                    addColuna.PDIRespostas = new List<PDIRespostasModel>();

                    for (var z = 0; z < pdiColuna.Count; z++)
                    {
                        var getQuestao = pdiColuna[z];

                        var addRespostas = new PDIRespostasModel();
                        addRespostas.Titulo = getQuestao.Titulo != "" ? getQuestao.Titulo : "TITULO";
                        addRespostas.TituloStyle = getQuestao.Titulo == "" ? "white" : corAzul;
                        addRespostas.Subtitulo = getQuestao.Subtitulo;
                        var subtituloStyle = "style=\"align-self:center;background-color:#F2F2F2;font-weight:bolder;width:40%;min-width:80px;text-align:center;padding:2px\"";
                        addRespostas.SubtituloStyle = getQuestao.Subtitulo != "" ? subtituloStyle : "";
                        addRespostas.FlexGrow = getQuestao.ColunaTamanho.ToString().Replace(",", ".");
                        addRespostas.FlexGrow += getQuestao.FixTamanho > -1 ? ";min-height:" + getQuestao.FixTamanho.ToString() + "%;max-height:" + getQuestao.FixTamanho.ToString() + "%" : "";
                        addRespostas.Icone = getQuestao.Icone != "" ? getQuestao.Icone : "assets/images/icon/empty.png";

                        string styleBordas = "";
                        styleBordas += getQuestao.BordaEsquerda == false ? "border-left:none;" : "";
                        styleBordas += getQuestao.BordaDireita == false ? "border-right:none;" : "";
                        styleBordas += getQuestao.BordaCima == false ? "border-top:none;" : "";
                        styleBordas += getQuestao.BordaBaixo == false ? "border-bottom:none;" : "";
                        addRespostas.BorderStyle = styleBordas;

                        var getResposta = pdiPeriodoRespostas.Where(x => x.idPDIQuestao == getQuestao.idPDIQuestoes).ToList()[0];
                        addRespostas.Resposta = getResposta.Resposta.Replace(Convert.ToChar(10).ToString(), "fsdfsdfs");
                        addRespostas.Resposta = addRespostas.Resposta.Replace(Convert.ToChar(13).ToString(), "fsdfsdfs");
                        addRespostas.RespostaEnabled = getPeriodo.IdPeriodo == periodoUltimo.IdPeriodo;
                        addRespostas.idPDIResposta = getResposta.idPDIRespostas;

                        string onInput = "action_AtualizaResposta('" + getResposta.idPDIRespostas + "', this); return false; this.focus();";
                        addRespostas.OnInput = j == pdiPeriodos.Count - 1 ? onInput : "";

                        addColuna.PDIRespostas.Add(addRespostas);
                    }

                    pdiColunasModel.Add(addColuna);
                }

                addPeriodo.PDIColunas = pdiColunasModel;
                pdiPeriodosModel.Add(addPeriodo);
            }

            rptPeriodos.DataSource = pdiPeriodosModel;
            rptPeriodos.DataBind();
        }

        public void ValidaPDIRespostas(int idAssociado, int idPeriodo)
        {
            var pdiService = new PDIService();
            var pdiQuestoes = pdiService.ObterPDIQuestoes();
            var pdiRespostas = pdiService.ObterPDIRespostas(idAssociado: idAssociado, idPeriodo: idPeriodo);

            for (var i = 0; i < pdiQuestoes.Count; i++)
            {
                var getQuestao = pdiQuestoes[i];
                var getResposta = pdiRespostas.Where(x => x.idPDIQuestao == getQuestao.idPDIQuestoes).ToList();

                if (getResposta == null || getResposta.Count == 0)
                {
                    var addPDIResposta = new PDI_RESPOSTAS();
                    addPDIResposta.idAssociado = idAssociado;
                    addPDIResposta.idPeriodo = idPeriodo;
                    addPDIResposta.idPDIQuestao = getQuestao.idPDIQuestoes;
                    addPDIResposta.Resposta = "Preencha aqui";
                    addPDIResposta.DHC = DateTime.Now;

                    pdiService.GerirPDIResposta(addPDIResposta);
                }
            }
        }

        public void CarregaPillsPeriodos(int idAssociado)
        {
            var periodoUltimo = new PeriodoService().ObterPeriodoUltimo();
            var pdiPillsModel = new List<PDIPillsModel>();
            var pdiService = new PDIService();
            var pdiRespostas = pdiService.ObterPDIRespostas(idAssociado: idAssociado);

            var pdiPeriodos = pdiRespostas.Select(x => x.PERIODOSAVALIACOES).Distinct().ToList();

            for (var i = 0; i < pdiPeriodos.Count; i++)
            {
                var getPeriodo = pdiPeriodos[i];
                var addPill = new PDIPillsModel();
                addPill.id = "tab-" + i.ToString() + "-tab";
                addPill.href = "#tab-" + i.ToString();
                addPill.ariacontrols = "tab-" + i.ToString();
                addPill.ariaselected = i == pdiPeriodos.Count - 1 ? "true" : "false";
                addPill.active = i == pdiPeriodos.Count - 1 ? "active" : "";
                addPill.Periodo = getPeriodo.Periodo;

                pdiPillsModel.Add(addPill);
            }

            rptPills.DataSource = pdiPillsModel;
            rptPills.DataBind();
        }

        protected void btnAction_AtualizaResposta_Click(object sender, EventArgs e)
        {
            var IdPDIResposta = int.Parse(txtAction_IdPDIResposta.Text);
            var textResposta = txtAction_Resposta.Text;


            var pdiService = new PDIService();
            var pdiResposta = pdiService.ObterPDIRespostas(IdPDIResposta)[0];
            pdiResposta.Resposta = textResposta;

            pdiService.GerirPDIResposta(pdiResposta);
        }
    }
}