using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SistemaAvaliacao
{
    public partial class Projetos : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                LimparCampos();
            }
            else
                montaListaAssociadosProjeto();

        }

        protected override void Render(HtmlTextWriter writer)
        {

            foreach (RepeaterItem row in rptAssociados.Items)

                ClientScript.RegisterForEventValidation(row.UniqueID.ToString() + ":_ctl0");

            base.Render(writer);
        }

        private void LimparCampos()
        {
            montaComboResponsavel();
            montaComboGestor();
            montaComboCliente();
            montaComboTipo();
            montaComboComplexidade();
            montaComboListaProjetos();
            montaListaAssociadosProjeto();
            montaAssociadosAlocacao();
            txtCodigo.Text = "";
            txtProjeto.Text = "";
            txtDataInicio.Value = "";
            txtDataTermino.Value = "";
            Session["ASSOCIADOS"] = new List<ASSOCIADOS>();
        }


        private void montaAssociadosAlocacao()
        {
            var profissionalList = new AssociadosService().ObterAssociados(true);
            ddlassociadoProjeto.DataValueField = "IdAssociado";
            ddlassociadoProjeto.DataTextField = "Nome";
            ddlassociadoProjeto.DataSource = profissionalList;
            ddlassociadoProjeto.DataBind();
            ddlassociadoProjeto.Items.Insert(0, "[Selecionar]");
        }
        


        public void montaComboResponsavel()
        {
            PERFIS perfil = new PERFIS() { IdPerfil = Convert.ToInt32(Session["IDPERFIL"]=3.ToString()) };

            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados(perfil);
            this.ddlResponsavel.DataValueField = "IdAssociado";
            this.ddlResponsavel.DataTextField = "Nome";
            this.ddlResponsavel.DataSource = associados;
            this.ddlResponsavel.DataBind();
            this.ddlResponsavel.Items.Insert(0, "[Selecionar]");
        }

        public void montaListaAssociadosProjeto()
        {
            if (Session["ASSOCIADOS"] != null)
            {
                List<ASSOCIADOS> associados = Session["ASSOCIADOS"] as List<ASSOCIADOS>;
                
                this.rptAssociados.DataSource = associados;
                this.rptAssociados.DataBind();
            }
        }

        public void montaComboListaProjetos()
        {
            List<PROJETOS> listaprojetos = new ProjetosService().ObterListaProjetos();

            this.rptProjetos.DataSource = listaprojetos;
            this.rptProjetos.DataBind();
        }

        public void montaComboGestor()
        {
            PERFIS perfil = new PERFIS() { IdPerfil = Convert.ToInt32(Session["IDPERFIL"]=2.ToString()) };
            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados(perfil);

            this.ddlGestor.DataValueField = "IdAssociado";
            this.ddlGestor.DataTextField = "Nome";
            this.ddlGestor.DataSource = associados;
            this.ddlGestor.DataBind();
            this.ddlGestor.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboCliente()
        {
            List<CLIENTES> clientes = new ClientesService().ObterListaClientes();

            this.ddlCliente.DataSource = clientes;
            this.ddlCliente.DataValueField = "IdCliente";
            this.ddlCliente.DataTextField = "Cliente";
            this.ddlCliente.DataBind();
            this.ddlCliente.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboTipo()
        {
            List<PROJETOSTIPOS> tipoprojeto = new TipoProjetoService().ObterListaTipos();

            this.ddlTipoProjeto.DataValueField = "IdTipo";
            this.ddlTipoProjeto.DataTextField = "ProjetoTipo";
            this.ddlTipoProjeto.DataSource = tipoprojeto;
            this.ddlTipoProjeto.DataBind();
            this.ddlTipoProjeto.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboComplexidade()
        {
            List<PROJETOSCOMPLEXIDADES> projetocomplexidade = new ComplexidadesService().ObterListaComplexidades();

            this.ddlComplexidade.DataValueField = "IdComplexidade";
            this.ddlComplexidade.DataTextField = "Complexidade";
            this.ddlComplexidade.Items.Insert(0, "[Selecionar]");
            this.ddlComplexidade.DataSource = projetocomplexidade;
            this.ddlComplexidade.DataBind();
        }


        protected void rpt_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (Session["ASSOCIADOS"] != null)
            {
                List<ASSOCIADOS> associadosList = Session["ASSOCIADOS"] as List<ASSOCIADOS>;

                ASSOCIADOS associado = new ASSOCIADOS();
                associado = new AssociadosService().ObterAssociado(Convert.ToInt32(ddlassociadoProjeto.SelectedValue));

                PROJETOS projeto = new PROJETOS();
                projeto.DataInicio = Convert.ToDateTime(txtInicioAlocacao.Text);
                projeto.DataFim = Convert.ToDateTime(txtTérminoAlocacao.Text);
                associado.PROJETOS.Add(projeto);

                associadosList.Add(associado);

                ddlassociadoProjeto.SelectedIndex = 0;
                txtInicioAlocacao.Text = "";
                txtTérminoAlocacao.Text = "";

                Session["ASSOCIADOS"] = associadosList;

                montaListaAssociadosProjeto();

                MessageBox.Show("Cargo Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);
            }
            else
            {
                Session["ASSOCIADOS"] = new List<ASSOCIADOS>();
            }
        }

            


        protected void btnCadastrarAlocacao_Click(object sender, EventArgs e)
        {
            if (Session["ASSOCIADOS"] != null)
            {
                List<ASSOCIADOS> associadosList = Session["ASSOCIADOS"] as List<ASSOCIADOS>;

                ASSOCIADOS associado = new ASSOCIADOS();
                associado = new AssociadosService().ObterAssociado(Convert.ToInt32(ddlassociadoProjeto.SelectedValue));

                PROJETOS projeto = new PROJETOS();
                projeto.DataInicio = Convert.ToDateTime(txtInicioAlocacao.Text);
                projeto.DataFim = Convert.ToDateTime(txtTérminoAlocacao.Text);
                associado.PROJETOS.Add(projeto);

                associadosList.Add(associado);

                ddlassociadoProjeto.SelectedIndex = 0;
                txtInicioAlocacao.Text = "";
                txtTérminoAlocacao.Text = "";

                Session["ASSOCIADOS"] = associadosList;

                montaListaAssociadosProjeto();

                MessageBox.Show("Cargo Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);
            }
            else
            {
                Session["ASSOCIADOS"] = new List<ASSOCIADOS>();
            }

            
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            ProjetosService ps = new ProjetosService();
            PROJETOS projetos = new PROJETOS();

            projetos.Codigo = txtCodigo.Text;
            projetos.Projeto = txtProjeto.Text;

            var datainicio = txtDataInicio.Value;
            var datatermino = txtDataTermino.Value;

            if (txtDataInicio.Value != "")
            {
                if (txtDataTermino.Value != "")
                {
                    projetos.DataInicio = Convert.ToDateTime(datainicio);
                    projetos.DataFim = Convert.ToDateTime(datatermino);
                    projetos.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
                    projetos.IdCliente = Convert.ToInt16(ddlCliente.SelectedItem.Value);
                    projetos.IdAssociadoGestor = Convert.ToInt16(ddlGestor.SelectedItem.Value);
                    projetos.IdAssociadoResponsavel = Convert.ToInt16(ddlResponsavel.SelectedItem.Value);
                    projetos.IdTipo = Convert.ToInt16(ddlTipoProjeto.SelectedItem.Value);
                    projetos.IdComplexidade = Convert.ToInt16(ddlComplexidade.SelectedItem.Value);
                    projetos.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());;
                    projetos.DHC = DateTime.Now;
                    if (ddlStatus.Text == "Ativo")
                    {
                        projetos.ATV = 1;
                        projetos.IdStatus = 1;
                    }
                    else
                    {
                        projetos.ATV = 0;
                        projetos.IdStatus = 0;
                    }

                    ps.InserirProjeto(projetos);
                    montaComboListaProjetos();
                }
            }
            else
            {
                MessageBox.Show("Data de Início e Término devem ser preenchidos.", "", TIPO.Warning, MessageBoxHandler);
            }
        }




        protected void rpt_ItemDataBound(object sender, RepeaterItemEventArgs e)
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


    }
}