using Business.DataAccess;
using Business.Model;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ConsolidacaoService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConsolidacaoService));
        DataModel _context;
        public ConsolidacaoService()
        {
            _context = new DataModel();
        }

        public ResultadoProjetosModel RadarFeedback(int idassociado, int idprojeto, int idperiodo, int idcargo)
        {
            try
            {
                var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
                ResultadoServices resultadoService = new ResultadoServices();
                ResultadoProjetosModel projeto = new ResultadoProjetosModel();
                IEnumerable<EIXOS> listEixos = _context.EIXOS.Where(x => x.ATV.HasValue && x.ATV.Value == 1 && x.TipoAvaliacao == "desempenho");
                List<ResultadoCompetenciaModel> competencias = resultadoService.SimulaNotaCompetenciaFinalN1N2(idassociado, idprojeto, idperiodo);
                
                List<string> eixosCompetencias = competencias.Select(x => x.IdEixo.ToString()).ToList();
                eixosCompetencias.Sort();
                IEnumerable<EIXOS> tempEixos = listEixos;
                tempEixos = listEixos.Where(x => eixosCompetencias.Contains(x.IdEixo.ToString()));
                listEixos = tempEixos;

                var listIdEixos = listEixos.Select(eix => eix.IdEixo);
                competencias = competencias.Where(x => listIdEixos.Contains(x.IdEixo)).ToList();

                projeto.ListSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>();
                
                foreach (var eixo in listEixos)
                {
                    var listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);


                    if (listCompetenciasProjetoEixo != null)
                    {                                               
                        //Feedback
                        var somaNotaNivel1 = listCompetenciasProjetoEixo.Sum(x => x.NotaFinalNivel1);

                        decimal? percentualNivel1 = 0;

                        if (somaNotaNivel1 > 0)
                        {
                            if (somaNotaNivel1 == 0.99M)
                            {
                                somaNotaNivel1 = 1;
                            }
                            percentualNivel1 = (somaNotaNivel1 * 100);
                        }
                                            
                        //AVALIADO
                        var somaNotaNivel1Avaliado = listCompetenciasProjetoEixo.Sum(x => x.NotaFinalNivel1Avaliado);

                        decimal? percentualNivel1Avaliado = 0;

                        if (somaNotaNivel1Avaliado > 0)
                        {
                            if (somaNotaNivel1Avaliado == 0.99M)
                            {
                                somaNotaNivel1Avaliado = 1;
                            }
                            percentualNivel1Avaliado = (somaNotaNivel1Avaliado * 100);
                        }
                                               
                        //FEEDBACK
                        var somaNotaNivel2 = listCompetenciasProjetoEixo.Sum(x => x.NotaFinalNivel2);

                        decimal? percentualNivel2 = 0;

                        if (somaNotaNivel2 > 0)
                        {
                            if (somaNotaNivel2 == 0.99M)
                            {
                                somaNotaNivel2 = 1;
                            }
                            percentualNivel2 = (somaNotaNivel2 * 100);
                        }

                        //AVALIADO
                        var somaNotaNivel2Avaliado = listCompetenciasProjetoEixo.Sum(x => x.NotaFinalNivel2Avaliado);

                        decimal? percentualNivel2Avaliado = 0;

                        if (somaNotaNivel2Avaliado > 0)
                        {
                            if (somaNotaNivel2Avaliado == 0.99M)
                            {
                                somaNotaNivel2Avaliado = 1;
                            }
                            percentualNivel2Avaliado = (somaNotaNivel2Avaliado * 100);
                        }

                        decimal? somaPercentualN1N2 = percentualNivel2 + percentualNivel1;
                        decimal? somaPercentualN1N2Avaliado = percentualNivel2Avaliado + percentualNivel1Avaliado;

                        ResultadoCompetenciaModel n1n2 = new ResultadoCompetenciaModel
                        {                            
                            Eixo = eixo.Eixo,
                            PercentualNotaFinalNivel1 = percentualNivel1,
                            PercentualNotaFinalNivel1Avaliado = percentualNivel1Avaliado,
                            PercentualNotaFinalNivel2 = percentualNivel2,
                            PercentualNotaFinalNivel2Avaliado = percentualNivel2Avaliado,
                            PercentualSomaNotaFinalN1N2 = somaPercentualN1N2,
                            PercentualSomaNotaFinalN1N2Avaliado = somaPercentualN1N2Avaliado
                        };

                        projeto.ListSomaCompetenciasN1N2.Add(n1n2);


                    }

                }            

                return projeto;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao recalcular as notas do radar: " + ex.Message);
            }
        }

        // NÃO USADO
        public ResultadoProjetosModel RadarConsolidacao(int idassociado, int idprojeto, int idperiodo, int idcargo, int idAvaliacao)
        {
            try
            {
                var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
                ResultadoServices resultadoService = new ResultadoServices();
                ResultadoProjetosModel projeto = new ResultadoProjetosModel();
                IEnumerable<EIXOS> listEixos = _context.EIXOS.Where(x => x.ATV.HasValue && x.ATV.Value == 1);
                IEnumerable<ResultadoCompetenciaModel> competencias = resultadoService.ObterListAvaliacoesCompetenciasProjeto(idassociado, idprojeto, idperiodo, idAvaliacao);

                var notaRadarCargo = 0;

                var premissasradar = _context.PREMISSAS_RADAR.FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdCargo == idcargo);

                if (premissasradar != null)
                {
                    notaRadarCargo = premissasradar.ValorBaseAvaliacaoGestor;
                }

                projeto.ListSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>();

                foreach (var eixo in listEixos)
                {
                    var listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);

                    if (listCompetenciasProjetoEixo != null)
                    {
                        if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel1.HasValue) > 0)
                        {
                            resultadoService.CalculaNotaCompetenciaFinalN1N2(idassociado, idprojeto, idperiodo);
                            competencias = resultadoService.ObterListAvaliacoesCompetenciasProjeto(idassociado, idprojeto, idperiodo, idAvaliacao);
                            listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);
                        }

                        if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel2.HasValue) > 0)
                        {
                            resultadoService.CalculaNotaCompetenciaFinalN1N2(idassociado, idprojeto, idperiodo);
                            competencias = resultadoService.ObterListAvaliacoesCompetenciasProjeto(idassociado, idprojeto, idperiodo, idAvaliacao);
                            listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);
                        }

                        var somaNotaNivel1 = listCompetenciasProjetoEixo.Sum(x => x.NotaFinalNivel1);

                        decimal? percentualNivel1 = 0;

                        if (somaNotaNivel1 > 0)
                        {
                            if (somaNotaNivel1 == 0.99M)
                            {
                                somaNotaNivel1 = 1;
                            }
                            percentualNivel1 = (somaNotaNivel1 * 100);
                        }

                        var somaNotaNivel2 = listCompetenciasProjetoEixo.Sum(x => x.NotaFinalNivel2);

                        decimal? percentualNivel2 = 0;

                        if (somaNotaNivel2 > 0)
                        {
                            if (somaNotaNivel2 == 0.99M)
                            {
                                somaNotaNivel2 = 1;
                            }
                            percentualNivel2 = (somaNotaNivel2 * 100);
                        }

                        decimal? somaPercentualN1N2 = percentualNivel2 + percentualNivel1;

                        ResultadoCompetenciaModel soman1n2 = new ResultadoCompetenciaModel
                        {
                            Eixo = eixo.Eixo,
                            PercentualSomaNotaFinalN1N2 = somaPercentualN1N2,
                            NotaCompetenciaRadar = somaPercentualN1N2.HasValue && somaPercentualN1N2.Value > 0 ? ((somaPercentualN1N2.Value / 100) + notaRadarCargo) * 100 : 0
                        };

                        projeto.ListSomaCompetenciasN1N2.Add(soman1n2);
                    }

                }

                projeto.SomaNotaCompetenciaRadar = projeto.ListSomaCompetenciasN1N2.Average(x => x.NotaCompetenciaRadar);

                return projeto;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao recalcular as notas do radar: " + ex.Message);
            }
        }

        public decimal? CalculaNotaPerformanceComite(int idavaliacaoperformance, int idnota)
        {
            try
            {
                var avaliacaoperformance = _context.AVALIACOESPERFORMANCES.FirstOrDefault(x => x.IdAvaliacaoPerformance == idavaliacaoperformance);
                decimal? notacomite = null;

                if (avaliacaoperformance != null)
                {
                    var pesoNota = _context.AVALIACOESPERFORMANCESNOTAS.FirstOrDefault(x => x.IdNota == idnota);

                    if (pesoNota != null)
                    {
                        avaliacaoperformance.IdNotaComite = idnota;
                        avaliacaoperformance.NotaFinal = pesoNota.Peso;
                        notacomite = avaliacaoperformance.NotaFinal;
                        _context.SaveChanges();
                    }
                    else
                    {
                        if (avaliacaoperformance.IdNotaComite.HasValue)
                        {
                            var pesoNotaGestor = _context.AVALIACOESPERFORMANCESNOTAS.FirstOrDefault(a => a.IdNota == avaliacaoperformance.IdNotaNivel1Feedback.Value);
                            avaliacaoperformance.IdNotaComite = null;
                            avaliacaoperformance.NotaFinal = pesoNotaGestor != null ? pesoNotaGestor.Peso : 0;
                            notacomite = avaliacaoperformance.NotaFinal;
                            _context.SaveChanges();
                        }
                    }
                }

                return notacomite;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao tentar calcular a nota de competência do comitê: " + ex.Message);
            }
        }

        public decimal? CalculaNotaCompetenciaComite(int idavaliacaocompetencia, int nivel, int idnota)
        {
            try
            {
                var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
                var avaliacoesService = new AvaliacoesService();
                var avaliacaoassociado = _context.AVALIACOESCOMPETENCIAS.FirstOrDefault(x => x.IdAvaliacaoCompetencia == idavaliacaocompetencia);
                var competencia = avaliacaoassociado.COMPETENCIAS;
                var pesoNota = _context.AVALIACOESCOMPETENCIASNOTAS.FirstOrDefault(x => x.IdNota == idnota);

                decimal? notacomite = null;

                if (avaliacaoassociado != null && competencia != null)
                {
                    var listCompetenciasEixo = (from a in _context.AVALIACOESCOMPETENCIAS
                                                join c in _context.COMPETENCIAS
                                                on a.IdCompetencia equals c.IdCompetencia
                                                where a.IdAssociado == avaliacaoassociado.IdAssociado
                                                && a.IdProjeto == avaliacaoassociado.IdProjeto
                                                && a.IdPeriodo == avaliacaoassociado.IdPeriodo
                                                && c.IdEixo == competencia.IdEixo
                                                && a.IdEmpresa == idEmpresa
                                                select a);

                    var qtdCompetenciasEixo = listCompetenciasEixo.Count();

                    if (pesoNota != null)
                    {
                        if (pesoNota.Peso > 0)
                        {
                            if (competencia.IdModo == 1)
                            {
                                notacomite = (pesoNota.Peso * 1) / qtdCompetenciasEixo;
                            }
                        }
                        else
                        {
                            notacomite = 0;
                        }

                        if (competencia.IdModo == 2)
                        {
                            var getResultado = avaliacoesService.ObterResultadoLiderComoCompetencia(avaliacaoassociado.IdAssociado, avaliacaoassociado.IdPeriodo);
                            notacomite = getResultado.mediaTotal / qtdCompetenciasEixo;
                        }

                        if (nivel == 1)
                        {
                            avaliacaoassociado.NotaFinalNivel1 = notacomite / 100;
                            avaliacaoassociado.IdNotaNivel1Comite = idnota;
                            _context.SaveChanges();
                        }
                        else if (nivel == 2)
                        {
                            avaliacaoassociado.NotaFinalNivel2 = notacomite / 100;
                            avaliacaoassociado.IdNotaNivel2Comite = idnota;
                            _context.SaveChanges();
                        }

                        if (notacomite > 0)
                        {
                            notacomite = notacomite / 100;
                        }
                    }
                    else
                    {
                        if (nivel == 1)
                        {
                            if (avaliacaoassociado.IdNotaNivel1Comite.HasValue && avaliacaoassociado.IdNotaNivel1Comite.Value > 0)
                            {
                                var notasCompetencias = _context.AVALIACOESCOMPETENCIASNOTAS;
                                int idNaoSeAplica = notasCompetencias.FirstOrDefault(x => x.CodigoNota == "Não se Aplica").IdNota;
                                var pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == avaliacaoassociado.IdNotaNivel1Feedback.Value).Peso;

                                var qtdCompetenciasNaoSeAplica = listCompetenciasEixo.Count(x => x.IdNotaNivel1Feedback.HasValue && x.IdNotaNivel1Feedback.Value == idNaoSeAplica);


                                if (pesoNotaGestor > 0)
                                {
                                    avaliacaoassociado.NotaFinalNivel1 = ((pesoNotaGestor / 100) / (qtdCompetenciasEixo - qtdCompetenciasNaoSeAplica));
                                    notacomite = avaliacaoassociado.NotaFinalNivel1;
                                }

                                avaliacaoassociado.IdNotaNivel1Comite = null;
                                _context.SaveChanges();
                            }
                        }
                        else if (nivel == 2)
                        {
                            if (avaliacaoassociado.IdNotaNivel2Comite.HasValue && avaliacaoassociado.IdNotaNivel2Comite.Value > 0)
                            {
                                var notasCompetencias = _context.AVALIACOESCOMPETENCIASNOTAS;
                                int idNaoSeAplica = notasCompetencias.FirstOrDefault(x => x.CodigoNota == "Não se Aplica").IdNota;
                                var pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == avaliacaoassociado.IdNotaNivel2Feedback.Value).Peso;

                                var qtdCompetenciasNaoSeAplica = listCompetenciasEixo.Count(x => x.IdNotaNivel2Feedback.HasValue && x.IdNotaNivel2Feedback.Value == idNaoSeAplica);

                                if (pesoNotaGestor > 0)
                                {
                                    avaliacaoassociado.NotaFinalNivel2 = ((pesoNotaGestor / 100) / (qtdCompetenciasEixo - qtdCompetenciasNaoSeAplica));
                                    notacomite = avaliacaoassociado.NotaFinalNivel2;
                                }

                                avaliacaoassociado.IdNotaNivel2Comite = null;
                                _context.SaveChanges();
                            }
                        }
                    }

                }

                return notacomite;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao tentar calcular a nota de competência do comitê: " + ex.Message);
            }

        }

        public List<ResultadoPerfomanceModel> PerformanceProjetoAssociado(int idassociado, int idprojeto, int idperiodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            var listNotasAvaliacoesPerformance = _context.AVALIACOESPERFORMANCESNOTAS;

            return (from a in _context.AVALIACOESPERFORMANCES
                    join p in _context.PERFORMANCES
                    on a.IdPerformance equals p.IdPerformance
                    where a.IdEmpresa == idEmpresa
                    && a.IdAssociado == idassociado
                    && a.IdPeriodo == idperiodo
                    && a.IdProjeto == idprojeto
                    select new ResultadoPerfomanceModel()
                    {
                        IdAvaliacaoPerformance = a.IdAvaliacaoPerformance,
                        Perfomance = p.Performance,
                        IdNotaNivel1AutoAvaliacao = a.IdNotaNivel1AutoAvaliacao,
                        NotaNivel1AutoAvaliacao = listNotasAvaliacoesPerformance.FirstOrDefault(x => x.IdNota == a.IdNotaNivel1AutoAvaliacao).CodigoNota,
                        IdNotaNivel1Feedback = a.IdNotaNivel1Feedback,
                        NotaNivel1Feedback = a.IdNotaNivel1Feedback.HasValue ? listNotasAvaliacoesPerformance.FirstOrDefault(x => x.IdNota == a.IdNotaNivel1Feedback).CodigoNota : string.Empty,
                        ComentariosAutoAvaliacao = a.ComentariosAutoAvaliacao,
                        ComentarioFeedback = a.ComentariosFeedback,
                        IdNotaComite = a.IdNotaComite,
                        NotaPerfomancePonderada = a.IdNotaNivel1Feedback.HasValue ? listNotasAvaliacoesPerformance.FirstOrDefault(x => x.IdNota == a.IdNotaNivel1Feedback).Peso : 0,
                        NotaPerfomance = a.NotaFinal
                    }).ToList();
        }

        public List<ResultadoCompetenciaModel> CompetenciaProjetoAssociado(int idassociado, int idprojeto, int idperiodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            var avaliacoesService = new AvaliacoesService();

            var listNotasAvaliacoesCompetencia = _context.AVALIACOESCOMPETENCIASNOTAS;

            var listCompetencias = from ac in _context.AVALIACOESCOMPETENCIAS
                                   join c in _context.COMPETENCIAS
                                   on ac.IdCompetencia equals c.IdCompetencia
                                   join e in _context.EIXOS
                                   on c.IdEixo equals e.IdEixo
                                   where ac.IdEmpresa == idEmpresa
                                   && ac.IdAssociado == idassociado
                                   && ac.IdProjeto == idprojeto
                                   && ac.IdPeriodo == idperiodo
                                   && ac.ATV == 1
                                   select new ResultadoCompetenciaModel()
                                   {
                                       IdAvaliacaoCompetencia = ac.IdAvaliacaoCompetencia,
                                       Eixo = e.Eixo,
                                       IdEixo = e.IdEixo,
                                       IdNotaNivel1AutoAvaliacao = ac.IdNotaNivel1AutoAvaliacao,
                                       NotaNivel1AutoAvaliacao = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel1AutoAvaliacao).CodigoNota,
                                       IdNotaNivel2AutoAvaliacao = ac.IdNotaNivel2AutoAvaliacao,
                                       NotaNivel2AutoAvaliacao = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel2AutoAvaliacao).CodigoNota,
                                       IdNotaNivel1Feedback = ac.IdNotaNivel1Feedback,
                                       NotaNivel1Feedback = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel1Feedback).CodigoNota,
                                       IdNotaNivel2Feedback = ac.IdNotaNivel2Feedback,
                                       NotaNivel2Feedback = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel2Feedback).CodigoNota,
                                       ComentarioAvaliado = ac.ComentariosAutoAvaliacao,
                                       ComentarioFeedback = ac.ComentariosFeedback,
                                       NotaCompetenciaAvaliado = ac.NotaCompetenciaAvaliado,
                                       NotaCompetenciaGestor = ac.NotaCompetenciaGestor,
                                       NotaSubcompetenciaAvaliadoN1 = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel1AutoAvaliacao).Peso,
                                       NotaSubcompetenciaAvaliadoN2 = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel2AutoAvaliacao).Peso,
                                       NotaSubcompetenciaGestorN1 = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel1Feedback).Peso,
                                       NotaSubcompetenciaGestorN2 = listNotasAvaliacoesCompetencia.FirstOrDefault(x => x.IdNota == ac.IdNotaNivel2Feedback).Peso,
                                       NotaFinalNivel1 = ac.NotaFinalNivel1,
                                       NotaFinalNivel2 = ac.NotaFinalNivel2,
                                       DetalheNivelAtual = c.CompetenciaJRDetalhe,
                                       CompetenciaAtual = c.CompetenciaJR,
                                       CompetenciaProximo = c.CompetenciaPL,
                                       DetalheProximoNivel = c.CompetenciaPLDetalhe,
                                       IdNotaNivel1Comite = ac.IdNotaNivel1Comite,
                                       IdNotaNivel2Comite = ac.IdNotaNivel2Comite,
                                       Competencia = ac.COMPETENCIAS,
                                       idAvaliado = ac.IdAssociado,
                                       Periodo = ac.PERIODOSAVALIACOES,
                                       enableNivel1 = true,
                                       enableNivel2 = true
                                   };

            var listReturn = listCompetencias.ToList();

            foreach (var item in listReturn)
            {
                if (item.Competencia.IdModo == 2) // AUTO PREENCHIMENTO - MÉDIA AV. LIDERANÇA
                {
                    var getResultado = avaliacoesService.ObterResultadoLiderComoCompetencia(item.idAvaliado, item.Periodo.IdPeriodo);
                    item.NotaNivel1AutoAvaliacao = item.Competencia.VisivelNivel1 ? getResultado.textoMediaTotal : "N/A";
                    item.NotaNivel2AutoAvaliacao = item.Competencia.VisivelNivel2 ? getResultado.textoMediaTotal : "N/A";
                    item.NotaNivel1Feedback = "N/A"; // item.Competencia.VisivelFeedback && item.Competencia.VisivelNivel1 ? getResultado.textoMediaTotal : "N/A";
                    item.NotaNivel2Feedback = "N/A"; // item.Competencia.VisivelFeedback && item.Competencia.VisivelNivel2 ? getResultado.textoMediaTotal : "N/A";
                    item.NotaSubcompetenciaAvaliadoN1 = null; // item.Competencia.VisivelAutoAvaliacao && item.Competencia.VisivelNivel1 ? getResultado.mediaTotal : 0;
                    item.NotaSubcompetenciaAvaliadoN2 = null; // item.Competencia.VisivelAutoAvaliacao && item.Competencia.VisivelNivel2 ? getResultado.mediaTotal : 0;
                    item.NotaSubcompetenciaGestorN1 = null; // item.Competencia.VisivelFeedback && item.Competencia.VisivelNivel1 ? getResultado.mediaTotal : 0;
                    item.NotaSubcompetenciaGestorN2 = null; // item.Competencia.VisivelFeedback && item.Competencia.VisivelNivel2 ? getResultado.mediaTotal : 0;
                    item.NotaCompetenciaAvaliado = null;
                    item.NotaCompetenciaGestor = null;
                    item.enableNivel1 = false;
                    item.enableNivel2 = false;

                    CalculaNotaCompetenciaComite(item.IdAvaliacaoCompetencia, 1, 0);
                    CalculaNotaCompetenciaComite(item.IdAvaliacaoCompetencia, 2, 0);
                }
            }

            return listReturn;

        }

        public List<AssociadosModel> ObterListConsolidacao(int idprojeto, int idassociado, int idperiodo, string TipoAvaliacao, string Escopo)
        {

            var cargos = _context.CARGOS;
            var mentores = _context.ASSOCIADOS;
            var periodos = _context.PERIODOSAVALIACOES;

            return (from ava in _context.AVALIACAO
                    join a in _context.ASSOCIADOS
                    on ava.idAssociado equals a.IdAssociado
                    join p in _context.PROJETOS
                    on ava.idProjeto equals p.IdProjeto
                    where idassociado > 0 ? a.IdAssociado == idassociado : 1 == 1
                    && idperiodo > 0 ? ava.idPeriodo == idperiodo : 1 == 1
                    && idprojeto > 0 ? ava.idProjeto == idprojeto : 1 == 1
                    where ava.TipoAvaliacao == TipoAvaliacao
                    && ava.Escopo == Escopo
                    select new AssociadosModel()
                    {
                        Nome = a.Nome,
                        Cargo = cargos.FirstOrDefault(x => x.IdCargo == a.IdCargo).Cargo,
                        Periodo = periodos.FirstOrDefault(x => x.IdPeriodo == ava.idPeriodo).Periodo,
                        IdAssociado = ava.idAssociado,
                        IdPeriodo = ava.idPeriodo,
                        Projeto = p.Projeto,
                        IdProjeto = ava.idProjeto,
                        TipoAvaliacao = ava.TipoAvaliacao,
                        Escopo = ava.Escopo
                    }).ToList();


        }
    }
}
