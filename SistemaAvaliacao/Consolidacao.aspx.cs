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
using TriaSoftware.Util.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class Consolidacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = WebStorage.GetUsuarioLogado();
            Session["LISTACONSIDERACOES"] = new List<CONSIDERACOESMENTOR>();

            if (WebStorage.Get("DisparosRealizados", "") != "")
            {
                MessageBox.Show("Disparos realizados", "", TIPO.Info, MessageBoxHandler);
                WebStorage.Set("DisparosRealizados", "");
            }

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
            }
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

            var rptFeed = new ConsolidacaoService().ObterListConsolidacao(idprojeto, idassociado, idperiodo, "desempenho", "projeto");
            if (idprojeto != 0) { rptFeed = rptFeed.Where(x => x.IdProjeto == idprojeto).ToList(); }
            if (idperiodo != 0) { rptFeed = rptFeed.Where(x => x.IdPeriodo == idperiodo).ToList(); }
            rptProjetos.DataSource = rptFeed;
            rptProjetos.DataBind();

        }

        #region carrega combos
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

            selectExportAssociado.DataValueField = "IdAssociado";
            selectExportAssociado.DataTextField = "Nome";
            selectExportAssociado.DataSource = profissionalList;
            selectExportAssociado.DataBind();
            selectExportAssociado.Items.Insert(0, "[Selecionar]");
        }

        private void CarregaComboPeriodos()
        {
            var statusList = new PeriodoService().ListaPeriodos(WebStorage.GetUsuarioLogado().IdEmpresa);

            ddlPeriodos.DataValueField = "IdPeriodo";
            ddlPeriodos.DataTextField = "Periodo";
            ddlPeriodos.DataSource = statusList;
            ddlPeriodos.DataBind();
            ddlPeriodos.Items.Insert(0, "[Selecionar]");

            selectExportPeriodo.DataValueField = "IdPeriodo";
            selectExportPeriodo.DataTextField = "Periodo";
            selectExportPeriodo.DataSource = statusList;
            selectExportPeriodo.DataBind();
            selectExportPeriodo.Items.Insert(0, "[Selecionar]");
        }
        #endregion

        protected void btnExport_Click(object sender, EventArgs e)
        {
            List<ConsideracoesMentorModel> listConsideracoesMentor = new List<ConsideracoesMentorModel>();
            var consideracoesMentorService = new ConsideracoesMentorService();
            var associadosService = new AssociadosService();
            var periodosService = new PeriodoService();
            int idassociado = 0;
            int idperiodo = 0;
            if (selectExportAssociado.SelectedIndex > 0)
            {
                idassociado = Convert.ToInt32(selectExportAssociado.Items[selectExportAssociado.SelectedIndex].Value);
            }
            if (selectExportPeriodo.SelectedIndex > 0)
            {
                idperiodo = Convert.ToInt32(selectExportPeriodo.Items[selectExportPeriodo.SelectedIndex].Value);
            }
            var getConsideracoesMentor = consideracoesMentorService.ObterConsideracoesMentorListaExport(idassociado, idperiodo);
            foreach (var consideracoesMentor in getConsideracoesMentor)
            {
                ConsideracoesMentorModel addConsideracoesMentor = new ConsideracoesMentorModel();
                addConsideracoesMentor.idConsideracoesMentor = consideracoesMentor.idConsideracoesMentor;
                addConsideracoesMentor.idMentor = consideracoesMentor.idMentor;
                addConsideracoesMentor.Mentor = associadosService.ObterAssociado(consideracoesMentor.idMentor).Nome;
                addConsideracoesMentor.idAssociado = consideracoesMentor.idAssociado;
                addConsideracoesMentor.Associado = associadosService.ObterAssociado(consideracoesMentor.idAssociado).Nome;
                addConsideracoesMentor.idPeriodo = consideracoesMentor.idPeriodo;
                addConsideracoesMentor.Periodo = periodosService.ObterPeriodo(consideracoesMentor.idPeriodo).Periodo;
                addConsideracoesMentor.TipoAvaliacao = consideracoesMentor.TipoAvaliacao;
                addConsideracoesMentor.Escopo = consideracoesMentor.Escopo;
                addConsideracoesMentor.ElegivelPromocao = consideracoesMentor.ElegivelPromocao ? "Sim" : "Não";
                addConsideracoesMentor.InputPromocao = consideracoesMentor.InputPromocao ? "Sim" : "Não";
                addConsideracoesMentor.TrajetoriaAssociado = consideracoesMentor.TrajetoriaAssociado;
                addConsideracoesMentor.PontosFortes = consideracoesMentor.PontosFortes;
                addConsideracoesMentor.PontosFracos = consideracoesMentor.PontosFracos;
                addConsideracoesMentor.MentorPodeVer = (bool)consideracoesMentor.LiberadoRH ? "Sim" : "Não";
                addConsideracoesMentor.AcaoComite = consideracoesMentor.AcaoComite;
                addConsideracoesMentor.PontosFortesRH = consideracoesMentor.PontosFortesRH;
                addConsideracoesMentor.PontosFracosRH = consideracoesMentor.PontosFracosRH;
                addConsideracoesMentor.SalarioAtual = consideracoesMentor.SalarioAtual.ToString();
                addConsideracoesMentor.SalarioNovo = consideracoesMentor.SalarioNovo.ToString();
                addConsideracoesMentor.RegimeContratacaoAtual = consideracoesMentor.RegimeContratacaoAtual;
                addConsideracoesMentor.RegimeContratacaoNovo = consideracoesMentor.RegimeContratacaoNovo;
                addConsideracoesMentor.MentoriaRealizada = (bool)consideracoesMentor.MentoriaRealizada ? "Sim" : "Não";
                listConsideracoesMentor.Add(addConsideracoesMentor);
            }
            string fileName = "ConsideracoesMentor" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listConsideracoesMentor);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUpload.HasFile)
                {
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUpload.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            MessageBox.Show("O arquivo Excel não possui nenhuma planilha.", "", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        List<CONSIDERACOESMENTOR> listSession = new List<CONSIDERACOESMENTOR>();
                        int rowCount = worksheet.Dimension.End.Row;
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            int IdConsideracoesMentor = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                            int IdMentor = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                            int IdAssociado = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                            int IdPeriodo = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                            string TipoAvaliacao = worksheet.Cells[row, 8].GetValue<string>() ?? "desempenho";
                            string Escopo = worksheet.Cells[row, 9].GetValue<string>() ?? "projeto";
                            string ElegivelPromocao = worksheet.Cells[row, 10].GetValue<string>() ?? "Não";
                            string InputPromocao = worksheet.Cells[row, 11].GetValue<string>() ?? "Não";
                            string TrajetoriaAssociado = worksheet.Cells[row, 12].GetValue<string>() ?? "";
                            string PontosFortes = worksheet.Cells[row, 13].GetValue<string>() ?? "";
                            string PontosFracos = worksheet.Cells[row, 14].GetValue<string>() ?? "";
                            string MentorVisualizar = worksheet.Cells[row, 15].GetValue<string>() ?? "Não";
                            string AcaoComite = worksheet.Cells[row, 16].GetValue<string>() ?? "";
                            string PontosFortesRH = worksheet.Cells[row, 17].GetValue<string>() ?? "";
                            string PontosFracosRH = worksheet.Cells[row, 18].GetValue<string>() ?? "";
                            double SalarioAtual = worksheet.Cells[row, 19].GetValue<double?>() ?? 0;
                            double SalarioNovo = worksheet.Cells[row, 20].GetValue<double?>() ?? 0;
                            string RegimeAtual = worksheet.Cells[row, 21].GetValue<string>() ?? "CLT";
                            string RegimeNovo = worksheet.Cells[row, 22].GetValue<string>() ?? "CLT";
                            string MentoriaRealizada = worksheet.Cells[row, 23].GetValue<string>() ?? "Não";
                            if (IdMentor > 0 && IdAssociado > 0 && IdPeriodo > 0)
                            {
                                CONSIDERACOESMENTOR importItem = new CONSIDERACOESMENTOR();
                                importItem.idMentor = IdMentor;
                                importItem.idAssociado = IdAssociado;
                                importItem.idPeriodo = IdPeriodo;
                                importItem.TipoAvaliacao = TipoAvaliacao;
                                importItem.Escopo = Escopo;
                                importItem.ElegivelPromocao = ElegivelPromocao.ToLower() == "sim" ? true : false;
                                importItem.InputPromocao = InputPromocao.ToLower() == "sim" ? true : false;
                                importItem.TrajetoriaAssociado = TrajetoriaAssociado;
                                importItem.PontosFortes = PontosFortes;
                                importItem.PontosFracos = PontosFracos;
                                importItem.LiberadoRH = MentorVisualizar.ToLower() == "sim" ? true : false;
                                importItem.AcaoComite = AcaoComite;
                                importItem.PontosFortesRH = PontosFortesRH;
                                importItem.PontosFracosRH = PontosFracosRH;
                                importItem.SalarioAtual = (decimal?)SalarioAtual;
                                importItem.SalarioNovo = (decimal?)SalarioNovo;
                                importItem.RegimeContratacaoAtual = RegimeAtual;
                                importItem.RegimeContratacaoNovo = RegimeNovo;
                                importItem.MentoriaRealizada = MentoriaRealizada.ToLower() == "sim" ? true : false;
                                var mainService = new ConsideracoesMentorService();
                                if (IdConsideracoesMentor == 0)
                                {
                                    mainService.AdicionarConsideracoesMentor(importItem);
                                    somaLinhasInseridas += 1;
                                }
                                else
                                {
                                    importItem.idConsideracoesMentor = IdConsideracoesMentor;
                                    var oldConsideracoesMentor = mainService.ObterConsideracoesMentorPorId(IdConsideracoesMentor);
                                    mainService.AtualizarConsideracoesMentor(oldConsideracoesMentor, importItem);
                                    somaLinhasAlteradas += 1;
                                }
                                listSession.Add(importItem);
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }
                        Session["LISTACONSIDERACOES"] = listSession;
                        Response.Redirect("~/DisparoMassivoRH", false);
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

        protected void btn_notas_ExportarPerf_Click(object sender, EventArgs e)
        {
            List<PerformanceNotasModelExport> listItems = new List<PerformanceNotasModelExport>();
            var mainService = new AvaliacoesService();
            var notasService = new NotasAvaliacaoService();
            var getItems = mainService.ObterAvaliacaoPerformanceLista();
            foreach (var item in getItems)
            {
                PerformanceNotasModelExport addItem = new PerformanceNotasModelExport();
                addItem.IdNotaPerformance = item.IdAvaliacaoPerformance;
                addItem.IdAssociado = item.IdAssociado;
                addItem.Associado = new AssociadosService().ObterAssociado(item.IdAssociado).Nome;
                addItem.IdCargo = item.IdCargo;
                addItem.Cargo = new CargosService().ObterCargo(item.IdCargo).Cargo;
                addItem.IdProjeto = item.IdProjeto;
                addItem.Projeto = new ProjetosService().ObterProjeto(item.IdProjeto).Projeto;
                addItem.IdPeriodo = item.IdPeriodo;
                addItem.Periodo = new PeriodoService().ObterPeriodo(item.IdPeriodo).Periodo;
                addItem.IdPerformance = item.IdPerformance;
                addItem.Performance = new PerformancesService().ObterPerformance(item.IdPerformance).Performance;
                addItem.IdNotaAutoAvaliacao = item.IdNotaNivel1AutoAvaliacao;
                addItem.NotaAutoAvaliacao = notasService.ObterNotaPerformance(item.IdNotaNivel1AutoAvaliacao).CodigoNota;
                addItem.ComentariosAutoAvaliacao = item.ComentariosAutoAvaliacao;
                addItem.IdNotaAvaliacaoAsCegas = item.IdNotaNivel1AvaliacaoCegas;
                addItem.NotaAvaliacaoAsCegas = item.IdNotaNivel1AvaliacaoCegas != null ? notasService.ObterNotaPerformance((int)item.IdNotaNivel1AvaliacaoCegas).CodigoNota : "-";
                addItem.ComentariosAvaliacaoAsCegas = item.ComentariosAvaliacaoCegas;
                addItem.IdNotaAvaliacaoGestor = item.IdNotaNivel1AvaliacaoGestor;
                addItem.NotaAvaliacaoGestor = item.IdNotaNivel1AvaliacaoGestor != null ? notasService.ObterNotaPerformance((int)item.IdNotaNivel1AvaliacaoGestor).CodigoNota : "-";
                addItem.ComentariosAvaliacaoGestor = item.ComentariosAvaliacaoGestor;
                addItem.IdNotaFeedback = item.IdNotaNivel1Feedback;
                addItem.NotaFeedback = item.IdNotaNivel1Feedback != null ? notasService.ObterNotaPerformance((int)item.IdNotaNivel1Feedback).CodigoNota : "-";
                addItem.ComentariosFeedback = item.ComentariosFeedback;
                addItem.IdNotaComite = item.IdNotaComite;
                addItem.NotaComite = item.IdNotaComite != null ? notasService.ObterNotaPerformance((int)item.IdNotaComite).CodigoNota : "-";
                addItem.NotaFinal = item.NotaFinal;
                listItems.Add(addItem);
            }
            string fileName = "NotasPerformances_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listItems);
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        protected void btn_notas_Importar_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUpload_notas.HasFile)
                {
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUpload_notas.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            MessageBox.Show("O arquivo Excel não possui nenhuma planilha.", "", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        var mainService = new AvaliacoesService();
                        int rowCount = worksheet.Dimension.End.Row;
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            int IdNotaPerformance = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                            int IdAssociado = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                            int IdCargo = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                            int IdProjeto = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                            int IdPeriodo = worksheet.Cells[row, 8].GetValue<int?>() ?? 0;
                            int IdPerformance = worksheet.Cells[row, 10].GetValue<int?>() ?? 0;
                            int IdNotaAutoAvaliacao = worksheet.Cells[row, 12].GetValue<int?>() ?? 0;
                            string ComentariosAutoAvaliacao = worksheet.Cells[row, 14].GetValue<string>() ?? "-";
                            int IdNotaAvaliacaoAsCegas = worksheet.Cells[row, 15].GetValue<int?>() ?? 0;
                            string ComentariosAvaliacaoAsCegas = worksheet.Cells[row, 17].GetValue<string>() ?? "-";
                            int IdNotaAvaliacaoGestor = worksheet.Cells[row, 18].GetValue<int?>() ?? 0;
                            string ComentariosAvaliacaoGestor = worksheet.Cells[row, 20].GetValue<string>() ?? "-";
                            int IdNotaFeedback = worksheet.Cells[row, 21].GetValue<int?>() ?? 0;
                            string ComentariosFeedback = worksheet.Cells[row, 23].GetValue<string>() ?? "-";
                            int IdNotaComite = worksheet.Cells[row, 24].GetValue<int?>() ?? 0;
                            int NotaFinal = worksheet.Cells[row, 26].GetValue<int?>() ?? 0;
                            if (IdAssociado > 0 && IdProjeto > 0 && IdCargo > 0 && IdPeriodo > 0 && IdPerformance > 0)
                            {
                                AVALIACOESPERFORMANCES importItem = new AVALIACOESPERFORMANCES();
                                importItem.IdEmpresa = 1;
                                importItem.IdAssociado = IdAssociado;
                                importItem.IdCargo = IdCargo;
                                importItem.IdNivel = 1;
                                importItem.IdProjeto = IdProjeto;
                                importItem.IdPeriodo = IdPeriodo;
                                importItem.IdPerformance = IdPerformance;
                                importItem.IdNotaNivel1AutoAvaliacao = IdNotaAutoAvaliacao;
                                importItem.ComentariosAutoAvaliacao = ComentariosAutoAvaliacao;
                                importItem.IdNotaNivel1AvaliacaoCegas = IdNotaAvaliacaoAsCegas;
                                importItem.ComentariosAvaliacaoCegas = ComentariosAvaliacaoAsCegas;
                                importItem.IdNotaNivel1AvaliacaoGestor = IdNotaAvaliacaoGestor;
                                importItem.ComentariosAvaliacaoGestor = ComentariosAvaliacaoGestor;
                                importItem.IdNotaNivel1Feedback = IdNotaFeedback;
                                importItem.ComentariosFeedback = ComentariosFeedback;
                                importItem.IdNotaComite = IdNotaComite;
                                importItem.NotaFinal = NotaFinal;
                                importItem.ATV = 1;
                                importItem.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                importItem.DHC = DateTime.Now;
                                var existeItem = mainService.ObterAvaliacaoPerformanceEspecifica(IdAssociado, IdProjeto, IdPeriodo, IdPerformance);
                                if (IdNotaPerformance == 0)
                                {
                                    importItem.USRAutoAvaliacao = IdAssociado;
                                    importItem.IdAvaliacaoStatus = 2; // Adiciona com Status "Em Andamento"
                                    importItem.PosicaoAtualFluxoAvaliacao = mainService.etapaAutoAvaliacao;
                                    importItem.DataHoraInicio = DateTime.Now;
                                    importItem.DHCAutoAvaliacao = DateTime.Now;
                                    importItem.DataHoraInicioAutoAvaliacao = DateTime.Now;
                                    mainService.SalvarAvaliacaoPerformance(importItem);
                                    somaLinhasInseridas += 1;
                                }
                                else
                                {
                                    var oldItem = mainService.ObterAvaliacaoPerformance(IdNotaPerformance);
                                    importItem.IdAvaliacaoPerformance = IdNotaPerformance;
                                    importItem.IdNotaNivel1AutoAvaliacao = IdNotaAutoAvaliacao != 0 ? IdNotaAutoAvaliacao : oldItem.IdNotaNivel1AutoAvaliacao;
                                    importItem.ComentariosAutoAvaliacao = ComentariosAutoAvaliacao != "-" ? ComentariosAutoAvaliacao : oldItem.ComentariosAutoAvaliacao;
                                    importItem.IdNotaNivel1AvaliacaoCegas = IdNotaAvaliacaoAsCegas != 0 ? IdNotaAvaliacaoAsCegas : oldItem.IdNotaNivel1AvaliacaoCegas;
                                    importItem.ComentariosAvaliacaoCegas = ComentariosAvaliacaoAsCegas != "-" ? ComentariosAvaliacaoAsCegas : oldItem.ComentariosAvaliacaoCegas;
                                    importItem.IdNotaNivel1AvaliacaoGestor = IdNotaAvaliacaoGestor != 0 ? IdNotaAvaliacaoGestor : oldItem.IdNotaNivel1AvaliacaoGestor;
                                    importItem.ComentariosAvaliacaoGestor = ComentariosAvaliacaoGestor != "-" ? ComentariosAvaliacaoGestor : oldItem.ComentariosAvaliacaoGestor;
                                    importItem.IdNotaNivel1Feedback = IdNotaFeedback != 0 ? IdNotaFeedback : oldItem.IdNotaNivel1Feedback;
                                    importItem.ComentariosFeedback = ComentariosFeedback != "-" ? ComentariosFeedback : oldItem.ComentariosFeedback;
                                    importItem.IdNotaComite = IdNotaComite != 0 ? IdNotaComite : oldItem.IdNotaComite;
                                    importItem.NotaFinal = NotaFinal != 0 ? NotaFinal : oldItem.NotaFinal;
                                    importItem.IdAvaliacaoStatus = oldItem.IdAvaliacaoStatus;
                                    importItem.USRAutoAvaliacao = oldItem.USRAutoAvaliacao;
                                    importItem.DHCAutoAvaliacao = oldItem.DHCAutoAvaliacao;
                                    importItem.PosicaoAtualFluxoAvaliacao = oldItem.PosicaoAtualFluxoAvaliacao;
                                    importItem.DataHoraInicio = oldItem.DataHoraInicio;
                                    importItem.DataHoraTermino = oldItem.DataHoraTermino;
                                    importItem.DHCAutoAvaliacao = oldItem.DHCAutoAvaliacao;
                                    importItem.USRAutoAvaliacao = oldItem.USRAutoAvaliacao;
                                    importItem.DHCAvaliacaoCegas = oldItem.DHCAvaliacaoCegas;
                                    importItem.USRAvaliacaoCegas = oldItem.USRAvaliacaoCegas;
                                    importItem.DHCAvaliacaoGestor = oldItem.DHCAvaliacaoGestor;
                                    importItem.USRAvaliacaoGestor = oldItem.USRAvaliacaoGestor;
                                    importItem.DHCFeedback = oldItem.DHCFeedback;
                                    importItem.USRFeedback = oldItem.USRFeedback;
                                    importItem.DataHoraInicioAutoAvaliacao = oldItem.DataHoraInicioAutoAvaliacao;
                                    importItem.DataHoraFimAutoAvaliacao = oldItem.DataHoraFimAutoAvaliacao;
                                    importItem.DataHoraInicioAvaliacaoCegas = oldItem.DataHoraInicioAvaliacaoCegas;
                                    importItem.DataHoraFimAvaliacaoCegas = oldItem.DataHoraFimAvaliacaoCegas;
                                    importItem.DataHoraInicioAvaliacaoGestor = oldItem.DataHoraInicioAvaliacaoGestor;
                                    importItem.DataHoraFimAvaliacaoGestor = oldItem.DataHoraFimAvaliacaoGestor;
                                    importItem.DataHoraInicioFeedback = oldItem.DataHoraInicioFeedback;
                                    importItem.DataHoraFimFeedback = oldItem.DataHoraFimFeedback;
                                    mainService.AlterarAvaliacaoPerformance(IdNotaPerformance, importItem);
                                    if (IdNotaComite != 0)
                                    {
                                        new ConsolidacaoService().CalculaNotaPerformanceComite(IdNotaPerformance, IdNotaComite);
                                    }
                                    somaLinhasAlteradas += 1;
                                }
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }
                        MessageBox.Show("Associações importadas com sucesso<br>Inseridas: " + somaLinhasInseridas.ToString() + "<br>Alteradas: " + somaLinhasAlteradas.ToString() +
                            "<br>Desconsideradas: " + somaLinhasDesconsideradas.ToString(), "", TIPO.Info, MessageBoxHandler);
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
    }
}