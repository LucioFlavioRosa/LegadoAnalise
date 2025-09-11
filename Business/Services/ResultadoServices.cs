using Business.DataAccess;
using Business.Model;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using TriaSoftware.Util.Framework.Domain.Service;

namespace Business.Services
{
    public class ResultadoServices
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ResultadoServices));
        DataModel _context;

        public ResultadoServices()
        {
            _context = new DataModel();
        }

        public List<AssociadosModel> ObterListResultado(int idprojeto, int idassociado, int idperiodo)
        {
            var cargos = _context.CARGOS;
            var mentores = _context.ASSOCIADOS;
            var periodos = _context.PERIODOSAVALIACOES;

            return (from ava in _context.AVALIACAO
                    join a in _context.ASSOCIADOS
                    on ava.idAssociado equals a.IdAssociado
                    where idassociado > 0 ? a.IdAssociado == idassociado : 1 == 1
                    && idperiodo > 0 ? ava.idPeriodo == idperiodo : 1 == 1
                    && idprojeto > 0 ? ava.idProjeto == idprojeto : 1 == 1
                    where ava.PosicaoAtualFluxoAvaliacao == "AFI" || ava.PosicaoAtualFluxoAvaliacao == "AME"
                    group new { a.IdAssociado, a.Nome, a.IdCargo, a.IdAssociadoMentor, ava.idProjeto, ava.idPeriodo, ava.TipoAvaliacao, ava.Escopo }
                    by new { a.IdAssociado, a.Nome, a.IdAssociadoMentor, ava.idPeriodo, ava.TipoAvaliacao, ava.Escopo } into grp
                    orderby grp.Key.idPeriodo descending
                    select new AssociadosModel()
                    {
                        Nome = grp.FirstOrDefault().Nome,
                        Cargo = cargos.FirstOrDefault(x => x.IdCargo == grp.FirstOrDefault().IdCargo).Cargo,
                        Mentor = mentores.FirstOrDefault(x => x.IdAssociado == grp.FirstOrDefault().IdAssociadoMentor).Nome,
                        Periodo = periodos.FirstOrDefault(x => x.IdPeriodo == grp.Key.idPeriodo).Periodo,
                        IdAssociado = grp.Key.IdAssociado,
                        IdPeriodo = grp.Key.idPeriodo,
                        QtdProjetos = grp.Count(),
                        TipoAvaliacao = grp.Key.TipoAvaliacao,
                        Escopo = grp.Key.Escopo
                    }).ToList();
        }

        public IEnumerable<ResultadoCompetenciaModel> ObterListAvaliacoesCompetenciasProjeto(int idassociado, int idprojeto, int idperiodo, int idAvaliacao)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            return from ac in _context.AVALIACOESCOMPETENCIAS
                   join c in _context.COMPETENCIAS
                   on ac.IdCompetencia equals c.IdCompetencia
                   join e in _context.EIXOS
                   on c.IdEixo equals e.IdEixo
                   where ac.IdEmpresa == idEmpresa
                   && ac.IdAssociado == idassociado
                   && ac.IdProjeto == idprojeto
                   && ac.IdPeriodo == idperiodo
                   && ac.ATV == 1
                   && ac.idAvaliacao == idAvaliacao
                   select new ResultadoCompetenciaModel()
                   {
                       Eixo = e.Eixo,
                       IdEixo = e.IdEixo,
                       IdCargo = ac.IdCargo,
                       NotaFinalNivel1 = ac.NotaFinalNivel1,
                       NotaFinalNivel2 = ac.NotaFinalNivel2
                   };

        }

        public IEnumerable<ResultadoCompetenciaModel> ObterListAvaliacoesCompetenciasProjeto_Otimizado(List<AVALIACOESCOMPETENCIAS> listAvCompetencias, List<COMPETENCIAS> listCompetencias,
            int idassociado, int idprojeto, int idperiodo, int idAvaliacao)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            return from ac in listAvCompetencias
                   join c in listCompetencias
                   on ac.IdCompetencia equals c.IdCompetencia
                   join e in _context.EIXOS
                   on c.IdEixo equals e.IdEixo
                   where ac.IdEmpresa == idEmpresa
                   && ac.IdAssociado == idassociado
                   && ac.IdProjeto == idprojeto
                   && ac.IdPeriodo == idperiodo
                   && ac.ATV == 1
                   && ac.idAvaliacao == idAvaliacao
                   select new ResultadoCompetenciaModel()
                   {
                       Eixo = e.Eixo,
                       IdEixo = e.IdEixo,
                       IdCargo = ac.IdCargo,
                       NotaFinalNivel1 = ac.NotaFinalNivel1,
                       NotaFinalNivel2 = ac.NotaFinalNivel2
                   };

        }

        // OBSOLETO DEVIDO A MÁ PERFORMANCE
        public List<ResultadoProjetosModel> ObterResultadoAssociado_OBSOLETO(int idassociado, int idperiodo, int idCargo, string TipoAvaliacao, string Escopo)
        {
            try
            {
                var avaliacoesService = new AvaliacoesService();
                string avalFim = avaliacoesService.etapaAvaliacaoFinalizada;
                string avalMentor = avaliacoesService.etapaAvaliacaoMentor;
                var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

                var periodo = _context.PERIODOSAVALIACOES.FirstOrDefault(x => x.IdPeriodo == idperiodo);
                var periodoProximo = _context.PERIODOSAVALIACOES.OrderByDescending(x => x.IdPeriodo).FirstOrDefault(x => x.IdPeriodo > periodo.IdPeriodo && x.ATV == 1);
                if (periodoProximo == null)
                {
                    periodoProximo = new PERIODOSAVALIACOES
                    {
                        DataInicio = _context.PROJETOSASSOCIADOS.Max(pa => pa.DataInicio).AddDays(1)
                    };
                }

                var projetosassociados = _context.PROJETOSASSOCIADOS.Where(x =>
                                                                            x.IdAssociado == idassociado
                                                                            && x.ATV == 1
                                                                            && x.TipoAvaliacao == TipoAvaliacao
                                                                            && x.Escopo == Escopo
                );

                var listTeste = projetosassociados.ToList();

                var avaliacoesTodas = _context.AVALIACAO.Where(x =>
                    x.idAssociado == idassociado &&
                    x.idEmpresa == idEmpresa &&
                    x.TipoAvaliacao == TipoAvaliacao &&
                    x.Escopo == Escopo &&
                    (x.PosicaoAtualFluxoAvaliacao == avalFim || x.PosicaoAtualFluxoAvaliacao == avalMentor));

                var listProjetosAssociadoExtendido = (from a in avaliacoesTodas
                                                      join pa in projetosassociados
                                                       on new
                                                       {
                                                           IdAssociado = a.idAssociado,
                                                           IdProjeto = a.idProjeto
                                                       }
                                                       equals new
                                                       {
                                                           IdAssociado = pa.IdAssociado,
                                                           IdProjeto = pa.IdProjeto
                                                       }
                                                      join p in _context.PROJETOS
                                                      on a.idProjeto equals p.IdProjeto
                                                      join g in _context.ASSOCIADOS
                                                      on pa.IdGestor equals g.IdAssociado
                                                      join c in _context.PROJETOSCOMPLEXIDADES
                                                      on p.IdComplexidade equals c.IdComplexidade
                                                      where a.idEmpresa == idEmpresa
                                                      && a.idAssociado == idassociado
                                                      && a.TipoAvaliacao == TipoAvaliacao
                                                      && a.Escopo == Escopo
                                                      && p.ATV == 1
                                                      && (a.PosicaoAtualFluxoAvaliacao == avalFim || a.PosicaoAtualFluxoAvaliacao == avalMentor)
                                                      select new ResultadoProjetosModel()
                                                      {
                                                          IdProjeto = a.idProjeto,
                                                          IdAssociado = a.idAssociado,
                                                          NomeGestor = g.Nome,
                                                          IdComplexidade = c.IdComplexidade,
                                                          IdAvaliador = (int)pa.IdAvaliador,
                                                          IdResponsavel = p.IdAssociadoResponsavel,
                                                          IdCargo = idCargo,
                                                          ProjetoComplexidade = c.Complexidade,
                                                          PesoProjetoComplexidade = c.Peso,
                                                          ProjetoNome = p.Projeto,
                                                          ClienteNome = p.CLIENTES.Cliente,
                                                          DataInicioAlocado = pa.DataInicio,
                                                          DataFimAlocado = pa.DataFim,
                                                          IdPeriodo = a.idPeriodo,
                                                          Periodo = a.PERIODOSAVALIACOES.Periodo,
                                                          IdGestor = a.idGestor ?? 0,
                                                          IdAvaliacao = a.idAvaliacao,
                                                          TipoAvaliacao = a.TipoAvaliacao,
                                                          Escopo = a.Escopo
                                                      }).ToList();

                var listProjetosAssociadoAmplo = listProjetosAssociadoExtendido.Where(x => x.IdPeriodo == idperiodo && x.DataInicioAlocado >= periodo.DataInicio).ToList();

                var listProjetosAssociado = listProjetosAssociadoAmplo.Where(x => projetosassociados.Select(pa => pa.IdGestor).Contains(x.IdGestor)).ToList();


                if (listProjetosAssociado == null || listProjetosAssociado.Count <= 0)
                {
                    listProjetosAssociado = listProjetosAssociadoAmplo;
                }
                if (listProjetosAssociado == null || listProjetosAssociado.Count <= 0)
                {
                    listProjetosAssociado = listProjetosAssociadoExtendido;
                }

                if (listProjetosAssociado != null)
                {
                    IEnumerable<EIXOS> listEixos = _context.EIXOS.Where(x => x.ATV.HasValue && x.ATV.Value == 1);

                    var notaRadarCargo = 0;

                    if (TipoAvaliacao == "lideranca") { idCargo = _context.CARGOS.FirstOrDefault(c => c.Cargo == "Líder").IdCargo; }
                    var premissasradar = _context.PREMISSAS_RADAR.FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdCargo == idCargo);

                    if (premissasradar != null)
                    {
                        notaRadarCargo = premissasradar.ValorBaseAvaliacaoGestor;
                    }

                    var rating = _context.RATING;

                    foreach (var projeto in listProjetosAssociado)
                    {
                        var competencias = ObterListAvaliacoesCompetenciasProjeto(idassociado, projeto.IdProjeto, idperiodo, projeto.IdAvaliacao).ToList();

                        List<string> eixosCompetencias = competencias.Select(x => x.IdEixo.ToString()).ToList();
                        eixosCompetencias.Sort();
                        IEnumerable<EIXOS> tempEixos = listEixos;
                        tempEixos = listEixos.Where(x => eixosCompetencias.Contains(x.IdEixo.ToString()));
                        listEixos = tempEixos;

                        projeto.ListCompetenciasNivel1 = new List<ResultadoCompetenciaModel>();
                        projeto.ListCompetenciasNivel2 = new List<ResultadoCompetenciaModel>();
                        projeto.ListSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>();

                        projeto.DiasProjeto = CalculoDiasProjeto(idperiodo, projeto.DataInicioAlocado, projeto.DataFimAlocado.Value);

                        foreach (var eixo in listEixos)
                        {
                            var listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);

                            if (listCompetenciasProjetoEixo != null)
                            {
                                //if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel1.HasValue) > 0)
                                //{
                                CalculaNotaCompetenciaFinalN1N2(idassociado, projeto.IdProjeto, idperiodo);
                                competencias = ObterListAvaliacoesCompetenciasProjeto(idassociado, projeto.IdProjeto, idperiodo, projeto.IdAvaliacao).ToList();
                                listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);
                                //}

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

                                ResultadoCompetenciaModel n1 = new ResultadoCompetenciaModel
                                {
                                    Nivel = 1,
                                    Eixo = eixo.Eixo,
                                    PercentualNotaFinalNivel1 = percentualNivel1
                                };
                                projeto.ListCompetenciasNivel1.Add(n1);


                                //if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel2.HasValue) > 0)
                                //{
                                CalculaNotaCompetenciaFinalN1N2(idassociado, projeto.IdProjeto, idperiodo);
                                competencias = ObterListAvaliacoesCompetenciasProjeto(idassociado, projeto.IdProjeto, idperiodo, projeto.IdAvaliacao).ToList();
                                listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);
                                //}

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

                                ResultadoCompetenciaModel n2 = new ResultadoCompetenciaModel
                                {
                                    Nivel = 2,
                                    Eixo = eixo.Eixo,
                                    PercentualNotaFinalNivel2 = percentualNivel2
                                };
                                projeto.ListCompetenciasNivel2.Add(n2);


                                decimal? somaPercentualN1N2 = percentualNivel2 + percentualNivel1;

                                ResultadoCompetenciaModel soman1n2 = new ResultadoCompetenciaModel
                                {
                                    IdEixo = eixo.IdEixo,
                                    Eixo = eixo.Eixo,
                                    PercentualSomaNotaFinalN1N2 = somaPercentualN1N2,
                                    NotaCompetenciaRadar = somaPercentualN1N2.HasValue && somaPercentualN1N2.Value > 0 ? ((somaPercentualN1N2.Value / 100) + notaRadarCargo) * 100 : 0
                                };

                                projeto.ListSomaCompetenciasN1N2.Add(soman1n2);

                            }

                            // CALCULO ALTERADO PARA DIVIDIR A NOTA MÉDIA DE COMPETÊNCIAS PELO NÚMERO DE PERGUNTAS RESPONDIDAS, NÃO PELO NÚMERO DE EIXOS
                            // projeto.NotaCompetencialNivel1 = projeto.ListCompetenciasNivel1.Sum(x => x.PercentualNotaFinalNivel1) / listEixos.Count();
                            // projeto.NotaCompetencialNivel2 = projeto.ListCompetenciasNivel2.Sum(x => x.PercentualNotaFinalNivel2) / listEixos.Count();
                            projeto.NotaCompetencialNivel1 = projeto.ListCompetenciasNivel1.Where(x => x.PercentualNotaFinalNivel1 != 0).Average(x => x.PercentualNotaFinalNivel1);
                            projeto.NotaCompetencialNivel2 = projeto.ListCompetenciasNivel2.Where(x => x.PercentualNotaFinalNivel2 != 0).Average(x => x.PercentualNotaFinalNivel2);
                            projeto.SomaNotaCompetencialN1N2 = projeto.NotaCompetencialNivel1 + projeto.NotaCompetencialNivel2;

                        }

                        projeto.SomaNotaCompetenciaRadar = projeto.ListSomaCompetenciasN1N2.Average(x => x.NotaCompetenciaRadar);

                        //var existCalculoPerfomance = _context.AVALIACOESPERFORMANCES.Where(x => x.IdEmpresa == idEmpresa && x.IdAssociado == idassociado && x.IdProjeto == projeto.IdProjeto && x.IdPeriodo == idperiodo).Count(x => !x.NotaFinal.HasValue);

                        //if (existCalculoPerfomance > 0)
                        //{
                        CalcularNotaPerfomance(idassociado, projeto.IdProjeto, idperiodo);
                        //}

                        projeto.ListPerfomance = (from a in _context.AVALIACOESPERFORMANCES
                                                  join p in _context.PERFORMANCES
                                                  on a.IdPerformance equals p.IdPerformance
                                                  where a.IdEmpresa == idEmpresa
                                                  && a.IdAssociado == idassociado
                                                  && a.IdPeriodo == idperiodo
                                                  && a.IdProjeto == projeto.IdProjeto
                                                  && a.IdPerformance != 364
                                                  select new ResultadoPerfomanceModel()
                                                  {
                                                      Perfomance = p.Performance,
                                                      NotaPerfomance = a.NotaFinal,
                                                      IdPerformance = p.IdPerformance
                                                  }).ToList();

                        projeto.SomaPerfomance = 0;

                        if (projeto.ListPerfomance != null && projeto.ListPerfomance.Count > 0)
                        {
                            projeto.SomaPerfomance = (projeto.ListPerfomance.Sum(x => x.NotaPerfomance) / projeto.ListPerfomance.Count());
                        }

                        //provisório
                        if (projeto.IdComplexidade == 8)
                        {
                            projeto.IdComplexidade = 7;
                        }

                        var result = rating.Where(d => d.IdComplexidade == projeto.IdComplexidade && projeto.SomaPerfomance >= d.FaixaInicial &&
                            projeto.SomaPerfomance <= d.FaixaFinal).OrderByDescending(x => x.FaixaFinal);

                        if (result != null)
                        {
                            projeto.RatingPerfomance = result.FirstOrDefault().Performance;
                        }

                    }

                }

                return listProjetosAssociado;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Ops!! Erro ao tentar efetuar os cálculos do resultado. " + ex.Message);
            }
        }

        public List<ResultadoProjetosModel> ObterResultadoAssociado_Otimizado(List<AVALIACAO> listAvaliacao, List<AVALIACOESCOMPETENCIAS> listAvCompetencias, List<COMPETENCIAS> listCompetencias,
            List<AVALIACOESPERFORMANCES> listAvPerformance, List<PERFORMANCES> listPerformances,
            int idassociado, int idperiodo, int idCargo, string TipoAvaliacao, string Escopo)
        {
            try
            {
                var avaliacoesService = new AvaliacoesService();
                string avalFim = avaliacoesService.etapaAvaliacaoFinalizada;
                string avalMentor = avaliacoesService.etapaAvaliacaoMentor;
                var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

                var periodo = _context.PERIODOSAVALIACOES.FirstOrDefault(x => x.IdPeriodo == idperiodo);
                var periodoProximo = _context.PERIODOSAVALIACOES.OrderByDescending(x => x.IdPeriodo).FirstOrDefault(x => x.IdPeriodo > periodo.IdPeriodo && x.ATV == 1);
                if (periodoProximo == null)
                {
                    periodoProximo = new PERIODOSAVALIACOES
                    {
                        DataInicio = _context.PROJETOSASSOCIADOS.Max(pa => pa.DataInicio).AddDays(1)
                    };
                }

                var projetosassociados = _context.PROJETOSASSOCIADOS.Where(x =>
                                                                            x.IdAssociado == idassociado
                                                                            && x.ATV == 1
                                                                            && x.TipoAvaliacao == TipoAvaliacao
                                                                            && x.Escopo == Escopo
                );

                var listTeste = projetosassociados.ToList();

                var avaliacoesTodas = listAvaliacao.Where(x =>
                    x.idAssociado == idassociado &&
                    x.idEmpresa == idEmpresa &&
                    x.TipoAvaliacao == TipoAvaliacao &&
                    x.Escopo == Escopo &&
                    (x.PosicaoAtualFluxoAvaliacao == avalFim || x.PosicaoAtualFluxoAvaliacao == avalMentor));

                var listProjetosAssociadoExtendido = (from a in avaliacoesTodas
                                                      join pa in projetosassociados
                                                       on new
                                                       {
                                                           IdAssociado = a.idAssociado,
                                                           IdProjeto = a.idProjeto
                                                       }
                                                       equals new
                                                       {
                                                           IdAssociado = pa.IdAssociado,
                                                           IdProjeto = pa.IdProjeto
                                                       }
                                                      join p in _context.PROJETOS
                                                      on a.idProjeto equals p.IdProjeto
                                                      join g in _context.ASSOCIADOS
                                                      on pa.IdGestor equals g.IdAssociado
                                                      join c in _context.PROJETOSCOMPLEXIDADES
                                                      on p.IdComplexidade equals c.IdComplexidade
                                                      where a.idEmpresa == idEmpresa
                                                      && a.idAssociado == idassociado
                                                      && a.TipoAvaliacao == TipoAvaliacao
                                                      && a.Escopo == Escopo
                                                      && p.ATV == 1
                                                      && (a.PosicaoAtualFluxoAvaliacao == avalFim || a.PosicaoAtualFluxoAvaliacao == avalMentor)
                                                      select new ResultadoProjetosModel()
                                                      {
                                                          IdProjeto = a.idProjeto,
                                                          IdAssociado = a.idAssociado,
                                                          NomeGestor = g.Nome,
                                                          IdComplexidade = c.IdComplexidade,
                                                          IdAvaliador = (int)pa.IdAvaliador,
                                                          IdResponsavel = p.IdAssociadoResponsavel,
                                                          IdCargo = idCargo,
                                                          ProjetoComplexidade = c.Complexidade,
                                                          PesoProjetoComplexidade = c.Peso,
                                                          ProjetoNome = p.Projeto,
                                                          ClienteNome = p.CLIENTES.Cliente,
                                                          DataInicioAlocado = pa.DataInicio,
                                                          DataFimAlocado = pa.DataFim,
                                                          IdPeriodo = a.idPeriodo,
                                                          Periodo = a.PERIODOSAVALIACOES.Periodo,
                                                          IdGestor = a.idGestor ?? 0,
                                                          IdAvaliacao = a.idAvaliacao,
                                                          TipoAvaliacao = a.TipoAvaliacao,
                                                          Escopo = a.Escopo
                                                      }).ToList();

                var listProjetosAssociadoAmplo = listProjetosAssociadoExtendido.Where(x => x.IdPeriodo == idperiodo && x.DataInicioAlocado >= periodo.DataInicio).ToList();

                var listProjetosAssociado = listProjetosAssociadoAmplo.Where(x => projetosassociados.Select(pa => pa.IdGestor).Contains(x.IdGestor)).ToList();


                if (listProjetosAssociado == null || listProjetosAssociado.Count <= 0)
                {
                    listProjetosAssociado = listProjetosAssociadoAmplo;
                }
                if (listProjetosAssociado == null || listProjetosAssociado.Count <= 0)
                {
                    listProjetosAssociado = listProjetosAssociadoExtendido;
                }

                if (listProjetosAssociado != null)
                {
                    IEnumerable<EIXOS> listEixos = _context.EIXOS.Where(x => x.ATV.HasValue && x.ATV.Value == 1);

                    var notaRadarCargo = 0;

                    if (TipoAvaliacao == "lideranca") { idCargo = _context.CARGOS.FirstOrDefault(c => c.Cargo == "Líder").IdCargo; }
                    var premissasradar = _context.PREMISSAS_RADAR.FirstOrDefault(x => x.IdEmpresa == idEmpresa && x.IdCargo == idCargo);

                    if (premissasradar != null)
                    {
                        notaRadarCargo = premissasradar.ValorBaseAvaliacaoGestor;
                    }

                    var rating = _context.RATING;

                    foreach (var projeto in listProjetosAssociado)
                    {
                        IEnumerable<ResultadoCompetenciaModel> competencias = ObterListAvaliacoesCompetenciasProjeto_Otimizado(listAvCompetencias, listCompetencias,
                            idassociado, projeto.IdProjeto, idperiodo, projeto.IdAvaliacao);

                        List<string> eixosCompetencias = competencias.Select(x => x.IdEixo.ToString()).ToList();
                        eixosCompetencias.Sort();
                        IEnumerable<EIXOS> tempEixos = listEixos;
                        tempEixos = listEixos.Where(x => eixosCompetencias.Contains(x.IdEixo.ToString()));
                        listEixos = tempEixos;

                        projeto.ListCompetenciasNivel1 = new List<ResultadoCompetenciaModel>();
                        projeto.ListCompetenciasNivel2 = new List<ResultadoCompetenciaModel>();
                        projeto.ListSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>();

                        projeto.DiasProjeto = CalculoDiasProjeto(idperiodo, projeto.DataInicioAlocado, projeto.DataFimAlocado.Value);

                        foreach (var eixo in listEixos)
                        {
                            var listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);

                            if (listCompetenciasProjetoEixo != null)
                            {
                                //if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel1.HasValue) > 0)
                                //{
                                CalculaNotaCompetenciaFinalN1N2_Otimizado(listAvaliacao, listAvCompetencias, listCompetencias, idassociado, projeto.IdProjeto, idperiodo);
                                competencias = ObterListAvaliacoesCompetenciasProjeto_Otimizado(listAvCompetencias, listCompetencias,
                                    idassociado, projeto.IdProjeto, idperiodo, projeto.IdAvaliacao);
                                listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);
                                //}

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

                                ResultadoCompetenciaModel n1 = new ResultadoCompetenciaModel
                                {
                                    Nivel = 1,
                                    Eixo = eixo.Eixo,
                                    PercentualNotaFinalNivel1 = percentualNivel1
                                };
                                projeto.ListCompetenciasNivel1.Add(n1);


                                //if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel2.HasValue) > 0)
                                //{
                                CalculaNotaCompetenciaFinalN1N2_Otimizado(listAvaliacao, listAvCompetencias, listCompetencias, idassociado, projeto.IdProjeto, idperiodo);
                                competencias = ObterListAvaliacoesCompetenciasProjeto_Otimizado(listAvCompetencias, listCompetencias,
                                    idassociado, projeto.IdProjeto, idperiodo, projeto.IdAvaliacao);
                                listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);
                                //}

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

                                ResultadoCompetenciaModel n2 = new ResultadoCompetenciaModel
                                {
                                    Nivel = 2,
                                    Eixo = eixo.Eixo,
                                    PercentualNotaFinalNivel2 = percentualNivel2
                                };
                                projeto.ListCompetenciasNivel2.Add(n2);


                                decimal? somaPercentualN1N2 = percentualNivel2 + percentualNivel1;

                                ResultadoCompetenciaModel soman1n2 = new ResultadoCompetenciaModel
                                {
                                    IdEixo = eixo.IdEixo,
                                    Eixo = eixo.Eixo,
                                    PercentualSomaNotaFinalN1N2 = somaPercentualN1N2,
                                    NotaCompetenciaRadar = somaPercentualN1N2.HasValue && somaPercentualN1N2.Value > 0 ? ((somaPercentualN1N2.Value / 100) + notaRadarCargo) * 100 : 0
                                };

                                projeto.ListSomaCompetenciasN1N2.Add(soman1n2);

                            }

                            // CALCULO ALTERADO PARA DIVIDIR A NOTA MÉDIA DE COMPETÊNCIAS PELO NÚMERO DE PERGUNTAS RESPONDIDAS, NÃO PELO NÚMERO DE EIXOS
                            // projeto.NotaCompetencialNivel1 = projeto.ListCompetenciasNivel1.Sum(x => x.PercentualNotaFinalNivel1) / listEixos.Count();
                            // projeto.NotaCompetencialNivel2 = projeto.ListCompetenciasNivel2.Sum(x => x.PercentualNotaFinalNivel2) / listEixos.Count();
                            projeto.NotaCompetencialNivel1 = projeto.ListCompetenciasNivel1.Where(x => x.PercentualNotaFinalNivel1 != 0).Average(x => x.PercentualNotaFinalNivel1);
                            projeto.NotaCompetencialNivel2 = projeto.ListCompetenciasNivel2.Where(x => x.PercentualNotaFinalNivel2 != 0).Average(x => x.PercentualNotaFinalNivel2);
                            projeto.SomaNotaCompetencialN1N2 = projeto.NotaCompetencialNivel1 + projeto.NotaCompetencialNivel2;

                        }

                        projeto.SomaNotaCompetenciaRadar = projeto.ListSomaCompetenciasN1N2.Average(x => x.NotaCompetenciaRadar);

                        //var existCalculoPerfomance = _context.AVALIACOESPERFORMANCES.Where(x => x.IdEmpresa == idEmpresa && x.IdAssociado == idassociado && x.IdProjeto == projeto.IdProjeto && x.IdPeriodo == idperiodo).Count(x => !x.NotaFinal.HasValue);

                        //if (existCalculoPerfomance > 0)
                        //{
                        CalcularNotaPerfomance_Otimizado(listAvPerformance, idassociado, projeto.IdProjeto, idperiodo);
                        //}

                        projeto.ListPerfomance = (from a in listAvPerformance
                                                  join p in listPerformances
                                                  on a.IdPerformance equals p.IdPerformance
                                                  where a.IdEmpresa == idEmpresa
                                                  && a.IdAssociado == idassociado
                                                  && a.IdPeriodo == idperiodo
                                                  && a.IdProjeto == projeto.IdProjeto
                                                  && a.IdPerformance != 364
                                                  select new ResultadoPerfomanceModel()
                                                  {
                                                      Perfomance = p.Performance,
                                                      NotaPerfomance = a.NotaFinal,
                                                      IdPerformance = p.IdPerformance
                                                  }).ToList();

                        projeto.SomaPerfomance = 0;

                        if (projeto.ListPerfomance != null && projeto.ListPerfomance.Count > 0)
                        {
                            projeto.SomaPerfomance = (projeto.ListPerfomance.Sum(x => x.NotaPerfomance) / projeto.ListPerfomance.Count());
                        }

                        //provisório
                        if (projeto.IdComplexidade == 8)
                        {
                            projeto.IdComplexidade = 7;
                        }

                        var result = rating.Where(d => d.IdComplexidade == projeto.IdComplexidade && projeto.SomaPerfomance >= d.FaixaInicial &&
                            projeto.SomaPerfomance <= d.FaixaFinal).OrderByDescending(x => x.FaixaFinal);

                        if (result != null)
                        {
                            projeto.RatingPerfomance = result.FirstOrDefault().Performance;
                        }

                    }

                }

                return listProjetosAssociado;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Ops!! Erro ao tentar efetuar os cálculos do resultado. " + ex.Message);
            }
        }

        public List<ResultadoSomaProjetosModel> SomaResultadoProjetos(List<ResultadoProjetosModel> listProjetosAssociado)
        {
            try
            {
                List<ResultadoSomaProjetosModel> source = new List<ResultadoSomaProjetosModel>();
                ResultadoSomaProjetosModel model = new ResultadoSomaProjetosModel();

                if (listProjetosAssociado != null)
                {
                    int totalDiasAlocadoEmProjetos = listProjetosAssociado.Sum(x => x.DiasProjeto);
                    var rating = _context.RATING;

                    List<ResultadoPerfomanceModel> listperformance = new List<ResultadoPerfomanceModel>();
                    List<ResultadoCompetenciaModel> listcompetencias = new List<ResultadoCompetenciaModel>();
                    decimal pesoPonderadoProjetoComplexidade = 0;
                    decimal competenciaCargo = 0;
                    decimal competenciaProximoCargo = 0;

                    foreach (var projeto in listProjetosAssociado)
                    {
                        decimal diasPonderadoProjeto = ((decimal)projeto.DiasProjeto / (decimal)totalDiasAlocadoEmProjetos);

                        foreach (var item in projeto.ListPerfomance)
                        {
                            ResultadoPerfomanceModel itemPerformance = listperformance.FirstOrDefault(x => x.Perfomance == item.Perfomance);

                            if (itemPerformance != null)
                            {
                                itemPerformance.NotaPerfomancePonderada += (diasPonderadoProjeto * item.NotaPerfomance);
                            }
                            else
                            {
                                listperformance.Add(new ResultadoPerfomanceModel
                                {
                                    Perfomance = item.Perfomance,
                                    NotaPerfomancePonderada = (diasPonderadoProjeto * item.NotaPerfomance),
                                    IdPerformance = item.IdPerformance
                                });
                            }
                        }

                        decimal soman1n2 = 0;
                        decimal soman1n2Radar = 0;
                        int count = 0;

                        foreach (var icomp in projeto.ListSomaCompetenciasN1N2)
                        {
                            ResultadoCompetenciaModel itemCompetencia = listcompetencias.FirstOrDefault(x => x.Eixo == icomp.Eixo);

                            if (itemCompetencia != null)
                            {
                                itemCompetencia.NotaProjetosCompetencia += (diasPonderadoProjeto * icomp.PercentualSomaNotaFinalN1N2);
                                itemCompetencia.NotaProjetoCompetenciaRadar += (diasPonderadoProjeto * icomp.NotaCompetenciaRadar);
                            }
                            else
                            {
                                listcompetencias.Add(new ResultadoCompetenciaModel
                                {
                                    Eixo = icomp.Eixo,
                                    NotaProjetosCompetencia = (diasPonderadoProjeto * icomp.PercentualSomaNotaFinalN1N2),
                                    NotaProjetoCompetenciaRadar = (diasPonderadoProjeto * icomp.NotaCompetenciaRadar),
                                    IdEixo = icomp.IdEixo
                                });
                            }

                            soman1n2Radar += icomp.NotaCompetenciaRadar ?? 0;
                            soman1n2 += icomp.PercentualSomaNotaFinalN1N2 ?? 0;

                            count++;
                        }

                        count = Math.Max(count, 1);
                        var calcn1n2 = (soman1n2 / count) * diasPonderadoProjeto;

                        if (model.SomaProjetosNotaCompetencialN1N2.HasValue)
                        {
                            model.SomaProjetosNotaCompetencialN1N2 += calcn1n2;
                        }
                        else
                        {
                            model.SomaProjetosNotaCompetencialN1N2 = calcn1n2;
                        }

                        var calcn1n2radar = (soman1n2Radar / count) * diasPonderadoProjeto;

                        if (model.SomaProjetosNotaCompetenciaRadar.HasValue)
                        {
                            model.SomaProjetosNotaCompetenciaRadar += calcn1n2radar;
                        }
                        else
                        {
                            model.SomaProjetosNotaCompetenciaRadar = calcn1n2radar;
                        }

                        if (projeto.PesoProjetoComplexidade.HasValue)
                        {
                            pesoPonderadoProjetoComplexidade += diasPonderadoProjeto * projeto.PesoProjetoComplexidade.Value;
                        }

                        if (projeto.NotaCompetencialNivel1.HasValue && projeto.DiasProjeto > 0)
                        {
                            competenciaCargo += projeto.NotaCompetencialNivel1.Value * projeto.DiasProjeto;
                        }

                        if (projeto.NotaCompetencialNivel2.HasValue && projeto.DiasProjeto > 0)
                        {
                            competenciaProximoCargo += projeto.NotaCompetencialNivel2.Value * projeto.DiasProjeto;
                        }
                    }


                    model.ListProjetosSomaPerfomance = new List<ResultadoPerfomanceModel>();
                    model.ListProjetosSomaPerfomance.AddRange(listperformance);
                    model.SomaProjetosPerfomance = listperformance.Average(x => x.NotaPerfomancePonderada);

                    model.ListProjetosSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>();
                    model.ListProjetosSomaCompetenciasN1N2.AddRange(listcompetencias);

                    var projetoComplexidade = _context.PROJETOSCOMPLEXIDADES;
                    var maiorValorComplexidade = projetoComplexidade.Max(x => x.Peso);
                    int idcomplexidade = 0;


                    if (pesoPonderadoProjetoComplexidade >= maiorValorComplexidade)
                    {
                        var pcomplexidade = projetoComplexidade.FirstOrDefault(x => x.Peso == maiorValorComplexidade);
                        model.ComplexidadeComite = pcomplexidade.Complexidade;
                        idcomplexidade = pcomplexidade.IdComplexidade;
                    }
                    else
                    {
                        var listPeso = projetoComplexidade.Where(x => x.PesoPonderado <= pesoPonderadoProjetoComplexidade);

                        foreach (var item in listPeso.OrderByDescending(x => x.Peso))
                        {
                            if (pesoPonderadoProjetoComplexidade >= item.PesoPonderado)
                            {
                                model.ComplexidadeComite = item.Complexidade;
                                idcomplexidade = item.IdComplexidade;
                                break;
                            }
                        }
                    }

                    var result = rating.Where(d => d.IdComplexidade == idcomplexidade && model.SomaProjetosPerfomance >= d.FaixaInicial && model.SomaProjetosPerfomance <= d.FaixaFinal).OrderByDescending(x => x.FaixaFinal).FirstOrDefault();

                    if (result != null)
                    {
                        model.RatingPerfomance = result.Performance;
                    }

                    if (totalDiasAlocadoEmProjetos > 0)
                    {
                        model.CompetenciaCargo = competenciaCargo / totalDiasAlocadoEmProjetos;
                        model.CompetenciaProximoCargo = competenciaProximoCargo / totalDiasAlocadoEmProjetos;
                    }
                    else
                    {
                        model.CompetenciaCargo = 0;
                        model.CompetenciaProximoCargo = 0;
                    }

                    source.Add(model);
                }

                return source;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Ops!! Erro ao tentar efetuar a soma dos projetos. " + ex.Message);
            }
        }

        public List<ResultadoSomaProjetosModel> SomaResultadoLideranca(int idAssociado, int idPeriodo, string TipoAvaliacao, string Escopo)
        {
            List<ResultadoSomaProjetosModel> resultadoSomaProjetosModels = new List<ResultadoSomaProjetosModel>();
            List<int> eixos = new List<int>();

            DataModel context = new DataModel();
            var avaliacoesService = new AvaliacoesService();
            var eixosService = new EixoService();

            var getAvaliacoesCompetencias = avaliacoesService.ObterAvaliacoesCompetenciasPeriodoFinalizadas(idAssociado, idPeriodo, TipoAvaliacao, Escopo);

            resultadoSomaProjetosModels.Add(new ResultadoSomaProjetosModel());

            resultadoSomaProjetosModels[0].ListProjetosSomaPerfomance = new List<ResultadoPerfomanceModel>();
            resultadoSomaProjetosModels[0].ListProjetosSomaPerfomance.Add(new ResultadoPerfomanceModel());
            resultadoSomaProjetosModels[0].ListProjetosSomaPerfomance[0].IdPerformance = context.PERFORMANCES.FirstOrDefault(p => p.Performance == "Performance Lideres").IdPerformance;
            resultadoSomaProjetosModels[0].ListProjetosSomaPerfomance[0].NotaPerfomancePonderada = 0;
            resultadoSomaProjetosModels[0].SomaProjetosPerfomance = 0;

            resultadoSomaProjetosModels[0].ListProjetosSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>();

            foreach (var avaliacao in getAvaliacoesCompetencias)
            {
                var avaliacaoEixo = eixosService.ObterEixo(avaliacao.COMPETENCIAS.IdEixo);
                if (!eixos.Contains(avaliacaoEixo.IdEixo))
                {
                    eixos.Add(avaliacaoEixo.IdEixo);

                    resultadoSomaProjetosModels[0].ListProjetosSomaCompetenciasN1N2.Add(new ResultadoCompetenciaModel());
                    int lastIndex = resultadoSomaProjetosModels[0].ListProjetosSomaCompetenciasN1N2.Count - 1;
                    resultadoSomaProjetosModels[0].ListProjetosSomaCompetenciasN1N2[lastIndex].IdEixo = avaliacaoEixo.IdEixo;

                    var getAvaliacoesEixo = getAvaliacoesCompetencias.FindAll(ac => ac.COMPETENCIAS.IdEixo == avaliacaoEixo.IdEixo);

                    int totalAvaliacoesEixo = getAvaliacoesEixo.Count();
                    int respostasTotalmente = getAvaliacoesEixo.FindAll(ae => ae.IdNotaNivel1AvaliacaoGestor == 2).Count();
                    int respostasParcialmente = getAvaliacoesEixo.FindAll(ae => ae.IdNotaNivel1AvaliacaoGestor == 3).Count();
                    int respostasNegativo = getAvaliacoesEixo.FindAll(ae => ae.IdNotaNivel1AvaliacaoGestor == 4).Count();

                    double resultado = ((respostasTotalmente) + (respostasParcialmente * 0.5) + (respostasNegativo * 0)) / totalAvaliacoesEixo;
                    resultado = resultado * 100;

                    resultadoSomaProjetosModels[0].ListProjetosSomaCompetenciasN1N2[lastIndex].NotaProjetosCompetencia = (decimal?)resultado;
                }
            }
            resultadoSomaProjetosModels[0].SomaProjetosNotaCompetencialN1N2 = resultadoSomaProjetosModels[0].ListProjetosSomaCompetenciasN1N2.Average(e => e.NotaProjetosCompetencia);

            return resultadoSomaProjetosModels;
        }

        private void CalcularNotaPerfomance(int idassociado, int idprojeto, int idperiodo)
        {
            DataModel _context = new DataModel();

            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            var listperfomances = (from a in _context.AVALIACOESPERFORMANCES
                                   join n in _context.AVALIACOESPERFORMANCESNOTAS
                                   on a.IdNotaNivel1Feedback equals n.IdNota into notas
                                   from anotas in notas.DefaultIfEmpty()
                                   where a.IdEmpresa == idEmpresa
                                   && a.IdAssociado == idassociado
                                    && a.IdProjeto == idprojeto
                                    && a.IdPeriodo == idperiodo
                                   select new
                                   {
                                       PERFOMANCE = a,
                                       NOTAS = anotas
                                   }).ToList();

            if (listperfomances != null)
            {
                foreach (var item in listperfomances)
                {
                    if (item.PERFOMANCE.IdNotaComite.HasValue)
                    {
                        decimal novanota = NotaPerformanceComite(item.PERFOMANCE.IdNotaComite.Value);

                        if (novanota > -1)
                        {
                            item.PERFOMANCE.NotaFinal = novanota;
                        }
                    }
                    else
                    {
                        item.PERFOMANCE.NotaFinal = item.NOTAS != null ? item.NOTAS.Peso : 0;
                    }

                    _context.SaveChanges();
                }
            }
        }

        public void ReportMentoria(int idPeriodo, HttpServerUtility Server)
        {
            var respostas = from mentorado in _context.ASSOCIADOS
                            join resposta in _context.MENTORADORESPOSTAS on mentorado.IdAssociado equals resposta.idMentorado into mentoradoRespostas
                            from resposta in mentoradoRespostas.DefaultIfEmpty()
                            where resposta == null || resposta.idPeriodo == idPeriodo
                            join mentor in _context.ASSOCIADOS on resposta.idMentor equals mentor.IdAssociado into mentorRespostas
                            from mentor in mentorRespostas.DefaultIfEmpty()
                            join pergunta in _context.MENTORPERGUNTASNOTAS on resposta.idNota equals pergunta.idNota into perguntaRespostas
                            from pergunta in perguntaRespostas.DefaultIfEmpty()
                            join questao in _context.MENTORPERGUNTAS on resposta.idPergunta equals questao.idMentorPergunta into questaoRespostas
                            from questao in questaoRespostas.DefaultIfEmpty()
                            select new
                            {
                                Mentorado = mentorado.Nome,
                                RespostaId = resposta.idResposta,
                                Mentor = mentor.Nome,
                                Questao = questao.Descricao,
                                Resposta = pergunta.Descricao,
                                Nota = pergunta.Valor,
                                Comentarios = resposta.Comentario,
                                MentoradoId = mentorado.IdAssociado,
                                MentorId = mentor.IdAssociado,
                                NotaValue = (int?)pergunta.Valor
                            };

            var groupedRespostas = respostas
                .GroupBy(r => new { r.MentoradoId, r.MentorId })
                .Select(g => new
                {
                    g.Key.MentoradoId,
                    g.Key.MentorId,
                    MediaNota = g.Average(r => r.NotaValue)
                });

            var queryResultado = from r in respostas
                                 join g in groupedRespostas on new { r.MentoradoId, r.MentorId } equals new { g.MentoradoId, g.MentorId }
                                 orderby r.Mentorado, r.Mentor, r.Questao
                                 select new AvaliacaoMentoradoModel
                                 {
                                     NomeMentorado = r.Mentorado,
                                     StatusAvaliacao = r.RespostaId > 0 ? "Sim" : "Não",
                                     NomeMentor = r.Mentor,
                                     DescricaoQuestao = r.Questao,
                                     DescricaoResposta = r.Resposta,
                                     ResultadoNota = r.Nota,
                                     Comentarios = r.Comentarios,
                                     ResultadoGeral = g.MediaNota
                                 };
            var resultado = queryResultado.ToList();

            string fileName = "ResultadosAvMentorado_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, resultado);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
        private void CalcularNotaPerfomance_Otimizado(List<AVALIACOESPERFORMANCES> listAvPerformance, int idassociado, int idprojeto, int idperiodo)
        {
            DataModel _context = new DataModel();

            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            var listperfomances = (from a in listAvPerformance
                                   join n in _context.AVALIACOESPERFORMANCESNOTAS
                                   on a.IdNotaNivel1Feedback equals n.IdNota into notas
                                   from anotas in notas.DefaultIfEmpty()
                                   where a.IdEmpresa == idEmpresa
                                   && a.IdAssociado == idassociado
                                    && a.IdProjeto == idprojeto
                                    && a.IdPeriodo == idperiodo
                                   select new
                                   {
                                       PERFOMANCE = a,
                                       NOTAS = anotas
                                   }).ToList();

            if (listperfomances != null)
            {
                foreach (var item in listperfomances)
                {
                    if (item.PERFOMANCE.IdNotaComite.HasValue)
                    {
                        decimal novanota = NotaPerformanceComite(item.PERFOMANCE.IdNotaComite.Value);

                        if (novanota > -1)
                        {
                            item.PERFOMANCE.NotaFinal = novanota;
                        }
                    }
                    else
                    {
                        item.PERFOMANCE.NotaFinal = item.NOTAS != null ? item.NOTAS.Peso : 0;
                    }

                    _context.SaveChanges();
                }
            }
        }

        private decimal NotaPerformanceComite(int idnota)
        {
            var nota = _context.AVALIACOESPERFORMANCESNOTAS.FirstOrDefault(x => x.IdNota == idnota);

            if (nota != null)
            {
                return nota.Peso.Value;
            }
            else
            {
                return -1;
            }

        }

        public void CalculaNotaCompetenciaFinalN1N2(int idassociado, int idprojeto, int idperiodo)
        {
            DataModel _context = new DataModel();
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            var avaliacoesService = new AvaliacoesService();

            var listcompetencias = (from a in _context.AVALIACOESCOMPETENCIAS
                                    join c in _context.COMPETENCIAS
                                    on a.IdCompetencia equals c.IdCompetencia
                                    where a.IdAssociado == idassociado
                                    && a.IdProjeto == idprojeto
                                    && a.IdPeriodo == idperiodo
                                    && a.IdEmpresa == idEmpresa
                                    && a.TipoAvaliacao == "desempenho"
                                    select new
                                    {
                                        a,
                                        c
                                    }).ToList();


            if (listcompetencias != null)
            {
                List<AVALIACOESCOMPETENCIASNOTAS> notasCompetencias = new AvaliacoesService().ObterAvaliacaoCompetenciasNotas(-1);
                int idNaoSeAplica = notasCompetencias.FirstOrDefault(x => x.CodigoNota == "Não se Aplica").IdNota;

                foreach (var item in listcompetencias)
                {
                    var resultCompetencias = listcompetencias.Where(x => x.c.IdEixo == item.c.IdEixo);
                    var hasResultLideranca = resultCompetencias.Where(x => x.c.IdModo == 2).Count() > 0;

                    //Nota Considerada N1
                    decimal? notaConsideradaN1 = 0;
                    decimal? notaConsideradaN2 = 0;
                    decimal? pesoNotaGestor = 0m;
                    var getResultado = avaliacoesService.ObterResultadoLiderComoCompetencia(item.a.IdAssociado, item.a.IdPeriodo);

                    var qtdCompetenciasAvaliadas = 0;
                    var qtdCompetenciasNaoSeAplica = 0;
                    if (item.a.IdNotaNivel1Comite.HasValue)
                    {
                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x =>
                            (x.a.IdNotaNivel1Comite.HasValue && x.a.IdNotaNivel1Comite.Value == idNaoSeAplica && x.c.IdModo != 2));

                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel1Comite.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }
                    }
                    else if (item.a.IdNotaNivel1Feedback.HasValue)
                    {
                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x =>
                            (x.a.IdNotaNivel1Feedback.HasValue && x.a.IdNotaNivel1Feedback.Value == idNaoSeAplica && x.c.IdModo != 2));

                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel1Feedback.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }
                    }
                    if (getResultado.mediaTotal == null && hasResultLideranca)
                    {
                        qtdCompetenciasNaoSeAplica += 1;
                    }
                    if (pesoNotaGestor > 0)
                    {
                        notaConsideradaN1 = ((pesoNotaGestor / 100) / (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplica));
                    }

                    //Nota Considerada N2

                    if (item.a.IdNotaNivel2Comite.HasValue)
                    {
                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x =>
                            (x.a.IdNotaNivel2Comite.HasValue && x.a.IdNotaNivel2Comite.Value == idNaoSeAplica && x.c.IdModo != 2));

                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel2Comite.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }
                    }
                    else if (item.a.IdNotaNivel2Feedback.HasValue)
                    {
                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x =>
                            (x.a.IdNotaNivel2Feedback.HasValue && x.a.IdNotaNivel2Feedback.Value == idNaoSeAplica && x.c.IdModo != 2));

                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel2Feedback.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }
                    }
                    if (getResultado.mediaTotal == null && hasResultLideranca)
                    {
                        qtdCompetenciasNaoSeAplica += 1;
                    }
                    if (pesoNotaGestor > 0)
                    {
                        notaConsideradaN2 = ((pesoNotaGestor / 100) / (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplica));
                    }

                    item.a.NotaFinalNivel1 = notaConsideradaN1;
                    item.a.NotaFinalNivel2 = notaConsideradaN2;
                    _context.SaveChanges();
                }
            }
        }

        public void CalculaNotaCompetenciaFinalN1N2_Otimizado(List<AVALIACAO> listAvaliacoes, List<AVALIACOESCOMPETENCIAS> listAvCompetencias, List<COMPETENCIAS> listCompetencias,
            int idassociado, int idprojeto, int idperiodo)
        {
            DataModel _context = new DataModel();
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            var avaliacoesService = new AvaliacoesService();

            var listcompetencias = (from a in listAvCompetencias
                                    join c in listCompetencias
                                    on a.IdCompetencia equals c.IdCompetencia
                                    where a.IdAssociado == idassociado
                                    && a.IdProjeto == idprojeto
                                    && a.IdPeriodo == idperiodo
                                    && a.IdEmpresa == idEmpresa
                                    && a.TipoAvaliacao == "desempenho"
                                    select new
                                    {
                                        a,
                                        c
                                    }).ToList();


            if (listcompetencias != null)
            {
                List<AVALIACOESCOMPETENCIASNOTAS> notasCompetencias = new AvaliacoesService().ObterAvaliacaoCompetenciasNotas(-1);
                int idNaoSeAplica = notasCompetencias.FirstOrDefault(x => x.CodigoNota == "Não se Aplica").IdNota;

                foreach (var item in listcompetencias)
                {
                    var resultCompetencias = listcompetencias.Where(x => x.c.IdEixo == item.c.IdEixo);
                    var hasResultLideranca = resultCompetencias.Where(x => x.c.IdModo == 2).Count() > 0;

                    //Nota Considerada N1
                    decimal? notaConsideradaN1 = 0;
                    decimal? notaConsideradaN2 = 0;
                    decimal? pesoNotaGestor = 0m;
                    var getResultado = avaliacoesService.ObterResultadoLiderComoCompetencia_Otimizado(listAvaliacoes, listAvCompetencias,
                        item.a.IdAssociado, item.a.IdPeriodo);

                    var qtdCompetenciasAvaliadas = 0;
                    var qtdCompetenciasNaoSeAplica = 0;
                    if (item.a.IdNotaNivel1Comite.HasValue)
                    {
                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel1Comite.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }

                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x => x.a.IdNotaNivel1Comite.HasValue && x.a.IdNotaNivel1Comite.Value == idNaoSeAplica && x.c.IdModo != 2);
                    }
                    else if (item.a.IdNotaNivel1Feedback.HasValue)
                    {
                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel1Feedback.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }

                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x => x.a.IdNotaNivel1Feedback.HasValue && x.a.IdNotaNivel1Feedback.Value == idNaoSeAplica && x.c.IdModo != 2);
                    }
                    if (getResultado.mediaTotal == null && hasResultLideranca)
                    {
                        qtdCompetenciasNaoSeAplica += 1;
                    }
                    if (pesoNotaGestor > 0)
                    {
                        notaConsideradaN1 = ((pesoNotaGestor / 100) / (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplica));
                    }

                    //Nota Considerada N2
                    if (item.a.IdNotaNivel2Comite.HasValue)
                    {
                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel2Comite.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }

                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x => x.a.IdNotaNivel2Comite.HasValue && x.a.IdNotaNivel2Comite.Value == idNaoSeAplica && x.c.IdModo != 2);
                    }
                    else if (item.a.IdNotaNivel2Feedback.HasValue)
                    {
                        if (item.c.IdModo == 1)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.a.IdNotaNivel2Feedback.Value).Peso;
                        }
                        else if (item.c.IdModo == 2)
                        {
                            pesoNotaGestor = getResultado.mediaTotal;
                        }

                        qtdCompetenciasAvaliadas = resultCompetencias.Count();
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x => x.a.IdNotaNivel2Feedback.HasValue && x.a.IdNotaNivel2Feedback.Value == idNaoSeAplica && x.c.IdModo != 2);
                    }
                    if (getResultado.mediaTotal == null && hasResultLideranca)
                    {
                        qtdCompetenciasNaoSeAplica += 1;
                    }
                    if (pesoNotaGestor > 0)
                    {
                        notaConsideradaN2 = ((pesoNotaGestor / 100) / (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplica));
                    }

                    item.a.NotaFinalNivel1 = notaConsideradaN1;
                    item.a.NotaFinalNivel2 = notaConsideradaN2;
                    _context.SaveChanges();
                }
            }
        }

        public int CalculoDiasProjeto(int idperiodo, DateTime dataInicioAlocado, DateTime dataFimAlocado)
        {
            PERIODOSAVALIACOES periodoAvaliacao = _context.PERIODOSAVALIACOES.FirstOrDefault(x => x.IdPeriodo == idperiodo);
            DateTime dataInicioCalc = dataInicioAlocado;
            DateTime dataFimCalc = dataFimAlocado;

            if (dataInicioAlocado < periodoAvaliacao.DataInicio)
            {
                dataInicioCalc = periodoAvaliacao.DataInicio.Value;
            }

            if (dataFimAlocado > periodoAvaliacao.DataFim.Value)
            {
                dataFimCalc = periodoAvaliacao.DataFim.Value;
            }

            if (dataInicioAlocado >= dataFimAlocado)
            {
                dataInicioCalc = periodoAvaliacao.DataInicio.Value;
                dataFimCalc = periodoAvaliacao.DataFim.Value;
            }

            return dataFimCalc.Subtract(dataInicioCalc).Days;
        }

        public List<ResultadoCompetenciaModel> SimulaNotaCompetenciaFinalN1N2(int idassociado, int idprojeto, int idperiodo)
        {
            DataModel _context = new DataModel();
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            var listcompetencias = (from a in _context.AVALIACOESCOMPETENCIAS
                                    join c in _context.COMPETENCIAS
                                    on a.IdCompetencia equals c.IdCompetencia
                                    join e in _context.EIXOS
                                    on c.IdEixo equals e.IdEixo
                                    where a.IdAssociado == idassociado
                                    && a.IdPeriodo == idperiodo
                                    && a.IdEmpresa == idEmpresa
                                    && a.COMPETENCIAS.TipoAvaliacao == "desempenho"
                                    select new
                                    {
                                        a.IdNotaNivel1AutoAvaliacao,
                                        a.IdNotaNivel2AutoAvaliacao,
                                        a.IdNotaNivel1Feedback,
                                        a.IdNotaNivel1AvaliacaoGestor,
                                        a.IdNotaNivel2Feedback,
                                        a.IdNotaNivel2AvaliacaoGestor,
                                        c.IdEixo,
                                        a.IdProjeto,
                                        e.Eixo
                                    }).ToList();

            if (idprojeto >= 0)
            {
                listcompetencias = listcompetencias.Where(a => a.IdProjeto == idprojeto).ToList();
            }

            List<ResultadoCompetenciaModel> listresultadocompetencias = new List<ResultadoCompetenciaModel>();

            if (listcompetencias != null)
            {
                List<AVALIACOESCOMPETENCIASNOTAS> notasCompetencias = new AvaliacoesService().ObterAvaliacaoCompetenciasNotas(-1);
                int idNaoSeAplica = notasCompetencias.FirstOrDefault(x => x.CodigoNota == "Não se Aplica").IdNota;

                var listCompetenciasValidas = listcompetencias.Where(x => x.IdNotaNivel1Feedback.HasValue && x.IdNotaNivel1AvaliacaoGestor.HasValue).ToList();

                foreach (var item in listCompetenciasValidas)
                {
                    var resultCompetencias = listcompetencias.Where(x => x.IdEixo == item.IdEixo && x.IdNotaNivel1Feedback.HasValue && x.IdNotaNivel1AvaliacaoGestor.HasValue);

                    //Nota Considerada N1 Feedback
                    decimal? notaConsideradaN1 = 0;
                    decimal? pesoNotaGestor = 0;
                    var qtdCompetenciasAvaliadas = resultCompetencias.Count();
                    int qtdCompetenciasNaoSeAplica = 0;

                    if (item.IdNotaNivel1Feedback.HasValue)
                    {
                        pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.IdNotaNivel1Feedback.Value).Peso;
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x => x.IdNotaNivel1Feedback.HasValue && x.IdNotaNivel1Feedback.Value == idNaoSeAplica);
                    }
                    else
                    {
                        if (item.IdNotaNivel1AvaliacaoGestor.HasValue)
                        {
                            pesoNotaGestor = notasCompetencias.FirstOrDefault(a => a.IdNota == item.IdNotaNivel1AvaliacaoGestor.Value).Peso;
                        }
                        qtdCompetenciasNaoSeAplica = resultCompetencias.Count(x => x.IdNotaNivel1AvaliacaoGestor.HasValue && x.IdNotaNivel1AvaliacaoGestor.Value == idNaoSeAplica);
                    }

                    if (pesoNotaGestor > 0)
                    {
                        notaConsideradaN1 = ((pesoNotaGestor / 100) / (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplica));
                    }

                    //N1 AVALIADO
                    decimal? notaConsideradaAvaliadoN1 = 0;
                    decimal? pesoNotaAvaliado = 0;
                    int qtdCompetenciasNaoSeAplicaAvaliado = 0;

                    if (item.IdNotaNivel1AutoAvaliacao > 0)
                    {
                        pesoNotaAvaliado = notasCompetencias.FirstOrDefault(a => a.IdNota == item.IdNotaNivel1AutoAvaliacao).Peso;
                        qtdCompetenciasNaoSeAplicaAvaliado = resultCompetencias.Count(x => x.IdNotaNivel1AutoAvaliacao == idNaoSeAplica);

                        var calcpesonotaavaliado = (pesoNotaAvaliado / 100);

                        if (calcpesonotaavaliado > 0)
                        {
                            notaConsideradaAvaliadoN1 = ((pesoNotaAvaliado / 100) / (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplicaAvaliado));
                        }
                        else
                        {
                            notaConsideradaAvaliadoN1 = 0;
                        }
                    }

                    //Nota Considerada N2 Feedback
                    decimal? notaConsideradaN2 = 0;
                    decimal? pesoNotaGestorN2 = 0;
                    int qtdCompetenciasNaoSeAplicaN2 = 0;

                    if (item.IdNotaNivel2Feedback.HasValue)
                    {
                        pesoNotaGestorN2 = notasCompetencias.FirstOrDefault(a => a.IdNota == item.IdNotaNivel2Feedback.Value).Peso;
                        qtdCompetenciasNaoSeAplicaN2 = resultCompetencias.Count(x => x.IdNotaNivel2Feedback.HasValue && x.IdNotaNivel2Feedback.Value == idNaoSeAplica);
                    }
                    else
                    {
                        if (item.IdNotaNivel2AvaliacaoGestor.HasValue)
                        {
                            pesoNotaGestorN2 = notasCompetencias.FirstOrDefault(a => a.IdNota == item.IdNotaNivel2AvaliacaoGestor.Value).Peso;
                        }
                        qtdCompetenciasNaoSeAplicaN2 = resultCompetencias.Count(x => x.IdNotaNivel2AvaliacaoGestor.HasValue && x.IdNotaNivel2AvaliacaoGestor.Value == idNaoSeAplica);
                    }

                    if (pesoNotaGestorN2 > 0)
                    {
                        notaConsideradaN2 = ((pesoNotaGestorN2 / 100) / (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplicaN2));
                    }

                    //N2 AVALIADO
                    decimal? notaConsideradaAvaliadoN2 = 0;
                    decimal? pesoNotaAvaliadoN2 = 0;
                    int qtdCompetenciasNaoSeAplicaAvaliadoN2 = 0;

                    if (item.IdNotaNivel2AutoAvaliacao > 0)
                    {
                        pesoNotaAvaliadoN2 = notasCompetencias.FirstOrDefault(a => a.IdNota == item.IdNotaNivel2AutoAvaliacao).Peso;
                        qtdCompetenciasNaoSeAplicaAvaliadoN2 = resultCompetencias.Count(x => x.IdNotaNivel2AutoAvaliacao == idNaoSeAplica);

                        var calcavalnaoseaplican2 = (qtdCompetenciasAvaliadas - qtdCompetenciasNaoSeAplicaAvaliadoN2);

                        if (calcavalnaoseaplican2 > 0)
                        {
                            notaConsideradaAvaliadoN2 = ((pesoNotaAvaliadoN2 / 100) / calcavalnaoseaplican2);
                        }
                        else
                        {
                            notaConsideradaAvaliadoN2 = 0;
                        }
                    }

                    ResultadoCompetenciaModel model = new ResultadoCompetenciaModel
                    {
                        Eixo = item.Eixo,
                        IdEixo = item.IdEixo,
                        NotaFinalNivel1 = notaConsideradaN1,
                        NotaFinalNivel2 = notaConsideradaN2,
                        NotaFinalNivel1Avaliado = notaConsideradaAvaliadoN1,
                        NotaFinalNivel2Avaliado = notaConsideradaAvaliadoN2
                    };
                    listresultadocompetencias.Add(model);

                }
            }

            return listresultadocompetencias;

        }

        public List<ResultadoProjetosModel> EvolucaoAssociado(int idassociado, int idempresa)
        {
            try
            {
                var idConcluido = _context.AVALIACOESSTATUS.FirstOrDefault(x => x.Status == "Concluído").IdStatus;

                var listProjetosAssociado = (from a in _context.AVALIACAO
                                             join pa in _context.PROJETOSASSOCIADOS
                                              on new
                                              {
                                                  IdAssociado = a.idAssociado,
                                                  IdProjeto = a.idProjeto,
                                                  IdGestor = a.idGestor
                                              }
                                              equals new
                                              {
                                                  IdAssociado = pa.IdAssociado,
                                                  IdProjeto = pa.IdProjeto,
                                                  IdGestor = pa.IdGestor
                                              }
                                             join p in _context.PROJETOS
                                             on a.idProjeto equals p.IdProjeto
                                             join g in _context.ASSOCIADOS
                                             on pa.IdGestor equals g.IdAssociado
                                             join c in _context.PROJETOSCOMPLEXIDADES
                                             on p.IdComplexidade equals c.IdComplexidade
                                             where a.idEmpresa == idempresa
                                             && a.idAssociado == idassociado
                                             && p.ATV == 1
                                             && a.Liberado == true
                                             && a.idStatus == idConcluido
                                             && a.idGestor == pa.IdGestor
                                             orderby a.idPeriodo descending
                                             select new ResultadoProjetosModel()
                                             {
                                                 IdProjeto = a.idProjeto,
                                                 IdAssociado = a.idAssociado,
                                                 NomeGestor = g.Nome,
                                                 ProjetoComplexidade = c.Complexidade,
                                                 PesoProjetoComplexidade = c.Peso,
                                                 ProjetoNome = p.Projeto,
                                                 DataInicioAlocado = pa.DataInicio,
                                                 DataFimAlocado = pa.DataFim,
                                                 IdPeriodo = a.idPeriodo,
                                                 IdGestor = (int)a.idGestor,
                                                 IdAvaliacao = a.idAvaliacao
                                             }).Take(4).ToList();


                if (listProjetosAssociado != null)
                {
                    IEnumerable<EIXOS> listEixos = _context.EIXOS.Where(x => x.ATV.HasValue && x.ATV.Value == 1);
                    var listPremissasRdar = _context.PREMISSAS_RADAR;
                    var listPeriodos = _context.PERIODOSAVALIACOES;

                    foreach (var projeto in listProjetosAssociado)
                    {
                        IEnumerable<ResultadoCompetenciaModel> competencias = ObterListAvaliacoesCompetenciasProjeto(idassociado, projeto.IdProjeto, projeto.IdPeriodo, projeto.IdAvaliacao);
                        projeto.ListSomaCompetenciasN1N2 = new List<ResultadoCompetenciaModel>();

                        projeto.Periodo = listPeriodos.FirstOrDefault(x => x.IdPeriodo == projeto.IdPeriodo).Periodo;

                        int idCargo = competencias.FirstOrDefault().IdCargo;

                        var premissasradar = listPremissasRdar.FirstOrDefault(x => x.IdEmpresa == idempresa && x.IdCargo == idCargo);
                        var notaRadarCargo = 0;

                        if (premissasradar != null)
                        {
                            notaRadarCargo = premissasradar.ValorBaseAvaliacaoGestor;
                        }

                        decimal? somaPercentualNivel1 = 0;
                        decimal? somaPercentualNivel2 = 0;

                        foreach (var eixo in listEixos)
                        {
                            var listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);

                            if (listCompetenciasProjetoEixo != null)
                            {
                                if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel1.HasValue) > 0)
                                {
                                    CalculaNotaCompetenciaFinalN1N2(idassociado, projeto.IdProjeto, projeto.IdPeriodo);
                                    competencias = ObterListAvaliacoesCompetenciasProjeto(idassociado, projeto.IdProjeto, projeto.IdPeriodo, projeto.IdAvaliacao);
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
                                    somaPercentualNivel1 += percentualNivel1;
                                }

                                if (listCompetenciasProjetoEixo.Count(x => !x.NotaFinalNivel2.HasValue) > 0)
                                {
                                    CalculaNotaCompetenciaFinalN1N2(idassociado, projeto.IdProjeto, projeto.IdPeriodo);
                                    competencias = ObterListAvaliacoesCompetenciasProjeto(idassociado, projeto.IdProjeto, projeto.IdPeriodo, projeto.IdAvaliacao);
                                    listCompetenciasProjetoEixo = competencias.Where(x => x.IdEixo == eixo.IdEixo);
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
                                    somaPercentualNivel2 += percentualNivel2;
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

                            projeto.NotaCompetencialNivel1 = somaPercentualNivel1 / listEixos.Count();
                            projeto.NotaCompetencialNivel2 = somaPercentualNivel2 / listEixos.Count();
                            projeto.SomaNotaCompetencialN1N2 = projeto.NotaCompetencialNivel1 + projeto.NotaCompetencialNivel2;

                        }

                        projeto.SomaNotaCompetenciaRadar = projeto.ListSomaCompetenciasN1N2.Average(x => x.NotaCompetenciaRadar);

                        //var existCalculoPerfomance = _context.AVALIACOESPERFORMANCES.Where(x => x.IdEmpresa == idempresa && x.IdAssociado == idassociado && x.IdProjeto == projeto.IdProjeto && x.IdPeriodo == projeto.IdPeriodo).Count(x => !x.NotaFinal.HasValue);

                        //if (existCalculoPerfomance > 0)
                        //{
                        CalcularNotaPerfomance(idassociado, projeto.IdProjeto, projeto.IdPeriodo);
                        //}

                        projeto.ListPerfomance = (from a in _context.AVALIACOESPERFORMANCES
                                                  join p in _context.PERFORMANCES
                                                  on a.IdPerformance equals p.IdPerformance
                                                  where a.IdEmpresa == idempresa
                                                  && a.IdAssociado == idassociado
                                                  && a.IdPeriodo == projeto.IdPeriodo
                                                  && a.IdProjeto == projeto.IdProjeto
                                                  select new ResultadoPerfomanceModel()
                                                  {
                                                      Perfomance = p.Performance,
                                                      NotaPerfomance = a.NotaFinal
                                                  }).ToList();

                        projeto.SomaPerfomance = 0;

                        if (projeto.ListPerfomance != null)
                        {
                            projeto.SomaPerfomance = (projeto.ListPerfomance.Sum(x => x.NotaPerfomance) / projeto.ListPerfomance.Count());
                        }

                    }

                }


                var projetosperiodos = listProjetosAssociado.GroupBy(x => x.IdPeriodo).Where(g => g.Count() > 1).Select(x => x.Key).ToList();

                foreach (var item in projetosperiodos)
                {
                    var projetoMaiorCompetencia = listProjetosAssociado.Where(x => x.IdPeriodo == item).Max(x => x.SomaNotaCompetencialN1N2);

                    var listProjetosPeriodos = listProjetosAssociado.Where(x => x.IdPeriodo == item && x.SomaNotaCompetencialN1N2.Value != projetoMaiorCompetencia).ToList();

                    foreach (var remove in listProjetosPeriodos)
                    {
                        listProjetosAssociado.Remove(remove);
                    }

                }


                return listProjetosAssociado;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Ops!! Erro ao tentar efetuar os cálculos da evolução. " + ex.Message);
            }
        }

        // RESULTADOS LIDERANÇA
        public List<RLM_Eixo> ObterResultadoLider(int idassociado, List<int> listPeriodos, string TipoAvaliacao, string Escopo, List<int> listProjetos, List<int> listLiderados)
        {
            try
            {
                string avalFim = new AvaliacoesService().etapaAvaliacaoFinalizada;
                var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
                List<RLM_Eixo> rlmEixos = new List<RLM_Eixo>();

                var periodos = _context.PERIODOSAVALIACOES.Where(x => listPeriodos.Contains(x.IdPeriodo)).ToList();
                if (listPeriodos == null || listPeriodos.Count <= 0)
                {
                    periodos = new PeriodoService().ListaTodosPeriodos(idEmpresa);
                }
                List<int> idsPeriodos = periodos.Select(p => p.IdPeriodo).ToList();

                List<ResultadoLiderancaModel> listProjetosAssociado = new List<ResultadoLiderancaModel>();
                foreach (var periodo in periodos)
                {
                    var projetosassociados = _context.PROJETOSASSOCIADOS.Where(x =>
                                                                            x.IdAssociado == idassociado
                                                                            && x.ATV == 1
                                                                            && x.DataInicio >= periodo.DataInicio
                                                                            && x.DataFim <= periodo.DataFim
                                                                            && x.TipoAvaliacao == TipoAvaliacao
                                                                            && x.Escopo == Escopo
                                                                            && listProjetos.Count >= 1 ? listProjetos.Contains(x.IdProjeto) : 1 == 1
                                                                            && listLiderados.Count >= 1 ? listLiderados.Contains((int)x.IdGestor) : 1 == 1);


                    listProjetosAssociado.AddRange((from a in _context.AVALIACAO
                                                    join pa in projetosassociados
                                                     on new
                                                     {
                                                         IdAssociado = a.idAssociado,
                                                         IdProjeto = a.idProjeto,
                                                         IdGestor = a.idGestor
                                                     }
                                                     equals new
                                                     {
                                                         IdAssociado = pa.IdAssociado,
                                                         IdProjeto = pa.IdProjeto,
                                                         IdGestor = pa.IdGestor
                                                     }
                                                    join p in _context.PROJETOS
                                                    on a.idProjeto equals p.IdProjeto
                                                    join g in _context.ASSOCIADOS
                                                    on pa.IdGestor equals g.IdAssociado
                                                    where a.idEmpresa == idEmpresa
                                                    && a.idAssociado == idassociado
                                                    && a.idPeriodo == periodo.IdPeriodo
                                                    && p.ATV == 1
                                                    && a.idGestor == pa.IdGestor
                                                    && a.PosicaoAtualFluxoAvaliacao == avalFim
                                                    select new ResultadoLiderancaModel()
                                                    {
                                                        IdProjeto = a.idProjeto,
                                                        IdAssociado = a.idAssociado,
                                                        NomeGestor = g.Nome,
                                                        ProjetoNome = p.Projeto,
                                                        DataInicioAlocado = pa.DataInicio,
                                                        DataFimAlocado = pa.DataFim,
                                                        IdPeriodo = a.idPeriodo,
                                                        IdGestor = (int)a.idGestor,
                                                        IdAvaliacao = a.idAvaliacao
                                                    }).ToList());
                }

                if (listProjetosAssociado != null)
                {
                    // SELETOR DE COMPETENCIAS, SUBS, EIXOS E VALORES RESPONDIDOS
                    var status = new StatusService().ObterStatusAvaliacao(3);
                    var listCompetencias = new List<COMPETENCIAS>();
                    var listEixos = new List<EIXOS>();
                    var listSubCompetencias = new List<SUBCOMPETENCIAS>();
                    var listValores = new List<AVALIACOESCOMPETENCIAS>();
                    foreach (var projeto in listProjetosAssociado)
                    {
                        var listCompetenciasRespostas = new AvaliacoesService().ObterAvaliacoesCompetencias(projeto.IdAssociado, projeto.IdProjeto, projeto.IdPeriodo, "AFI",
                            "lideranca", "projeto", projeto.IdAvaliacao);
                        listValores.AddRange(listCompetenciasRespostas);
                        foreach (var CompetenciaResposta in listCompetenciasRespostas)
                        {
                            var getCompetencia = new CompetenciasService().ObterCompetencia(CompetenciaResposta.IdCompetencia);
                            if (listCompetencias.Find(cpt => cpt.IdCompetencia == getCompetencia.IdCompetencia) == null)
                            {
                                listCompetencias.Add(getCompetencia);
                                var getSubCompetencia = new SubCompetenciasService().ObterCompetencia(getCompetencia.IdSubCompetencia);
                                if (listSubCompetencias.Find(sub => sub.IdSubCompetencia == getSubCompetencia.IdSubCompetencia) == null)
                                {
                                    listSubCompetencias.Add(getSubCompetencia);
                                }
                                var getEixo = new EixoService().ObterEixo(getCompetencia.IdEixo);
                                if (listEixos.Find(eix => eix.IdEixo == getEixo.IdEixo) == null)
                                {
                                    listEixos.Add(getEixo);
                                }
                            }
                        }
                    }

                    // CALCULADOR DO GRAFICO DE EIXOS, TABELAS DE CONTADORES DE RESPOSTAS POR EIXO, GRÁFICO DE PILARES E TABELA DE DETALHES DE RESPOSTAS
                    var serviceNotas = new AvaliacoesService();
                    foreach (var eixo in listEixos)
                    {
                        // DECLARADOR DO EIXO
                        rlmEixos.Add(new RLM_Eixo());
                        int ultimoLista = rlmEixos.Count - 1;
                        rlmEixos[ultimoLista].idEixo = eixo.IdEixo;
                        rlmEixos[ultimoLista].eixo = eixo.Eixo;
                        rlmEixos[ultimoLista].competencias = new List<RLM_Competencia>();

                        // OBTÉM NOTAS RESPONDIDAS E CONTADOR DAS AVALIAÇÕES PERTINENTES AO EIXO
                        var getCompetencias = listCompetencias.FindAll(cpt => cpt.IdEixo == eixo.IdEixo);
                        var getValores = listValores.FindAll(val => getCompetencias.Select(cpt => cpt.IdCompetencia).ToList().Contains(val.IdCompetencia));

                        rlmEixos[ultimoLista].notas_2 = getValores.FindAll(val => val.IdNotaNivel1AvaliacaoGestor == 2).Count();
                        rlmEixos[ultimoLista].notas_3 = getValores.FindAll(val => val.IdNotaNivel1AvaliacaoGestor == 3).Count();
                        rlmEixos[ultimoLista].notas_4 = getValores.FindAll(val => val.IdNotaNivel1AvaliacaoGestor == 4).Count();

                        // OBTEM NOTA MÁXIMA E DECLARA SOMA DAS NOTAS E DETALHES DAS RESPOSTAS
                        float notaMaxima = getValores.Count * (float)serviceNotas.ObterAvaliacaoCompetenciaNota(2).Peso;
                        float somaNotas = 0;
                        List<RLM_Detalhes> rlmDetalhes = new List<RLM_Detalhes>();
                        List<RLM_Liderado> rlmLiderados = new List<RLM_Liderado>();
                        List<RLM_Competencia> rlmCompetencias = new List<RLM_Competencia>();

                        // OBTEM SOMA DAS NOTAS E RESPOSTAS DETALHADAS
                        foreach (var valor in getValores)
                        {
                            // SOMA DAS NOTAS
                            somaNotas += (float)serviceNotas.ObterAvaliacaoCompetenciaNota((int)valor.IdNotaNivel1AvaliacaoGestor).Peso;

                            // DETALHES
                            // GET SET LIDERADOS
                            var avaliacao = new AvaliacoesService().ObterAvaliacao(valor.idAvaliacao);
                            var liderado = new AssociadosService().ObterAssociado((int)avaliacao.idGestor);
                            if (rlmLiderados.Find(rlm => rlm.idLiderado == liderado.IdAssociado) == null)
                            {
                                RLM_Liderado addLiderado = new RLM_Liderado();
                                addLiderado.idLiderado = liderado.IdAssociado;
                                addLiderado.liderado = liderado.Nome;
                                addLiderado.projetos = new List<RLM_Projeto>();
                                rlmLiderados.Add(addLiderado);
                            }
                            int indexLiderado = rlmLiderados.FindIndex(rlm => rlm.idLiderado == liderado.IdAssociado);

                            // GET SET PROJETOS
                            var projeto = new ProjetosService().ObterProjeto(valor.IdProjeto);
                            if (rlmLiderados[indexLiderado].projetos.Find(rlm => rlm.idProjeto == projeto.IdProjeto) == null)
                            {
                                RLM_Projeto addProjeto = new RLM_Projeto();
                                addProjeto.idProjeto = projeto.IdProjeto;
                                addProjeto.projeto = projeto.Projeto;
                                addProjeto.respostas = new List<RLM_Detalhes>();
                                rlmLiderados[indexLiderado].projetos.Add(addProjeto);
                            }
                            int indexProjeto = rlmLiderados[indexLiderado].projetos.FindIndex(rlm => rlm.idProjeto == projeto.IdProjeto);
                            rlmLiderados[indexLiderado].numProjetos = rlmLiderados[indexLiderado].projetos.Count();
                            rlmLiderados[indexLiderado].numProjetosTabela = rlmLiderados[indexLiderado].projetos.Count() * 2;

                            // GET SET COMPETENCIAS
                            var competencia = new CompetenciasService().ObterCompetencia(valor.IdCompetencia);
                            if (rlmEixos[ultimoLista].competencias.Find(rlm => rlm.idCompetencia == competencia.IdCompetencia) == null)
                            {
                                RLM_Competencia addCompetencia = new RLM_Competencia();
                                addCompetencia.idCompetencia = competencia.IdCompetencia;
                                addCompetencia.competencia = competencia.CompetenciaJRDetalhe;
                                addCompetencia.eixo = eixo.Eixo;
                                addCompetencia.respostas = new List<RLM_Detalhes>();
                                rlmEixos[ultimoLista].competencias.Add(addCompetencia);
                            }
                            int indexCompetencia = rlmEixos[ultimoLista].competencias.FindIndex(rlm => rlm.idCompetencia == competencia.IdCompetencia);

                            // GET SET RESPOSTAS
                            var respDetalhes = new RLM_Detalhes();
                            respDetalhes.idResposta = valor.IdAvaliacaoCompetencia;
                            respDetalhes.eixo = eixo.Eixo;
                            respDetalhes.idSubCompetencia = listCompetencias.Find(com => com.IdCompetencia == valor.IdCompetencia).IdSubCompetencia;
                            respDetalhes.subCompetencia = listSubCompetencias.Find(sub => sub.IdSubCompetencia == listCompetencias.Find(com => com.IdCompetencia == valor.IdCompetencia).IdSubCompetencia).SubCompetencia;
                            respDetalhes.competencia = competencia.CompetenciaJRDetalhe;
                            respDetalhes.idLiderado = liderado.IdAssociado;
                            respDetalhes.liderado = liderado.Nome;
                            respDetalhes.idProjeto = projeto.IdProjeto;
                            respDetalhes.projeto = projeto.Projeto;
                            respDetalhes.idNota = (int)valor.IdNotaNivel1AvaliacaoGestor;
                            respDetalhes.nota = serviceNotas.ObterAvaliacaoCompetenciaNota((int)valor.IdNotaNivel1AvaliacaoGestor).CodigoNotaAvaliador;
                            respDetalhes.comentario = valor.ComentariosAvaliacaoGestor;
                            rlmDetalhes.Add(respDetalhes);

                            rlmLiderados[indexLiderado].projetos[indexProjeto].respostas.Add(respDetalhes);

                            rlmEixos[ultimoLista].competencias[indexCompetencia].respostas.Add(respDetalhes);
                        }
                        float resultado = somaNotas / notaMaxima;
                        rlmEixos[ultimoLista].detalhes = rlmDetalhes;
                        rlmEixos[ultimoLista].liderados = rlmLiderados;
                        rlmEixos[ultimoLista].resultado = resultado;
                        rlmEixos[ultimoLista].resultado_percent = (resultado * 100).ToString() + "%";

                        // OBTÉM SUBCOMPETÊNCIAS PERTINENTES AO EIXO
                        var getSubs = getCompetencias.Select(sub => sub.IdSubCompetencia).ToList();

                        // CONTADOR E PERCENTIL POR SUBCOMPETENCIAS
                        List<RLM_SubCompetencias> rlmSubCompetencias = new List<RLM_SubCompetencias>();
                        foreach (var sub in getSubs)
                        {
                            if (rlmSubCompetencias.Find(rsc => rsc.idSubCompetencia == sub) == null)
                            {
                                // DECLARA SUBCOMPETENCIA NO DICIONARIO DE SUBS
                                RLM_SubCompetencias subDetalhes = new RLM_SubCompetencias();
                                subDetalhes.idSubCompetencia = sub;
                                subDetalhes.subCompetencia = listSubCompetencias.Find(lsub => lsub.IdSubCompetencia == sub).SubCompetencia;


                                // RETORNA OS VALORES RESPONDIDOS OS QUAIS AS COMPETENCIAS SÃO PERTINENTES À SUBCOMPETENCIA NO EIXO
                                var getComps = getCompetencias.FindAll(comp => comp.IdSubCompetencia == sub).Select(gcomp => gcomp.IdCompetencia);
                                var getVals = listValores.FindAll(val => getComps.Contains(val.IdCompetencia));

                                // CONTABILIZA NO DICIONÁRIO POR INTEIROS E PERCENTUAIS
                                float totalSub = getVals.Count();
                                subDetalhes.notas_2 = getVals.FindAll(val => val.IdNotaNivel1AvaliacaoGestor == 2).Count();
                                subDetalhes.notas_3 = getVals.FindAll(val => val.IdNotaNivel1AvaliacaoGestor == 3).Count();
                                subDetalhes.notas_4 = getVals.FindAll(val => val.IdNotaNivel1AvaliacaoGestor == 4).Count();
                                subDetalhes.notas_2_percent = (float)Math.Round(subDetalhes.notas_2 / totalSub, 2);
                                subDetalhes.notas_3_percent = (float)Math.Round(subDetalhes.notas_3 / totalSub, 2);
                                subDetalhes.notas_4_percent = (float)Math.Round(subDetalhes.notas_4 / totalSub, 2);
                                rlmSubCompetencias.Add(subDetalhes);
                            }
                        }
                        rlmEixos[ultimoLista].subcompetencias = rlmSubCompetencias;
                    }
                }

                return rlmEixos;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao tentar efetuar os cálculos do resultado de liderança! " + ex.Message);
            }
        }

        // EXPORT RESULTADOS DESEMPENHO
        public void ExportResultadoCompetencias(int idPeriodo, HttpServerUtility Server)
        {
            var resultTotal = new List<ResultadoSomaProjetosModel_Export>();
            var resultTotalCompetencias = new List<ResultadoTotalCompetenciaModel_Export>();
            var resultTotalPerformances = new List<ResultadoTotalPerfomanceModel_Export>();
            var resultProjetos = new List<ResultadoProjetosModel_Export>();
            var resultCompetencias = new List<ResultadoCompetenciaModel_Export>();
            var resultPerformances = new List<ResultadoPerfomanceModel_Export>();
            var avaliacoesService = new AvaliacoesService();
            var cargosService = new CargosService();
            var associadosService = new AssociadosService();
            var competenciasService = new CompetenciasService();
            var performancesService = new PerformancesService();

            var getAvaliacoes = avaliacoesService.ObterListaAvaliacao();
            var getCompetencias = competenciasService.ObterListaCompetencias();
            var getPerformances = performancesService.ObterListaPerformances();
            var getAvPerformances = avaliacoesService.ObterAvaliacaoPerformanceTodos(-1, -1, idPeriodo);

            var getAvCompetencias = avaliacoesService.ListaAvCompetencias();
            getAvCompetencias = getAvCompetencias.Where(x => x.ATV == 1).ToList();
            getAvCompetencias = getAvCompetencias.Where(x => x.IdAvaliacaoStatus == 3).ToList();
            getAvCompetencias = getAvCompetencias.Where(x => x.IdPeriodo == idPeriodo).ToList();

            var getAssociados = getAvCompetencias.Select(x => x.IdAssociado).Distinct().ToList();

            foreach (var associado in getAssociados)
            {
                var getAvaliacao = getAvCompetencias.FirstOrDefault(x => x.IdAssociado == associado);
                var getCargo = cargosService.ObterCargo(getAvaliacao.IdCargo);
                var getAssociado = associadosService.ObterAssociado(associado);

                var getResultadoProjetos = ObterResultadoAssociado_Otimizado(getAvaliacoes, getAvCompetencias, getCompetencias, getAvPerformances, getPerformances,
                    associado, idPeriodo, getCargo.IdCargo, "desempenho", "projeto");
                var getSomaProjetos = SomaResultadoProjetos(getResultadoProjetos);

                // TOTAL
                var getExportTotal = ConvertExport_ResultadoSomaProjetosModel(getSomaProjetos, getAssociado.Nome, getCargo.Cargo, getAvaliacao.PERIODOSAVALIACOES.Periodo,
                    getAvaliacao.TipoAvaliacao, getAvaliacao.Escopo);
                resultTotal.AddRange(getExportTotal);

                var getExportCompetenciasTotal = ConvertExport_ResultadoSomaCompetenciasModel(getSomaProjetos, getAssociado.Nome, getCargo.Cargo, getAvaliacao.PERIODOSAVALIACOES.Periodo,
                    getAvaliacao.TipoAvaliacao, getAvaliacao.Escopo);
                resultTotalCompetencias.AddRange(getExportCompetenciasTotal);

                var getExportPerformancesTotal = ConvertExport_ResultadoTotalPerfomanceModel(getSomaProjetos, getAssociado.Nome, getCargo.Cargo, getAvaliacao.PERIODOSAVALIACOES.Periodo,
                    getAvaliacao.TipoAvaliacao, getAvaliacao.Escopo);
                resultTotalPerformances.AddRange(getExportPerformancesTotal);

                // PROJETOS
                var getExportProjetos = ConvertExport_ResultadoProjetosModel(getResultadoProjetos);
                resultProjetos.AddRange(getExportProjetos);

                var getExportCompetenciasProjetos = ConvertExport_ResultadoCompetenciaModel(getResultadoProjetos);
                resultCompetencias.AddRange(getExportCompetenciasProjetos);

                var getExportPerformancesProjetos = ConvertExport_ResultadoPerfomanceModel(getResultadoProjetos);
                resultPerformances.AddRange(getExportPerformancesProjetos);
            }

            string fileName = "ResultadosAvDesempenho_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcel_ExportResultadoDesempenho(fileName, resultTotal, resultTotalCompetencias, resultTotalPerformances,
                resultProjetos, resultCompetencias, resultPerformances);

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public List<ResultadoSomaProjetosModel_Export> ConvertExport_ResultadoSomaProjetosModel(List<ResultadoSomaProjetosModel> listaSoma, string avaliado, string cargo, string periodo,
            string tipoAvaliacao, string escopo)
        {
            var returnResult = new List<ResultadoSomaProjetosModel_Export>();

            foreach (var item in listaSoma)
            {
                var addItem = new ResultadoSomaProjetosModel_Export();

                addItem.Avaliado = avaliado;
                addItem.Cargo = cargo;
                addItem.Periodo = periodo;
                addItem.TipoAvaliacao = tipoAvaliacao;
                addItem.Escopo = escopo;

                addItem.ComplexidadeComite = item.ComplexidadeComite;
                addItem.SomaProjetosPerfomance = item.SomaProjetosPerfomance;
                addItem.RatingPerfomance = item.RatingPerfomance;
                addItem.SomaProjetosNotaCompetenciaRadar = item.SomaProjetosNotaCompetenciaRadar;
                addItem.SomaProjetosNotaCompetencialN1N2 = item.SomaProjetosNotaCompetencialN1N2;
                addItem.CompetenciaCargo = item.CompetenciaCargo;
                addItem.CompetenciaProximoCargo = item.CompetenciaProximoCargo;

                returnResult.Add(addItem);
            }

            return returnResult;
        }

        public List<ResultadoTotalCompetenciaModel_Export> ConvertExport_ResultadoSomaCompetenciasModel(List<ResultadoSomaProjetosModel> listaSoma, string avaliado, string cargo, string periodo,
            string tipoAvaliacao, string escopo)
        {
            var returnResult = new List<ResultadoTotalCompetenciaModel_Export>();

            foreach (var item in listaSoma)
            {
                var listItems = item.ListProjetosSomaCompetenciasN1N2;
                foreach (var comp in listItems)
                {
                    var addItem = new ResultadoTotalCompetenciaModel_Export();

                    addItem.Avaliado = avaliado;
                    addItem.Cargo = cargo;
                    addItem.Periodo = periodo;
                    addItem.TipoAvaliacao = tipoAvaliacao;
                    addItem.Escopo = escopo;

                    addItem.Pilar = comp.Eixo;
                    addItem.NotaFinalNivel1 = comp.NotaFinalNivel1;
                    addItem.NotaFinalNivel2 = comp.NotaFinalNivel2;
                    addItem.PercentualNotaFinalNivel1 = comp.PercentualNotaFinalNivel1;
                    addItem.PercentualNotaFinalNivel1Avaliado = comp.PercentualNotaFinalNivel1Avaliado;
                    addItem.NotaProjetosCompetencia = comp.NotaProjetosCompetencia;
                    addItem.NotaProjetoCompetenciaRadar = comp.NotaProjetoCompetenciaRadar;
                    addItem.NotaCompetenciaAvaliado = comp.NotaCompetenciaAvaliado;
                    addItem.NotaCompetenciaGestor = comp.NotaCompetenciaGestor;
                    addItem.NotaSubcompetenciaAvaliadoN1 = comp.NotaSubcompetenciaAvaliadoN1;
                    addItem.NotaSubcompetenciaGestorN1 = comp.NotaSubcompetenciaGestorN1;
                    addItem.NotaSubcompetenciaAvaliadoN2 = comp.NotaSubcompetenciaAvaliadoN2;
                    addItem.NotaSubcompetenciaGestorN2 = comp.NotaSubcompetenciaGestorN2;
                    addItem.DetalheNivelAtual = comp.DetalheNivelAtual;
                    addItem.CompetenciaAtual = comp.CompetenciaAtual;
                    addItem.CompetenciaProximo = comp.CompetenciaProximo;
                    addItem.DetalheProximoNivel = comp.DetalheProximoNivel;
                    addItem.IdNotaNivel1Comite = comp.IdNotaNivel1Comite;
                    addItem.IdNotaNivel2Comite = comp.IdNotaNivel2Comite;
                    addItem.NotaFinalNivel1Avaliado = comp.NotaFinalNivel1Avaliado;
                    addItem.NotaFinalNivel2Avaliado = comp.NotaFinalNivel2Avaliado;
                    addItem.PercentualNotaFinalNivel2 = comp.PercentualNotaFinalNivel2;
                    addItem.PercentualNotaFinalNivel2Avaliado = comp.PercentualNotaFinalNivel2Avaliado;
                    addItem.PercentualSomaNotaFinalN1N2 = comp.PercentualSomaNotaFinalN1N2;
                    addItem.PercentualSomaNotaFinalN1N2Avaliado = comp.PercentualSomaNotaFinalN1N2Avaliado;
                    addItem.NotaCompetenciaRadar = comp.NotaCompetenciaRadar;
                    addItem.NotaCompetenciaRadarAvaliado = comp.NotaCompetenciaRadarAvaliado;

                    returnResult.Add(addItem);
                }
            }

            return returnResult;
        }

        public List<ResultadoTotalPerfomanceModel_Export> ConvertExport_ResultadoTotalPerfomanceModel(List<ResultadoSomaProjetosModel> listaSoma, string avaliado, string cargo, string periodo,
            string tipoAvaliacao, string escopo)
        {
            var returnResult = new List<ResultadoTotalPerfomanceModel_Export>();

            foreach (var item in listaSoma)
            {
                var listItems = item.ListProjetosSomaPerfomance;
                foreach (var comp in listItems)
                {
                    var addItem = new ResultadoTotalPerfomanceModel_Export();

                    addItem.Avaliado = avaliado;
                    addItem.Cargo = cargo;
                    addItem.Periodo = periodo;
                    addItem.TipoAvaliacao = tipoAvaliacao;
                    addItem.Escopo = escopo;

                    addItem.Perfomance = comp.Perfomance;
                    addItem.NotaPerfomance = comp.NotaPerfomance;
                    addItem.NotaPerfomancePonderada = comp.NotaPerfomancePonderada;

                    returnResult.Add(addItem);
                }
            }

            return returnResult;
        }

        public List<ResultadoProjetosModel_Export> ConvertExport_ResultadoProjetosModel(List<ResultadoProjetosModel> listaResultados)
        {
            var returnResult = new List<ResultadoProjetosModel_Export>();
            var associadosService = new AssociadosService();
            var cargoService = new CargosService();

            foreach (var item in listaResultados)
            {
                var addItem = new ResultadoProjetosModel_Export();

                addItem.Projeto = item.ProjetoNome;
                addItem.Avaliado = associadosService.ObterAssociado(item.IdAssociado).Nome;
                addItem.Cargo = cargoService.ObterCargo(item.IdCargo).Cargo;
                addItem.Periodo = item.Periodo;
                addItem.Avaliador = associadosService.ObterAssociado(item.IdAvaliador).Nome;
                addItem.GestorProjeto = item.NomeGestor;
                addItem.Responsavel = associadosService.ObterAssociado(item.IdResponsavel).Nome;
                addItem.TipoAvaliacao = item.TipoAvaliacao;
                addItem.Escopo = item.Escopo;
                addItem.ProjetoComplexidade = item.ProjetoComplexidade;
                addItem.PesoProjetoComplexidade = item.PesoProjetoComplexidade;
                addItem.PesoPonderadoProjetoComplexidade = item.PesoPonderadoProjetoComplexidade;
                addItem.DataInicioAlocado = item.DataInicioAlocado;
                addItem.DataFimAlocado = item.DataFimAlocado;
                addItem.NotaCompetencialNivel1 = item.NotaCompetencialNivel1;
                addItem.NotaCompetencialNivel2 = item.NotaCompetencialNivel2;
                addItem.SomaNotaCompetencialN1N2 = item.SomaNotaCompetencialN1N2;
                addItem.SomaPerfomance = item.SomaPerfomance;
                addItem.RatingPerfomance = item.RatingPerfomance;
                addItem.SomaNotaCompetenciaRadar = item.SomaNotaCompetenciaRadar;
                addItem.DiasProjeto = item.DiasProjeto;
                addItem.CompetenciaCargo = item.CompetenciaCargo;
                addItem.CompetenciaProximoCargo = item.CompetenciaProximoCargo;

                returnResult.Add(addItem);
            }

            return returnResult;
        }

        public List<ResultadoCompetenciaModel_Export> ConvertExport_ResultadoCompetenciaModel(List<ResultadoProjetosModel> listaResultados)
        {
            var returnResult = new List<ResultadoCompetenciaModel_Export>();
            var associadosService = new AssociadosService();
            var cargoService = new CargosService();

            foreach (var item in listaResultados)
            {
                var listCompetencias = item.ListCompetenciasNivel1;
                foreach (var comp in listCompetencias)
                {
                    var addItem = new ResultadoCompetenciaModel_Export();

                    addItem.Projeto = item.ProjetoNome;
                    addItem.Avaliado = associadosService.ObterAssociado(item.IdAssociado).Nome;
                    addItem.Cargo = cargoService.ObterCargo(item.IdCargo).Cargo;
                    addItem.Periodo = item.Periodo;
                    addItem.Avaliador = associadosService.ObterAssociado(item.IdAvaliador).Nome;
                    addItem.GestorProjeto = item.NomeGestor;
                    addItem.Responsavel = associadosService.ObterAssociado(item.IdResponsavel).Nome;
                    addItem.TipoAvaliacao = item.TipoAvaliacao;
                    addItem.Escopo = item.Escopo;
                    addItem.ProjetoComplexidade = item.ProjetoComplexidade;

                    addItem.Pilar = comp.Eixo;
                    addItem.NotaFinalNivel1 = comp.NotaFinalNivel1;
                    addItem.NotaFinalNivel2 = comp.NotaFinalNivel2;
                    addItem.PercentualNotaFinalNivel1 = comp.PercentualNotaFinalNivel1;
                    addItem.PercentualNotaFinalNivel1Avaliado = comp.PercentualNotaFinalNivel1Avaliado;
                    addItem.NotaProjetosCompetencia = comp.NotaProjetosCompetencia;
                    addItem.NotaProjetoCompetenciaRadar = comp.NotaProjetoCompetenciaRadar;
                    addItem.ComentarioFeedback = comp.ComentarioFeedback;
                    addItem.ComentarioAvaliado = comp.ComentarioAvaliado;
                    addItem.IdNotaNivel1AutoAvaliacao = comp.IdNotaNivel1AutoAvaliacao;
                    addItem.NotaNivel1AutoAvaliacao = comp.NotaNivel1AutoAvaliacao;
                    addItem.IdNotaNivel2AutoAvaliacao = comp.IdNotaNivel2AutoAvaliacao;
                    addItem.NotaNivel2AutoAvaliacao = comp.NotaNivel2AutoAvaliacao;
                    addItem.IdNotaNivel1Feedback = comp.IdNotaNivel1Feedback;
                    addItem.NotaNivel1Feedback = comp.NotaNivel1Feedback;
                    addItem.IdNotaNivel2Feedback = comp.IdNotaNivel2Feedback;
                    addItem.NotaNivel2Feedback = comp.NotaNivel2Feedback;
                    addItem.IdAvaliacaoCompetencia = comp.IdAvaliacaoCompetencia;
                    addItem.NotaCompetenciaAvaliado = comp.NotaCompetenciaAvaliado;
                    addItem.NotaCompetenciaGestor = comp.NotaCompetenciaGestor;
                    addItem.NotaSubcompetenciaAvaliadoN1 = comp.NotaSubcompetenciaAvaliadoN1;
                    addItem.NotaSubcompetenciaGestorN1 = comp.NotaSubcompetenciaGestorN1;
                    addItem.NotaSubcompetenciaAvaliadoN2 = comp.NotaSubcompetenciaAvaliadoN2;
                    addItem.NotaSubcompetenciaGestorN2 = comp.NotaSubcompetenciaGestorN2;
                    addItem.DetalheNivelAtual = comp.DetalheNivelAtual;
                    addItem.CompetenciaAtual = comp.CompetenciaAtual;
                    addItem.CompetenciaProximo = comp.CompetenciaProximo;
                    addItem.DetalheProximoNivel = comp.DetalheProximoNivel;
                    addItem.IdNotaNivel1Comite = comp.IdNotaNivel1Comite;
                    addItem.IdNotaNivel2Comite = comp.IdNotaNivel2Comite;
                    addItem.NotaFinalNivel1Avaliado = comp.NotaFinalNivel1Avaliado;
                    addItem.NotaFinalNivel2Avaliado = comp.NotaFinalNivel2Avaliado;

                    var getCompetenciaNivel2 = item.ListCompetenciasNivel2.FirstOrDefault(x => x.Eixo == comp.Eixo);
                    addItem.PercentualNotaFinalNivel2 = getCompetenciaNivel2.PercentualNotaFinalNivel2;
                    addItem.PercentualNotaFinalNivel2Avaliado = getCompetenciaNivel2.PercentualNotaFinalNivel2Avaliado;

                    var getCompetenciaSoma = item.ListSomaCompetenciasN1N2.FirstOrDefault(x => x.Eixo == comp.Eixo);
                    addItem.PercentualSomaNotaFinalN1N2 = getCompetenciaSoma.PercentualSomaNotaFinalN1N2;
                    addItem.PercentualSomaNotaFinalN1N2Avaliado = getCompetenciaSoma.PercentualSomaNotaFinalN1N2Avaliado;
                    addItem.NotaCompetenciaRadar = getCompetenciaSoma.NotaCompetenciaRadar;
                    addItem.NotaCompetenciaRadarAvaliado = getCompetenciaSoma.NotaCompetenciaRadarAvaliado;

                    returnResult.Add(addItem);
                }
            }

            return returnResult;
        }

        public List<ResultadoPerfomanceModel_Export> ConvertExport_ResultadoPerfomanceModel(List<ResultadoProjetosModel> listaResultados)
        {
            var returnResult = new List<ResultadoPerfomanceModel_Export>();
            var associadosService = new AssociadosService();
            var cargoService = new CargosService();

            foreach (var item in listaResultados)
            {
                var listPerformances = item.ListPerfomance;
                foreach (var comp in listPerformances)
                {
                    var addItem = new ResultadoPerfomanceModel_Export();

                    addItem.Projeto = item.ProjetoNome;
                    addItem.Avaliado = associadosService.ObterAssociado(item.IdAssociado).Nome;
                    addItem.Cargo = cargoService.ObterCargo(item.IdCargo).Cargo;
                    addItem.Periodo = item.Periodo;
                    addItem.Avaliador = associadosService.ObterAssociado(item.IdAvaliador).Nome;
                    addItem.GestorProjeto = item.NomeGestor;
                    addItem.Responsavel = associadosService.ObterAssociado(item.IdResponsavel).Nome;
                    addItem.TipoAvaliacao = item.TipoAvaliacao;
                    addItem.Escopo = item.Escopo;
                    addItem.ProjetoComplexidade = item.ProjetoComplexidade;

                    addItem.Perfomance = comp.Perfomance;
                    addItem.NotaPerfomance = comp.NotaPerfomance;
                    addItem.NotaPerfomancePonderada = comp.NotaPerfomancePonderada;
                    addItem.NotaNivel1AutoAvaliacao = comp.NotaNivel1AutoAvaliacao;
                    addItem.NotaNivel1Feedback = comp.NotaNivel1Feedback;
                    addItem.ComentariosAutoAvaliacao = comp.ComentariosAutoAvaliacao;
                    addItem.ComentarioFeedback = comp.ComentarioFeedback;

                    returnResult.Add(addItem);
                }
            }

            return returnResult;
        }
    }
}
