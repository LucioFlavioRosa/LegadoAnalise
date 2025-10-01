using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;


namespace SistemaAvaliacao
{
    public partial class Complexidade : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LimparCampos();
            }
        }
        private void LimparCampos()
        { 
            montaComboListaComplexidade();
            this.txtComplexidade.Text = "";
            ddlStatus.SelectedIndex = 0;
            hdIdComplexidade.Value = "";
            txtPonderacao.Text = "";
            txtInicial.Text = "";
            txtFinal.Text = "";
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            if (txtComplexidade.Text != "")
            {
                if (txtPonderacao.Text != "")
                {
                    if (txtInicial.Text != "")
                    {
                        if (txtFinal.Text != "")
                        {
                            if (txtPeso.Text != "" && txtPesoPonderado.Text != "")
                            {
                                ComplexidadesService complexidadeservice = new ComplexidadesService();
                                PROJETOSCOMPLEXIDADES complexidade = new PROJETOSCOMPLEXIDADES();

                                decimal decponderacao = 0;
                                decimal decponderacaoIncial = 0;
                                decimal decponderacaoFinal = 0;
                                decimal peso = 0;
                                decimal pesoponderado = 0;

                                decponderacao = Convert.ToDecimal(txtPonderacao.Text.Replace(".", ","));
                                decponderacaoIncial = Convert.ToDecimal(txtInicial.Text.Replace(".", ","));
                                decponderacaoFinal = Convert.ToDecimal(txtFinal.Text.Replace(".", ","));
                                peso = Convert.ToDecimal(txtPeso.Text.Replace(".", ","));
                                pesoponderado = Convert.ToDecimal(txtPesoPonderado.Text.Replace(".", ","));

                                complexidade.Ponderacao = decponderacao;
                                complexidade.FaixaInicial = decponderacaoFinal;
                                complexidade.FaixaFinal = decponderacaoIncial;
                                complexidade.Peso = peso;
                                complexidade.PesoPonderado = pesoponderado;
                                complexidade.DHC = DateTime.Now;
                                complexidade.USR = "1";
                                complexidade.IdComplexidade = 1;
                                complexidade.Complexidade = txtComplexidade.Text;
                                complexidade.Codigo = txtComplexidade.Text;

                                if (ddlStatus.SelectedValue == "1")
                                    complexidade.ATV = 1;
                                else
                                    complexidade.ATV = 0;

                                if (hdIdComplexidade.Value == "")
                                {
                                    complexidadeservice.InserirComplexidade(complexidade);
                                    LimparCampos();


                                    MessageBox.Show("Complexidade Inserida com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                                }
                                else
                                {
                                    complexidade.IdComplexidade = Convert.ToInt32(hdIdComplexidade.Value);

                                    complexidadeservice.AlterarComplexidade(complexidade);
                                    LimparCampos();
                                    MessageBox.Show("Complexidade Alterada com sucesso !!", "", TIPO.Info, MessageBoxHandler);

                                }
                            }
                            else
                            {                                
                                MessageBox.Show("Favor Preencher o Peso e Peso Ponderado", "", TIPO.Info, MessageBoxHandler);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Favor Preencher a Faixa Final", "", TIPO.Info, MessageBoxHandler);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Favor Preencher a Faixa Inicial", "", TIPO.Info, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Favor Preencher a Ponderação", "", TIPO.Info, MessageBoxHandler);
                }
            }
            else
            {
                MessageBox.Show("Favor Preencher a Complexidade", "", TIPO.Info, MessageBoxHandler);
            }


        }

        public void montaComboListaComplexidade()
        {
            List<PROJETOSCOMPLEXIDADES> complexidade = new ComplexidadesService().ObterListaComplexidades();

            this.rptComplexidade.DataSource = complexidade;
            this.rptComplexidade.DataBind();
        }


        protected void rptComplexidade_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AlterarComplexidade")
            {
                var idComplexidade = ((Label)e.Item.FindControl("lblIdComplexidade"));
                hdIdComplexidade.Value = Convert.ToString(idComplexidade.Text);
                MontaCampos(Convert.ToInt32(idComplexidade.Text));
            }
            else
            {
                var idComplexidade = ((Label)e.Item.FindControl("lblIdComplexidade"));

                
                ComplexidadesService cs = new ComplexidadesService();
                var statusExclusao = cs.ExcluirComplexidade(Convert.ToInt32(idComplexidade.Text));

                if (statusExclusao)
                {
                    LimparCampos();
                    MessageBox.Show("Complexidade Inativada com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                          }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Cargo !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void rptComplexidade_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

        private void MontaCampos(int idCargo)
        {
            var complexidade = new ComplexidadesService().ObterComplexidade(Convert.ToInt32(idCargo));
            txtComplexidade.Text = complexidade.Complexidade;
            ddlStatus.SelectedValue = complexidade.ATV.ToString();
            txtPonderacao.Text = complexidade.Ponderacao.ToString().Replace(",", ".");
            txtInicial.Text = complexidade.FaixaInicial.ToString().Replace(",", ".");
            txtFinal.Text = complexidade.FaixaFinal.ToString().Replace(",", ".");
            txtPeso.Text = complexidade.Peso.ToString().Replace(",", ".");
            txtPesoPonderado.Text = complexidade.PesoPonderado.ToString().Replace(",", ".");
        }
    }
}