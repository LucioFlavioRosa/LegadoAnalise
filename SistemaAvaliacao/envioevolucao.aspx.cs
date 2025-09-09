using Business.DataAccess;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tria.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public class VerticalType
    {
        public string Descricao { get; set; }
        public string IdVertical { get; set; }
    }

    public partial class envioevolucao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboAssociados();
                CarregaComboPeriodos();
                CarregaComboVerticais();
            }
        }

        private void CarregaComboPeriodos()
        {
            var statusList = new PeriodoService().ListaPeriodos(WebStorage.GetUsuarioLogado().IdEmpresa);
            ddlPeriodos.DataValueField = "IdPeriodo";
            ddlPeriodos.DataTextField = "Periodo";
            ddlPeriodos.DataSource = statusList.OrderByDescending(x => x.DataFim.Value);
            ddlPeriodos.DataBind();
        }

        private void CarregaComboVerticais()
        {
            var verticalList = new VerticalService().ListarVerticais();

            ddlEmpresa.DataValueField = "IdVertical";
            ddlEmpresa.DataTextField = "Descricao";
            ddlEmpresa.DataSource = verticalList;
            ddlEmpresa.DataBind();
            ddlEmpresa.Items.Insert(0, "[Selecionar]");
        }

        protected void btnGerarTodos_Click(object sender, EventArgs e)
        {
            if (ddlEmpresa.Items[ddlEmpresa.SelectedIndex].Value != "0")
            {
                int idVertical = Convert.ToInt32(ddlEmpresa.Items[ddlEmpresa.SelectedIndex].Value);

                try
                {
                    int idperiodo = Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value);
                    var listEvolucoesPeriodo = new EvolucaoAssociadoServices().ListAssociadosEvolucao(null, idperiodo, idVertical);

                    // Filtrando vertical Actar provisóriamente
                    // var listEvolucoesPeriodoActar = listEvolucoesPeriodo.ToList();
                    //.Where(x => !x.Vertical.Trim().Equals(ddlEmpresa.Items[ddlEmpresa.SelectedIndex].Value, StringComparison.OrdinalIgnoreCase)).ToList();

                    var avaliacoesService = new AvaliacoesService();
                    var competenciasService = new CompetenciasService();
                    var performancesService = new PerformancesService();

                    if (listEvolucoesPeriodo != null)
                    {
                        if (!chkReenviar.Checked)
                        {
                            listEvolucoesPeriodo = listEvolucoesPeriodo.Where(x => x.Enviado == "Não").ToList();
                        }

                        var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
                        var listassociados = new AssociadosService().ObterAssociados();
                        var listperiodos = new PeriodoService().ListaPeriodos(idEmpresa);

                        var getAvaliacoes = avaliacoesService.ObterListaAvaliacao();
                        var getCompetencias = competenciasService.ObterListaCompetencias();
                        var getPerformances = performancesService.ObterListaPerformances();
                        var getAvPerformances = avaliacoesService.ObterAvaliacaoPerformanceTodos(-1, -1, -1);
                        var getAvCompetencias = avaliacoesService.ListaAvCompetencias();
                        var verticalDescription = new VerticalService().ListarVerticais().Where(v => v.IDVERTICAL == idVertical).ToList()[0].DESCRICAO;

                        foreach (var item in listEvolucoesPeriodo)
                        {
                            new EvolucaoAssociadoServices().GerarEvolucaoAssociado_Otimizado(getAvaliacoes, getAvCompetencias, getAvPerformances, getCompetencias, getPerformances,
                                item.IdAssociado, item.IdPeriodo, item.IdCargo, item.TipoAvaliacao, item.Escopo);

                            var associado = listassociados.FirstOrDefault(x => x.IdAssociado == item.IdAssociado);
                            var periodo = listperiodos.FirstOrDefault(x => x.IdPeriodo == item.IdPeriodo);
                            // EnvioEmail(associado, periodo);

                        }
                        if (listEvolucoesPeriodo.Count() > 0)
                        {
                            MessageBox.Show("<p>Evolução da Avaliação Gerada e Enviada com Sucesso.</p>Vertical:  " + verticalDescription + " <p></p><p> Quantidade enviado:   " + listEvolucoesPeriodo.Count() + "</p>", "Evolução", TIPO.Info, MessageBoxHandler);
                        }
                        else
                        {
                            MessageBox.Show("<p>As Evoluções da Avaliação já foram geradas e enviadas.</p>Vertical:  " + verticalDescription + " <p></p>", "Evolução", TIPO.Info, MessageBoxHandler);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Não foram encontrados avaliações para o último período vigente.", "Info", TIPO.Info, MessageBoxHandler);
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Erro ao tentar enviar todas as evoluções: " + ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
                }
            }
            else
            {
                MessageBox.Show("O campo Vertical deve ser preenchido!", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if(ddlEmpresa.Items[ddlEmpresa.SelectedIndex].Value != "[Selecionar]")
            {
                try
                {
                    int? idassociado = null;

                    if (ddlAssociados.SelectedIndex > 0)
                    {
                        idassociado = Convert.ToInt32(ddlAssociados.Items[ddlAssociados.SelectedIndex].Value);
                    }

                    int idperiodo = Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value);
                    int idVertical = Convert.ToInt32(ddlEmpresa.Items[ddlEmpresa.SelectedIndex].Value);

                    rptProjetos.DataSource = new EvolucaoAssociadoServices().ListAssociadosEvolucao(idassociado, idperiodo, idVertical);
                    rptProjetos.DataBind();

                    SecEnvio.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao tentar enviar a evolução selecionada: " + ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
                }
            }
            else
            {
                MessageBox.Show("O campo Vertical deve ser preenchido!", "", TIPO.Warning, MessageBoxHandler);
            }
        }

        protected void btnGerarUnico_Click(object sender, EventArgs e)
        {

            try
            {
                string listIds = (sender as System.Web.UI.WebControls.LinkButton).CommandArgument;
                string[] vtrIds = listIds.Split(';');

                int idAssociado = int.Parse(vtrIds[0]);
                int idPeriodo = int.Parse(vtrIds[1]);
                int idCargo = int.Parse(vtrIds[2]);
                string strTipoAvaliacao = vtrIds[3];
                string strEscopo = vtrIds[4];

                var associado = new AssociadosService().ObterAssociado(idAssociado);
                var periodo = new PeriodoService().ObterPeriodo(idPeriodo);

                var avaliacoesService = new AvaliacoesService();
                var competenciasService = new CompetenciasService();
                var performancesService = new PerformancesService();
                var getAvaliacoes = avaliacoesService.ObterListaAvaliacao();
                var getCompetencias = competenciasService.ObterListaCompetencias();
                var getPerformances = performancesService.ObterListaPerformances();
                var getAvPerformances = avaliacoesService.ObterAvaliacaoPerformanceTodos(-1, -1, -1);
                var getAvCompetencias = avaliacoesService.ListaAvCompetencias();

                new EvolucaoAssociadoServices().GerarEvolucaoAssociado_Otimizado(getAvaliacoes, getAvCompetencias, getAvPerformances, getCompetencias, getPerformances,
                    idAssociado, idPeriodo, idCargo, strTipoAvaliacao, strEscopo);
                EnvioEmail(associado, periodo);
                MessageBox.Show("Evolução da Avaliação Gerada e Enviada com Sucesso.", "Enviar Avaliação", TIPO.Info, MessageBoxHandler);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar enviar a evolução ao associado: " + ex.Message, "Erro", TIPO.Error, MessageBoxHandler);
            }

        }


        private void EnvioEmail(ASSOCIADOS associado, PERIODOSAVALIACOES periodo)
        {

            try
            {
                var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

                var paramEmail = new EmailParametroService().ObterParametro(idEmpresa);
                var configEmail = new ConfigEmail();
                configEmail.From = paramEmail.RemetenteEmail;
                configEmail.SmtpServer = paramEmail.SMTPServer;
                configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
                configEmail.Dominio = paramEmail.Dominio;
                configEmail.Senha = paramEmail.Password;
                configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
                configEmail.Remetente = paramEmail.RemetenteNome;

                var destinatario = new Destinatario
                {
                    Nome = associado.Nome,
                    Email = associado.Email
                };

                // MONTA O EMAIL
                var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailEvolucao);
                _corpo = ConfiguraBody(_corpo, associado, periodo);

                var mensagem = new Mensagem
                {
                    Titulo = "[RH Peers] - Resultado da Avaliação - Período: " + periodo.Periodo,
                    Corpo = _corpo
                };

                var emailService = new EmailService(configEmail, destinatario, mensagem);

                var thread = new Thread(new ThreadStart(emailService.Enviar));
                thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception)
            {
            }

        }


        private string ConfiguraBody(string corpo, ASSOCIADOS associado, PERIODOSAVALIACOES periodo)
        {

            // Dados Pessoais
            var newCorpo = corpo.Replace("[NOME]", associado.Nome);
            newCorpo = newCorpo.Replace("[CARGO]", associado.CARGOS.Cargo);
            newCorpo = newCorpo.Replace("[MENTOR]", associado.ASSOCIADOS2.Nome);
            newCorpo = newCorpo.Replace("[DATA_INICIO]", periodo.DataInicio.Value.ToString("dd/MM/yyyy"));
            newCorpo = newCorpo.Replace("[DATA_FINAL]", periodo.DataFim?.ToString("dd/MM/yyyy"));

            return newCorpo;
        }

        private void CarregaComboAssociados()
        {
            var profissionalList = new AssociadosService().ObterAssociados(true);
            ddlAssociados.DataValueField = "IdAssociado";
            ddlAssociados.DataTextField = "Nome";
            ddlAssociados.DataSource = profissionalList;
            ddlAssociados.DataBind();
            ddlAssociados.Items.Insert(0, "[Selecionar]");
        }
    }
}