using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TriaSoftware.Util.Framework.Domain.Service;

namespace SistemaAvaliacao
{
    public partial class exportaravaliacoes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregaComboPeriodos();
                CarregaComboProjetos();
                CarregaComboAssociados();
            }
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
            ddlPeriodos.DataSource = statusList;
            ddlPeriodos.DataBind();
            ddlPeriodos.Items.Insert(0, "[Selecionar]");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            PROJETOS projeto = null;
            PERIODOSAVALIACOES periodo = null;
            ASSOCIADOS profissional = null;

            var projetoService = new ProjetosService();
            var periodoService = new PeriodoService();
            var associadosService = new AssociadosService();
            var performanceService = new PerformancesService();
            var avaliacaoService = new AvaliacoesService();
            var notasPerformanceService = new NotasAvaliacaoService();
            var cargosService = new CargosService();

            if (ddlProjetos.SelectedIndex > 0)
                projeto = projetoService.ObterProjeto(Convert.ToInt32(ddlProjetos.Items[ddlProjetos.SelectedIndex].Value));

            if (ddlAssociados.SelectedIndex > 0)
                profissional = associadosService.ObterAssociado(Convert.ToInt32(ddlAssociados.Items[ddlAssociados.SelectedIndex].Value));

            if (ddlPeriodos.SelectedIndex > 0)
                periodo = periodoService.ObterPeriodo(Convert.ToInt32(ddlPeriodos.Items[ddlPeriodos.SelectedIndex].Value));

            var status = new StatusService().ObterStatusAvaliacao("Concluído");

            AVALIACOESSTATUS statusAvaliacao = new AVALIACOESSTATUS();
            statusAvaliacao.IdStatus = 3;

            List<AvaliacaoModel> listaAvaliacoes = new List<AvaliacaoModel>();


            // COMPETÊNCIAS - AV DESEMPENHO
            var listaAvaliacoesCompetencia = new List<AvaliacaoCompetenciaModel>();
            var avaliacoesTodas = avaliacaoService.ObterAvaliacoesCompetencias(profissional, null, projeto, periodo)
                .OrderBy(o => o.IdPeriodo).ThenBy(o => o.IdAssociado).ThenBy(o => o.IdProjeto).ThenBy(o => o.COMPETENCIAS.IdEixo)
                .ThenBy(o => o.COMPETENCIAS.IdSubCompetencia).ThenBy(o => o.IdNotaNivel1AvaliacaoCegas).ThenBy(o => o.IdNotaNivel1Feedback).ToList();

            int contIdRegistro = 0;


            List<AVALIACOESCOMPETENCIASNOTAS> notasCompetencias = new AvaliacoesService().ObterAvaliacaoCompetenciasNotas(ATV: -1);
            List<AVALIACOESPERFORMANCESNOTAS> notasPerformances = new AvaliacoesService().ObterAvaliacaoPerformancesNotas();
            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados();

            var user = WebStorage.GetUsuarioLogado();

            var avaliacoesCompetencia = avaliacoesTodas.Where(x => x.TipoAvaliacao == "desempenho").ToList();
            foreach (var itemProjeto in avaliacoesCompetencia)
            {
                contIdRegistro++;

                var umaAvaliacao = new AvaliacaoCompetenciaModel();
                var alocacao = projetoService.ObterProjetoAssociadoAtributos(itemProjeto.IdAssociado, itemProjeto.IdProjeto, -1, itemProjeto.IdPeriodo, itemProjeto.TipoAvaliacao, itemProjeto.Escopo);

                umaAvaliacao.Nivel = 1;
                //Insere a informação do nível  1
                umaAvaliacao.IdAvaliacaoCompetencia = itemProjeto.IdAvaliacaoCompetencia;
                umaAvaliacao.Período = itemProjeto.PERIODOSAVALIACOES.Periodo;
                umaAvaliacao.Cargo = itemProjeto.ASSOCIADOS.CARGOS.Cargo;
                umaAvaliacao.Projeto = itemProjeto.PROJETOS.Projeto;
                umaAvaliacao.Nome_Avaliado = itemProjeto.ASSOCIADOS.Nome;
                umaAvaliacao.Nome_Gestor = associados.FirstOrDefault(a => a.IdAssociado == itemProjeto.PROJETOS.IdAssociadoGestor).Nome;
                umaAvaliacao.Competencia = itemProjeto.COMPETENCIAS.EIXOS.Eixo;
                umaAvaliacao.SubCompetencia = itemProjeto.COMPETENCIAS.SUBCOMPETENCIAS.SubCompetencia;
                umaAvaliacao.DescricaoCompetencia = itemProjeto.COMPETENCIAS.CompetenciaJR;
                // AUTO AV
                umaAvaliacao.RespondenteAutoAvaliacao = associadosService.ObterAssociado(itemProjeto.IdAssociado).Nome;
                umaAvaliacao.NotaCompetenciaAvaliado = notasCompetencias.FirstOrDefault(a => a.IdNota == itemProjeto.IdNotaNivel1AutoAvaliacao).CodigoNota;
                umaAvaliacao.ComentariosAutoAvaliacao = itemProjeto.ComentariosAutoAvaliacao;
                umaAvaliacao.DHCAutoAv = itemProjeto.DHCAutoAvaliacao;
                // AV CEGAS
                umaAvaliacao.RespondenteAvAsCegas = associadosService.ObterAssociado(itemProjeto.USRAvaliacaoCegas ?? (int)alocacao.IdAvaliador).Nome;
                umaAvaliacao.NotaCompetenciaAvCegas = notasCompetencias.FirstOrDefault(a => a.IdNota == (itemProjeto.IdNotaNivel1AvaliacaoCegas ?? 0)).CodigoNota;
                umaAvaliacao.ComentariosAvCegas = itemProjeto.ComentariosAvaliacaoCegas;
                umaAvaliacao.DHCAvCegas = itemProjeto.DHCAvaliacaoCegas;
                // AV GESTOR
                umaAvaliacao.RespondenteAvGestor = associadosService.ObterAssociado(itemProjeto.USRAvaliacaoGestor ?? (int)alocacao.IdGestor).Nome;
                umaAvaliacao.NotaCompetenciaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == (itemProjeto.IdNotaNivel1AvaliacaoGestor ?? 0)).CodigoNota;
                umaAvaliacao.ComentariosAvGestor = itemProjeto.ComentariosAvaliacaoGestor;
                umaAvaliacao.DHCAvGestor = itemProjeto.DHCAvaliacaoGestor;
                // FEEDBACK
                umaAvaliacao.RespondenteFeedback = associadosService.ObterAssociado(itemProjeto.USRFeedback ?? (int)alocacao.IdGestor).Nome;
                umaAvaliacao.NotaCompetenciaFeedback = notasCompetencias.FirstOrDefault(a => a.IdNota == (itemProjeto.IdNotaNivel1Feedback ?? 0)).CodigoNota;
                umaAvaliacao.ComentariosFeedback = itemProjeto.ComentariosFeedback;
                umaAvaliacao.DHCFeedback = itemProjeto.DHCFeedback;

                umaAvaliacao.NotaCompetenciaAvaliadoPercentual = 0M;
                if (umaAvaliacao.NotaCompetenciaAvaliado.ToUpper() == "POSSUO TOTALMENTE")
                    umaAvaliacao.NotaCompetenciaAvaliadoPercentual = 1M;
                if (umaAvaliacao.NotaCompetenciaAvaliado.ToUpper() == "POSSUO PARCIALMENTE")
                    umaAvaliacao.NotaCompetenciaAvaliadoPercentual = 0.5M;

                umaAvaliacao.NotaCompetenciaGestorPercentual = 0;
                if (umaAvaliacao.NotaCompetenciaGestor.ToUpper() == "POSSUO TOTALMENTE")
                    umaAvaliacao.NotaCompetenciaGestorPercentual = 1M;
                if (umaAvaliacao.NotaCompetenciaGestor.ToUpper() == "POSSUO PARCIALMENTE")
                    umaAvaliacao.NotaCompetenciaGestorPercentual = 0.5M;
                if (umaAvaliacao.NotaCompetenciaGestor.ToUpper() == "POSSUO PARCIALMENTE / NÃO CONCORDA")
                    umaAvaliacao.NotaCompetenciaGestorPercentual = 0.5M;

                umaAvaliacao.IdCompetencia = itemProjeto.COMPETENCIAS.EIXOS.IdEixo;
                umaAvaliacao.IdSubCompetencia = itemProjeto.COMPETENCIAS.SUBCOMPETENCIAS.IdSubCompetencia;
                umaAvaliacao.IdAvaliado = itemProjeto.IdAssociado;
                umaAvaliacao.IdGestor = itemProjeto.PROJETOS.IdAssociadoGestor;
                umaAvaliacao.IdProjeto = itemProjeto.IdProjeto;
                umaAvaliacao.IdPeríodo = itemProjeto.IdPeriodo;
                umaAvaliacao.IdNotaCompetenciaAvaliado = notasCompetencias.FirstOrDefault(a => a.IdNota == Convert.ToInt32(itemProjeto.IdNotaNivel1AutoAvaliacao)).IdNota;
                umaAvaliacao.IdNotaCompetenciaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == Convert.ToInt32(itemProjeto.IdNotaNivel1Feedback)).IdNota;

                umaAvaliacao.IdRegistro = contIdRegistro;

                umaAvaliacao.TipoAvaliacao = itemProjeto.TipoAvaliacao;
                umaAvaliacao.Escopo = itemProjeto.Escopo;

                listaAvaliacoesCompetencia.Add(umaAvaliacao);

                //Insere a nota do nível 2
                umaAvaliacao = new AvaliacaoCompetenciaModel();

                umaAvaliacao.Nivel = 2;
                //Insere a informação do nível  1
                umaAvaliacao.IdAvaliacaoCompetencia = itemProjeto.IdAvaliacaoCompetencia;
                umaAvaliacao.Período = itemProjeto.PERIODOSAVALIACOES.Periodo;
                umaAvaliacao.Cargo = itemProjeto.ASSOCIADOS.CARGOS.Cargo;
                umaAvaliacao.Projeto = itemProjeto.PROJETOS.Projeto;
                umaAvaliacao.Nome_Avaliado = itemProjeto.ASSOCIADOS.Nome;
                umaAvaliacao.Nome_Gestor = associados.FirstOrDefault(a => a.IdAssociado == itemProjeto.PROJETOS.IdAssociadoGestor).Nome;
                umaAvaliacao.Competencia = itemProjeto.COMPETENCIAS.EIXOS.Eixo;
                umaAvaliacao.SubCompetencia = itemProjeto.COMPETENCIAS.SUBCOMPETENCIAS.SubCompetencia;
                umaAvaliacao.DescricaoCompetencia = itemProjeto.COMPETENCIAS.CompetenciaPL;
                // AUTO AV
                umaAvaliacao.RespondenteAutoAvaliacao = associadosService.ObterAssociado(itemProjeto.IdAssociado).Nome;
                umaAvaliacao.NotaCompetenciaAvaliado = notasCompetencias.FirstOrDefault(a => a.IdNota == itemProjeto.IdNotaNivel2AutoAvaliacao).CodigoNota;
                umaAvaliacao.ComentariosAutoAvaliacao = itemProjeto.ComentariosAutoAvaliacao;
                // AV CEGAS
                umaAvaliacao.RespondenteAvAsCegas = associadosService.ObterAssociado(itemProjeto.USRAvaliacaoCegas ?? (int)alocacao.IdAvaliador).Nome;
                umaAvaliacao.NotaCompetenciaAvCegas = notasCompetencias.FirstOrDefault(a => a.IdNota == (itemProjeto.IdNotaNivel2AvaliacaoCegas ?? 0)).CodigoNota;
                umaAvaliacao.ComentariosAvCegas = itemProjeto.ComentariosAvaliacaoCegas;
                // AV GESTOR
                umaAvaliacao.RespondenteAvGestor = associadosService.ObterAssociado(itemProjeto.USRAvaliacaoGestor ?? (int)alocacao.IdGestor).Nome;
                umaAvaliacao.NotaCompetenciaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == (itemProjeto.IdNotaNivel2AvaliacaoGestor ?? 0)).CodigoNota;
                umaAvaliacao.ComentariosAvGestor = itemProjeto.ComentariosAvaliacaoGestor;
                // FEEDBACK
                umaAvaliacao.RespondenteFeedback = associadosService.ObterAssociado(itemProjeto.USRFeedback ?? (int)alocacao.IdGestor).Nome;
                umaAvaliacao.NotaCompetenciaFeedback = notasCompetencias.FirstOrDefault(a => a.IdNota == (itemProjeto.IdNotaNivel2Feedback ?? 0)).CodigoNota;
                umaAvaliacao.ComentariosFeedback = itemProjeto.ComentariosFeedback;

                umaAvaliacao.IdRegistro = contIdRegistro;

                umaAvaliacao.NotaCompetenciaAvaliadoPercentual = 0M;
                if (umaAvaliacao.NotaCompetenciaAvaliado.ToUpper() == "POSSUO TOTALMENTE")
                    umaAvaliacao.NotaCompetenciaAvaliadoPercentual = 1M;
                if (umaAvaliacao.NotaCompetenciaAvaliado.ToUpper() == "POSSUO PARCIALMENTE")
                    umaAvaliacao.NotaCompetenciaAvaliadoPercentual = 0.5M;


                umaAvaliacao.NotaCompetenciaGestorPercentual = 0;
                if (umaAvaliacao.NotaCompetenciaGestor.ToUpper() == "POSSUO TOTALMENTE")
                    umaAvaliacao.NotaCompetenciaGestorPercentual = 1M;
                if (umaAvaliacao.NotaCompetenciaGestor.ToUpper() == "POSSUO PARCIALMENTE")
                    umaAvaliacao.NotaCompetenciaGestorPercentual = 0.5M;
                if (umaAvaliacao.NotaCompetenciaGestor.ToUpper() == "POSSUO PARCIALMENTE / NÃO CONCORDA")
                    umaAvaliacao.NotaCompetenciaGestorPercentual = 0.5M;

                umaAvaliacao.IdCompetencia = itemProjeto.COMPETENCIAS.EIXOS.IdEixo;
                umaAvaliacao.IdSubCompetencia = itemProjeto.COMPETENCIAS.SUBCOMPETENCIAS.IdSubCompetencia;
                umaAvaliacao.IdAvaliado = itemProjeto.IdAssociado;
                umaAvaliacao.IdGestor = itemProjeto.PROJETOS.IdAssociadoGestor;
                umaAvaliacao.IdProjeto = itemProjeto.IdProjeto;
                umaAvaliacao.IdPeríodo = itemProjeto.IdPeriodo;
                umaAvaliacao.IdNotaCompetenciaAvaliado = notasCompetencias.FirstOrDefault(a => a.IdNota == Convert.ToInt32(itemProjeto.IdNotaNivel1AutoAvaliacao)).IdNota;
                umaAvaliacao.IdNotaCompetenciaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == Convert.ToInt32(itemProjeto.IdNotaNivel1Feedback)).IdNota;

                umaAvaliacao.TipoAvaliacao = itemProjeto.TipoAvaliacao;
                umaAvaliacao.Escopo = itemProjeto.Escopo;

                listaAvaliacoesCompetencia.Add(umaAvaliacao);
            }


            var avaliacoes = avaliacaoService.ObterListaAvaliacoes(profissional, status, projeto, periodo)
                .OrderBy(o => o.idPeriodo).ThenBy(o => o.idAssociado).ThenBy(o => o.idProjeto);

            int contLinha = 0;

            foreach (AVALIACAO avaliacao in avaliacoes)
            {
                foreach (AvaliacaoCompetenciaModel linha in listaAvaliacoesCompetencia)
                {

                    if (avaliacao.idAssociado == linha.IdAvaliado && avaliacao.idProjeto == linha.IdProjeto && avaliacao.idPeriodo == linha.IdPeríodo)
                    {

                        //Avaliações Associado
                        int totalLinhasConsideradasAvaliacaoAssociado = listaAvaliacoesCompetencia.Count(a => a.NotaCompetenciaAvaliado.ToUpper() != "NÃO SE APLICA"
                        && a.IdCompetencia == linha.IdCompetencia
                        //&& a.IdSubCompetencia == linha.IdSubCompetencia
                        && a.IdAvaliado == avaliacao.idAssociado
                        && a.IdPeríodo == avaliacao.idPeriodo
                        && a.IdProjeto == avaliacao.idProjeto
                        && a.Nivel == 1);

                        linha.PercentualCompetenciaAssociado = 0M;
                        if (totalLinhasConsideradasAvaliacaoAssociado != 0)
                            linha.PercentualCompetenciaAssociado = Convert.ToDecimal(100 / totalLinhasConsideradasAvaliacaoAssociado);

                        decimal valorTotalNiveisAutoAvaliacao = Convert.ToDecimal(listaAvaliacoesCompetencia.Where(a => a.IdRegistro == linha.IdRegistro).Select(s => s.NotaCompetenciaAvaliadoPercentual).Sum());

                        linha.NotaCompetenciaAvaliadoPercentualFinal = Convert.ToDecimal(valorTotalNiveisAutoAvaliacao * linha.PercentualCompetenciaAssociado);




                        //Avaliações Gestor
                        int totalLinhasConsideradasAvaliacaoGestor = listaAvaliacoesCompetencia.Count(a => a.NotaCompetenciaGestor.ToUpper() != "NÃO SE APLICA"
                        && a.IdCompetencia == linha.IdCompetencia
                        //&& a.IdSubCompetencia == linha.IdSubCompetencia
                        && a.IdAvaliado == avaliacao.idAssociado
                        && a.IdPeríodo == avaliacao.idPeriodo
                        && a.IdProjeto == avaliacao.idProjeto
                        && a.Nivel == 1);


                        linha.PercentualCompetenciaGestor = 0M;
                        if (totalLinhasConsideradasAvaliacaoGestor != 0)
                            linha.PercentualCompetenciaGestor = Convert.ToDecimal(100 / totalLinhasConsideradasAvaliacaoGestor);

                        decimal valorTotalNiveisFeedback = Convert.ToDecimal(listaAvaliacoesCompetencia.Where(a => a.IdRegistro == linha.IdRegistro).Select(s => s.NotaCompetenciaGestorPercentual).Sum());

                        linha.NotaCompetenciaGestorPercentualFinal = Convert.ToDecimal(valorTotalNiveisFeedback * linha.PercentualCompetenciaGestor);

                    }
                    contLinha++;
                }
            }

            //Armazena a Lista Completa para Exportação
            WebStorage.SetList(WebStorage.EnumTipoLista.Competencias, listaAvaliacoesCompetencia);


            // PERFORMANCES - AV DESEMPENHO
            var listaAvaliacoesPerformance = new List<AvaliacaoPerformanceModel>();
            var avaliacoesPerformance = avaliacaoService.ObterAvaliacoesPerformances(profissional, status, projeto, periodo)
                .OrderBy(o => o.IdPeriodo).ThenBy(o => o.IdProjeto).ThenBy(o => o.IdAssociado).ThenBy(o => o.IdPerformance);

            foreach (var itemProjeto in avaliacoesPerformance)
            {
                var umaAvaliacao = new AvaliacaoModel();

                umaAvaliacao.Id = itemProjeto.IdAvaliacaoPerformance;
                umaAvaliacao.Nome_Avaliado = itemProjeto.ASSOCIADOS.Nome;
                umaAvaliacao.Cargo = itemProjeto.ASSOCIADOS.CARGOS.Cargo;
                umaAvaliacao.Nome_Gestor = associados.FirstOrDefault(a => a.IdAssociado == itemProjeto.PROJETOS.IdAssociadoGestor).Nome;
                umaAvaliacao.Período = itemProjeto.PERIODOSAVALIACOES.Periodo;
                umaAvaliacao.Projeto = itemProjeto.PROJETOS.Projeto;
                umaAvaliacao.Tipo = "Performance";
                umaAvaliacao.Nível = performanceService.ObterPerformance(itemProjeto.IdPerformance).Performance;

                listaAvaliacoes.Add(umaAvaliacao);


                //Popula Lista Completa
                var avaliacaoPerformance = new AvaliacaoPerformanceModel();

                avaliacaoPerformance.Nome_Avaliado = umaAvaliacao.Nome_Avaliado;
                avaliacaoPerformance.Cargo = umaAvaliacao.Cargo;
                avaliacaoPerformance.Nome_Gestor = umaAvaliacao.Nome_Gestor;
                avaliacaoPerformance.Período = umaAvaliacao.Período;
                avaliacaoPerformance.Projeto = umaAvaliacao.Projeto;
                avaliacaoPerformance.Performance = umaAvaliacao.Nível;


                string notaAvaliado = notasPerformances.FirstOrDefault(a => a.IdNota == itemProjeto.IdNotaNivel1AutoAvaliacao).CodigoNota;
                string notaFeedback = "0";
                if (itemProjeto.IdNotaNivel1Feedback.HasValue)
                {
                    notaFeedback = notasPerformances.FirstOrDefault(a => a.IdNota == (int)itemProjeto.IdNotaNivel1Feedback).CodigoNota;
                }



                avaliacaoPerformance.Nota_Avaliado = notaAvaliado != null ? notaAvaliado : "0";
                avaliacaoPerformance.Nota_Gestor = notaFeedback != null ? notaFeedback : "0"; ;
                avaliacaoPerformance.Considerações_Avaliado = itemProjeto.ComentariosAutoAvaliacao;
                avaliacaoPerformance.Considerações_Gestor = itemProjeto.ComentariosFeedback;

                listaAvaliacoesPerformance.Add(avaliacaoPerformance);
            }

            //Armazena a Lista Completa para Exportação
            WebStorage.SetList(WebStorage.EnumTipoLista.Performances, listaAvaliacoesPerformance);


            // AV LIDERANÇA
            var listaAvaliacoesLideranca = new List<AvaliacaoLiderancaModel>();
            var podeExtrairLideranca = user.Email == "alexandra.nunes@peers.com.br"
                                        || user.Email == "andre.lima@peers.com.br"
                                        || user.Email == "daniel.silva@peers.com.br"
                                        || user.Email == "dalva.gomes@peers.com.br"
                                        || user.Email == "albert.silva@peers.com.br";
            var anonimizado = user.Email == "andre.lima@peers.com.br";
            if (podeExtrairLideranca)
            {
                var avaliacoesLideranca = avaliacoesTodas.Where(x => x.TipoAvaliacao == "lideranca").ToList();
                var listRandoms = new Dictionary<int, int>();
                foreach (var avaliacao in avaliacoesLideranca)
                {
                    var addAvaliacaoLideranca = new AvaliacaoLiderancaModel();
                    var idLiderado = (int)avaliacaoService.ObterAvaliacao(avaliacao.idAvaliacao).idGestor;
                    var randLiderado = new Random().Next(1, 9999);
                    if (!listRandoms.ContainsKey(idLiderado))
                    {
                        while (listRandoms.ContainsValue(randLiderado))
                        {
                            randLiderado = new Random().Next(1, 9999);
                        }
                        listRandoms[idLiderado] = randLiderado;
                    }
                    else
                    {
                        randLiderado = listRandoms[idLiderado];
                    }

                    addAvaliacaoLideranca.Periodo = avaliacao.PERIODOSAVALIACOES.Periodo;
                    addAvaliacaoLideranca.Projeto = avaliacao.PROJETOS.Projeto;
                    addAvaliacaoLideranca.Lider = associadosService.ObterAssociado(avaliacao.IdAssociado).Nome;
                    addAvaliacaoLideranca.Pilar = avaliacao.COMPETENCIAS.EIXOS.Eixo;
                    addAvaliacaoLideranca.SubCompetencia = avaliacao.COMPETENCIAS.SUBCOMPETENCIAS.SubCompetencia;
                    addAvaliacaoLideranca.Descricao = avaliacao.COMPETENCIAS.CompetenciaJR;
                    addAvaliacaoLideranca.TipoAvaliacao = avaliacao.TipoAvaliacao;
                    addAvaliacaoLideranca.Escopo = avaliacao.Escopo;
                    addAvaliacaoLideranca.Liderado = anonimizado ? randLiderado.ToString() : associadosService.ObterAssociado(idLiderado).Nome;
                    addAvaliacaoLideranca.Nota = notasCompetencias.FirstOrDefault(x => x.IdNota == (avaliacao.IdNotaNivel1AvaliacaoGestor ?? 0)).CodigoNota;
                    addAvaliacaoLideranca.Comentarios = avaliacao.ComentariosAvaliacaoGestor;
                    addAvaliacaoLideranca.DHCLiderado = avaliacao.DHCAvaliacaoGestor;

                    listaAvaliacoesLideranca.Add(addAvaliacaoLideranca);
                }
            }
            WebStorage.SetList(WebStorage.EnumTipoLista.Lideranca, listaAvaliacoesLideranca);

            rptProjetos.DataSource = listaAvaliacoes;
            rptProjetos.DataBind();

            btnExportar.Visible = true;
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            //Obtem as listas
            var listaCompetencia = WebStorage.GetList<AvaliacaoCompetenciaModel>(WebStorage.EnumTipoLista.Competencias);
            var listaPerformance = WebStorage.GetList<AvaliacaoPerformanceModel>(WebStorage.EnumTipoLista.Performances);
            var listaLideranca = WebStorage.GetList<AvaliacaoLiderancaModel>(WebStorage.EnumTipoLista.Lideranca);

            string fileName = "Avaliacao_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            //Gera arquivo
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcel(fileName, listaCompetencia, listaPerformance, listaLideranca);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
    }
}