using Business.DataAccess;
using Business.Model;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class EvolucaoAssociadoServices
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EvolucaoAssociadoServices));
        DataModel _context;
        public EvolucaoAssociadoServices()
        {
            _context = new DataModel();
        }

        public void GerarEvolucaoAssociado(int idAssociado, int idPeriodo, int idCargo, string TipoAvaliacao, string Escopo)
        {
            try
            {
                var resultadoAssociado = new ResultadoServices().ObterResultadoAssociado_OBSOLETO(idAssociado, idPeriodo, idCargo, TipoAvaliacao, Escopo);
                var rating = _context.RATING; 
                var projetosService = new ProjetosService();

                if (resultadoAssociado != null)
                {
                    var somaProjetos = new ResultadoServices().SomaResultadoProjetos(resultadoAssociado);
                    if (TipoAvaliacao == "lideranca") { somaProjetos = new ResultadoServices().SomaResultadoLideranca(idAssociado, idPeriodo, TipoAvaliacao, Escopo); }
                    var projeto = somaProjetos.FirstOrDefault();
                    var cargo = _context.CARGOS.FirstOrDefault(x => x.IdCargo == idCargo).Cargo;

                    var existEvolucao = _context.EVOLUCAOASSOCIADO.Where(x => x.IdAssociado == idAssociado && x.IdPeriodo == idPeriodo && x.TipoAvaliacao == TipoAvaliacao && x.Escopo == Escopo).ToList();

                    if (existEvolucao != null && existEvolucao.Count > 0)
                    {
                        foreach (var item in existEvolucao)
                        {
                            if (item.EVOLUCAOCOMPETENCIAS != null)
                            {
                                foreach (var comp in item.EVOLUCAOCOMPETENCIAS.ToList())
                                {
                                    _context.EVOLUCAOCOMPETENCIAS.Remove(comp);
                                    _context.SaveChanges();
                                }
                            }

                            if (item.EVOLUCAOPERFORMANCE != null)
                            {
                                foreach (var perf in item.EVOLUCAOPERFORMANCE.ToList())
                                {
                                    _context.EVOLUCAOPERFORMANCE.Remove(perf);
                                    _context.SaveChanges();
                                }
                            }

                            _context.EVOLUCAOASSOCIADO.Remove(item);
                            _context.SaveChanges();
                        }
                    }

                    var getFirst = somaProjetos[0];
                    //var result = rating.Where(d => d.IdComplexidade == getFirst.IdComplexidade && getFirst.SomaPerfomance >= d.FaixaInicial &&
                    //            getFirst.SomaPerfomance <= d.FaixaFinal).OrderByDescending(x => x.FaixaFinal);

                    EVOLUCAOASSOCIADO evolucao = new EVOLUCAOASSOCIADO();
                    evolucao.IdAssociado = idAssociado;
                    evolucao.IdPeriodo = idPeriodo;
                    evolucao.DHC = DateTime.Now;
                    evolucao.Cargo = cargo;
                    evolucao.NotaPerfomance = projeto.SomaProjetosPerfomance;
                    evolucao.RatingPerformance = getFirst.RatingPerfomance;
                    evolucao.NotaCompetencia = projeto.SomaProjetosNotaCompetencialN1N2.HasValue && projeto.SomaProjetosNotaCompetencialN1N2.Value > 0 ? projeto.SomaProjetosNotaCompetencialN1N2.Value / 100 : 0;
                    evolucao.TipoAvaliacao = TipoAvaliacao;
                    evolucao.Escopo = Escopo;

                    foreach (var item in projeto.ListProjetosSomaPerfomance)
                    {
                        evolucao.EVOLUCAOPERFORMANCE.Add(new EVOLUCAOPERFORMANCE()
                        {
                            IdPerformance = item.IdPerformance,
                            Nota = item.NotaPerfomancePonderada
                        });
                    }

                    foreach (var item in projeto.ListProjetosSomaCompetenciasN1N2)
                    {
                        evolucao.EVOLUCAOCOMPETENCIAS.Add(new EVOLUCAOCOMPETENCIAS()
                        {
                            IdEixo = item.IdEixo,
                            Nota = item.NotaProjetosCompetencia.HasValue && item.NotaProjetosCompetencia.Value > 0 ? item.NotaProjetosCompetencia.Value / 100 : 0
                        });
                    }

                    _context.EVOLUCAOASSOCIADO.Add(evolucao);
                    _context.SaveChanges();

                }


            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao tentar gerar a evolução do associado: " + ex.Message);
            }
        }

        public void GerarEvolucaoAssociado_Otimizado(List<AVALIACAO> listAvaliacao, List<AVALIACOESCOMPETENCIAS> listAvCompetencias, List<AVALIACOESPERFORMANCES> listAvPerformances,
            List<COMPETENCIAS> listCompetencias, List<PERFORMANCES> listPerformances,
            int idAssociado, int idPeriodo, int idCargo, string TipoAvaliacao, string Escopo)
        {
            try
            {
                var resultadoAssociado = new ResultadoServices().ObterResultadoAssociado_Otimizado(listAvaliacao, listAvCompetencias, listCompetencias, listAvPerformances, listPerformances,
                    idAssociado, idPeriodo, idCargo, TipoAvaliacao, Escopo);
                var rating = _context.RATING;
                var projetosService = new ProjetosService();

                if (resultadoAssociado != null)
                {
                    var somaProjetos = new ResultadoServices().SomaResultadoProjetos(resultadoAssociado);
                    if (TipoAvaliacao == "lideranca") { somaProjetos = new ResultadoServices().SomaResultadoLideranca(idAssociado, idPeriodo, TipoAvaliacao, Escopo); }
                    var projeto = somaProjetos.FirstOrDefault();
                    var cargo = _context.CARGOS.FirstOrDefault(x => x.IdCargo == idCargo).Cargo;

                    var existEvolucao = _context.EVOLUCAOASSOCIADO.Where(x => x.IdAssociado == idAssociado && x.IdPeriodo == idPeriodo && x.TipoAvaliacao == TipoAvaliacao && x.Escopo == Escopo).ToList();

                    if (existEvolucao != null && existEvolucao.Count > 0)
                    {
                        foreach (var item in existEvolucao)
                        {
                            if (item.EVOLUCAOCOMPETENCIAS != null)
                            {
                                foreach (var comp in item.EVOLUCAOCOMPETENCIAS.ToList())
                                {
                                    _context.EVOLUCAOCOMPETENCIAS.Remove(comp);
                                    _context.SaveChanges();
                                }
                            }

                            if (item.EVOLUCAOPERFORMANCE != null)
                            {
                                foreach (var perf in item.EVOLUCAOPERFORMANCE.ToList())
                                {
                                    _context.EVOLUCAOPERFORMANCE.Remove(perf);
                                    _context.SaveChanges();
                                }
                            }

                            _context.EVOLUCAOASSOCIADO.Remove(item);
                            _context.SaveChanges();
                        }
                    }

                    var getFirst = somaProjetos[0];
                    //var result = rating.Where(d => d.IdComplexidade == getFirst.IdComplexidade && getFirst.SomaPerfomance >= d.FaixaInicial &&
                    //            getFirst.SomaPerfomance <= d.FaixaFinal).OrderByDescending(x => x.FaixaFinal);

                    EVOLUCAOASSOCIADO evolucao = new EVOLUCAOASSOCIADO();
                    evolucao.IdAssociado = idAssociado;
                    evolucao.IdPeriodo = idPeriodo;
                    evolucao.DHC = DateTime.Now;
                    evolucao.Cargo = cargo;
                    evolucao.NotaPerfomance = projeto.SomaProjetosPerfomance;
                    evolucao.RatingPerformance = getFirst.RatingPerfomance;
                    evolucao.NotaCompetencia = projeto.SomaProjetosNotaCompetencialN1N2.HasValue && projeto.SomaProjetosNotaCompetencialN1N2.Value > 0 ? projeto.SomaProjetosNotaCompetencialN1N2.Value / 100 : 0;
                    evolucao.TipoAvaliacao = TipoAvaliacao;
                    evolucao.Escopo = Escopo;

                    foreach (var item in projeto.ListProjetosSomaPerfomance)
                    {
                        evolucao.EVOLUCAOPERFORMANCE.Add(new EVOLUCAOPERFORMANCE()
                        {
                            IdPerformance = item.IdPerformance,
                            Nota = item.NotaPerfomancePonderada
                        });
                    }

                    foreach (var item in projeto.ListProjetosSomaCompetenciasN1N2)
                    {
                        evolucao.EVOLUCAOCOMPETENCIAS.Add(new EVOLUCAOCOMPETENCIAS()
                        {
                            IdEixo = item.IdEixo,
                            Nota = item.NotaProjetosCompetencia.HasValue && item.NotaProjetosCompetencia.Value > 0 ? item.NotaProjetosCompetencia.Value / 100 : 0
                        });
                    }

                    _context.EVOLUCAOASSOCIADO.Add(evolucao);
                    _context.SaveChanges();

                }


            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao tentar gerar a evolução do associado: " + ex.Message);
            }
        }

        public List<AssociadosModel> ListAssociadosEvolucao(int? idassociado, int idperiodo, int idVertical = 0)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            var periodoatual = new PeriodoService().ObterPeriodo(idperiodo);
            var dataatual = DateTime.Now;

            var cargos = _context.CARGOS;
            var mentores = _context.ASSOCIADOS;
            var avalFim = new AvaliacoesService().etapaAvaliacaoFinalizada;
            var avalMentor = new AvaliacoesService().etapaAvaliacaoMentor;

            var listevolucao = _context.EVOLUCAOASSOCIADO;

            var result = (from ava in _context.AVALIACAO
                          join a in _context.ASSOCIADOS
                          on ava.idAssociado equals a.IdAssociado
                          where ava.idPeriodo == idperiodo 
                          && (ava.PosicaoAtualFluxoAvaliacao == avalFim || ava.PosicaoAtualFluxoAvaliacao == avalMentor)
                          && ava.TipoAvaliacao == "desempenho"
                          && a.IdVertical == idVertical
                          group new { a.Nome, a.IdCargo, a.IdAssociadoMentor, ava.idPeriodo, ava.TipoAvaliacao, ava.Escopo, a.Vertical }
                          by new { a.IdAssociado, ava.idPeriodo, ava.TipoAvaliacao, ava.Escopo } into grp
                          orderby grp.Key.idPeriodo descending
                          select new AssociadosModel()
                          {
                              Nome = grp.FirstOrDefault().Nome,
                              IdCargo = grp.FirstOrDefault().IdCargo,
                              Cargo = cargos.FirstOrDefault(x => x.IdCargo == grp.FirstOrDefault().IdCargo).Cargo,
                              Mentor = mentores.FirstOrDefault(x => x.IdAssociado == grp.FirstOrDefault().IdAssociadoMentor).Nome,
                              Periodo = periodoatual.Periodo,
                              IdAssociado = grp.Key.IdAssociado,
                              IdPeriodo = grp.Key.idPeriodo,
                              QtdProjetos = grp.Count(),
                              Enviado = listevolucao.Count(x => x.IdPeriodo == idperiodo && x.IdAssociado == grp.Key.IdAssociado) > 0 ? "Sim" : "Não",
                              TipoAvaliacao = grp.Key.TipoAvaliacao,
                              Escopo = grp.Key.Escopo,
                              Vertical = grp.FirstOrDefault().Vertical
                          }).ToList();

            if (idassociado.HasValue && idassociado.Value > 0)
            {
                result = result.Where(x => x.IdAssociado == idassociado.Value).ToList();
            }                      

            return result;

        }

        private const int resultadoprojetopeers = 1;
        private const int resultadoprojetoclientes = 2;
        private const int atuacaonivel = 3;
        private const int qualidadeentrega = 4;
        private const int relacionamentocliente = 5;
        private const int valorempresa = 6;
        private const int comercial = 7;

        public List<EVOLUCAOASSOCIADO> Evolucao(int idassociado, string TipoAvaliacao, string Escopo)
        {
            try
            {
                var resultadoService = new ResultadoServices();
                var avaliacoesService = new AvaliacoesService();
                var result = new List<EVOLUCAOASSOCIADO>();
                var competenciasService = new CompetenciasService();
                var performancesService = new PerformancesService();

                var getAvaliacoes = avaliacoesService.ObterListaAvaliacao();
                var getCompetencias = competenciasService.ObterListaCompetencias();
                var getPerformances = performancesService.ObterListaPerformances();
                var getAvPerformances = avaliacoesService.ObterAvaliacaoPerformanceTodos(-1, -1, -1);
                var getAvCompetencias = avaliacoesService.ListaAvCompetencias();

                result = (from e in _context.EVOLUCAOASSOCIADO
                                join a in _context.ASSOCIADOS
                                on e.IdAssociado equals a.IdAssociado
                                join p in _context.PERIODOSAVALIACOES
                                on e.IdPeriodo equals p.IdPeriodo
                                where e.IdAssociado == idassociado
                                && e.TipoAvaliacao == TipoAvaliacao
                                && e.Escopo == Escopo
                                orderby p.DataInicio descending
                                select e).Take(4).ToList();

                foreach (var item in result)
                {
                    var checkAvaliacao = new AvaliacoesService().ObterAvaliacaoCompetenciaQualquerNoPeriodo(item.IdAssociado, item.IdPeriodo, item.TipoAvaliacao, item.Escopo);
                    if (checkAvaliacao != null)
                    {
                        item.Cargo = checkAvaliacao[0].CARGOS.Cargo;
                    }


                    if (item.NotaCompetencia.HasValue && item.NotaCompetencia.Value > 0)
                    {
                        item.NotaCompetencia = item.NotaCompetencia.Value * 100;
                    }

                    if (item.EVOLUCAOCOMPETENCIAS != null)
                    {
                        foreach (var comp in item.EVOLUCAOCOMPETENCIAS)
                        {
                            var associado = new AssociadosService().ObterAssociado(idassociado);
                            var cargo_atual = new CargosService().ObterCargo(associado.IdCargo);
                            var premissa_atual = cargo_atual != null ? new PremissaService().ObterPremissaPorCardo(cargo_atual.IdCargo) : null;

                            if (comp.Nota.HasValue)
                            {
                                var cargo = new CargosService().ObterCargoPorNome(item.Cargo);
                                var premissaradar = cargo != null ? new PremissaService().ObterPremissaPorCardo(cargo.IdCargo) : null;

                                float multiPremissa = premissaradar != null ? premissaradar.ValorRadarPeers : 1;
                                multiPremissa = multiPremissa < 1 ? 1 : multiPremissa;
                                float multiAtual = premissa_atual != null ? premissa_atual.ValorRadarPeers : 1;
                                multiAtual = multiAtual < 1 ? 1 : multiAtual;

                                float stepAtual = 200 / (multiAtual + 1);

                                float menorNota = multiPremissa * stepAtual - stepAtual;
                                float resultadoNota = float.Parse(comp.Nota.ToString()) * stepAtual;

                                float notaFinal = menorNota + resultadoNota;

                                comp.NotaNeutra = comp.Nota * 100;
                                comp.Nota = Convert.ToDecimal(notaFinal);
                                comp.Nota += 0;
                            }
                        }
                    }

                    if (item.EVOLUCAOPERFORMANCE != null)
                    {
                        foreach (var perf in item.EVOLUCAOPERFORMANCE)
                        {
                            switch (perf.PERFORMANCES.Performance)
                            {
                                case "Resultado do projeto (peers)":
                                    perf.IdPerformance = resultadoprojetopeers;
                                    break;
                                case "Resultado do projeto (cliente)":
                                    perf.IdPerformance = resultadoprojetoclientes;
                                    break;
                                case "Atuação no nível":
                                    perf.IdPerformance = atuacaonivel;
                                    break;
                                case "Qualidade de entrega":
                                    perf.IdPerformance = qualidadeentrega;
                                    break;
                                case "Relacionamento gerado com cliente":
                                    perf.IdPerformance = relacionamentocliente;
                                    break;
                                case "Valor para empresa":
                                    perf.IdPerformance = valorempresa;
                                    break;
                                case "Comercial":
                                    perf.IdPerformance = comercial;
                                    break;
                            }
                        }

                        if (item.RatingPerformance == "" || item.RatingPerformance == null)
                        {
                            try
                            {
                                var getAvaliacaoPeriodo = avaliacoesService.ObterAvaliacaoCompetenciaQualquerNoPeriodo(item.IdAssociado, item.IdPeriodo, item.TipoAvaliacao, item.Escopo)[0];
                                var resultadoAssociado = resultadoService.ObterResultadoAssociado_Otimizado(getAvaliacoes, getAvCompetencias, getCompetencias, getAvPerformances, getPerformances,
                                    item.IdAssociado, item.IdPeriodo, getAvaliacaoPeriodo.IdCargo, item.TipoAvaliacao, item.Escopo);
                                var somaResultado = resultadoService.SomaResultadoProjetos(resultadoAssociado);
                                var getPrimeiro = somaResultado[0];
                                item.RatingPerformance = getPrimeiro.RatingPerfomance;
                            }
                            catch{
                                item.RatingPerformance = "-";
                            }
                        }
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw new Exception("Erro ao tentar obter os dados da evolução: " + ex.Message);
            }
        }
    }
}


