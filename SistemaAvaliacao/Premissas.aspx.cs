using Business.DataAccess;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao
{
    public partial class Premissas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                montaListaPremissas();
                montaComboEixos();
                montaComboCargos();
                montaComboNiveis();
            }
        }

        private void MontaCampos(int idpremissa)
        {
            var premissa = new PremissaService().Obter(Convert.ToInt32(idpremissa));
            ddlEixo.SelectedValue = premissa.IdEixo.ToString();
            ddlCargo.SelectedValue = premissa.IdCargo.ToString();
            ddlNivel.SelectedValue = premissa.IdNivel.ToString();
            txtPeers.Text = premissa.ValorRadarPeers.ToString();
            txtAvaliado.Text = premissa.ValorBaseAutoAvaliacao.ToString();
            txtGestor.Text = premissa.ValorBaseAvaliacaoGestor.ToString();
        }

        public string StatusRadar(int atv)
        {
            if (atv == 0)
            {
                return "Inativo";
            }
            else
            {
                return "Ativo";
            }
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            var premissaService = new PremissaService();
            var premissa = new PREMISSAS_RADAR();

            try
            {
                if (!string.IsNullOrEmpty(hdId.Value))
                {
                    var update = premissaService.Obter(Convert.ToInt32(hdId.Value));
                    update.IdEixo = Convert.ToInt16(ddlEixo.SelectedItem.Value);
                    update.IdCargo = Convert.ToInt16(ddlCargo.SelectedItem.Value);
                    update.IdNivel = Convert.ToInt16(ddlNivel.SelectedItem.Value);
                    update.IdEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
                    update.USR = WebStorage.GetUsuarioLogado().Id;
                    update.DHC = DateTime.Now;
                    update.ValorRadarPeers = Convert.ToInt16(txtPeers.Text);
                    update.ValorBaseAutoAvaliacao = Convert.ToInt16(txtAvaliado.Text);
                    update.ValorBaseAvaliacaoGestor = Convert.ToInt16(txtGestor.Text);

                    if (premissaService.Alterar(update.IdEixo, update))
                    {
                        MessageBox.Show("Premissa alterada com sucesso.", "Sucesso", TIPO.Info, MessageBoxHandler);
                        montaListaPremissas();
                    }
                    else
                    {
                        MessageBox.Show("Houve uma falha na tentativa de salvamento.", "Faha ao Alterar", TIPO.Error, MessageBoxHandler);
                    }
                }
                else
                {
                    premissa.IdEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
                    premissa.USR = WebStorage.GetUsuarioLogado().Id;
                    premissa.DHC = DateTime.Now;
                    premissa.ATV = 1;

                    premissa.IdEixo = Convert.ToInt16(ddlEixo.SelectedItem.Value);
                    premissa.IdCargo = Convert.ToInt16(ddlCargo.SelectedItem.Value);
                    premissa.IdNivel = Convert.ToInt16(ddlNivel.SelectedItem.Value);

                    premissa.ValorRadarPeers = Convert.ToInt16(txtPeers.Text);
                    premissa.ValorBaseAutoAvaliacao = Convert.ToInt16(txtAvaliado.Text);
                    premissa.ValorBaseAvaliacaoGestor = Convert.ToInt16(txtGestor.Text);

                    if (premissaService.Inserir(premissa))
                        montaListaPremissas();
                    else
                        MessageBox.Show("Houve uma falha na tentativa de salvamento.", "Faha ao Incluir", TIPO.Error, MessageBoxHandler);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Preencha Todos os Campos", "Campos Obrigatórios", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void montaComboCargos()
        {
            List<CARGOS> cargos = new CargosService().ObterListaCargos();
            this.ddlCargo.DataValueField = "IdCargo";
            this.ddlCargo.DataTextField = "Cargo";
            this.ddlCargo.DataSource = cargos;
            this.ddlCargo.DataBind();
            this.ddlCargo.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboNiveis()
        {
            List<CARGOSNIVEIS> cargosniveis = new CargosNiveisService().ObterListaNiveis();
            this.ddlNivel.DataValueField = "IdNivel";
            this.ddlNivel.DataTextField = "Nivel";
            this.ddlNivel.DataSource = cargosniveis;
            this.ddlNivel.DataBind();
            this.ddlNivel.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboEixos()
        {
            List<EIXOS> eixos = new EixoService().ListaEixos();
            this.ddlEixo.DataValueField = "IdEixo";
            this.ddlEixo.DataTextField = "Eixo";
            this.ddlEixo.DataSource = eixos;
            this.ddlEixo.DataBind();
            this.ddlEixo.Items.Insert(0, "[Selecionar]");
        }

        public void montaListaPremissas()
        {
            List<PREMISSAS_RADAR> premissas = new PremissaService().ObterLista();

            this.rptPremissas.DataSource = premissas;
            this.rptPremissas.DataBind();
        }



        protected void rptPremissas_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            var idpremissa = ((Label)e.Item.FindControl("lblIdPremissa"));

            if (e.CommandName == "Alterar")
            {                
                hdId.Value = Convert.ToString(idpremissa.Text);
                MontaCampos(Convert.ToInt32(idpremissa.Text));
            }
            else
            {
               
                var premissaAtivo = new PremissaService().InativarPremissa(Convert.ToInt32(idpremissa.Text));

                if (premissaAtivo)
                {
                    MessageBox.Show("Premissa alterada com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                    montaListaPremissas();
                }
                else
                {
                    MessageBox.Show("Erro ao alterar a premissa !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }
    }
}