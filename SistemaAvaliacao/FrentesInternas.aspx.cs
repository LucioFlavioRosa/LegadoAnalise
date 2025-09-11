using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Business.Model;
using System.IO;
using TriaSoftware.Util.Framework.Domain.Service;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using Business.Util;

namespace SistemaAvaliacao
{
    public partial class FrenteInterna : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimparCampos();
            }
        }

        private void LimparCampos()
        {
            hdId.Value = "";
            btnCadastrar.Text = "Cadastrar";
            txtFrenteInterna.Text = "";
            ddlStatus.SelectedIndex = 0;
            divLideres.Visible = false;
            divParticipantes.Visible = false;
            carregaFrentesInternas();
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            var frenteInternaNome = txtFrenteInterna.Text;
            var ativoStatus = ddlStatus.SelectedValue;

            if (frenteInternaNome.Trim() == "") { MessageBox.Show("Preencha o campo Frente Interna", "PENDÊNCIA", TIPO.Warning, MessageBoxHandler1); return; }

            var frenteService = new FrenteInternaService();
            var frenteInterna = new FRENTEINTERNA();
            if (hdId.Value != "")
            {
                frenteInterna = frenteService.ObterFrenteInterna(int.Parse(hdId.Value))[0];
            }

            frenteInterna.FrenteInterna1 = frenteInternaNome;
            frenteInterna.ATV = ativoStatus == "1" ? true : false;
            frenteInterna.DHC = DateTime.Now;
            frenteInterna.USR = WebStorage.GetUsuarioLogado().Id;

            frenteService.GerirFrenteInterna(frenteInterna);

            if (hdId.Value == "")
            {
                MessageBox.Show("Frente interna cadastrada!", "", TIPO.Default, MessageBoxHandler1);
            }
            else
            {
                MessageBox.Show("Frente interna atualizada!", "", TIPO.Default, MessageBoxHandler1);
            }
            LimparCampos();
        }

        protected void carregaFrentesInternas()
        {
            var frenteService = new FrenteInternaService();
            var frentes = frenteService.ObterFrentesInternas();
            var lideresFrentes = frenteService.ObterLideresFrentesInternas();
            var associadosService = new AssociadosService();
            var frentesLideradas = frenteService.ObterLiderFrenteInterna(idAssociado: WebStorage.GetUsuarioLogado().Id).Select(x => x.idFrenteInterna);

            var listaFrentes = (from f in frentes
                                select new FrenteInternaModel
                                {
                                    idFrenteInterna = f.idFrenteInterna,
                                    FrenteInterna = f.FrenteInterna1,
                                    TotalLideres = lideresFrentes.Where(x => x.idFrenteInterna == f.idFrenteInterna && x.ATV == true).Distinct().Count(),
                                    Lideres = string.Join("<br/>", lideresFrentes.Where(x => x.idFrenteInterna == f.idFrenteInterna && x.ATV == true).Distinct().Select(x => x.ASSOCIADOS.Nome)),
                                    ATV = f.ATV ? 1 : 0
                                }).ToList();

            if (WebStorage.GetUsuarioLogado().IdPerfil < 3)
            {
                txtFrenteInterna.Enabled = false;
                ddlStatus.Enabled = false;
                btnCadastrar.Visible = false;
                divExport.Visible = false;

                listaFrentes = listaFrentes.Where(x => frentesLideradas.Contains(x.idFrenteInterna)).ToList();
            }

            if (frentesLideradas == null || frentesLideradas.Count() == 0)
            {
                divAvaliar.Visible = false;
            }
            else
            {
                labelDisclaimerAvaliar.InnerText += new PeriodoService().ObterPeriodoUltimo().Codigo;
            }

            rptFrentesInternas.DataSource = listaFrentes;
            rptFrentesInternas.DataBind();
        }

        protected void rptFrentesInternas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Alterar")
            {
                var id = ((Label)e.Item.FindControl("lblId"));
                hdId.Value = Convert.ToString(id.Text);
                MontaCampos(Convert.ToInt32(id.Text));
                btnCadastrar.Text = "Alterar";

                var frenteService = new FrenteInternaService();
                var frenteInterna = frenteService.ObterFrenteInterna(Convert.ToInt32(id.Text))[0];
                var lideresFrentes = frenteService.ObterLideresFrentesInternas();
                var lideres = lideresFrentes.Where(x => x.idFrenteInterna == frenteInterna.idFrenteInterna).Distinct().ToList();
                rptLideres.DataSource = lideres;
                rptLideres.DataBind();
                divLideres.Visible = true;


                divParticipantes.Visible = true;
            }
            else
            {
                var id = int.Parse(((Label)e.Item.FindControl("lblId")).Text);

                var frenteService = new FrenteInternaService();
                var frenteInterna = frenteService.ObterFrenteInterna(id)[0];
                frenteInterna.ATV = false;
                frenteService.GerirFrenteInterna(frenteInterna);
            }
        }

        protected void rptFrentesInternas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (((Label)e.Item.FindControl("lblATV")).Text == "1")
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Ativo";
                }
                else
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Inativo";
                    ((Button)e.Item.FindControl("btnDeletar")).Visible = false;
                }
            }
        }
        private void MontaCampos(int idFrenteInterna)
        {
            var frenteService = new FrenteInternaService();

            var frenteInterna = frenteService.ObterFrenteInterna(Convert.ToInt32(idFrenteInterna))[0];
            txtFrenteInterna.Text = frenteInterna.FrenteInterna1;
            ddlStatus.SelectedValue = frenteInterna.ATV ? "1" : "0";

            var lideresFrentes = frenteService.ObterLideresFrentesInternas();
            var lideres = lideresFrentes.Where(x => x.idFrenteInterna == frenteInterna.idFrenteInterna).Distinct().ToList();
            rptLideres.DataSource = lideres;
            rptLideres.DataBind();
            divLideres.Visible = true;

            var participantesFrentes = frenteService.ObterParticipantesFrentesInternas();
            var participantes = participantesFrentes.Where(x => x.idFrenteInterna == frenteInterna.idFrenteInterna).Distinct().ToList();
            rptParticipantes.DataSource = participantes;
            rptParticipantes.DataBind();
            divParticipantes.Visible = true;

            carregaFrentesInternas();
        }

        protected void rptLideres_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                var id = int.Parse(((Label)e.Item.FindControl("lblIdLiderFrente")).Text);

                var frenteService = new FrenteInternaService();
                var liderFrenteInterna = frenteService.ObterLiderFrenteInterna(id)[0];
                liderFrenteInterna.ATV = false;
                frenteService.GerirLiderFrenteInterna(liderFrenteInterna);
            }
            else if (e.CommandName == "Adicionar")
            {
                var lblDdl = e.Item.FindControl("ddlAssociados") as DropDownList;
                var associadoId = int.Parse(lblDdl.SelectedValue);

                var liderFrenteInterna = new LIDERESFRENTEINTERNA();
                liderFrenteInterna.idFrenteInterna = int.Parse(hdId.Value);
                liderFrenteInterna.idAssociado = associadoId;
                liderFrenteInterna.DHC = DateTime.Now;
                liderFrenteInterna.ATV = true;
                liderFrenteInterna.USR = WebStorage.GetUsuarioLogado().Id;

                var frenteService = new FrenteInternaService();
                frenteService.GerirLiderFrenteInterna(liderFrenteInterna);
            }

            MontaCampos(int.Parse(hdId.Value));
        }

        protected void rptLideres_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var texto = ((Label)e.Item.FindControl("lblATV")).Text;
                if (texto == "True" || texto == "Verdadeiro")
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Ativo";
                }
                else
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Inativo";
                    ((Button)e.Item.FindControl("btnDeletar")).Visible = false;
                }
            }
        }

        protected void rptLideres_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var lblDdl = e.Item.FindControl("ddlAssociados") as DropDownList;
                var associados = new AssociadosService().ObterAssociados();

                lblDdl.DataTextField = "Nome";
                lblDdl.DataValueField = "IdAssociado";
                lblDdl.DataSource = associados;
                lblDdl.DataBind();
            }
        }

        protected void rptParticipantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Excluir")
            {
                var id = int.Parse(((Label)e.Item.FindControl("lblIdParticipante")).Text);

                var frenteService = new FrenteInternaService();
                var participanteFrenteInterna = frenteService.ObterParticipantesFrentesInternas(id)[0];
                participanteFrenteInterna.ATV = false;
                frenteService.GerirParticipanteFrenteInterna(participanteFrenteInterna);
            }
            else if (e.CommandName == "Adicionar")
            {
                var lblDdl = e.Item.FindControl("ddlAssociados") as DropDownList;
                var associadoId = int.Parse(lblDdl.SelectedValue);

                var participante = new PARTICIPANTESFRENTEINTERNA();
                participante.idFrenteInterna = int.Parse(hdId.Value);
                participante.idAssociado = associadoId;
                participante.DHC = DateTime.Now;
                participante.ATV = true;
                participante.USR = WebStorage.GetUsuarioLogado().Id;

                var frenteService = new FrenteInternaService();
                frenteService.GerirParticipanteFrenteInterna(participante);
            }

            MontaCampos(int.Parse(hdId.Value));
        }

        protected void rptParticipantes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var texto = ((Label)e.Item.FindControl("lblATV")).Text;
                if (texto == "True" || texto == "Verdadeiro")
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Ativo";
                }
                else
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Inativo";
                    ((Button)e.Item.FindControl("btnDeletar")).Visible = false;
                }
            }
        }

        protected void rptParticipantes_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var lblDdl = e.Item.FindControl("ddlAssociados") as DropDownList;
                var associados = new AssociadosService().ObterAssociados();

                lblDdl.DataTextField = "Nome";
                lblDdl.DataValueField = "IdAssociado";
                lblDdl.DataSource = associados;
                lblDdl.DataBind();
            }
        }

        protected void btnAvaliar_Click(object sender, EventArgs e)
        {
            Response.Redirect("avaliacao_FrentesInternas");
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            var exportAlocacoes = getAvaliacoesAlocacao();

            string fileName = "AvaliacoesAlocacaoInterna_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            //Gera arquivo
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, exportAlocacoes);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public List<AlocacaoExport> getAvaliacoesAlocacao()
        {
            var frenteService = new FrenteInternaService();
            var associadosService = new AssociadosService();
            var periodoService = new PeriodoService();
            var returnAlocacoes = new List<AlocacaoExport>();

            var alocacoesTodas = frenteService.ObterAvaliacoesAlocacoesInternas().OrderBy(x => x.idPeriodo).ToList();
            foreach (var item in alocacoesTodas)
            {
                var addAvaliacao = new AlocacaoExport();
                addAvaliacao.idAvaliacaoAlocacao = item.idAvaliacaoAlocacaoInterna;
                addAvaliacao.idAlocacaoInterna = item.FRENTEINTERNA.idFrenteInterna;
                addAvaliacao.AlocacaoInterna = item.FRENTEINTERNA.FrenteInterna1;
                addAvaliacao.idPeriodo = item.idPeriodo;
                addAvaliacao.Periodo = item.PERIODOSAVALIACOES.Periodo;
                addAvaliacao.idLiderAlocacao = item.idAvaliador;
                addAvaliacao.LiderAlocacao = associadosService.ObterAssociado(item.idAvaliador).Nome;
                addAvaliacao.idAvaliado = item.idAssociado;
                addAvaliacao.Avaliado = associadosService.ObterAssociado(item.idAssociado).Nome;
                addAvaliacao.idNota = item.idNota;
                addAvaliacao.Nota = item.NOTASALOCACOESINTERNAS.Descricao;
                addAvaliacao.Comentarios = item.Comentarios;
                addAvaliacao.DHCNota = item.DHC.ToString();
                addAvaliacao.ValidadoMD = item.ValidadoMD == true ? "Validado" : "Pendente";
                addAvaliacao.DHCValidadoMD = item.DHCValidadoMD.ToString();

                returnAlocacoes.Add(addAvaliacao);
            }

            return returnAlocacoes;
        }
    }
}