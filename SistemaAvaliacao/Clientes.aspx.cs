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
    public partial class Clientes : System.Web.UI.Page
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
            montaComboListaClientes();
            montaComboSocio();
            txtCliente.Text = "";
            txtTelefone.Text = "";
            ddlSocio.SelectedIndex = 0;
            txtGestorCliente.Text = "";
            txtEmail.Text = "";
            hdIdCliente.Value = "";
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            ClientesService cs = new ClientesService();
            CLIENTES cliente = new CLIENTES();
            cliente.Cliente = txtCliente.Text;
            cliente.Telefones = txtTelefone.Text;
            cliente.IdAssociacoResponsavel = Convert.ToInt32(ddlSocio.SelectedValue);
            cliente.GestorCliente = txtGestorCliente.Text;
            cliente.Email = txtEmail.Text;
            cliente.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
            cliente.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
            cliente.DHC = DateTime.Now;
            if (ddlStatus.SelectedItem.Text == "Ativo")
                cliente.ATV = 1;
            else
                cliente.ATV = 0;

            if (txtCliente.Text != "")
            {
                if (ddlSocio.SelectedIndex != 0)
                {
                    if(txtGestorCliente.Text != "")
                    {
                        if(txtEmail.Text != "")
                        {
                            if(txtTelefone.Text != "")
                            {
                                if (hdIdCliente.Value == "")
                                {
                                    cs.InserirCliente(cliente);
                                    LimparCampos();

                                    MessageBox.Show("Cliente inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                }
                                else
                                {
                                    cliente.IdCliente = Convert.ToInt32(hdIdCliente.Value);
                                    cs.AlterarCliente(cliente);
                                    LimparCampos(); 
                                    MessageBox.Show("Cliente alterado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Campo Telefone não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Campo E-mail do Cliente não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Campo Gestor do Cliente não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
                    }

                }
                else
                {
                    MessageBox.Show("Campo Cliente não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            else
            {
                MessageBox.Show("Campo Cliente não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void montaComboSocio()
        {
            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociadosSocios(true);
            this.ddlSocio.DataValueField = "IdAssociado";
            this.ddlSocio.DataTextField = "Nome";
            this.ddlSocio.DataSource = associados;
            this.ddlSocio.DataBind();
            this.ddlSocio.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboListaClientes()
        {
            List<CLIENTES> cargos = new ClientesService().ObterListaClientes();

            this.rptClientes.DataSource = cargos;
            this.rptClientes.DataBind();
        }



        protected void rptClientes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AlterarCliente")
            {
                var idCliente = ((Label)e.Item.FindControl("lblIdCliente"));
                hdIdCliente.Value = Convert.ToString(idCliente.Text);

                MontaCamposClientes(Convert.ToInt32(hdIdCliente.Value));
            }
            else
            {
                var idCliente = ((Label)e.Item.FindControl("lblIdCliente"));

                var statusExclusao = new ClientesService().ExcluirCliente(Convert.ToInt32(idCliente.Text));

                if (statusExclusao)
                {
                    LimparCampos();
                    MessageBox.Show("Cliente Inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Cliente !!", "", TIPO.Warning, MessageBoxHandler);
                }
            }
        }

        protected void rptClientes_ItemDataBound(object sender, RepeaterItemEventArgs e)
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




        private void MontaCamposClientes(int idCliente)
        {
            var cliente = new ClientesService().ObterCliente(Convert.ToInt32(idCliente));
            txtCliente.Text = cliente.Cliente;
            ddlSocio.SelectedValue = Convert.ToString(cliente.IdAssociacoResponsavel);
            ddlStatus.SelectedValue = cliente.ATV.ToString();
            txtGestorCliente.Text = cliente.GestorCliente;
            txtEmail.Text = cliente.Email;
            txtTelefone.Text = cliente.Telefones;
        }

    }
}