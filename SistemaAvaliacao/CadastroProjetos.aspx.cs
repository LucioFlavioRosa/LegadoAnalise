using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Services;
using Business.Util;
using HtmlAgilityPack;
using System.Web.UI;
using Business.Model;
using System.Web;
using System.IO;
using TriaSoftware.Util.Framework.Domain.Service;
using System.Data.OleDb;
using Tria.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class CadastroProjetos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimparCampos();

            }
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            //ProjetosService ps = new ProjetosService();
            PROJETOS projetos = new PROJETOS();

            var datainicio = txtDataInicio.Text;
            var datatermino = txtDataTermino.Text;

            if (txtCodigo.Text != "")
            {
                if (txtProjeto.Text != "")
                {
                    if (ddlCliente.SelectedIndex != 0)
                    {
                        if (ddlStatusProjeto.SelectedIndex != 0)
                        {
                            if (txtDataInicio.Text != "")
                            {
                                if (txtDataTermino.Text != "")
                                {
                                    if (Convert.ToDateTime(txtDataTermino.Text) > Convert.ToDateTime(txtDataInicio.Text))
                                    {
                                        if (ddlResponsavel.SelectedIndex != 0)
                                        {
                                            if (ddlGestor.SelectedIndex != 0)
                                            {
                                                if (ddlTipoProjeto.SelectedIndex != 0)
                                                {
                                                    if (ddlComplexidade.SelectedIndex != 0)
                                                    {
                                                        projetos.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
                                                        projetos.IdCliente = Convert.ToInt16(ddlCliente.SelectedItem.Value);
                                                        projetos.IdAssociadoResponsavel = Convert.ToInt16(ddlResponsavel.SelectedItem.Value);
                                                        projetos.IdAssociadoGestor = Convert.ToInt16(ddlGestor.SelectedItem.Value);
                                                        projetos.IdStatus = Convert.ToInt16(ddlStatusProjeto.SelectedItem.Value);
                                                        projetos.IdTipo = Convert.ToInt16(ddlTipoProjeto.SelectedItem.Value);
                                                        projetos.IdComplexidade = Convert.ToInt16(ddlComplexidade.SelectedItem.Value);
                                                        projetos.Codigo = txtCodigo.Text;
                                                        projetos.Projeto = txtProjeto.Text;
                                                        projetos.DataInicio = Convert.ToDateTime(txtDataInicio.Text);
                                                        projetos.DataFim = Convert.ToDateTime(txtDataTermino.Text);
                                                        projetos.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                                        projetos.DHC = DateTime.Now;
                                                        if (ddlStatus.SelectedIndex == 0)
                                                            projetos.ATV = 1;
                                                        else
                                                            projetos.ATV = 0;

                                                        // NOVO PROJETO
                                                        if (hdIdProjeto.Value == "")
                                                        {
                                                            if (new ProjetosService().InserirProjeto(projetos))
                                                            {
                                                                if (Session["PROJETOSASSOCIADOS"] != null)
                                                                {
                                                                    PROJETOS projetoInserido = new ProjetosService().ObterProjeto(projetos.Projeto, projetos.Codigo, projetos.IdStatus, projetos.IdAssociadoResponsavel);

                                                                    List<PROJETOSASSOCIADOS> alocacaoList = Session["PROJETOSASSOCIADOS"] as List<PROJETOSASSOCIADOS>;

                                                                    foreach (PROJETOSASSOCIADOS alocacao in alocacaoList)
                                                                    {
                                                                        ProjetosService psInsercao = new ProjetosService();

                                                                        PROJETOSASSOCIADOS alocacaoInsercao = new PROJETOSASSOCIADOS();
                                                                        alocacaoInsercao.IdProjeto = projetoInserido.IdProjeto;
                                                                        alocacaoInsercao.IdGestor = projetoInserido.IdAssociadoGestor;
                                                                        alocacaoInsercao.IdAvaliador = alocacao.IdAvaliador;
                                                                        alocacaoInsercao.IdAssociado = alocacao.IdAssociado;
                                                                        alocacaoInsercao.DataInicio = alocacao.DataInicio;
                                                                        alocacaoInsercao.DataFim = alocacao.DataFim;
                                                                        alocacaoInsercao.Comentario = alocacao.Comentario;
                                                                        alocacaoInsercao.USR = alocacao.USR;
                                                                        alocacaoInsercao.DHC = DateTime.Now;
                                                                        alocacaoInsercao.ATV = 1;
                                                                        alocacaoInsercao.TipoAvaliacao = "desempenho";
                                                                        alocacaoInsercao.Escopo = "projeto";

                                                                        if (!psInsercao.InserirProjetoAssociado(alocacaoInsercao))
                                                                        {
                                                                            MessageBox.Show("Problema no Cadastro das alocações do projeto!", "", TIPO.Info, MessageBoxHandler);
                                                                        }

                                                                    }
                                                                }
                                                                if (Session["PROJETOSASSOCIADOS_LIDERANCA"] != null)
                                                                {
                                                                    PROJETOS projetoInserido = new ProjetosService().ObterProjeto(projetos.Projeto, projetos.Codigo, projetos.IdStatus, projetos.IdAssociadoResponsavel);

                                                                    List<PROJETOSASSOCIADOS> alocacaoList = Session["PROJETOSASSOCIADOS_LIDERANCA"] as List<PROJETOSASSOCIADOS>;

                                                                    foreach (PROJETOSASSOCIADOS alocacao in alocacaoList)
                                                                    {
                                                                        ProjetosService psInsercao = new ProjetosService();

                                                                        PROJETOSASSOCIADOS alocacaoInsercao = new PROJETOSASSOCIADOS();
                                                                        alocacaoInsercao.IdProjeto = projetoInserido.IdProjeto;
                                                                        alocacaoInsercao.IdGestor = alocacao.IdGestor;
                                                                        alocacaoInsercao.IdAvaliador = alocacao.IdAvaliador;
                                                                        alocacaoInsercao.IdAssociado = alocacao.IdAssociado;
                                                                        alocacaoInsercao.DataInicio = alocacao.DataInicio;
                                                                        alocacaoInsercao.DataFim = alocacao.DataFim;
                                                                        alocacaoInsercao.Comentario = alocacao.Comentario;
                                                                        alocacaoInsercao.USR = alocacao.USR;
                                                                        alocacaoInsercao.DHC = DateTime.Now;
                                                                        alocacaoInsercao.ATV = 1;
                                                                        alocacaoInsercao.TipoAvaliacao = "lideranca";
                                                                        alocacaoInsercao.Escopo = alocacao.Escopo;

                                                                        if (!psInsercao.InserirProjetoAssociado(alocacaoInsercao))
                                                                        {
                                                                            MessageBox.Show("Problema no Cadastro das alocações de avaliação de liderança!", "", TIPO.Info, MessageBoxHandler);
                                                                        }
                                                                    }
                                                                }

                                                                MessageBox.Show("Projeto Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                                                LimparCampos();
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Problema no cadastro do Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                                                            }
                                                        }
                                                        // ATUALIZANDO PROJETO
                                                        else
                                                        {
                                                            projetos.IdProjeto = Convert.ToInt32(hdIdProjeto.Value);

                                                            if (new ProjetosService().AlterarProjeto(projetos))
                                                            {
                                                                if (Session["PROJETOSASSOCIADOS"] != null)
                                                                {
                                                                    List<PROJETOSASSOCIADOS> alocacaoList = Session["PROJETOSASSOCIADOS"] as List<PROJETOSASSOCIADOS>;

                                                                    foreach (PROJETOSASSOCIADOS alocacao in alocacaoList)
                                                                    {
                                                                        PROJETOSASSOCIADOS alocacaoInsercao = new PROJETOSASSOCIADOS();
                                                                        alocacaoInsercao.IdProjeto = projetos.IdProjeto;
                                                                        alocacaoInsercao.IdAssociado = alocacao.IdAssociado;
                                                                        alocacaoInsercao.DataInicio = alocacao.DataInicio;
                                                                        alocacaoInsercao.DataFim = alocacao.DataFim;
                                                                        alocacaoInsercao.Comentario = alocacao.Comentario;
                                                                        alocacaoInsercao.USR = alocacao.USR;
                                                                        alocacaoInsercao.DHC = DateTime.Now;
                                                                        alocacaoInsercao.ATV = 1;
                                                                        alocacaoInsercao.TipoAvaliacao = "desempenho";
                                                                        alocacaoInsercao.Escopo = "projeto";
                                                                        alocacaoInsercao.IdGestor = projetos.IdAssociadoGestor;
                                                                        alocacaoInsercao.IdAvaliador = alocacao.IdAvaliador;

                                                                        if ((alocacao.flagNovaAlocacao != null && (bool)alocacao.flagNovaAlocacao) &&
                                                                            (alocacao.flagExcluirAlocacao == null || (bool)!alocacao.flagExcluirAlocacao))
                                                                        {
                                                                            if (!new ProjetosService().InserirProjetoAssociado(alocacaoInsercao))
                                                                            {
                                                                                MessageBox.Show("Problema no Cadastro das alocações do projeto  !!", "", TIPO.Info, MessageBoxHandler);
                                                                            }
                                                                        }
                                                                        else if (alocacao.flagExcluirAlocacao != null && (bool)!alocacao.flagExcluirAlocacao)
                                                                        {
                                                                            alocacaoInsercao.IdProjetoAssociado = alocacao.IdProjetoAssociado;

                                                                            if (!new ProjetosService().ExcluirProjetoAssociado(alocacaoInsercao))
                                                                            {
                                                                                MessageBox.Show("Problema no Cadastro das alocações do projeto  !!", "", TIPO.Info, MessageBoxHandler);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                if (Session["PROJETOSASSOCIADOS_LIDERANCA"] != null)
                                                                {
                                                                    List<PROJETOSASSOCIADOS> alocacaoList = Session["PROJETOSASSOCIADOS_LIDERANCA"] as List<PROJETOSASSOCIADOS>;

                                                                    foreach (PROJETOSASSOCIADOS alocacao in alocacaoList)
                                                                    {
                                                                        PROJETOSASSOCIADOS alocacaoInsercao = new PROJETOSASSOCIADOS();
                                                                        alocacaoInsercao.IdProjeto = projetos.IdProjeto;
                                                                        alocacaoInsercao.IdGestor = alocacao.IdGestor;
                                                                        alocacaoInsercao.IdAvaliador = alocacao.IdAvaliador;
                                                                        alocacaoInsercao.IdAssociado = alocacao.IdAssociado;
                                                                        alocacaoInsercao.DataInicio = alocacao.DataInicio;
                                                                        alocacaoInsercao.DataFim = alocacao.DataFim;
                                                                        alocacaoInsercao.Comentario = alocacao.Comentario;
                                                                        alocacaoInsercao.USR = alocacao.USR;
                                                                        alocacaoInsercao.DHC = DateTime.Now;
                                                                        alocacaoInsercao.ATV = 1;
                                                                        alocacaoInsercao.TipoAvaliacao = "lideranca";
                                                                        alocacaoInsercao.Escopo = alocacao.Escopo;


                                                                        if ((alocacao.flagNovaAlocacao != null && (bool)alocacao.flagNovaAlocacao) &&
                                                                            (alocacao.flagExcluirAlocacao != null && (bool)!alocacao.flagExcluirAlocacao))
                                                                        {
                                                                            if (!new ProjetosService().InserirProjetoAssociado(alocacaoInsercao))
                                                                            {
                                                                                MessageBox.Show("Problema no Cadastro das alocações de avaliação de liderança!", "", TIPO.Info, MessageBoxHandler);
                                                                            }
                                                                        }
                                                                        else if (alocacao.flagExcluirAlocacao != null && (bool)!alocacao.flagExcluirAlocacao)
                                                                        {
                                                                            alocacaoInsercao.IdProjetoAssociado = alocacao.IdProjetoAssociado;

                                                                            if (!new ProjetosService().ExcluirProjetoAssociado(alocacaoInsercao))
                                                                            {
                                                                                MessageBox.Show("Problema no Cadastro das alocações de avaliação de liderança!", "", TIPO.Info, MessageBoxHandler);
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                MessageBox.Show("Projeto Atualizado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                                                LimparCampos();
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Problema no cadastro do Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Selecione o Campo Complexidade !!", "", TIPO.Warning, MessageBoxHandler);
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Selecione o Campo Tipo do Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Selecione o Campo Gestor  !!", "", TIPO.Warning, MessageBoxHandler);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Selecione o Campo Responsável !!", "", TIPO.Warning, MessageBoxHandler);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("A Data de Início do projeto deve ser menor que a Data de Término do projeto !!", "", TIPO.Warning, MessageBoxHandler);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Informe o Campo Data de Término do projeto !!", "", TIPO.Warning, MessageBoxHandler);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Informe o Campo Data de Inicio do projeto !!", "", TIPO.Warning, MessageBoxHandler);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Selecione o Campo Status do Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Selecione o Campo Cliente !!", "", TIPO.Warning, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Informe o Campo Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            else
            {
                MessageBox.Show("Informe o Campo Código !!", "", TIPO.Warning, MessageBoxHandler);
            }


        }

        protected void btnCadastrarAlocacao_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlGestor.SelectedIndex != 0)
                {
                    if (ddlAssociadoAlocado.SelectedIndex != 0)
                    {
                        if (txtInicioAlocacao.Text != "")
                        {
                            if (txtTerminoAlocacao.Text != "")
                            {
                                if (ddlAvaliadorAlocacao.SelectedIndex != 0)
                                {
                                    if (txtDataInicio.Text != "" && txtDataTermino.Text != "")
                                    {
                                        if (Convert.ToDateTime(txtTerminoAlocacao.Text) > Convert.ToDateTime(txtInicioAlocacao.Text))
                                        {
                                            if (Convert.ToDateTime(txtInicioAlocacao.Text) >= Convert.ToDateTime(txtDataInicio.Text) &&
                                                Convert.ToDateTime(txtInicioAlocacao.Text) < Convert.ToDateTime(txtDataTermino.Text) &&
                                                Convert.ToDateTime(txtTerminoAlocacao.Text) > Convert.ToDateTime(txtDataInicio.Text) &&
                                                Convert.ToDateTime(txtTerminoAlocacao.Text) <= Convert.ToDateTime(txtDataTermino.Text))
                                            {
                                                if (Session["PROJETOSASSOCIADOS"] != null)
                                                {
                                                    if (!string.IsNullOrEmpty(hdlIdProjetoAssociado.Value))
                                                    {
                                                        if (int.TryParse(hdlIdProjetoAssociado.Value, out int idprojetoassociado))
                                                        {
                                                            var projetoassociado = new ProjetosService().ObterProjetoAssociado(idprojetoassociado);

                                                            if (projetoassociado != null)
                                                            {
                                                                DateTime datainicioalocacao = Convert.ToDateTime(txtInicioAlocacao.Text);
                                                                DateTime datafimalocacao = Convert.ToDateTime(txtTerminoAlocacao.Text);
                                                                int idAvaliador = Convert.ToInt32(ddlAvaliadorAlocacao.SelectedValue);
                                                                int idassociado = Convert.ToInt32(ddlAssociadoAlocado.SelectedValue);
                                                                int idGestor = Convert.ToInt32(ddlGestor.SelectedValue);

                                                                var existavaliacao = new AvaliacoesService().ObterAvaliacao(projetoassociado.IdProjeto, projetoassociado.IdAssociado,
                                                                    projetoassociado.TipoAvaliacao, projetoassociado.Escopo, (int)projetoassociado.IdGestor);
                                                                bool validaUpdate = true;

                                                                if (existavaliacao != null && existavaliacao.Count() > 0)
                                                                {
                                                                    var periodo = new PeriodoService().ObterPeriodoAtual();
                                                                    periodo = new PeriodoService().ObterPeriodoUltimo(); // PATCHJUN2022 - LANÇAR NO ÚLTIMO PERÍODO

                                                                    if (periodo == null)
                                                                    {
                                                                        MessageBox.Show(string.Format("Atenção! Não existe um período vigente. A data atual {0} é superior a data término do último semestre cadastrado.", DateTime.Now.ToString("dd/MM/yyyy")), "", TIPO.Warning, MessageBoxHandler);
                                                                        validaUpdate = false;
                                                                    }
                                                                    else
                                                                    {
                                                                        var avaliacaocorrente = existavaliacao.FirstOrDefault(x => x.idPeriodo == periodo.IdPeriodo);

                                                                        if (avaliacaocorrente != null)
                                                                        {
                                                                            if (idassociado != avaliacaocorrente.idAssociado)
                                                                            {
                                                                                MessageBox.Show("O Associado cadastrado já possui avaliação deste projeto, não será possivel a alteração, utilize a funcionalidade de inativar!", "", TIPO.Warning, MessageBoxHandler);
                                                                                validaUpdate = false;
                                                                            }

                                                                            if (projetoassociado.DataInicio.Date != datainicioalocacao.Date)
                                                                            {
                                                                                if (datainicioalocacao.Date < periodo.DataInicio.Value.Date || datainicioalocacao.Date > periodo.DataFim.Value.Date)
                                                                                {
                                                                                    MessageBox.Show("Data de Início da alocação está fora de período de avaliação vigente!", "", TIPO.Warning, MessageBoxHandler);
                                                                                    validaUpdate = false;
                                                                                }
                                                                            }

                                                                            if (projetoassociado.DataFim.Value.Date != datafimalocacao.Date && 1 == 2) // PATCHJUN2022 - IGNORAR DATA DE TÉRMINO DO PERÍODO
                                                                            {
                                                                                if (datafimalocacao.Date < periodo.DataInicio.Value.Date || datafimalocacao.Date > periodo.DataFim.Value.Date)
                                                                                {
                                                                                    MessageBox.Show("Data de Término da alocação está fora do período de avaliação vigente!", "", TIPO.Warning, MessageBoxHandler);
                                                                                    validaUpdate = false;
                                                                                }
                                                                            }

                                                                            if (idAvaliador != projetoassociado.IdAvaliador)
                                                                            {
                                                                                var projeto = new ProjetosService().ObterProjeto(projetoassociado.IdProjeto);
                                                                                var avaliacaoEmail = new AvaliacoesService().ObterAvaliacaoEmail(projetoassociado.IdProjeto, projetoassociado.IdAssociado, periodo.IdPeriodo,
                                                                                    projeto.IdEmpresa, projetoassociado.TipoAvaliacao, projetoassociado.Escopo, (int)projetoassociado.IdGestor);

                                                                                var existavaliacaocompetencia = new AvaliacoesService().ObterAvaliacoesCompetencias(projetoassociado.IdAssociado,
                                                                                    projetoassociado.IdProjeto, periodo.IdPeriodo, projetoassociado.TipoAvaliacao, projetoassociado.Escopo, avaliacaoEmail.idAvaliacao);

                                                                                if (existavaliacaocompetencia != null && existavaliacaocompetencia.Count(x => x.IdNotaNivel1AvaliacaoCegas.HasValue) > 0)
                                                                                {
                                                                                    MessageBox.Show("A avaliação existente para o alocado já possui notas do avaliador cadastrado, alteração não permitida!", "", TIPO.Warning, MessageBoxHandler);
                                                                                    validaUpdate = false;
                                                                                }
                                                                            }

                                                                        }
                                                                        else
                                                                        {
                                                                            MessageBox.Show("Alocação fora de período de avaliação!", "", TIPO.Warning, MessageBoxHandler);
                                                                            validaUpdate = false;
                                                                        }
                                                                    }
                                                                }

                                                                if (validaUpdate)
                                                                {
                                                                    projetoassociado.IdAvaliador = idAvaliador;
                                                                    projetoassociado.IdGestor = idGestor;
                                                                    projetoassociado.IdAssociado = idassociado;
                                                                    projetoassociado.DataFim = datafimalocacao;
                                                                    projetoassociado.DataInicio = datainicioalocacao;
                                                                    projetoassociado.Comentario = txtComentariosAlocacao.Text;
                                                                    projetoassociado.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                                                    new ProjetosService().AlterarProjetoAssociado(projetoassociado);
                                                                    MessageBox.Show("Alocação alterada com sucesso!", "", TIPO.Info, MessageBoxHandler);
                                                                    Session["PROJETOSASSOCIADOS"] = new List<PROJETOSASSOCIADOS>();
                                                                    montaListaAssociadosProjeto(projetoassociado.IdProjeto);
                                                                }

                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Projeto Associado não encontrado!", "", TIPO.Warning, MessageBoxHandler);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Alocação não encontrada!", "", TIPO.Warning, MessageBoxHandler);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        List<PROJETOSASSOCIADOS> alocacoesList = Session["PROJETOSASSOCIADOS"] as List<PROJETOSASSOCIADOS>;
                                                        string comentarios = txtComentariosAlocacao.Text;
                                                        if (comentarios == "") comentarios = "Não informado";

                                                        PROJETOSASSOCIADOS alocacao = new PROJETOSASSOCIADOS()
                                                        {
                                                            IdProjeto = 0,
                                                            IdAssociado = Convert.ToInt32(ddlAssociadoAlocado.SelectedValue),
                                                            IdGestor = Convert.ToInt32(ddlGestor.SelectedValue),
                                                            IdAvaliador = Convert.ToInt32(ddlAvaliadorAlocacao.SelectedValue),
                                                            DataInicio = Convert.ToDateTime(txtInicioAlocacao.Text),
                                                            DataFim = Convert.ToDateTime(txtTerminoAlocacao.Text),
                                                            Comentario = comentarios,
                                                            USR = 1,
                                                            DHC = DateTime.Now,
                                                            ATV = 1,
                                                            flagNovaAlocacao = true,
                                                            TipoAvaliacao = "desempenho",
                                                            Escopo = "projeto"
                                                        };

                                                        if (!ValidaExistenciaAlocacao(alocacao))
                                                        {

                                                            alocacao.ASSOCIADOS = new AssociadosService().ObterAssociado(alocacao.IdAssociado);
                                                            alocacao.ASSOCIADOS.CARGOS = new CargosService().ObterCargo(alocacao.ASSOCIADOS.IdCargo);
                                                            alocacao.ASSOCIADOS2 = new AssociadosService().ObterAssociado(Convert.ToInt32(alocacao.IdAvaliador));

                                                            alocacoesList.Add(alocacao);

                                                            ddlAssociadoAlocado.SelectedIndex = 0;
                                                            txtInicioAlocacao.Text = "";
                                                            txtTerminoAlocacao.Text = "";
                                                            txtComentariosAlocacao.Text = "";

                                                            Session["PROJETOSASSOCIADOS"] = alocacoesList;

                                                            montaListaAssociadosProjeto();

                                                            MessageBox.Show("Alocação Inserida PROVISORIAMENTE. A mesma será efetivada ao FINAL do cadastro do projeto !!", "", TIPO.Info, MessageBoxHandler);

                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Alocação já existente ou o período indicado invade alguma outra alocação já cadastrada !!", "", TIPO.Info, MessageBoxHandler);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Session["PROJETOSASSOCIADOS"] = new List<PROJETOSASSOCIADOS>();
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("A Data de Início e Término da Alocação devem estar em conformidade com a Data de Início e Término do Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Informe os Campos Data de Início e Término do Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("A data de Término da Alocação deve ser maior que a Data de Início da Alocação !!", "", TIPO.Warning, MessageBoxHandler);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Selecione o Campo Avaliador !!", "", TIPO.Warning, MessageBoxHandler);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Informe o Campo Término da alocação !!", "", TIPO.Warning, MessageBoxHandler);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Informe o Campo Início da alocação !!", "", TIPO.Warning, MessageBoxHandler);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Selecione o Campo Associado !!", "", TIPO.Warning, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Selecione o Campo Gestor do Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Erro ao tentar salvar a alocação: " + ex.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }

        protected void btn_CadastrarAvaliacaoLideranca_Click(object sender, EventArgs e)
        {
            try
            {
                #region VALIDAÇÃO
                bool camposOkay = true;
                if (ddl_AvalLideranca_Lider.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha o líder que será avaliado!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && text_AvalLideranca_Inicio.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de início da avaliação de liderança!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && text_AvalLideranca_Termino.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de término da avaliação de liderança!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && ddl_AvalLideranca_Liderado.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha o liderado que fará a avaliação", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && txtDataInicio.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && txtDataTermino.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && ddl_Escopo.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha um escopo", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }

                DateTime data_Inicio_Projeto = Convert.ToDateTime(txtDataInicio.Text),
                    data_Termino_Projeto = Convert.ToDateTime(txtDataTermino.Text),
                    data_Inicio_Avaliacao = Convert.ToDateTime(text_AvalLideranca_Inicio.Text),
                    data_Termino_Avaliacao = Convert.ToDateTime(text_AvalLideranca_Termino.Text);
                if (camposOkay && data_Termino_Avaliacao < data_Inicio_Avaliacao) { camposOkay = false; MessageBox.Show("A data de término da avaliação deve ser maior que a data de início da avaliação!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && data_Inicio_Avaliacao > data_Termino_Projeto) { camposOkay = false; MessageBox.Show("A data de início da avaliação não poder ser maior que a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && data_Inicio_Avaliacao < data_Inicio_Projeto) { camposOkay = false; MessageBox.Show("A data de início da avaliação não poder ser menor que a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && data_Termino_Avaliacao < data_Inicio_Projeto) { camposOkay = false; MessageBox.Show("A data de término da avaliação não poder ser menor que a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && data_Termino_Avaliacao > data_Termino_Projeto) { camposOkay = false; MessageBox.Show("A data de término da avaliação não poder ser maior que a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && Session["PROJETOSASSOCIADOS_LIDERANCA"] == null) { camposOkay = false; Session["PROJETOSASSOCIADOS_LIDERANCA"] = new List<PROJETOSASSOCIADOS>(); MessageBox.Show("Houve um erro no navegador. Atualize a página e tente novamente.", "Erro", TIPO.Warning, MessageBoxHandler); }
                #endregion
                #region EXECUÇÃO
                if (camposOkay)
                {
                    if (!string.IsNullOrEmpty(hdl_AvalLideranca_Lider.Value)) // ALTERA ALOCAÇÃO DE AVALIAÇÃO DE LIDERANÇA
                    {
                        if (int.TryParse(hdl_AvalLideranca_Lider.Value, out int idprojetoassociado))
                        {
                            var projetoassociado = new ProjetosService().ObterProjetoAssociado(idprojetoassociado);
                            if (projetoassociado != null)
                            {
                                int idAvaliador = 275;
                                int idassociado = Convert.ToInt32(ddl_AvalLideranca_Lider.SelectedValue);
                                var periodo = new PeriodoService().ObterPeriodoAtual();
                                bool validaUpdate = true;

                                var existavaliacao = new AvaliacoesService().ObterAvaliacao(projetoassociado.IdProjeto, projetoassociado.IdAssociado, projetoassociado.TipoAvaliacao,
                                    projetoassociado.Escopo, (int)projetoassociado.IdGestor);

                                if (existavaliacao != null && existavaliacao.Count() > 0)
                                {
                                    if (periodo == null)
                                    {
                                        MessageBox.Show(string.Format("Atenção! Não existe um período vigente. A data atual {0} é superior a data término do último semestre cadastrado.",
                                        DateTime.Now.ToString("dd/MM/yyyy")), "", TIPO.Warning, MessageBoxHandler); validaUpdate = false;
                                    }
                                    else
                                    {
                                        var avaliacaocorrente = existavaliacao.FirstOrDefault(x => x.idPeriodo == periodo.IdPeriodo);
                                        if (avaliacaocorrente != null)
                                        {
                                            if (idassociado != avaliacaocorrente.idAssociado) { MessageBox.Show("O Associado cadastrado já possui avaliação de liderança deste projeto, não será possivel a alteração, utilize a funcionalidade de inativar!", "", TIPO.Warning, MessageBoxHandler); validaUpdate = false; }

                                            if (projetoassociado.DataInicio.Date != data_Inicio_Avaliacao.Date)
                                            {
                                                if (data_Inicio_Avaliacao.Date < periodo.DataInicio.Value.Date || data_Inicio_Avaliacao.Date > periodo.DataFim.Value.Date)
                                                {
                                                    MessageBox.Show("Data de início da avaliação está fora de período de avaliação vigente!", "", TIPO.Warning, MessageBoxHandler);
                                                    validaUpdate = false;
                                                }
                                            }
                                            if (projetoassociado.DataFim.Value.Date != data_Termino_Avaliacao.Date)
                                            {
                                                if (data_Termino_Avaliacao.Date < periodo.DataInicio.Value.Date || data_Termino_Avaliacao.Date > periodo.DataFim.Value.Date)
                                                {
                                                    MessageBox.Show("Data de término da avaliação está fora do período de avaliação vigente!", "", TIPO.Warning, MessageBoxHandler);
                                                    validaUpdate = false;
                                                }
                                            }

                                        }
                                        else { MessageBox.Show("Avaliação fora de período!", "", TIPO.Warning, MessageBoxHandler); validaUpdate = false; }
                                    }
                                }

                                if (validaUpdate)
                                {
                                    projetoassociado.IdAvaliador = idAvaliador;
                                    projetoassociado.IdAssociado = idassociado;
                                    projetoassociado.DataInicio = data_Inicio_Avaliacao;
                                    projetoassociado.DataFim = data_Termino_Avaliacao;
                                    projetoassociado.Escopo = ddl_Escopo.SelectedItem.Text;
                                    projetoassociado.Comentario = text_AvalLideranca_Comentarios.Text;
                                    projetoassociado.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                    new ProjetosService().AlterarProjetoAssociado(projetoassociado);
                                    MessageBox.Show("Alocação de avaliação de liderança alterada com sucesso!", "", TIPO.Info, MessageBoxHandler);
                                    Session["PROJETOSASSOCIADOS_LIDERANCA"] = new List<PROJETOSASSOCIADOS>();
                                    montaListaAssociadosProjeto(projetoassociado.IdProjeto);
                                }
                            }
                            else { MessageBox.Show("Projeto associado não encontrado!", "Erro", TIPO.Warning, MessageBoxHandler); }
                        }
                        else { MessageBox.Show("Avaliação não encontrada!", "Erro", TIPO.Warning, MessageBoxHandler); }
                    }
                    else // INSERE ALOCAÇÃO DE AVALIAÇÃO DE LIDERANÇA
                    {
                        List<PROJETOSASSOCIADOS> alocacoesList = Session["PROJETOSASSOCIADOS_LIDERANCA"] as List<PROJETOSASSOCIADOS>;
                        string comentarios = text_AvalLideranca_Comentarios.Text;
                        if (comentarios == "") comentarios = "Não informado";

                        PROJETOSASSOCIADOS alocacaoLideranca = new PROJETOSASSOCIADOS()
                        {
                            IdProjeto = 0,
                            IdAssociado = Convert.ToInt32(ddl_AvalLideranca_Lider.SelectedValue),
                            IdGestor = Convert.ToInt32(ddl_AvalLideranca_Liderado.SelectedValue),
                            IdAvaliador = 275, // ID DO ASSOCIDADO 'NÃO APLICÁVEL'
                            DataInicio = data_Inicio_Avaliacao,
                            DataFim = data_Termino_Avaliacao,
                            Comentario = comentarios,
                            USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()),
                            DHC = DateTime.Now,
                            ATV = 1,
                            flagNovaAlocacao = true,
                            TipoAvaliacao = "lideranca",
                            Escopo = ddl_Escopo.SelectedItem.Text
                        };

                        if (!ValidaExistenciaAlocacao(alocacaoLideranca, alocacaoLideranca.TipoAvaliacao))
                        {
                            alocacaoLideranca.ASSOCIADOS = new AssociadosService().ObterAssociado(alocacaoLideranca.IdAssociado);
                            alocacaoLideranca.ASSOCIADOS.CARGOS = new CargosService().ObterCargo(alocacaoLideranca.ASSOCIADOS.IdCargo);
                            alocacaoLideranca.ASSOCIADOS2 = new AssociadosService().ObterAssociado(Convert.ToInt32(alocacaoLideranca.IdGestor));

                            alocacoesList.Add(alocacaoLideranca);

                            ddl_AvalLideranca_Lider.SelectedIndex = 0;
                            text_AvalLideranca_Inicio.Text = "";
                            text_AvalLideranca_Termino.Text = "";
                            text_AvalLideranca_Comentarios.Text = "";

                            Session["PROJETOSASSOCIADOS_LIDERANCA"] = alocacoesList;

                            montaListaAssociadosProjeto();

                            MessageBox.Show("Alocação de avaliação de liderança inserida PROVISORIAMENTE. A mesma será efetivada ao FINAL do cadastro do projeto!", "", TIPO.Info, MessageBoxHandler);
                        }
                        else
                        {
                            MessageBox.Show("Alocação de avaliação de liderança já existente ou o período indicado invade alguma outra alocação já cadastrada!", "", TIPO.Info, MessageBoxHandler);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar salvar a avaliação: " + ex.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }

        private bool ValidaExistenciaAlocacao(PROJETOSASSOCIADOS alocacao, string TipoAvaliacao = "desempenho")
        {
            List<PROJETOSASSOCIADOS> alocacoesList = Session["PROJETOSASSOCIADOS"] as List<PROJETOSASSOCIADOS>;
            if (TipoAvaliacao == "lideranca") { alocacoesList = Session["PROJETOSASSOCIADOS_LIDERANCA"] as List<PROJETOSASSOCIADOS>; }

            foreach (PROJETOSASSOCIADOS alocacaoList in alocacoesList)
            {
                if (alocacaoList.IdAssociado == alocacao.IdAssociado &&
                    alocacaoList.IdGestor == alocacao.IdGestor &&
                    ((alocacao.DataInicio >= alocacaoList.DataInicio &&
                    alocacao.DataInicio <= alocacaoList.DataFim) ||
                    (alocacao.DataFim <= alocacaoList.DataFim &&
                    alocacao.DataFim >= alocacaoList.DataInicio)) &&
                    (alocacaoList.flagExcluirAlocacao == false))
                {
                    return true;
                }
            }

            return false;
        }

        protected void rptAlocacoes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ExcluirAssociado")
            {
                var id = ((Label)e.Item.FindControl("lblIdProjetoAssociado"));
                if (int.TryParse(id.Text, out int idprojetoassociado))
                {
                    PROJETOSASSOCIADOS alocacao;
                    alocacao = new ProjetosService().ObterProjetoAssociado(idprojetoassociado);
                    List<PROJETOSASSOCIADOS> associadosList = Session["PROJETOSASSOCIADOS"] as List<PROJETOSASSOCIADOS>;
                    if (alocacao.TipoAvaliacao == "lideranca") { associadosList = Session["PROJETOSASSOCIADOS_LIDERANCA"] as List<PROJETOSASSOCIADOS>; }

                    if (hdIdProjeto.Value == "")
                    {
                        associadosList.RemoveAt(Convert.ToInt32(e.Item.ItemIndex));
                    }
                    else
                    {
                        associadosList[e.Item.ItemIndex].ATV = 0;
                        associadosList[e.Item.ItemIndex].flagExcluirAlocacao = true;
                    }

                    if (alocacao.TipoAvaliacao == "desempenho")
                        Session["PROJETOSASSOCIADOS"] = associadosList;
                    else if (alocacao.TipoAvaliacao == "lideranca")
                        Session["PROJETOSASSOCIADOS_LIDERANCA"] = associadosList;

                    montaListaAssociadosProjeto();

                    MessageBox.Show("Alocação excluída TEMPORARIAMENTE com sucesso. A exclusão final só será efetivada ao final do cadastro !!", "", TIPO.Info, MessageBoxHandler);
                }
                else { MessageBox.Show("Alocação não encontrada.", "", TIPO.Info, MessageBoxHandler); }

            }
            else if (e.CommandName == "EditarAssociado")
            {
                var id = ((Label)e.Item.FindControl("lblIdProjetoAssociado"));

                if (int.TryParse(id.Text, out int idprojetoassociado))
                {
                    PROJETOSASSOCIADOS alocacao;
                    alocacao = new ProjetosService().ObterProjetoAssociado(idprojetoassociado);

                    if (alocacao != null)
                    {
                        if (alocacao.TipoAvaliacao == "desempenho")
                        {
                            var existassociado = ddlAssociadoAlocado.Items.FindByValue(alocacao.IdAssociado.ToString());

                            if (existassociado != null)
                            {
                                ddlAssociadoAlocado.SelectedValue = alocacao.IdAssociado.ToString();
                                txtInicioAlocacao.Text = alocacao.DataInicio.ToString("yyyy-MM-dd");
                                txtTerminoAlocacao.Text = alocacao.DataFim.HasValue ? alocacao.DataFim.Value.ToString("yyyy-MM-dd") : string.Empty;

                                var existavaliador = ddlAvaliadorAlocacao.Items.FindByValue(alocacao.IdAvaliador.ToString());

                                if (existavaliador != null)
                                {
                                    ddlAvaliadorAlocacao.SelectedValue = alocacao.IdAvaliador.ToString();
                                }

                                txtComentariosAlocacao.Text = alocacao.Comentario;
                                hdlIdProjetoAssociado.Value = alocacao.IdProjetoAssociado.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Associado inativo ou não encontrado!", "", TIPO.Warning, MessageBoxHandler);
                            }
                        }
                        else if (alocacao.TipoAvaliacao == "lideranca")
                        {
                            var existassociado = ddl_AvalLideranca_Lider.Items.FindByValue(alocacao.IdAssociado.ToString());

                            if (existassociado != null)
                            {
                                ddl_AvalLideranca_Lider.SelectedValue = alocacao.IdAssociado.ToString();
                                text_AvalLideranca_Inicio.Text = alocacao.DataInicio.ToString("yyyy-MM-dd");
                                text_AvalLideranca_Termino.Text = alocacao.DataFim.HasValue ? alocacao.DataFim.Value.ToString("yyyy-MM-dd") : string.Empty;

                                var existavaliador = ddl_AvalLideranca_Liderado.Items.FindByValue(alocacao.IdGestor.ToString());

                                if (existavaliador != null)
                                {
                                    ddl_AvalLideranca_Liderado.SelectedValue = alocacao.IdGestor.ToString();
                                }

                                text_AvalLideranca_Comentarios.Text = alocacao.Comentario;
                                ddl_Escopo.SelectedValue = ddl_Escopo.Items.FindByText(alocacao.Escopo).Value;
                                hdl_AvalLideranca_Lider.Value = alocacao.IdProjetoAssociado.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Associado inativo ou não encontrado!", "", TIPO.Warning, MessageBoxHandler);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Alocação não encontrada!", "", TIPO.Warning, MessageBoxHandler);
                    }

                }
                else
                {
                    MessageBox.Show("Associado não encontrado!", "", TIPO.Warning, MessageBoxHandler);
                }
            }
        }

        protected void rptProjetos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditarProjeto")
            {
                var idProjeto = ((Label)e.Item.FindControl("lblIdProjeto"));
                hdIdProjeto.Value = Convert.ToString(idProjeto.Text);
                MontaCamposProjetos(Convert.ToInt32(idProjeto.Text));
                WebStorage.Set("idProjeto", idProjeto.Text);

                // RESET CAMPOS DESEMPENHO
                hdlIdProjetoAssociado.Value = string.Empty;
                ddlAssociadoAlocado.SelectedIndex = 0;
                ddlAvaliadorAlocacao.SelectedIndex = 0;
                txtInicioAlocacao.Text = string.Empty;
                txtTerminoAlocacao.Text = string.Empty;
                txtComentariosAlocacao.Text = string.Empty;

                // RESET CAMPOS LIDERANÇA
                hdl_AvalLideranca_Lider.Value = string.Empty;
                ddl_AvalLideranca_Lider.SelectedIndex = 0;
                ddl_AvalLideranca_Liderado.SelectedIndex = 0;
                text_AvalLideranca_Inicio.Text = string.Empty;
                text_AvalLideranca_Termino.Text = string.Empty;
                text_AvalLideranca_Comentarios.Text = string.Empty;
            }
            else
            {
                var idProjeto = ((Label)e.Item.FindControl("lblIdProjeto"));

                var statusExclusao = new ProjetosService().ExcluirProjeto(Convert.ToInt32(idProjeto.Text));

                if (statusExclusao)
                {
                    LimparCampos();
                    MessageBox.Show("Projeto Inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Projeto !!", "", TIPO.Warning, MessageBoxHandler);
                }

            }
        }

        protected void rptProjetos_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

        protected void rptAlocacoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

                var labelCiclo = (Label)e.Item.FindControl("lblCiclo");
                if (labelCiclo != null)
                {
                    var textoCiclo = labelCiclo.Text;
                    if (textoCiclo == "-1")
                    {
                        ((Label)e.Item.FindControl("lblCiclo")).Text = "Próximo";
                    }
                    else if (string.IsNullOrEmpty(textoCiclo))
                    {
                        ((Label)e.Item.FindControl("lblCiclo")).Text = "";
                    }
                    else
                    {
                        var idCiclo = int.Parse(textoCiclo);
                        ((Label)e.Item.FindControl("lblCiclo")).Text = new PeriodoService().ObterPeriodo(idCiclo).Periodo;
                    }
                }
            }
        }

        private void LimparCampos()
        {
            Session["PROJETOSASSOCIADOS"] = new List<PROJETOSASSOCIADOS>();
            Session["PROJETOSASSOCIADOS_LIDERANCA"] = new List<PROJETOSASSOCIADOS>();
            Session["ESTEIRASLOTS"] = new List<EquipeModel>();
            montaComboResponsavel();
            montaComboGestor();
            montaComboCliente();
            montaComboTipo();
            montaComboComplexidade();
            montaComboAssociados();
            montaListaAssociadosProjeto();
            montaComboStatusProjeto();
            MontaListaProjetos();
            montaComboEscopo();
            carregaAlocacaoValidacaoPendente();
            txtCodigo.Text = "";
            txtProjeto.Text = "";
            txtDataInicio.Text = "";
            txtDataTermino.Text = "";
            hdIdProjeto.Value = "";
            hdlIdProjetoAssociado.Value = string.Empty;
            hdl_AvalLideranca_Lider.Value = string.Empty;
            WebStorage.Set("idProjeto", "0");
            var equipeService = new EquipeService();
            equipeService.ResetEquipeProjeto(0);
        }

        #region MONTA COMBOS
        private void MontaCamposProjetos(int idProjeto)
        {
            PROJETOS projeto = new ProjetosService().ObterProjeto(idProjeto);

            txtCodigo.Text = projeto.Codigo;
            txtProjeto.Text = projeto.Projeto;
            ddlCliente.SelectedValue = projeto.IdCliente.ToString();
            txtDataInicio.Text = projeto.DataInicio.ToString("yyyy-MM-dd");
            DateTime dataFinal = Convert.ToDateTime(projeto.DataFim);
            txtDataTermino.Text = dataFinal.ToString("yyyy-MM-dd");

            var existassociadoresp = ddlResponsavel.Items.FindByValue(projeto.IdAssociadoResponsavel.ToString());

            if (existassociadoresp != null)
            {
                ddlResponsavel.SelectedValue = projeto.IdAssociadoResponsavel.ToString();
            }

            ddlGestor.SelectedValue = ddlGestor.Items.FindByValue(projeto.IdAssociadoGestor.ToString()) != null ? projeto.IdAssociadoGestor.ToString() : "[Selecionar]";
            ddlAvaliadorAlocacao.SelectedValue = ddlGestor.Items.FindByValue(projeto.IdAssociadoGestor.ToString()) != null ? projeto.IdAssociadoGestor.ToString() : "[Selecionar]";
            ddlStatusProjeto.SelectedValue = projeto.IdStatus.ToString();
            ddlTipoProjeto.SelectedValue = projeto.IdTipo.ToString();

            var existItem = ddlComplexidade.Items.FindByValue(projeto.IdComplexidade.ToString());

            if (existItem != null)
            {
                ddlComplexidade.SelectedValue = projeto.IdComplexidade.ToString();
            }
            ddlStatus.SelectedValue = projeto.ATV.ToString();

            montaListaAssociadosProjeto(idProjeto);

        }

        public void MontaListaProjetos()
        {
            List<PROJETOS> projetos = new ProjetosService().ObterListaProjetos();

            if (projetos.Count > 0)
                this.rptProjetos.DataSource = projetos;
            else
                this.rptProjetos.DataSource = null;


            this.rptProjetos.DataBind();
        }

        public void montaListaAssociadosProjeto()
        {
            if (Session["PROJETOSASSOCIADOS"] != null)
            {
                List<PROJETOSASSOCIADOS> associados = Session["PROJETOSASSOCIADOS"] as List<PROJETOSASSOCIADOS>;

                this.rptAlocacoes.DataSource = associados;
                this.rptAlocacoes.DataBind();
            }
            else
            {
                this.rptAlocacoes.DataSource = null;
                this.rptAlocacoes.DataBind();
            }

            if (Session["PROJETOSASSOCIADOS_LIDERANCA"] != null)
            {
                List<PROJETOSASSOCIADOS> associadosLideranca = Session["PROJETOSASSOCIADOS_LIDERANCA"] as List<PROJETOSASSOCIADOS>;
                foreach (PROJETOSASSOCIADOS projAssoc in associadosLideranca) { projAssoc.ASSOCIADOS2 = new AssociadosService().ObterAssociado((int)projAssoc.IdGestor); }

                this.rpt_AvalLideranca_Avaliacoes.DataSource = associadosLideranca;
                this.rpt_AvalLideranca_Avaliacoes.DataBind();
            }
            else
            {
                this.rpt_AvalLideranca_Avaliacoes.DataSource = null;
                this.rpt_AvalLideranca_Avaliacoes.DataBind();
            }
        }

        public void montaListaAssociadosProjeto(int idProjeto)
        {
            #region LISTA ALOCAÇÕES DESEMPENHO
            List<PROJETOSASSOCIADOS> associados = new ProjetosService().ObterListaAssociados(true, idProjeto, "desempenho");

            this.rptAlocacoes.DataSource = associados;
            this.rptAlocacoes.DataBind();

            if (associados.Count > 0)
                Session["PROJETOSASSOCIADOS"] = associados;
            else
                Session["PROJETOSASSOCIADOS"] = new List<PROJETOSASSOCIADOS>();
            #endregion
            #region LISTA ALOCAÇÕES LIDERANÇA
            List<PROJETOSASSOCIADOS> associadosLideranca = new ProjetosService().ObterListaAssociados(true, idProjeto, "lideranca");
            foreach (PROJETOSASSOCIADOS projAssoc in associadosLideranca) { projAssoc.ASSOCIADOS2 = new AssociadosService().ObterAssociado((int)projAssoc.IdGestor); }

            this.rpt_AvalLideranca_Avaliacoes.DataSource = associadosLideranca;
            this.rpt_AvalLideranca_Avaliacoes.DataBind();

            if (associadosLideranca.Count > 0)
                Session["PROJETOSASSOCIADOS_LIDERANCA"] = associadosLideranca;
            else
                Session["PROJETOSASSOCIADOS_LIDERANCA"] = new List<PROJETOSASSOCIADOS>();
            #endregion
        }

        public void montaComboResponsavel()
        {
            PERFIS perfil = new PERFIS() { IdPerfil = Convert.ToInt32(Session["IDPERFIL"] = 3.ToString()) };

            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados(perfil);
            this.ddlResponsavel.DataValueField = "IdAssociado";
            this.ddlResponsavel.DataTextField = "Nome";
            this.ddlResponsavel.DataSource = associados;
            this.ddlResponsavel.DataBind();
            this.ddlResponsavel.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboStatusProjeto()
        {
            this.ddlStatusProjeto.DataValueField = "IdStatus";
            this.ddlStatusProjeto.DataTextField = "Status";
            this.ddlStatusProjeto.DataSource = new ProjetosService().ListaStatusProjetos();
            this.ddlStatusProjeto.DataBind();
            this.ddlStatusProjeto.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboGestor()
        {

            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados(true);

            this.ddlGestor.DataValueField = "IdAssociado";
            this.ddlGestor.DataTextField = "Nome";
            this.ddlGestor.DataSource = associados;
            this.ddlGestor.DataBind();
            this.ddlGestor.Items.Insert(0, "[Selecionar]");

            this.ddlAvaliadorAlocacao.DataValueField = "IdAssociado";
            this.ddlAvaliadorAlocacao.DataTextField = "Nome";
            this.ddlAvaliadorAlocacao.DataSource = associados;
            this.ddlAvaliadorAlocacao.DataBind();
            this.ddlAvaliadorAlocacao.Items.Insert(0, "[Selecionar]");

            this.ddl_AvalLideranca_Liderado.DataValueField = "IdAssociado";
            this.ddl_AvalLideranca_Liderado.DataTextField = "Nome";
            this.ddl_AvalLideranca_Liderado.DataSource = associados;
            this.ddl_AvalLideranca_Liderado.DataBind();
            this.ddl_AvalLideranca_Liderado.Items.Insert(0, "[Selecionar]");

        }

        public void montaComboCliente()
        {
            List<CLIENTES> clientes = new ClientesService().ObterListaClientes(true);

            this.ddlCliente.DataSource = clientes;
            this.ddlCliente.DataValueField = "IdCliente";
            this.ddlCliente.DataTextField = "Cliente";
            this.ddlCliente.DataBind();
            this.ddlCliente.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboAssociados()
        {
            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados(true);

            this.ddlAssociadoAlocado.DataValueField = "IdAssociado";
            this.ddlAssociadoAlocado.DataTextField = "Nome";
            this.ddlAssociadoAlocado.DataSource = associados;
            this.ddlAssociadoAlocado.DataBind();
            this.ddlAssociadoAlocado.Items.Insert(0, "[Selecionar]");

            this.ddl_AvalLideranca_Lider.DataValueField = "IdAssociado";
            this.ddl_AvalLideranca_Lider.DataTextField = "Nome";
            this.ddl_AvalLideranca_Lider.DataSource = associados;
            this.ddl_AvalLideranca_Lider.DataBind();
            this.ddl_AvalLideranca_Lider.Items.Insert(0, "[Selecionar]");

            this.ddlEsteiraSlotsAssociado.DataValueField = "IdAssociado";
            this.ddlEsteiraSlotsAssociado.DataTextField = "Nome";
            this.ddlEsteiraSlotsAssociado.DataSource = associados;
            this.ddlEsteiraSlotsAssociado.DataBind();
            this.ddlEsteiraSlotsAssociado.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboTipo()
        {
            List<PROJETOSTIPOS> tipoprojeto = new TipoProjetoService().ObterListaTipos(true);

            this.ddlTipoProjeto.DataValueField = "IdTipo";
            this.ddlTipoProjeto.DataTextField = "ProjetoTipo";
            this.ddlTipoProjeto.DataSource = tipoprojeto;
            this.ddlTipoProjeto.DataBind();
            this.ddlTipoProjeto.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboComplexidade()
        {
            List<PROJETOSCOMPLEXIDADES> projetocomplexidade = new ComplexidadesService().ObterListaComplexidades(true);

            this.ddlComplexidade.DataValueField = "IdComplexidade";
            this.ddlComplexidade.DataTextField = "Complexidade";
            this.ddlComplexidade.DataSource = projetocomplexidade;
            this.ddlComplexidade.DataBind();
            this.ddlComplexidade.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboEscopo()
        {
            this.ddl_Escopo.Items.Clear();
            this.ddl_Escopo.Items.Insert(0, "[Selecionar]");
            this.ddl_Escopo.Items.Insert(1, "projeto");
            this.ddl_Escopo.Items.Insert(2, "líder");
            this.ddl_Escopo.Items.Insert(2, "backoffice");
        }
        #endregion

        [WebMethod]
        public static List<object> GetChartData()
        {
            var equipeService = new EquipeService();
            var associadoService = new AssociadosService();
            var fotosAssociadosServise = new FotosAssociadosService();
            List<object> chartData = new List<object>();

            int idProjeto = int.Parse(WebStorage.Get("idProjeto", "0"));

            List<EQUIPE> getEquipes = equipeService.ObterEquipePorProjeto(idProjeto);
            if (getEquipes.Count <= 0)
            {
                equipeService.ResetEquipeProjeto(idProjeto);
                getEquipes = equipeService.ObterEquipePorProjeto(idProjeto);
            }

            int menorIdEquipe = -1;
            foreach (var equipe in getEquipes)
            {
                if (equipe.idAssociado < menorIdEquipe) { menorIdEquipe = equipe.idAssociado; }
            }
            menorIdEquipe -= 1;
            WebStorage.Set("numAtualVazios", menorIdEquipe.ToString());

            foreach (var equipe in getEquipes)
            {
                #region SLOT ASSOCIADO
                var associado = associadoService.ObterAssociado(equipe.idAssociado);
                var associadoHierarquia = associadoService.ObterAssociado((int)(equipe.idAssociadoHierarquia != null ? equipe.idAssociadoHierarquia : 0));
                var fotoAssociado = fotosAssociadosServise.ObterFotoPorAssociado(equipe.idAssociado);
                string associadoNomeFormatado = associado != null ? associado.Nome.Split(' ')[0] + " " + associado.Nome.Split(' ')[associado.Nome.Split(' ').Count() - 1].Substring(0, 1) + "." : "";
                string respondeDesempenho = (bool)equipe.RespondeAvaliacaoDesempenho ? "checked" : "";
                string respondeLideranca = (bool)equipe.RespondeAvaliacaoLideranca ? "checked" : "";
                string valueRespDesempenho = (bool)equipe.RespondeAvaliacaoDesempenho ? "1" : "0";
                string valueRespLideranca = (bool)equipe.RespondeAvaliacaoLideranca ? "1" : "0";

                string hideSlotAssociado = "";
                string hideSlotNovo = "hidden";
                string corBackground = "#002060";
                string dragSlot = "ondrop=\"drop(event)\" ondragover=\"allowDrop(event)\"";
                string dragAssociado = "draggable=\"true\" ondragstart=\"drag(event)\"";

                chartData.Add(new object[]
                {
                    equipe.idAssociado.ToString(),
                    associadoNomeFormatado,
                    "aaa",
                    equipe.idAssociadoHierarquia.ToString(),
                    fotoAssociado != null ? fotoAssociado.Imagem : "",
                    respondeDesempenho,
                    respondeLideranca,
                    valueRespDesempenho,
                    valueRespLideranca,
                    hideSlotAssociado,
                    hideSlotNovo,
                    corBackground,
                    dragSlot,
                    dragAssociado,
                    equipe.idAssociado > 0 ? true : false
                });
                #endregion
            }
            foreach (var equipe in getEquipes)
            {
                #region SLOT ADD SLOT
                var associado = associadoService.ObterAssociado(equipe.idAssociado);
                var fotoAssociado = fotosAssociadosServise.ObterFotoPorAssociado(equipe.idAssociado);
                string associadoNomeFormatado = associado != null ? associado.Nome.Split(' ')[0] + " " + associado.Nome.Split(' ')[associado.Nome.Split(' ').Count() - 1].Substring(0, 1) + "." : "";
                string respondeDesempenho = (bool)equipe.RespondeAvaliacaoDesempenho ? "checked" : "";
                string respondeLideranca = (bool)equipe.RespondeAvaliacaoLideranca ? "checked" : "";
                string valueRespDesempenho = (bool)equipe.RespondeAvaliacaoDesempenho ? "1" : "0";
                string valueRespLideranca = (bool)equipe.RespondeAvaliacaoLideranca ? "1" : "0";

                string hideSlotAssociado = "hidden";
                string hideSlotNovo = "";
                string corBackground = "#FFC000";
                string dragSlot = "";
                string dragAssociado = "";

                chartData.Add(new object[]
                {
                    "addSlot_" + equipe.idAssociado.ToString(),
                    associadoNomeFormatado,
                    "aaa",
                    equipe.idAssociado.ToString(),
                    fotoAssociado != null ? fotoAssociado.Imagem : "",
                    respondeDesempenho,
                    respondeLideranca,
                    valueRespDesempenho,
                    valueRespLideranca,
                    hideSlotAssociado,
                    hideSlotNovo,
                    corBackground,
                    dragSlot,
                    dragAssociado,
                    true
                });

                #endregion
            }
            return chartData;
        }
        [WebMethod]
        public static int AddNovoSlot(int idAssociadoHierarquia)
        {
            EQUIPE addEquipe = new EQUIPE();
            var equipeService = new EquipeService();
            int idProjeto = int.Parse(WebStorage.Get("idProjeto", "0"));
            int numAtualVazios = int.Parse(WebStorage.Get("numAtualVazios", "0"));

            addEquipe.idAssociado = numAtualVazios;
            addEquipe.idAssociadoHierarquia = idAssociadoHierarquia;
            addEquipe.idProjeto = idProjeto;
            addEquipe.RespondeAvaliacaoDesempenho = false;
            addEquipe.RespondeAvaliacaoLideranca = false;
            addEquipe.DHC = DateTime.Now;
            addEquipe.ATV = true;
            equipeService.AdicionarEquipe(addEquipe);

            numAtualVazios -= 1;
            WebStorage.Set("numAtualVazios", numAtualVazios.ToString());

            return 1;
        }

        protected void btnGerarAvaliacoes_Click(object sender, EventArgs e)
        {
            try
            {
                AtualizaHierarquiaEquipe();
            }
            catch (Exception)
            {
                MessageBox.Show("Erro durante a atualização da hierarquia. Por favor, recarregue a página.", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        public void AtualizaHierarquiaEquipe()
        {
            var dictDraggables = getEstruturaAvaliacoes();
            if (dictDraggables != null && dictDraggables.Count > 0)
            {
                AdicionarHierarquiaNasListasDeAlocacoes(dictDraggables);
                int idProjeto = int.Parse(WebStorage.Get("idProjeto", "0"));
                new EquipeService().AtualizaEquipeProjeto(idProjeto, dictDraggables);
            }
        }
        public Dictionary<int, Dictionary<string, object>> getEstruturaAvaliacoes()
        {
            string htmlCode = hiddenHtmlCode.Value;

            var conteudoTabela = htmlCode.Split(new string[] { "<tbody>" }, StringSplitOptions.None)[1];
            conteudoTabela = conteudoTabela.Split(new string[] { "</tbody>" }, StringSplitOptions.None)[0];

            int numLinhas = conteudoTabela.Split(new string[] { "<tr" }, StringSplitOptions.None).Count() - 1;

            var splitUltimaLinha = conteudoTabela.Split(new string[] { "<tr" }, StringSplitOptions.None)[numLinhas];
            splitUltimaLinha = splitUltimaLinha.Split(new string[] { "colspan=\"" }, StringSplitOptions.None)[1];
            splitUltimaLinha = splitUltimaLinha.Split(new string[] { "\"" }, StringSplitOptions.None)[0];
            int numColunas = int.Parse(splitUltimaLinha);

            Dictionary<int, Dictionary<string, object>> dictDraggables = new Dictionary<int, Dictionary<string, object>>();
            Dictionary<int, Dictionary<string, object>> dictRanges = new Dictionary<int, Dictionary<string, object>>();

            if (numLinhas >= 2)
            {
                // REGISTRA NODES E RANGES
                for (int i = 2; i <= numLinhas; i++)
                {
                    var atualLinha = conteudoTabela.Split(new string[] { "<tr" }, StringSplitOptions.None)[i];
                    atualLinha = atualLinha.Split(new string[] { "</tr>" }, StringSplitOptions.None)[0];
                    int countTDs = atualLinha.Split(new string[] { "<td" }, StringSplitOptions.None).Count() - 1;
                    int atualPosicao = 0;
                    for (int z = 1; z <= countTDs; z++)
                    {
                        var atualColuna = atualLinha.Split(new string[] { "<td" }, StringSplitOptions.None)[z];
                        string atualColunaSpan = "";
                        int colunaSpan = 1;
                        if (atualColuna.Contains("colspan"))
                        {
                            atualColunaSpan = atualColuna.Split(new string[] { "colspan=\"" }, StringSplitOptions.None)[1];
                            atualColunaSpan = atualColunaSpan.Split(new string[] { "\"" }, StringSplitOptions.None)[0];
                            colunaSpan = int.Parse(atualColunaSpan);
                        }

                        // NODE ASSOCIADOS
                        if (atualColuna.Contains("orgchart-node"))
                        {
                            var divSlot = atualColuna.Split(new string[] { "<div id=\"div_" }, StringSplitOptions.None)[1];
                            int divSlotID = int.Parse(divSlot.Split(new string[] { "\"" }, StringSplitOptions.None)[0]);

                            if (divSlot.Contains("<div id=\"drag_"))
                            {
                                #region OBTER DADOS DO DRAGGABLE
                                var divDrag = divSlot.Split(new string[] { "<div id=\"drag_" }, StringSplitOptions.None)[1];
                                if (divDrag.Contains("Add_"))
                                {
                                    string tempDivDrag = "";
                                    int tempCounter = 0;
                                    foreach (var splitPart in divDrag.Split(new string[] { "Add_" }, StringSplitOptions.None))
                                    {
                                        if (tempCounter != 0)
                                        {
                                            tempDivDrag += splitPart;
                                        }
                                        tempCounter += 1;
                                    }
                                    divDrag = tempDivDrag;
                                }
                                int divDragID = int.Parse(divDrag.Split(new string[] { "\"" }, StringSplitOptions.None)[0]);

                                var splitRespondeLideranca = divDrag.Split(new string[] { "<input type=\"checkbox\"" }, StringSplitOptions.None)[1];
                                splitRespondeLideranca = splitRespondeLideranca.Split(new string[] { ">" }, StringSplitOptions.None)[0];
                                splitRespondeLideranca = splitRespondeLideranca.Split(new string[] { "value=\"" }, StringSplitOptions.None)[1];
                                splitRespondeLideranca = splitRespondeLideranca.Split(new string[] { "\"" }, StringSplitOptions.None)[0];
                                bool respondeLideranca = splitRespondeLideranca.Contains("1");

                                var splitRespondeDesempenho = divDrag.Split(new string[] { "<input type=\"checkbox\"" }, StringSplitOptions.None)[2];
                                splitRespondeDesempenho = splitRespondeDesempenho.Split(new string[] { ">" }, StringSplitOptions.None)[0];
                                splitRespondeDesempenho = splitRespondeDesempenho.Split(new string[] { "value=\"" }, StringSplitOptions.None)[1];
                                splitRespondeDesempenho = splitRespondeDesempenho.Split(new string[] { "\"" }, StringSplitOptions.None)[0];
                                bool respondeDesempenho = splitRespondeDesempenho.Contains("1");

                                var splitIdAssociado = divDrag.Split(new string[] { "<div id=\"idAssociado_" }, StringSplitOptions.None)[1];
                                //if (splitIdAssociado.Contains("Add_")) { splitIdAssociado = splitIdAssociado.Split(new string[] { "Add_" }, StringSplitOptions.None)[1]; }
                                splitIdAssociado = splitIdAssociado.Split(new string[] { "</div>" }, StringSplitOptions.None)[0];
                                splitIdAssociado = splitIdAssociado.Split(new string[] { ">" }, StringSplitOptions.None)[1];
                                bool naoNuloAssociado = int.TryParse(splitIdAssociado, out int idAssociado);

                                var splitNome = divDrag.Split(new string[] { "<div style=\"" }, StringSplitOptions.None)[1];
                                splitNome = splitNome.Split(new string[] { "</div>" }, StringSplitOptions.None)[0];
                                splitNome = splitNome.Split(new string[] { ">" }, StringSplitOptions.None)[1];
                                #endregion
                                #region ADICIONAR NO DICTDRAGGABLES
                                if (naoNuloAssociado)
                                {
                                    dictDraggables.Add(dictDraggables.Count + 1, new Dictionary<string, object>());
                                    dictDraggables[dictDraggables.Count].Add("nome", splitNome);
                                    dictDraggables[dictDraggables.Count].Add("respondeLideranca", respondeLideranca);
                                    dictDraggables[dictDraggables.Count].Add("respondeDesempenho", respondeDesempenho);
                                    dictDraggables[dictDraggables.Count].Add("idAssociado", idAssociado);
                                    dictDraggables[dictDraggables.Count].Add("linha", i);
                                    dictDraggables[dictDraggables.Count].Add("minSpan", atualPosicao + 1);
                                    dictDraggables[dictDraggables.Count].Add("maxSpan", atualPosicao + colunaSpan);
                                }
                                #endregion
                            }

                        }
                        // NODE RANGES
                        else if (atualColuna.Contains("orgchart-linenode"))
                        {
                            if (!dictRanges.ContainsKey(i))
                            {
                                dictRanges.Add(i, new Dictionary<string, object>());
                            }

                            string desenhoCelula = "";
                            if (atualColuna.Contains("linebottom") && atualColuna.Contains("lineleft"))
                            {
                                desenhoCelula = "bottom-left";
                            }
                            else if (atualColuna.Contains("linebottom"))
                            {
                                desenhoCelula = "bottom";
                            }
                            else if (atualColuna.Contains("lineleft"))
                            {
                                desenhoCelula = "left";
                            }
                            else if (atualColuna.Contains("lineright"))
                            {
                                desenhoCelula = "right";
                            }
                            else
                            {
                                desenhoCelula = "vazio";
                            }

                            int dictRangeCount = dictRanges[i].Count + 1;
                            for (int y = atualPosicao + 1; y <= dictRangeCount + colunaSpan - 1; y++)
                            {
                                dictRanges[i].Add(y.ToString(), desenhoCelula);
                            }
                        }

                        atualPosicao += colunaSpan;
                    }
                }

                // CONFERE SEQUENCIAMENTO DE NODES
                foreach (var node in dictDraggables)
                {
                    int nodeLinha = int.Parse(dictDraggables[node.Key]["linha"].ToString());
                    int nodeMinSpan = int.Parse(dictDraggables[node.Key]["minSpan"].ToString());
                    int nodeMaxSpan = int.Parse(dictDraggables[node.Key]["maxSpan"].ToString());
                    int conectorGancho = 0;
                    int conectorCol = -1;
                    if (dictRanges.ContainsKey(nodeLinha - 1))
                    {
                        for (int i = nodeMaxSpan; i >= nodeMinSpan; i--)
                        {
                            if (dictRanges[nodeLinha - 1][i.ToString()].ToString() == "left")
                            {
                                conectorGancho = 1;
                                conectorCol = i;
                                i = nodeMinSpan - 1;
                            }
                            else if (dictRanges[nodeLinha - 1][i.ToString()].ToString() == "right")
                            {
                                conectorGancho = -1;
                                conectorCol = i;
                                i = nodeMinSpan - 1;
                            }
                        }

                        bool hieraquiaObtida = false;
                        int conectorColCount = conectorCol;
                        while (conectorColCount > 0 && conectorColCount <= numColunas)
                        {
                            foreach (var hieraquia in dictDraggables)
                            {
                                if (!hieraquiaObtida)
                                {
                                    int hierLinha = int.Parse(dictDraggables[hieraquia.Key]["linha"].ToString());
                                    int hierMinSpan = int.Parse(dictDraggables[hieraquia.Key]["minSpan"].ToString());
                                    int hierMaxSpan = int.Parse(dictDraggables[hieraquia.Key]["maxSpan"].ToString());
                                    int hierIdAssociado = int.Parse(dictDraggables[hieraquia.Key]["idAssociado"].ToString());

                                    if (hierLinha == nodeLinha - 3)
                                    {
                                        if (conectorColCount >= hierMinSpan && conectorColCount <= hierMaxSpan)
                                        {
                                            hieraquiaObtida = true;
                                            dictDraggables[node.Key].Add("associadoHierarquia", hierIdAssociado);
                                            conectorColCount = -1;
                                        }
                                    }
                                }
                            }
                            conectorColCount += conectorGancho;
                        }
                    }

                    if (!dictDraggables[node.Key].ContainsKey("associadoHierarquia"))
                    {
                        dictDraggables[node.Key].Add("associadoHierarquia", null);
                    }
                }
            }

            return dictDraggables;
        }
        public void AdicionarHierarquiaNasListasDeAlocacoes(Dictionary<int, Dictionary<string, object>> dictDraggables)
        {
            var associadosService = new AssociadosService();
            foreach (var slotAssociado in dictDraggables)
            {
                // AVALIAÇÕES DE DESEMPENHO
                #region VALIDAÇÃO
                bool camposOkay = true;
                if (ddlGestor.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha o gestor do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && text_AvalLideranca_Inicio.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de início da avaliação de liderança!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && text_AvalLideranca_Termino.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de término da avaliação de liderança!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && ddl_AvalLideranca_Liderado.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha o liderado que fará a avaliação", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && txtDataInicio.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && txtDataTermino.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && (bool)dictDraggables[slotAssociado.Key]["respondeDesempenho"] == false) { camposOkay = false; }
                //if (camposOkay && ddl_Escopo.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha um escopo", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }

                //DateTime data_Inicio_Projeto = Convert.ToDateTime(txtDataInicio.Text),
                //    data_Termino_Projeto = Convert.ToDateTime(txtDataTermino.Text),
                //    data_Inicio_Avaliacao = Convert.ToDateTime(text_AvalLideranca_Inicio.Text),
                //    data_Termino_Avaliacao = Convert.ToDateTime(text_AvalLideranca_Termino.Text);
                //if (camposOkay && data_Termino_Avaliacao < data_Inicio_Avaliacao) { camposOkay = false; MessageBox.Show("A data de término da avaliação deve ser maior que a data de início da avaliação!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Inicio_Avaliacao > data_Termino_Projeto) { camposOkay = false; MessageBox.Show("A data de início da avaliação não poder ser maior que a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Inicio_Avaliacao < data_Inicio_Projeto) { camposOkay = false; MessageBox.Show("A data de início da avaliação não poder ser menor que a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Termino_Avaliacao < data_Inicio_Projeto) { camposOkay = false; MessageBox.Show("A data de término da avaliação não poder ser menor que a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Termino_Avaliacao > data_Termino_Projeto) { camposOkay = false; MessageBox.Show("A data de término da avaliação não poder ser maior que a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && Session["PROJETOSASSOCIADOS"] == null) { camposOkay = false; Session["PROJETOSASSOCIADOS"] = new List<PROJETOSASSOCIADOS>(); MessageBox.Show("Houve um erro no navegador. Atualize a página e tente novamente.", "Erro", TIPO.Warning, MessageBoxHandler); }
                #endregion
                #region EXECUÇÃO
                if (camposOkay)
                {
                    foreach (var slotAvaliado in dictDraggables)
                    {
                        int slotAvaliadoIdAssociado = dictDraggables[slotAvaliado.Key]["idAssociado"] != null ? (int)dictDraggables[slotAvaliado.Key]["idAssociado"] : 0;
                        int slotAvaliadoIdAssociadoHierarquia = dictDraggables[slotAvaliado.Key]["associadoHierarquia"] != null ? (int)dictDraggables[slotAvaliado.Key]["associadoHierarquia"] : 0;

                        if (slotAvaliadoIdAssociado != 0 && slotAvaliadoIdAssociadoHierarquia != 0)
                        {
                            if (slotAvaliadoIdAssociado != (int)dictDraggables[slotAssociado.Key]["idAssociado"] && slotAvaliadoIdAssociadoHierarquia == (int)dictDraggables[slotAssociado.Key]["idAssociado"])
                            {
                                List<PROJETOSASSOCIADOS> alocacoesList = Session["PROJETOSASSOCIADOS"] as List<PROJETOSASSOCIADOS>;
                                string comentarios = "";
                                if (comentarios == "") comentarios = "Não informado";

                                PROJETOSASSOCIADOS alocacaoDesempenho = new PROJETOSASSOCIADOS()
                                {
                                    IdProjeto = 0,
                                    IdAssociado = slotAvaliadoIdAssociado,
                                    IdGestor = Convert.ToInt32(ddlGestor.SelectedValue),
                                    IdAvaliador = (int)dictDraggables[slotAssociado.Key]["idAssociado"],
                                    DataInicio = Convert.ToDateTime(txtDataInicio.Text),
                                    DataFim = Convert.ToDateTime(txtDataTermino.Text),
                                    Comentario = comentarios,
                                    USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()),
                                    DHC = DateTime.Now,
                                    ATV = 1,
                                    flagNovaAlocacao = true,
                                    TipoAvaliacao = "desempenho",
                                    Escopo = "projeto"
                                };

                                if (!ValidaExistenciaAlocacao(alocacaoDesempenho, alocacaoDesempenho.TipoAvaliacao))
                                {
                                    alocacaoDesempenho.ASSOCIADOS = new AssociadosService().ObterAssociado(alocacaoDesempenho.IdAssociado);
                                    alocacaoDesempenho.ASSOCIADOS.CARGOS = new CargosService().ObterCargo(alocacaoDesempenho.ASSOCIADOS.IdCargo);
                                    alocacaoDesempenho.ASSOCIADOS2 = new AssociadosService().ObterAssociado(Convert.ToInt32(alocacaoDesempenho.IdAvaliador));

                                    alocacoesList.Add(alocacaoDesempenho);
                                    Session["PROJETOSASSOCIADOS"] = alocacoesList;
                                }
                            }
                        }
                    }
                }
                #endregion

                // AVALIAÇÕES DE LIDERANÇA
                #region VALIDAÇÃO
                camposOkay = true;
                //if (ddl_AvalLideranca_Lider.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha o líder que será avaliado!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && text_AvalLideranca_Inicio.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de início da avaliação de liderança!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && text_AvalLideranca_Termino.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de término da avaliação de liderança!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && ddl_AvalLideranca_Liderado.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha o liderado que fará a avaliação", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && txtDataInicio.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && txtDataTermino.Text == "") { camposOkay = false; MessageBox.Show("Preencha a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && dictDraggables[slotAssociado.Key]["associadoHierarquia"] == null) { camposOkay = false; }
                if (camposOkay && (bool)dictDraggables[slotAssociado.Key]["respondeLideranca"] == false) { camposOkay = false; }
                //if (camposOkay && ddl_Escopo.SelectedIndex == 0) { camposOkay = false; MessageBox.Show("Escolha um escopo", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }

                //DateTime data_Inicio_Projeto = Convert.ToDateTime(txtDataInicio.Text),
                //    data_Termino_Projeto = Convert.ToDateTime(txtDataTermino.Text),
                //    data_Inicio_Avaliacao = Convert.ToDateTime(text_AvalLideranca_Inicio.Text),
                //    data_Termino_Avaliacao = Convert.ToDateTime(text_AvalLideranca_Termino.Text);
                //if (camposOkay && data_Termino_Avaliacao < data_Inicio_Avaliacao) { camposOkay = false; MessageBox.Show("A data de término da avaliação deve ser maior que a data de início da avaliação!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Inicio_Avaliacao > data_Termino_Projeto) { camposOkay = false; MessageBox.Show("A data de início da avaliação não poder ser maior que a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Inicio_Avaliacao < data_Inicio_Projeto) { camposOkay = false; MessageBox.Show("A data de início da avaliação não poder ser menor que a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Termino_Avaliacao < data_Inicio_Projeto) { camposOkay = false; MessageBox.Show("A data de término da avaliação não poder ser menor que a data de início do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                //if (camposOkay && data_Termino_Avaliacao > data_Termino_Projeto) { camposOkay = false; MessageBox.Show("A data de término da avaliação não poder ser maior que a data de término do projeto!", "Campos Pendentes", TIPO.Warning, MessageBoxHandler); }
                if (camposOkay && Session["PROJETOSASSOCIADOS_LIDERANCA"] == null) { camposOkay = false; Session["PROJETOSASSOCIADOS_LIDERANCA"] = new List<PROJETOSASSOCIADOS>(); MessageBox.Show("Houve um erro no navegador. Atualize a página e tente novamente.", "Erro", TIPO.Warning, MessageBoxHandler); }
                #endregion
                #region EXECUÇÃO
                if (camposOkay)
                {
                    List<PROJETOSASSOCIADOS> alocacoesList = Session["PROJETOSASSOCIADOS_LIDERANCA"] as List<PROJETOSASSOCIADOS>;
                    string comentarios = "";
                    if (comentarios == "") comentarios = "Não informado";

                    PROJETOSASSOCIADOS alocacaoLideranca = new PROJETOSASSOCIADOS()
                    {
                        IdProjeto = 0,
                        IdAssociado = (int)dictDraggables[slotAssociado.Key]["associadoHierarquia"],
                        IdGestor = (int)dictDraggables[slotAssociado.Key]["idAssociado"],
                        IdAvaliador = associadosService.ObterAssociadoPeloNome("Não se aplica").IdAssociado,
                        DataInicio = Convert.ToDateTime(txtDataInicio.Text),
                        DataFim = Convert.ToDateTime(txtDataTermino.Text),
                        Comentario = comentarios,
                        USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString()),
                        DHC = DateTime.Now,
                        ATV = 1,
                        flagNovaAlocacao = true,
                        TipoAvaliacao = "lideranca",
                        Escopo = "projeto"
                    };

                    if (!ValidaExistenciaAlocacao(alocacaoLideranca, alocacaoLideranca.TipoAvaliacao))
                    {
                        alocacaoLideranca.ASSOCIADOS = new AssociadosService().ObterAssociado(alocacaoLideranca.IdAssociado);
                        alocacaoLideranca.ASSOCIADOS.CARGOS = new CargosService().ObterCargo(alocacaoLideranca.ASSOCIADOS.IdCargo);
                        alocacaoLideranca.ASSOCIADOS2 = new AssociadosService().ObterAssociado(Convert.ToInt32(alocacaoLideranca.IdGestor));

                        alocacoesList.Add(alocacaoLideranca);
                        Session["PROJETOSASSOCIADOS_LIDERANCA"] = alocacoesList;
                    }
                }
                #endregion
            }

            montaListaAssociadosProjeto();
            MessageBox.Show("Alocações inseridas PROVISORIAMENTE. As mesmas será efetivada ao FINAL do cadastro do projeto!", "", TIPO.Info, MessageBoxHandler);
        }

        protected void btnAddSlot_Click(object sender, EventArgs e)
        {
            List<EquipeModel> esteiraSlots = Session["ESTEIRASLOTS"] as List<EquipeModel>;
            if (ddlEsteiraSlotsAssociado.SelectedIndex != 0)
            {
                var associadoService = new AssociadosService();
                var fotosAssociadosServise = new FotosAssociadosService();
                var associado = associadoService.ObterAssociado(int.Parse(ddlEsteiraSlotsAssociado.SelectedItem.Value));
                var fotosAssociado = fotosAssociadosServise.ObterFotoPorAssociado(int.Parse(ddlEsteiraSlotsAssociado.SelectedItem.Value));
                if (associado != null)
                {
                    EquipeModel addEquipeModel = new EquipeModel();
                    addEquipeModel.idEquipe = 0;
                    addEquipeModel.idAssociado = associado.IdAssociado;
                    addEquipeModel.nome = associado.Nome.Split(' ')[0] + " " + associado.Nome.Split(' ')[associado.Nome.Split(' ').Count() - 1].Substring(0, 1) + ".";
                    addEquipeModel.idAssociadoHierarquia = 0;
                    addEquipeModel.RespondeAvaliacaoDesempenho = true;
                    addEquipeModel.RespondeAvaliacaoLideranca = true;
                    addEquipeModel.i = esteiraSlots.Count;
                    addEquipeModel.FotoNome = fotosAssociado.Imagem;
                    esteiraSlots.Add(addEquipeModel);
                    Session["ESTEIRASLOTS"] = esteiraSlots;
                }
            }
            rptEsteiraSlots.DataSource = esteiraSlots;
            rptEsteiraSlots.DataBind();
        }

        protected void btnAtualizaEquipe_Click(object sender, EventArgs e)
        {
            try
            {
                var dictDraggables = getEstruturaAvaliacoes();
                if (dictDraggables != null && dictDraggables.Count > 0)
                {
                    int idProjeto = int.Parse(WebStorage.Get("idProjeto", "0"));
                    new EquipeService().AtualizaEquipeProjeto(idProjeto, dictDraggables);
                }
            }
            catch (Exception)
            {
            }
        }

        protected void btnExportProjetos_Click(object sender, EventArgs e)
        {
            List<ProjetoModelExport> listaExport = new List<ProjetoModelExport>();
            var projetosService = new ProjetosService();
            var getProjetos = projetosService.ObterListaProjetos();
            var complexidadeService = new ComplexidadesService();

            foreach (var item in getProjetos)
            {
                ProjetoModelExport addProjeto = new ProjetoModelExport();
                addProjeto.IdProjeto = item.IdProjeto;
                addProjeto.Projeto = item.Projeto;
                addProjeto.Codigo = item.Codigo;
                addProjeto.IdEmpresa = item.IdEmpresa;
                addProjeto.Empresa = item.EMPRESAS.Empresa;
                addProjeto.IdCliente = item.IdCliente;
                addProjeto.Cliente = item.CLIENTES.Cliente;
                addProjeto.IdResponsavel = item.IdAssociadoResponsavel;
                addProjeto.Responsavel = item.ASSOCIADOS.Nome;
                addProjeto.IdGestor = item.IdAssociadoGestor;
                addProjeto.Gestor = item.ASSOCIADOS1.Nome;
                addProjeto.IdStatus = item.IdStatus;
                addProjeto.Status = item.PROJETOSSTATUS.Status;
                addProjeto.IdTipo = item.IdTipo;
                addProjeto.Tipo = item.PROJETOSTIPOS.ProjetoTipo;
                addProjeto.IdComplexidade = item.IdComplexidade;
                addProjeto.Complexidade = item.PROJETOSCOMPLEXIDADES.Complexidade;
                addProjeto.DataInicio = item.DataInicio;
                addProjeto.DataTermino = item.DataFim;
                addProjeto.ATV = item.ATV;

                // VERIFICA SE TODAS AS COMPLEXIDADES FORAM VALIDADAS COM O MD
                var getFatoresProjeto = complexidadeService.ObterFatoresProjetos(idProjeto: item.IdProjeto);
                var countPendencia = 0;
                addProjeto.FatoresComplexidadeValidadosMD = "Pendente";
                foreach (var itemFatorProjeto in getFatoresProjeto)
                {
                    if (itemFatorProjeto.ValidGestor == null && itemFatorProjeto.ValidResponsavel == null)
                    {
                        countPendencia += 1;
                    }
                    else if (itemFatorProjeto.ValidResponsavel == false)
                    {
                        countPendencia += 1;
                    }
                    else if (itemFatorProjeto.ValidGestor == false)
                    {
                        countPendencia += 1;
                    }
                }
                if (countPendencia <= 0 && getFatoresProjeto.Count > 1)
                {
                    addProjeto.FatoresComplexidadeValidadosMD = "Validado";
                }

                listaExport.Add(addProjeto);
            }
            string fileName = "Projeto_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listaExport);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        protected void btnImportProjetos_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUploadProjetos.HasFile)
                {
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUploadProjetos.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            MessageBox.Show("O arquivo Excel não possui nenhuma planilha.", "", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        var projetosService = new ProjetosService();
                        int rowCount = worksheet.Dimension.End.Row;
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            int IdProjeto = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                            string Projeto = worksheet.Cells[row, 2].GetValue<string>() ?? "-";
                            string Codigo = worksheet.Cells[row, 3].GetValue<string>() ?? "-";
                            int IdEmpresa = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                            int IdCliente = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                            int IdResponsavel = worksheet.Cells[row, 8].GetValue<int?>() ?? 0;
                            int IdGestor = worksheet.Cells[row, 10].GetValue<int?>() ?? 0;
                            int IdStatus = worksheet.Cells[row, 12].GetValue<int?>() ?? 1;
                            int IdTipo = worksheet.Cells[row, 14].GetValue<int?>() ?? 0;
                            int IdComplexidade = worksheet.Cells[row, 16].GetValue<int?>() ?? 0;
                            DateTime? DataInicio = worksheet.Cells[row, 19].GetValue<DateTime?>();
                            DateTime? DataTermino = worksheet.Cells[row, 20].GetValue<DateTime?>();
                            int ATV = worksheet.Cells[row, 21].GetValue<int?>() ?? 1;
                            if (Projeto != "-" && IdEmpresa > 0 && IdCliente > 0 && IdResponsavel > 0 && IdGestor > 0 && IdTipo > 0 && IdComplexidade > 0 && DataInicio != null && DataTermino != null)
                            {
                                PROJETOS importItem = new PROJETOS();
                                importItem.Projeto = Projeto;
                                importItem.Codigo = Codigo;
                                importItem.IdEmpresa = IdEmpresa;
                                importItem.IdCliente = IdCliente;
                                importItem.IdAssociadoResponsavel = IdResponsavel;
                                importItem.IdAssociadoGestor = IdGestor;
                                importItem.IdStatus = IdStatus;
                                importItem.IdTipo = IdTipo;
                                importItem.IdComplexidade = IdComplexidade;
                                importItem.DataInicio = (DateTime)DataInicio;
                                importItem.DataFim = DataTermino;
                                importItem.ATV = ATV;
                                importItem.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                importItem.DHC = DateTime.Now;
                                var existeItem = projetosService.ObterProjetoNome(Projeto);
                                if (existeItem == null)
                                {
                                    projetosService.InserirProjeto(importItem);
                                    somaLinhasInseridas += 1;
                                }
                                else
                                {
                                    importItem.IdProjeto = existeItem.IdProjeto;
                                    projetosService.AlterarProjeto(importItem);
                                    somaLinhasAlteradas += 1;
                                }
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }
                        MessageBox.Show("Projetos importados com sucesso<br>Inseridos: " + somaLinhasInseridas.ToString() + "<br>Alterados: " + somaLinhasAlteradas.ToString() +
                            "<br>Desconsiderados: " + somaLinhasDesconsideradas.ToString(), "", TIPO.Info, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo selecionado", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }

        protected void btnExportAssociacoes_Click(object sender, EventArgs e)
        {
            List<AssociacoesModelExport> listaExport = new List<AssociacoesModelExport>();
            var projetosService = new ProjetosService();
            var getProjetos = projetosService.ObterListaProjetosAssociadosTodos();

            foreach (var item in getProjetos)
            {
                AssociacoesModelExport addItem = new AssociacoesModelExport();
                addItem.IdAssociacao = item.IdProjetoAssociado;
                addItem.IdProjeto = item.IdProjeto;
                addItem.Projeto = item.PROJETOS.Projeto;
                addItem.IdAssociado = item.IdAssociado;
                addItem.Associado = item.ASSOCIADOS.Nome;
                addItem.IdAvaliador = item.IdAvaliador;
                addItem.Avaliador = item.ASSOCIADOS2.Nome;
                addItem.IdGestor = item.IdGestor;
                addItem.Gestor = item.ASSOCIADOS3.Nome;
                addItem.InicioAlocacao = item.DataInicio;
                addItem.TerminoAlocacao = item.DataFim;
                addItem.TipoAvaliacao = item.TipoAvaliacao;
                addItem.Escopo = item.Escopo;
                addItem.Comentario = item.Comentario;
                addItem.ATV = item.ATV;
                listaExport.Add(addItem);
            }

            string fileName = "Associações_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listaExport);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        protected void btnImportAssociacoes_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUploadAssociacoes.HasFile)
                {
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUploadAssociacoes.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            MessageBox.Show("O arquivo Excel não possui nenhuma planilha.", "", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        var mainService = new ProjetosService();
                        int rowCount = worksheet.Dimension.End.Row;
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            int IdAssociacao = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                            int IdProjeto = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                            int IdAssociado = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                            int IdAvaliador = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                            int IdGestor = worksheet.Cells[row, 8].GetValue<int?>() ?? 0;
                            DateTime? DataInicio = worksheet.Cells[row, 10].GetValue<DateTime?>();
                            DateTime? DataTermino = worksheet.Cells[row, 11].GetValue<DateTime?>();
                            string TipoAvaliacao = worksheet.Cells[row, 12].GetValue<string>() ?? "desempenho";
                            string Escopo = worksheet.Cells[row, 13].GetValue<string>() ?? "projeto";
                            string Comentario = worksheet.Cells[row, 14].GetValue<string>() ?? "-";
                            int ATV = worksheet.Cells[row, 15].GetValue<int?>() ?? 1;
                            // ALOCAÇÕES PARA VALIDAÇÃO DO GESTOR
                            if (IdAvaliador == 0)
                            {
                                if (TipoAvaliacao == "desempenho")
                                {
                                    IdAvaliador = new AssociadosService().ObterAssociadoPeloNome("Validação Gestor").IdAssociado;
                                }
                                else if (TipoAvaliacao == "lideranca")
                                {
                                    IdAvaliador = new AssociadosService().ObterAssociadoPeloNome("Não se aplica").IdAssociado;
                                }
                            }
                            if (IdProjeto > 0 && IdAssociado > 0 && IdAvaliador > 0 && IdGestor > 0 && DataInicio != null && DataTermino != null)
                            {
                                PROJETOSASSOCIADOS importItem = new PROJETOSASSOCIADOS();
                                importItem.IdProjeto = IdProjeto;
                                importItem.IdAssociado = IdAssociado;
                                importItem.IdAvaliador = IdAvaliador;
                                importItem.IdGestor = IdGestor;
                                importItem.DataInicio = (DateTime)DataInicio;
                                importItem.DataFim = DataTermino;
                                importItem.TipoAvaliacao = TipoAvaliacao;
                                importItem.Escopo = Escopo;
                                importItem.Comentario = Comentario;
                                importItem.ATV = ATV;
                                importItem.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                importItem.DHC = DateTime.Now;
                                var existeItem = mainService.ObterListaAssociados(IdProjeto, IdAssociado, IdGestor, TipoAvaliacao, Escopo);
                                if (existeItem.Count == 0)
                                {
                                    mainService.InserirProjetoAssociado(importItem);
                                    somaLinhasInseridas += 1;
                                }
                                else
                                {
                                    importItem.IdProjetoAssociado = existeItem[0].IdProjetoAssociado;
                                    mainService.AlterarProjetoAssociado(importItem);
                                    somaLinhasAlteradas += 1;
                                }
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }
                        MessageBox.Show("Associações importadas com sucesso<br>Inseridos: " + somaLinhasInseridas.ToString() + "<br>Alterados: " + somaLinhasAlteradas.ToString() +
                            "<br>Desconsiderados: " + somaLinhasDesconsideradas.ToString(), "", TIPO.Info, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo selecionado", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }

        // ALOCAÇÕES COM VALIDAÇÃO PENDENTE
        public void carregaAlocacaoValidacaoPendente()
        {
            var projetoService = new ProjetosService();
            var periodosService = new PeriodoService();
            var associadosService = new AssociadosService();

            var ultimoPeriodo = periodosService.ObterPeriodoUltimo();
            var alocacoes = projetoService.ObterListaAssociadosNoPeriodo(-1, -1, ultimoPeriodo);
            var dummyValidacao = associadosService.ObterAssociadoPeloNome("Validação Gestor");

            var alocacoesValidacaoPendente = alocacoes.Where(x =>
                x.IdAssociado == dummyValidacao.IdAssociado ||
                x.IdAvaliador == dummyValidacao.IdAssociado ||
                x.IdGestor == dummyValidacao.IdAssociado).ToList();

            if (alocacoesValidacaoPendente != null && alocacoesValidacaoPendente.Count == 0)
            {
                lblValidaAlocacaoPendente.Text = "Não há alocações com pendência de validação pelo gestor para o ciclo " + ultimoPeriodo.Periodo + ".";
                btnNotificarValidaAlocacao.Enabled = false;
            }
            else
            {
                lblValidaAlocacaoPendente.Text = "Há " + alocacoesValidacaoPendente.Count.ToString() + " validações pendentes pelos gestores para o ciclo " + ultimoPeriodo.Periodo + "!";
                btnNotificarValidaAlocacao.Enabled = true;
            }

        }

        protected async void btnNotificarValidaAlocacao_Click(object sender, EventArgs e)
        {
            var bulkEmailService = new EmailBulk();

            var projetoService = new ProjetosService();
            var periodosService = new PeriodoService();
            var associadosService = new AssociadosService();
            var paramEmail = new EmailParametroService().ObterParametro(1);

            var ultimoPeriodo = periodosService.ObterPeriodoUltimo();
            var alocacoes = projetoService.ObterListaAssociadosNoPeriodo(-1, -1, ultimoPeriodo);
            var dummyValidacao = associadosService.ObterAssociadoPeloNome("Validação Gestor");

            var alocacoesValidacaoPendente = alocacoes.Where(x =>
                x.IdAssociado == dummyValidacao.IdAssociado ||
                x.IdAvaliador == dummyValidacao.IdAssociado ||
                x.IdGestor == dummyValidacao.IdAssociado).ToList();

            var gestoresIds = alocacoesValidacaoPendente.Select(x => x.IdGestor).Distinct().ToList();
            var associadosTodos = associadosService.ObterAssociados();
            var gestores = associadosTodos.Where(x => gestoresIds.Contains(x.IdAssociado)).Distinct().ToList();

            // TESTE BULK
            //var periodoTeste = periodosService.ObterPeriodo(12);
            //var alocacoesTeste = projetoService.ObterListaAssociadosNoPeriodo(-1, -1, periodoTeste);
            //gestoresIds = alocacoesTeste.Select(x => x.IdGestor).Distinct().ToList();
            //associadosTodos = associadosService.ObterAssociados();
            //gestores = associadosTodos.Where(x => gestoresIds.Contains(x.IdAssociado)).Distinct().ToList();

            bulkEmailService.bulkEmailData = new List<EmailData>();

            foreach (var gestor in gestores)
            {
                string textoAlocacoes = "";
                var gestorAlocacoes = alocacoesValidacaoPendente.Where(x => x.IdGestor == gestor.IdAssociado && x.TipoAvaliacao == "desempenho").ToList();
                foreach (var alocacao in gestorAlocacoes)
                {
                    var associadoAlocado = associadosService.ObterAssociado(alocacao.IdAssociado);
                    textoAlocacoes += "&emsp;&emsp;" + associadoAlocado.Nome + ", Projeto: " + alocacao.PROJETOS.Projeto + "<br/>";
                }

                textoAlocacoes += "<br/><br/>";

                var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
                _corpo = ConfiguraBody_ResultadoLideranca(_corpo, ultimoPeriodo.Periodo, gestor.Nome, textoAlocacoes);

                var emailData = new EmailData();
                emailData.emailTo = gestor.Email;
                emailData.subject = "[RH Peers] - Alocações - Pendência de Validação - Ciclo: " + ultimoPeriodo.Periodo;
                emailData.body = _corpo;

                bulkEmailService.bulkEmailData.Add(emailData);
            }

            await bulkEmailService.Send();

            MessageBox.Show("As notificações estão sendo enviadas. Você pode sair desta tela agora.", "Sucesso", TIPO.Info, MessageBoxHandler);
        }

        private string ConfiguraBody_ResultadoLideranca(string corpo, string periodo, string nomeGestor, string alocacoes)
        {
            // Dados Pessoais
            var newCorpo = corpo.Replace("[NOME]", nomeGestor);
            newCorpo = newCorpo.Replace("[DESCRICAO]", "Você possui alocações que necessitam de sua validação!");

            // ALTERAR MENSAGENS PADRÃO
            newCorpo = newCorpo.Replace("http://avaliacao.peers.com.br/login", "http://avaliacao.peers.com.br/GerenciarProjetos");
            newCorpo = newCorpo.Replace("Link para acesso ao Sistema de Avaliação", "Link para acesso à página (necessário estar logado)");

            // REMOVER CAMPOS NÃO UTILIZADOS
            newCorpo = newCorpo.Replace("<p>Seguem informações do processo de avaliação.</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Etapa atual:</strong> [ETAPA_AVALIACAO]</p>", "");
            newCorpo = newCorpo.Replace("<p style=\"color:#ff0000;\"><strong>Prazo para finalização da [ETAPA_AVALIACAO]:</strong> [PRAZO_FINAL]</p>",
                "<p style=\"color:#ff0000;\"><strong>Alocações com validação pendente:</strong></p>");
            newCorpo = newCorpo.Replace("<p><strong>Projeto:</strong> [PROJETO]</p>", alocacoes);
            newCorpo = newCorpo.Replace("<p><strong>Associado:</strong> [NOME_AVALIADO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Cargo:</strong> [CARGO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Mentor:</strong> [MENTOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Avaliador:</strong> [AVALIADOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Gestor do Projeto:</strong> [GESTOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Início:</strong> [DATA_INICIO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Término:</strong> [DATA_FINAL]</p>", "");

            return newCorpo;
        }

        protected void btnExportFatores_Click(object sender, EventArgs e)
        {
            var listaExport = new ComplexidadesService().ObterExportFatores();
            string fileName = "FatoresComplexidade_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listaExport);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
    }
}