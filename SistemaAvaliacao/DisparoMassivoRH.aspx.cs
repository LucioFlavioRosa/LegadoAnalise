using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tria.Framework.Domain.Service;
using TriaSoftware.Util.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class DisparoMassivoRH : System.Web.UI.Page
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
                CarregarListaNovosDisparos();
            }
        }

        protected void CarregarListaNovosDisparos()
        {
            List<CONSIDERACOESMENTOR> listSession = Session["LISTACONSIDERACOES"] as List<CONSIDERACOESMENTOR>;
            List<ConsideracoesMentorModel> listModel = (from item in listSession
                                                        where item.LiberadoRH == true
                                                        select new ConsideracoesMentorModel()
                                                        {
                                                            idConsideracoesMentor = item.idConsideracoesMentor,
                                                            idAssociado = item.idAssociado,
                                                            Associado = new AssociadosService().ObterAssociado(item.idAssociado).Nome,
                                                            idMentor = item.idMentor,
                                                            Mentor = new AssociadosService().ObterAssociado(item.idMentor).Nome,
                                                            idPeriodo = item.idPeriodo,
                                                            Periodo = new PeriodoService().ObterPeriodo(item.idPeriodo).Periodo
                                                        }).ToList();

            rptDisparos.DataSource = listModel;
            rptDisparos.DataBind();
        }

        protected void btnDispararTodos_Click(object sender, EventArgs e)
        {
            try
            {
                List<CONSIDERACOESMENTOR> listSession = Session["LISTACONSIDERACOES"] as List<CONSIDERACOESMENTOR>;
                List<ConsideracoesMentorModel> listModel = (from item in listSession
                                                            where item.LiberadoRH == true
                                                            select new ConsideracoesMentorModel()
                                                            {
                                                                idConsideracoesMentor = item.idConsideracoesMentor,
                                                                idAssociado = item.idAssociado,
                                                                Associado = new AssociadosService().ObterAssociado(item.idAssociado).Nome,
                                                                idMentor = item.idMentor,
                                                                Mentor = new AssociadosService().ObterAssociado(item.idMentor).Nome,
                                                                idPeriodo = item.idPeriodo,
                                                                Periodo = new PeriodoService().ObterPeriodo(item.idPeriodo).Periodo
                                                            }).ToList();

                foreach (var item in listModel)
                {
                    var associado = new AssociadosService().ObterAssociado(item.idAssociado);
                    var mentor = new AssociadosService().ObterAssociado(item.idMentor);
                    var periodo = new PeriodoService().ObterPeriodo(item.idPeriodo);

                    var paramEmail = new EmailParametroService().ObterParametro(mentor.IdEmpresa);
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
                        Nome = mentor.Nome,
                        Email = mentor.Email
                    };

                    var dataFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
                    
                    // MONTA O EMAIL
                    var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
                    _corpo = ConfiguraBody(_corpo, associado, "", "", "", "", dataFinal, mentor);

                    var mensagem = new Mensagem
                    {
                        Titulo = "[RH Peers] - Processo de Avaliação - Etapa: Consolidação - Período: " + periodo.Periodo,
                        Corpo = _corpo
                    };

                    var emailService = new EmailService(configEmail, destinatario, mensagem);

                    var thread = new Thread(new ThreadStart(emailService.Enviar));
                    thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
                    thread.IsBackground = true;
                    thread.Start();
                }

                WebStorage.Set("DisparosRealizados", "1");
                Response.Redirect("~/Consolidacao");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message.ToString(), "Erro", TIPO.Error, MessageBoxHandler);
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
            newCorpo = newCorpo.Replace("<p><strong>Gestor:</strong> [GESTOR]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Início:</strong> [DATA_INICIO]</p>", "");
            newCorpo = newCorpo.Replace("<p><strong>Data de Término:</strong> [DATA_FINAL]</p>", "");
            newCorpo = newCorpo.Replace("[PRAZO_FINAL]", prazoFinal);
            newCorpo = newCorpo.Replace("[ETAPA_AVALIACAO]", "Consolidação");

            // NomeAvaliaco e Descrição - Atualização RH - Setembro/2021
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associado.Nome);
            newCorpo = newCorpo.Replace("[DESCRICAO]", "O resultado do comitê de avaliação - feedback do(a) associado(a) [NOME_AVALIADO] já está disponível para consulta no sistema de avaliação. Lembre-se de agendar uma reunião com seu mentorado para repassar as informações do seu desempenho e possíveis movimentações nos próximos DOIS DIAS.");
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associado.Nome);

            return newCorpo;
        }
    }
}