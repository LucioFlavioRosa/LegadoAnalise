using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Tria.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class avalizacao_consolidacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = WebStorage.GetUsuarioLogado();

            if (user == null || !user.IsLogged)
            {
                Response.Redirect("~/Login");
            }
            else
            {
                if (user.IdPerfil < 3)
                {
                    Response.Redirect("~/Index");
                }
            }

            if (!IsPostBack)
            {
                var strProjeto = Request.QueryString["IdProjeto"];
                var strAssociado = Request.QueryString["IdAssociado"];
                var strPeriodo = Request.QueryString["IdPeriodo"];
                var strTipoAvaliacao = Request.QueryString["TipoAvaliacao"];
                var strEscopo = Request.QueryString["Escopo"];

                if (!int.TryParse(strProjeto, out int idProjeto))
                {
                    strProjeto = WebStorage.Get("IdProjeto", "");

                    if (!int.TryParse(strProjeto, out idProjeto))
                    {
                        MessageBox.Show("É obrigatório a seleção de um Projeto.", "Projeto Não Encontrado", TIPO.Info, MessageBoxHandler);
                        return;
                    }
                }

                if (!int.TryParse(strAssociado, out int idAssociado))
                {
                    strAssociado = WebStorage.Get("IdAssociado", "");

                    if (!int.TryParse(strAssociado, out idAssociado))
                    {
                        MessageBox.Show("É obrigatório a seleção de um Associado.", "Associado Não Encontrado", TIPO.Info, MessageBoxHandler);
                        return;
                    }
                }

                if (!int.TryParse(strPeriodo, out int idPeriodo))
                {
                    strPeriodo = WebStorage.Get("IdPeriodo", "");

                    if (!int.TryParse(strPeriodo, out idPeriodo))
                    {
                        MessageBox.Show("É obrigatório a seleção de um Período.", "Período Não Encontrado", TIPO.Info, MessageBoxHandler);
                        return;
                    }
                }

                var associado = new AssociadosService().ObterAssociadoMentorCargo(idAssociado);
                if (associado == null)
                {
                    MessageBox.Show("É obrigatório a seleção de um Associado cadastrado.", "Associado Não Encontrado", TIPO.Info, MessageBoxHandler);
                    return;
                }

                var periodoService = new PeriodoService();
                var util = new Util();
                var serviceConsolidacao = new ConsolidacaoService();

                PERIODOSAVALIACOES periodo = periodoService.ObterPeriodo(idPeriodo);
                lblPeriodo.InnerText = periodo.Periodo;

                lblAssociado.InnerText = associado.Nome;
                lblMentor.InnerText = associado.Mentor;
                lblCargo.InnerText = associado.Cargo;
                lblProximoCargo.InnerText = associado.ProximoCargo;
                // TEMPO DE PEERS E TEMPO DE CARGO
                lblTempoPeers.InnerText = util.TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDePeers).ToString();
                lblTempoCargo.InnerText = util.TempoAssociado(associado.IdAssociado, ReturnTempo.texto_TempoDeCargo).ToString();

                DataModel dtMod = new DataModel();
                string foto = "";

                var fotoAssociado = dtMod.FOTOSASSOCIADOS.FirstOrDefault(x => x.IdAssociado == idAssociado);

                if (fotoAssociado == null) { foto = "assets/images/users/usernophoto.jpg"; }
                else { foto = fotoAssociado.Imagem; }

                foto = foto.Replace(" ", "%20");
                imgUser1Conso.Src = foto;

                WebStorage.Set("IdAssociado", idAssociado.ToString());
                WebStorage.Set("IdPeriodo", idPeriodo.ToString());
                WebStorage.Set("TipoAvaliacao", strTipoAvaliacao);
                WebStorage.Set("Escopo", strEscopo);

                var listConsolidacaoCompetetencia = serviceConsolidacao.CompetenciaProjetoAssociado(idAssociado, idProjeto, idPeriodo);

                if (listConsolidacaoCompetetencia != null)
                {
                    this.rptCompetencias.ItemDataBound += RptCompetencias_ItemDataBound;
                    this.rptCompetencias.DataSource = listConsolidacaoCompetetencia;
                    this.rptCompetencias.DataBind();
                }

                var listConsolidacaoPerformace = serviceConsolidacao.PerformanceProjetoAssociado(idAssociado, idProjeto, idPeriodo);

                if (listConsolidacaoPerformace != null)
                {
                    this.rptPerformance.ItemDataBound += RptPerformance_ItemDataBound;
                    this.rptPerformance.DataSource = listConsolidacaoPerformace;
                    this.rptPerformance.DataBind();
                }

                CarregarConsideracoesMentor();

                //var radarprojeto = serviceConsolidacao.RadarConsolidacao(idAssociado, idProjeto, idPeriodo, associado.IdCargo);
                //var radar = JsonRadar(radarprojeto);
                //hfJsonRadar.Value = radar;
            }
        }

        private string JsonRadar(ResultadoProjetosModel projeto)
        {
            var listradar = new List<dynamic>();

            dynamic obj = new JObject();

            List<string> labels = new List<string>();
            List<decimal> datasetCompetencia = new List<decimal>();
            List<decimal> datasetNivelAtual = new List<decimal>();

            var nivelAtual = Math.Ceiling(projeto.SomaNotaCompetenciaRadar.Value / 100m) * 100m;

            foreach (var item in projeto.ListSomaCompetenciasN1N2)
            {
                labels.Add(item.Eixo);
                datasetCompetencia.Add(item.NotaCompetenciaRadar.HasValue ? item.NotaCompetenciaRadar.Value : 0);
                datasetNivelAtual.Add(nivelAtual);
            }

            obj.labels = new JArray(labels);
            obj.datasetcompetencias = new JArray(datasetCompetencia);
            obj.datasetnivelatual = new JArray(datasetNivelAtual);

            listradar.Add(obj);

            return JsonConvert.SerializeObject(listradar);

        }

        private void RptPerformance_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ResultadoPerfomanceModel item = (ResultadoPerfomanceModel)e.Item.DataItem;

                HtmlSelect ddl1 = (HtmlSelect)e.Item.FindControl("ddlPerformanceNotaComite");

                if (ddl1 != null)
                {
                    CarregaCombosNotaPerformance(ref ddl1);

                    if (item.IdNotaComite.HasValue)
                    {
                        ddl1.Value = item.IdNotaComite.ToString();
                    }
                    else if (item.IdNotaNivel1Feedback.HasValue)
                    {
                        ddl1.Value = item.IdNotaNivel1Feedback.ToString();
                    }
                    else
                    {
                        ddl1.Value = "-1";
                    }
                }
            }
        }

        private void RptCompetencias_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                ResultadoCompetenciaModel item = (ResultadoCompetenciaModel)e.Item.DataItem;

                HtmlSelect ddl1 = (HtmlSelect)e.Item.FindControl("ddlNotaComite");

                if (ddl1 != null)
                {
                    CarregaCombosNota(ref ddl1);

                    if (item.IdNotaNivel1Comite.HasValue)
                    {
                        ddl1.Value = item.IdNotaNivel1Comite.ToString();
                        
                    }
                    else if (item.IdNotaNivel1Feedback.HasValue)
                    {
                        ddl1.Value = item.IdNotaNivel1Feedback.ToString();
                    }
                    else
                    {
                        ddl1.Value = "-1";
                    }

                    ddl1.Disabled = !item.enableNivel1;
                }

                HtmlSelect ddl2 = (HtmlSelect)e.Item.FindControl("ddlNotaComiteNivel2");

                if (ddl2 != null)
                {
                    CarregaCombosNota(ref ddl2);

                    if (item.IdNotaNivel2Comite.HasValue)
                    {
                        ddl2.Value = item.IdNotaNivel2Comite.ToString();
                    }
                    else if (item.IdNotaNivel2Feedback.HasValue)
                    {
                        ddl2.Value = item.IdNotaNivel2Feedback.ToString();
                    }
                    else
                    {
                        ddl2.Value = "-1";
                    }


                    ddl2.Disabled = !item.enableNivel2;
                }

            }
        }

        private void CarregaCombosNotaPerformance(ref HtmlSelect ddl)
        {
            var statusList = new NotasAvaliacaoService().ListaNotasPerformances(false);

            ddl.DataValueField = "IdNota";
            ddl.DataTextField = "CodigoNota";
            ddl.DataSource = statusList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("[Selecionar]", "-1"));
        }

        private void CarregaCombosNota(ref HtmlSelect ddl)
        {
            var statusList = new NotasAvaliacaoService().ListaNotasCompetencias(false);

            ddl.DataValueField = "IdNota";
            ddl.DataTextField = "CodigoNota";
            ddl.DataSource = statusList;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("[Selecionar]", "-1"));
        }

        protected void btnResultado_Click(object sender, EventArgs e)
        {
            var strAssociado = Request.QueryString["IdAssociado"];
            var strPeriodo = Request.QueryString["IdPeriodo"];
            var strTipoAvaliacao = Request.QueryString["TipoAvaliacao"];
            var strEscopo = Request.QueryString["Escopo"];

            SalvarConsideracoesMentor();

            Response.Redirect(string.Format("avalizacao_resultado.aspx?IdAssociado={0}&IdPeriodo={1}&TipoAvaliacao={2}&Escopo={3}",strAssociado,strPeriodo,strTipoAvaliacao,strEscopo));
        }

        [WebMethod]
        public static string SaveCompetencia(int id, int nivel, int nota)
        {
            try
            {
                var notacomite = new ConsolidacaoService().CalculaNotaCompetenciaComite(id, nivel, nota);

                notacomite = Math.Round(notacomite.Value, 2);

                string strNota = notacomite.HasValue ? notacomite.Value.ToString().Replace(".", ",") : "0";

                return JsonConvert.SerializeObject(new { nota = strNota });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { erro = "Erro ao tentar Salvar a Nota Competência Comitê: " + ex.Message });
            }
        }

        [WebMethod]
        public static string SavePerformance(int id, int nota)
        {
            try
            {
                var notacomite = new ConsolidacaoService().CalculaNotaPerformanceComite(id, nota);

                string strNota = notacomite.HasValue ? notacomite.Value.ToString().Replace(".", ",") : "0";

                return JsonConvert.SerializeObject(new { nota = strNota });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { erro = "Erro ao tentar Salvar a Nota Performance Comitê: " + ex.Message });
            }
        }

        public void CarregarConsideracoesMentor()
        {
            int idAssociado = Convert.ToInt32(WebStorage.Get("IdAssociado", "0"));
            int idPeriodo = Convert.ToInt32(WebStorage.Get("IdPeriodo", "0"));
            string TipoAvaliacao = WebStorage.Get("TipoAvaliacao", "0");
            string Escopo = WebStorage.Get("Escopo", "0");

            var associadosService = new AssociadosService();
            var cargosService = new CargosService();
            var consideracoesMentorService = new ConsideracoesMentorService();
            var avaliacoesService = new AvaliacoesService();
            var projetosService = new ProjetosService();
            var complexidadeService = new ComplexidadesService();

            var associado = associadosService.ObterAssociado(idAssociado);
            var mentor = associadosService.ObterAssociado(associado.IdAssociadoMentor);

            var consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorAtributos(mentor.IdAssociado, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
            if (consideracoesMentor == null)
            {
                CONSIDERACOESMENTOR addConsideracoesMentor = new CONSIDERACOESMENTOR();
                addConsideracoesMentor.idMentor = mentor.IdAssociado;
                addConsideracoesMentor.idAssociado = idAssociado;
                addConsideracoesMentor.idPeriodo = idPeriodo;
                addConsideracoesMentor.TipoAvaliacao = TipoAvaliacao;
                addConsideracoesMentor.Escopo = Escopo;
                addConsideracoesMentor.LiberadoRH = false;
                addConsideracoesMentor.AcaoComite = "-";
                addConsideracoesMentor.PontosFortesRH = "-";
                addConsideracoesMentor.PontosFracosRH = "-";
                addConsideracoesMentor.SalarioAtual = 1;
                addConsideracoesMentor.SalarioNovo = 1;
                addConsideracoesMentor.RegimeContratacaoAtual = "-";
                addConsideracoesMentor.RegimeContratacaoNovo = "-";
                addConsideracoesMentor.MentoriaRealizada = false;

                CONSIDERACOESMENTOR ultimaConsideracoesMentor = new CONSIDERACOESMENTOR();
                ultimaConsideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorUltimaDoAvaliado(idAssociado, idPeriodo, TipoAvaliacao, Escopo);
                if (ultimaConsideracoesMentor != null && ultimaConsideracoesMentor.idConsideracoesMentor > 0)
                {
                    addConsideracoesMentor.SalarioAtual = ultimaConsideracoesMentor.SalarioAtual;
                    addConsideracoesMentor.RegimeContratacaoAtual = ultimaConsideracoesMentor.RegimeContratacaoAtual;
                }

                consideracoesMentorService.AdicionarConsideracoesMentor(addConsideracoesMentor);
                consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorAtributos(mentor.IdAssociado, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
            }

            WebStorage.Set("idConsideracoesMentor", consideracoesMentor.idConsideracoesMentor.ToString());

            var avaliacoes = avaliacoesService.ObterAvaliacoesAssociadoSemestre(idAssociado, idPeriodo, TipoAvaliacao);
            string addTextProjetosEnvolvidos = "";
            foreach (var avaliacao in avaliacoes)
            {
                addTextProjetosEnvolvidos += projetosService.ObterProjeto(avaliacao.idProjeto).Projeto + " (" +
                    complexidadeService.ObterComplexidade(projetosService.ObterProjeto(avaliacao.idProjeto).IdComplexidade).Complexidade + ")";
                if (avaliacoes.IndexOf(avaliacao) < avaliacoes.Count - 1) { addTextProjetosEnvolvidos += "<br />"; }
            }

            var cargoAtual = cargosService.ObterCargo(associadosService.ObterAssociado(consideracoesMentor.idAssociado).IdCargo);

            // ACCORDION DADOS RH
            cboxLiberadoRH.Checked = (bool)consideracoesMentor.LiberadoRH;
            if ((bool)consideracoesMentor.LiberadoRH){
                labelLiberadoRH.Text = "O mentor pode visualizar os dados abaixo";}
            else{
                labelLiberadoRH.Text = "O mentor NÃO pode visualizar os dados abaixo";}
            txtAcaoComite.Text = consideracoesMentor.AcaoComite;
            txtPontosFortesRH.Text = consideracoesMentor.PontosFortesRH == "-" ? consideracoesMentor.PontosFortes : consideracoesMentor.PontosFortesRH;
            txtPontosFracosRH.Text = consideracoesMentor.PontosFracosRH == "-" ? consideracoesMentor.PontosFracos : consideracoesMentor.PontosFracosRH;
            labelProximoCargo.Text = "-";
            txtSalarioAtual.Text = "R$ " + consideracoesMentor.SalarioAtual.ToString().Replace(".", ",");
            txtProximoSalario.Text = "R$ " + consideracoesMentor.SalarioNovo.ToString().Replace(".", ",");
            labelIncremento.Text = Math.Round((double)(((consideracoesMentor.SalarioNovo / consideracoesMentor.SalarioAtual) - 1) * 100), 2).ToString() + "%";
            txtRegimeContratacaoAtual.Text = consideracoesMentor.RegimeContratacaoAtual;
            txtRegimeContratacaoNovo.Text = consideracoesMentor.RegimeContratacaoNovo;
            if (cargoAtual.idProximoCargo != null)
            {
                labelProximoCargo.Text = cargosService.ObterCargo((int)cargoAtual.idProximoCargo).Cargo;
            }
        }

        public void SalvarConsideracoesMentor()
        {
            var consideracoesMentorService = new ConsideracoesMentorService();

            int idConsideracoesMentor = Convert.ToInt32(WebStorage.Get("idConsideracoesMentor", "0"));
            var consideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorPorId(idConsideracoesMentor);

            if (consideracoesMentor != null)
            {
                bool antigoLiberadoRH = (bool)consideracoesMentor.LiberadoRH;

                CONSIDERACOESMENTOR updConsideracoesMentor = new CONSIDERACOESMENTOR();
                updConsideracoesMentor = consideracoesMentor;
                updConsideracoesMentor.LiberadoRH = cboxLiberadoRH.Checked;
                updConsideracoesMentor.DataLiberadoRH = cboxLiberadoRH.Checked ? DateTime.Today : (DateTime?)null;
                updConsideracoesMentor.AcaoComite = txtAcaoComite.Text;
                updConsideracoesMentor.PontosFortesRH = txtPontosFortesRH.Text;
                updConsideracoesMentor.PontosFracosRH = txtPontosFracosRH.Text;
                updConsideracoesMentor.SalarioAtual = Convert.ToDecimal(txtSalarioAtual.Text.Replace("R", "").Replace("$", "").Replace(",", ".").Replace(" ", ""), CultureInfo.InvariantCulture);
                updConsideracoesMentor.SalarioNovo = Convert.ToDecimal(txtProximoSalario.Text.Replace("R", "").Replace("$", "").Replace(",", ".").Replace(" ", ""), CultureInfo.InvariantCulture);
                updConsideracoesMentor.RegimeContratacaoAtual = txtRegimeContratacaoAtual.Text;
                updConsideracoesMentor.RegimeContratacaoNovo = txtRegimeContratacaoNovo.Text;
                string resultUpdate = consideracoesMentorService.AtualizarConsideracoesMentor(consideracoesMentor, updConsideracoesMentor);
                if (resultUpdate == "okay")
                {
                    if ((bool)updConsideracoesMentor.LiberadoRH && !antigoLiberadoRH)
                    {
                        var paramEmail = new EmailParametroService().ObterParametro(updConsideracoesMentor.ASSOCIADOS1.IdEmpresa);
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
                            Nome = updConsideracoesMentor.ASSOCIADOS1.Nome,
                            Email = updConsideracoesMentor.ASSOCIADOS1.Email
                        };

                        var dataFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");

                        try
                        {
                            // MONTA O EMAIL
                            var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
                            _corpo = ConfiguraBody(_corpo, updConsideracoesMentor.ASSOCIADOS, "", "", "", "", dataFinal, updConsideracoesMentor.ASSOCIADOS1);

                            var mensagem = new Mensagem
                            {
                                Titulo = "[RH Peers] - Processo de Avaliação - Etapa: Consolidação - Período: " + updConsideracoesMentor.PERIODOSAVALIACOES.Periodo,
                                Corpo = _corpo
                            };

                            var emailService = new EmailService(configEmail, destinatario, mensagem);

                            var thread = new Thread(new ThreadStart(emailService.Enviar));
                            thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
                            thread.IsBackground = true;
                            thread.Start();

                            MessageBox.Show("Considerações salvas e e-mail enviado ao Mentor!", "Salvar", TIPO.Default, MessageBoxHandler);

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Considerações salvas! E-mail NÃO enviado ao Mentor!", "Salvar", TIPO.Error, MessageBoxHandler);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Considerações salvas!", "Salvar", TIPO.Default, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show(resultUpdate, "Salvar", TIPO.Error, MessageBoxHandler);
                }
            }
        }
        private string ConfiguraBody(string corpo, ASSOCIADOS associado,
            string texto_inicial, string texto_principal, string info_final, string despedida, 
            string prazoFinal, ASSOCIADOS mentor)
        {
            // Padrão Email
            var newCorpo = corpo.Replace("[INFO_INICIAL]", texto_inicial);
            newCorpo = newCorpo.Replace("[TEXTO_PRINCIPAL]", texto_principal);
            newCorpo = newCorpo.Replace("[INFO_FINAL]", info_final);
            newCorpo = newCorpo.Replace("[CUMPRIMENTOS]", despedida);

            // Dados Pessoais
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associado.Nome);
            newCorpo = newCorpo.Replace("[NOME]", mentor.Nome);
            newCorpo = newCorpo.Replace("[CARGO]", associado.CARGOS.Cargo);
            newCorpo = newCorpo.Replace("[MENTOR]", mentor.Nome);
            newCorpo = newCorpo.Replace("<p><strong>Projeto:</strong> [PROJETO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Gestor do Projeto:</strong> [GESTOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Início:</strong> [DATA_INICIO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Término:</strong> [DATA_FINAL]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Avaliador:</strong> [AVALIADOR]</p>", "");
            newCorpo = newCorpo.Replace("[PRAZO_FINAL]", prazoFinal);
            newCorpo = newCorpo.Replace("[ETAPA_AVALIACAO]", "Consolidação");

            // NomeAvaliaco e Descrição - Atualização RH - Setembro/2021
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associado.Nome);
            newCorpo = newCorpo.Replace("[DESCRICAO]", "O resultado do comitê de avaliação - feedback do(a) associado(a) [NOME_AVALIADO] já está disponível para consulta no sistema de avaliação. Lembre-se de agendar uma reunião com seu mentorado para repassar as informações do seu desempenho e possíveis movimentações nos próximos DOIS DIAS.");
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associado.Nome);

            return newCorpo;
        }

        protected void cboxLiberadoRH_CheckedChanged(object sender, EventArgs e)
        {
            if (cboxLiberadoRH.Checked)
            {
                labelLiberadoRH.Text = "O mentor pode visualizar os dados abaixo";
            }
            else
            {
                labelLiberadoRH.Text = "O mentor NÃO pode visualizar os dados abaixo";
            }
        }

        protected void txtSalarioAtual_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double salarioAtual = Convert.ToDouble(txtSalarioAtual.Text.Replace("R", "").Replace("$", "").Replace(".", ",").Replace(" ", ""));
                double salarioNovo = Convert.ToDouble(txtProximoSalario.Text.Replace("R", "").Replace("$", "").Replace(".", ",").Replace(" ", ""));
                labelIncremento.Text = Math.Round((((salarioNovo / salarioAtual) - 1) * 100), 2).ToString() + "%";
            }
            catch
            {
                labelIncremento.Text = "-";
            }
        }
    }
}