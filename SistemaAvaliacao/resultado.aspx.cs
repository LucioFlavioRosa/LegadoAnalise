using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TriaSoftware.Util.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class resultado : System.Web.UI.Page
    {

        private static PeriodoService periodoService = new PeriodoService();
        private static AssociadosService associadoService = new AssociadosService();
        private static MentoriaService mentoriaService = new MentoriaService();

        private List<PERIODOSAVALIACOES> periodosAvaliacaoList;
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
                CarregaComboPeriodos();
                CarregaComboProjetos();
                CarregaComboAssociados();

                TrataBotaoLideranca();
                TrataBotaoMentoria();


            }
        }

        private void TrataBotaoLideranca()
        {
            var periodoCodigo = getPeriodoCodigo(x => !x.fl_lib_res_lideranca);

            if (periodoCodigo != null)
            {
                btnLiberaLideranca.Visible = true;
                btnLiberaLideranca.Text = "Liberar Liderança " + periodoCodigo;
            }
        }

        private void TrataBotaoMentoria()
        {
            var periodoCodigo = getPeriodoCodigo(x => !x.fl_lib_res_mentoria);

            if (periodoCodigo != null)
            {
                btnLiberaMentoria.Visible = true;
                btnLiberaMentoria.Text = "Liberar Mentoria " + periodoCodigo;
            }
        }

        private String getPeriodoCodigo(Func<PERIODOSAVALIACOES, bool> predicate)
        {
            return getPeriodos(predicate).Select(x => x.Codigo)
                  .FirstOrDefault();
        }

        private List<PERIODOSAVALIACOES> getPeriodos(Func<PERIODOSAVALIACOES, bool> predicate)
        {

            if(periodosAvaliacaoList == null)
            {
                periodosAvaliacaoList = periodoService.ListaPeriodos(1)
                   .Where(x => x.ATV == 1)
                   .OrderByDescending(x => x.IdPeriodo).ToList();

            }

            return periodosAvaliacaoList.Where(predicate)
                   .OrderByDescending(x => x.IdPeriodo).ToList();
                   
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int idprojeto = 0;
            int idassociado = 0;
            int idperiodo = 0;

            if (ddlProjetos.SelectedIndex > 0)
            {
                idprojeto = Convert.ToInt32(ddlProjetos.Items[ddlProjetos.SelectedIndex].Value);
            }

            if (ddlAssociados.SelectedIndex > 0)
            {
                idassociado = Convert.ToInt32(ddlAssociados.Items[ddlAssociados.SelectedIndex].Value);
            }
                
            if (ddlPeriodos.SelectedIndex > 0)
            {
                idperiodo = Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value);
            }

            var resultado = new ResultadoServices().ObterListResultado(idprojeto, idassociado, idperiodo).Where(l => l.TipoAvaliacao == "desempenho").ToList();
            resultado = idperiodo != 0 ? resultado.Where(x => x.IdPeriodo == idperiodo).ToList() : resultado;

            rptProjetos.DataSource = resultado;
            rptProjetos.DataBind();

        }

        private void CarregaComboProjetos()
        {
            var projetosList = new ProjetosService().ListaProjetosAtivos();
            ddlProjetos.DataValueField = "IdProjeto";
            ddlProjetos.DataTextField = "Projeto";
            ddlProjetos.DataSource = projetosList;
            ddlProjetos.DataBind();
            ddlProjetos.Items.Insert(0, "[Selecionar]");
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

        private void CarregaComboPeriodos()
        {
            var statusList = new PeriodoService().ListaPeriodos(WebStorage.GetUsuarioLogado().IdEmpresa);

            ddlPeriodos.DataValueField = "IdPeriodo";
            ddlPeriodos.DataTextField = "Periodo";
            ddlPeriodos.DataSource = statusList.OrderByDescending(x => x.DataInicio);
            ddlPeriodos.DataBind();
            ddlPeriodos.Items.Insert(0, "[Selecionar]");

            ddlPeriodo_Export.DataValueField = "IdPeriodo";
            ddlPeriodo_Export.DataTextField = "Periodo";
            ddlPeriodo_Export.DataSource = statusList.OrderByDescending(x => x.DataInicio);
            ddlPeriodo_Export.DataBind();
            ddlPeriodo_Export.Items.Insert(0, "[Selecionar]");

            ddlPeriodo_Mentor_Export.DataValueField = "IdPeriodo";
            ddlPeriodo_Mentor_Export.DataTextField = "Periodo";
            ddlPeriodo_Mentor_Export.DataSource = statusList.OrderByDescending(x => x.DataInicio);
            ddlPeriodo_Mentor_Export.DataBind();
            ddlPeriodo_Mentor_Export.Items.Insert(0, "[Selecionar]");
        }

        protected void btnExtrair_Click(object sender, EventArgs e)
        {
            var avaliacoes = new AvaliacoesService().ObterAvaliacoesTipo("AFI", "lideranca");
            var associadosService = new AssociadosService();
            var avaliacoesService = new AvaliacoesService();
            var respostasTodos = new AvaliacoesService().ObterAvaliacoesCompetenciasPeriodo(new PeriodoService().ListaPeriodos(1).Select(x => x.IdPeriodo).ToList(), "lideranca");
            respostasTodos = respostasTodos.Where(x => x.IdNotaNivel1AvaliacaoGestor != 0 && x.IdNotaNivel1AvaliacaoGestor != 5 && x.IdNotaNivel1AvaliacaoGestor != null).ToList();

            var associados = associadoService.ObterAssociados();
            var avaliacoesCompetencias = avaliacoesService.ObterAvaliacaoCompetenciasNotas();

            var exportPeriodos = getPeriodos(avaliacoes, respostasTodos, associados, avaliacoesCompetencias);
            var exportProjetos = getProjetos(avaliacoes, respostasTodos, associados, avaliacoesCompetencias);
            var exportPilares = getPilares(respostasTodos, associados, avaliacoesCompetencias);
            var exportSubcompetencias = getSubcompetencias(respostasTodos, associados, avaliacoesCompetencias);

            //Seta nome do arquivo e diretório no servidor
            string dirPath = Server.MapPath("~/Exported_Files/");
            string fileName = "ResultadoLideranca_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            //Gera arquivo
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcel_ResultadoLideranca(Path.Combine(dirPath, fileName), exportPeriodos, exportProjetos, exportPilares, exportSubcompetencias);

            FileStream Stream = new FileStream(Path.Combine(dirPath, fileName), FileMode.Create);
            Stream.Write(result, 0, result.Length);
            Stream.Close();

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.TransmitFile(Path.Combine(dirPath, fileName));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        protected void btnLiberaLideranca_Click(object sender, EventArgs e)
        {
            var getPeriodo = getPeriodos(x => x.ATV == 1 & !x.fl_lib_res_lideranca)
                     .OrderByDescending(x => x.IdPeriodo)
                     .FirstOrDefault();

            getPeriodo.fl_lib_res_lideranca = true;

            periodoService.AlterarPeriodo(getPeriodo);

            btnLiberaLideranca.Visible = false;


        }

        protected void btnLiberaMentoria_Click(object sender, EventArgs e)
        {
            
            var getPeriodo = getPeriodos(x => x.ATV == 1 & !x.fl_lib_res_mentoria)
                    .OrderByDescending(x => x.IdPeriodo)
                    .FirstOrDefault();

            getPeriodo.fl_lib_res_mentoria = true;

            periodoService.AlterarPeriodo(getPeriodo);

            btnLiberaMentoria.Visible = false;


        }
        public List<ResultadoLiderModel_Export_Periodo> getPeriodos(List<AVALIACAO> avaliacoesTodos, List<AVALIACOESCOMPETENCIAS> respostasTodos, List<ASSOCIADOS> associados, List<AVALIACOESCOMPETENCIASNOTAS> avaliacoesCompetencias)
        {
            var returnList = new List<ResultadoLiderModel_Export_Periodo>();
            //var associadosService = new AssociadosService();
            //var avaliacoesService = new AvaliacoesService();

            foreach (var avaliacao in avaliacoesTodos)
            {
                var addResultado = new ResultadoLiderModel_Export_Periodo();
                addResultado.Periodo = avaliacao.PERIODOSAVALIACOES.Periodo;
                addResultado.Lider = associados.FirstOrDefault(a => a.IdAssociado == avaliacao.idAssociado).Nome;
                //addResultado.Lider = associadosService.ObterAssociado(avaliacao.idAssociado).Nome;

                var existeResultado = returnList.Where(x => x.Lider == addResultado.Lider && x.Periodo == addResultado.Periodo).ToList();
                if (existeResultado == null || existeResultado.Count <= 0)
                {
                    var respostasPeriodo = respostasTodos.Where(x => x.IdPeriodo == avaliacao.idPeriodo).ToList();
                    var respostasLiderPeriodo = respostasPeriodo.Where(x => x.IdAssociado == avaliacao.idAssociado).ToList();

                    var notasRespondidas = new List<decimal?>();

                    foreach (var resposta in respostasLiderPeriodo)
                    {
                        var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor);
                        // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                        notasRespondidas.Add(addNota.Peso);
                    }
                    addResultado.MediaLiderPeriodo = notasRespondidas.Average();

                    var existePeriodo = returnList.Where(x => x.Periodo == addResultado.Periodo).ToList();
                    if (existePeriodo == null || existePeriodo.Count <= 0)
                    {
                        notasRespondidas.Clear();
                        foreach (var resposta in respostasPeriodo)
                        {
                            var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor); ;
                            // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                            notasRespondidas.Add(addNota.Peso);
                        }
                        addResultado.MediaPeersPeriodo = notasRespondidas.Average();
                    }
                    else
                    {
                        addResultado.MediaPeersPeriodo = existePeriodo[0].MediaPeersPeriodo;
                    }

                    returnList.Add(addResultado);
                }
            }

            return returnList;
        }
        public List<ResultadoLiderModel_Export_Projeto> getProjetos (List<AVALIACAO> avaliacoesTodos, List<AVALIACOESCOMPETENCIAS> respostasTodos, List<ASSOCIADOS> associados, List<AVALIACOESCOMPETENCIASNOTAS> avaliacoesCompetencias)
        {
            var returnList = new List<ResultadoLiderModel_Export_Projeto>();
            //var associadosService = new AssociadosService();
            //var avaliacoesService = new AvaliacoesService();

            foreach (var avaliacao in avaliacoesTodos)
            {
                var addResultado = new ResultadoLiderModel_Export_Projeto();
                addResultado.idProjeto = avaliacao.idProjeto;
                addResultado.Projeto = avaliacao.PROJETOS.Projeto;
                addResultado.Periodo = avaliacao.PERIODOSAVALIACOES.Periodo;
                addResultado.Lider = associados.FirstOrDefault(a => a.IdAssociado == avaliacao.idAssociado).Nome;
                //addResultado.Lider = associadosService.ObterAssociado(avaliacao.idAssociado).Nome;

                var existeResultado = returnList.Where(x => x.Projeto == addResultado.Projeto && x.Lider == addResultado.Lider && x.Periodo == addResultado.Periodo).ToList();
                if (existeResultado == null || existeResultado.Count <= 0)
                {
                    var respostasPeriodo = respostasTodos.Where(x => x.IdPeriodo == avaliacao.idPeriodo).ToList();
                    var respostasProjeto = respostasPeriodo.Where(x => x.IdProjeto == avaliacao.idProjeto).ToList();
                    var respostasLiderProjeto = respostasProjeto.Where(x => x.IdAssociado == avaliacao.idAssociado).ToList();

                    var notasRespondidasLiderProjeto = new List<decimal?>();
                    var notasRespondidasTodosProjeto = new List<decimal?>();

                    foreach (var resposta in respostasLiderProjeto)
                    {
                        var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor);
                        // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                        notasRespondidasLiderProjeto.Add(addNota.Peso);
                    }
                    addResultado.MediaLiderProjeto = notasRespondidasLiderProjeto.Average();

                    var existeTodosProjeto = returnList.Where(x => x.Periodo == addResultado.Periodo && x.Projeto == addResultado.Projeto).ToList();
                    if (existeResultado == null || existeResultado.Count <= 0)
                    {
                        foreach (var resposta in respostasProjeto)
                        {
                            var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor);
                            // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                            notasRespondidasTodosProjeto.Add(addNota.Peso);
                        }
                        addResultado.MediaPeersProjeto = notasRespondidasTodosProjeto.Average();
                    }
                    else
                    {
                        addResultado.MediaPeersProjeto = existeResultado[0].MediaPeersProjeto;
                    }

                    returnList.Add(addResultado);
                }
            }

            return returnList;
        }
        public List<ResultadoLiderModel_Export_Pilar> getPilares(List<AVALIACOESCOMPETENCIAS> respostasTodos, List<ASSOCIADOS> associados, List<AVALIACOESCOMPETENCIASNOTAS> avaliacoesCompetencias)
        {
            var returnList = new List<ResultadoLiderModel_Export_Pilar>();
            //var associadosService = new AssociadosService();
            //var avaliacoesService = new AvaliacoesService();

            foreach (var avaliacao in respostasTodos)
            {
                var addResultado = new ResultadoLiderModel_Export_Pilar();
                addResultado.Periodo = avaliacao.PERIODOSAVALIACOES.Periodo;
                addResultado.Lider = associados.FirstOrDefault(a => a.IdAssociado == avaliacao.IdAssociado).Nome;
                //addResultado.Lider = associadosService.ObterAssociado(avaliacao.IdAssociado).Nome;
                addResultado.Pilar = avaliacao.COMPETENCIAS.EIXOS.Eixo;

                var existeNota = returnList.Where(x => x.Periodo == addResultado.Periodo && x.Lider == addResultado.Lider && x.Pilar == addResultado.Pilar).ToList();
                if (existeNota == null || existeNota.Count <= 0)
                {
                    var respostasPeriodo = respostasTodos.Where(x => x.IdPeriodo == avaliacao.IdPeriodo).ToList();
                    var respostasPilar = respostasPeriodo.Where(x => x.COMPETENCIAS.EIXOS.IdEixo == avaliacao.COMPETENCIAS.EIXOS.IdEixo).ToList();
                    var respostasLider = respostasPilar.Where(x => x.IdAssociado == avaliacao.IdAssociado).ToList();

                    var notas = new List<decimal?>();
                    foreach (var resposta in respostasLider)
                    {
                        var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor);
                        // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                        notas.Add(addNota.Peso);
                    }
                    addResultado.MediaLiderPilar = notas.Average();

                    var existeTodosPilares = returnList.Where(x => x.Periodo == addResultado.Periodo && x.Pilar == addResultado.Pilar).ToList();
                    if (existeTodosPilares == null || existeTodosPilares.Count <= 0)
                    {
                        notas = new List<decimal?>();
                        foreach (var resposta in respostasPilar)
                        {
                            var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor);
                            // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                            notas.Add(addNota.Peso);
                        }
                        addResultado.MediaPeersPilar = notas.Average();
                    }
                    else
                    {
                        addResultado.MediaPeersPilar = existeTodosPilares[0].MediaPeersPilar;
                    }

                    returnList.Add(addResultado);
                }
            }

            return returnList;
        }
        public List<ResultadoLiderModel_Export_Subcompetencia> getSubcompetencias(List<AVALIACOESCOMPETENCIAS> respostasTodos, List<ASSOCIADOS> associados, List<AVALIACOESCOMPETENCIASNOTAS> avaliacoesCompetencias)
        {
            var returnList = new List<ResultadoLiderModel_Export_Subcompetencia>();
            //var associadosService = new AssociadosService();
            //var avaliacoesService = new AvaliacoesService();

            foreach (var avaliacao in respostasTodos)
            {
                var addResultado = new ResultadoLiderModel_Export_Subcompetencia();
                addResultado.Periodo = avaliacao.PERIODOSAVALIACOES.Periodo;
                addResultado.Lider = associados.FirstOrDefault(a => a.IdAssociado == avaliacao.IdAssociado).Nome;
                //addResultado.Lider = associadosService.ObterAssociado(avaliacao.IdAssociado).Nome;
                addResultado.Subcompetencia = avaliacao.COMPETENCIAS.SUBCOMPETENCIAS.SubCompetencia;

                var existeNota = returnList.Where(x => x.Periodo == addResultado.Periodo && x.Lider == addResultado.Lider && x.Subcompetencia == addResultado.Subcompetencia).ToList();
                if (existeNota == null || existeNota.Count <= 0)
                {
                    var respostasPeriodo = respostasTodos.Where(x => x.IdPeriodo == avaliacao.IdPeriodo).ToList();
                    var respostasSubcompetencia = respostasPeriodo.Where(x => x.COMPETENCIAS.SUBCOMPETENCIAS.IdSubCompetencia == avaliacao.COMPETENCIAS.SUBCOMPETENCIAS.IdSubCompetencia).ToList();
                    var respostasLider = respostasSubcompetencia.Where(x => x.IdAssociado == avaliacao.IdAssociado).ToList();

                    var notas = new List<decimal?>();
                    foreach (var resposta in respostasLider)
                    {
                        var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor);
                        // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                        notas.Add(addNota.Peso);
                    }
                    addResultado.MediaLiderSubcompetencia = notas.Average();

                    var existeTodosSubcompetencia = returnList.Where(x => x.Periodo == addResultado.Periodo && x.Subcompetencia == addResultado.Subcompetencia).ToList();
                    if (existeTodosSubcompetencia == null || existeTodosSubcompetencia.Count <= 0)
                    {
                        notas = new List<decimal?>();
                        foreach (var resposta in respostasSubcompetencia)
                        {
                            var addNota = avaliacoesCompetencias.FirstOrDefault(n => n.IdNota == (int)resposta.IdNotaNivel1AvaliacaoGestor);
                            // var addNota = avaliacoesService.ObterAvaliacaoCompetenciaNota((int)resposta.IdNotaNivel1AvaliacaoGestor);
                            notas.Add(addNota.Peso);
                        }
                        addResultado.MediaPeersSubcompetencia = notas.Average();
                    }
                    else
                    {
                        addResultado.MediaPeersSubcompetencia = existeTodosSubcompetencia[0].MediaPeersSubcompetencia;
                    }

                    returnList.Add(addResultado);
                }
            }

            return returnList;
        }

        protected void btnExportDesempenho_Click(object sender, EventArgs e)
        {
            int idperiodo = 0;

            if (ddlPeriodo_Export.SelectedIndex > 0)
            {
                idperiodo = Convert.ToInt32(ddlPeriodo_Export.Items[ddlPeriodo_Export.SelectedIndex].Value);
            }
            else
            {
                MessageBox.Show("Selecione o período.", "Aviso", TIPO.Warning, MessageBoxHandler);
                return;
            }

            var resultadoService = new ResultadoServices();
            resultadoService.ExportResultadoCompetencias(idperiodo, Server);
        }
        protected void btnExportMentoria_Click(object sender, EventArgs e)
        {
            int idperiodo = 0;

            if (ddlPeriodo_Mentor_Export.SelectedIndex > 0)
            {
                idperiodo = Convert.ToInt32(ddlPeriodo_Mentor_Export.Items[ddlPeriodo_Mentor_Export.SelectedIndex].Value);
            }
            else
            {
                MessageBox.Show("Selecione o período.", "Aviso", TIPO.Warning, MessageBoxHandler);
                return;
            }

            var resultadoService = new ResultadoServices();

            resultadoService.ReportMentoria(idperiodo, Server);
            MessageBox.Show("Relatório exportado com sucesso.", "Aviso", TIPO.Info, MessageBoxHandler);
           

        }
    }
}