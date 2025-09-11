using Business.DataAccess;
using Business.Model;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class AssociadosService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AssociadosService));
        public AssociadosModel ObterAssociadoMentorCargo(int idAssociado)
        {
            DataModel _context = new DataModel();

            AssociadosModel dadosAssociado = (from a in _context.ASSOCIADOS
                                              join c in _context.CARGOS
                                              on a.IdCargo equals c.IdCargo
                                              where a.IdAssociado == idAssociado
                                              select new AssociadosModel()
                                              {
                                                  Nome = a.Nome,
                                                  Cargo = c.Cargo,
                                                  IdCargo = c.IdCargo,
                                                  ProximoCargo = c.idProximoCargo.HasValue ? _context.CARGOS.FirstOrDefault(x => x.IdCargo == c.idProximoCargo).Cargo : string.Empty,
                                                  Mentor = _context.ASSOCIADOS.FirstOrDefault(x => x.IdAssociado == a.IdAssociadoMentor).Nome,
                                                  IdAssociado = a.IdAssociado
                                              }).FirstOrDefault();

            if (dadosAssociado != null)
            {
                return dadosAssociado;
            }
            else
            {
                return null;
            }

        }

        public AssociadosModel ObterAssociadoCargoUltimaAvaliacao(int idAssociado, string TipoAvaliacao, string Escopo)
        {
            DataModel _context = new DataModel();

            AVALIACOESCOMPETENCIAS avalcompetencias = _context.AVALIACOESCOMPETENCIAS.OrderByDescending(av => av.IdAvaliacaoCompetencia).
                FirstOrDefault(av => av.IdAssociado == idAssociado && av.TipoAvaliacao == TipoAvaliacao && av.Escopo == Escopo && av.ATV == 1 && av.IdAvaliacaoStatus == 3 && 
                (av.PosicaoAtualFluxoAvaliacao == "AME" || av.PosicaoAtualFluxoAvaliacao == "AFI" || av.PosicaoAtualFluxoAvaliacao == "FED"));

            ASSOCIADOS associado = _context.ASSOCIADOS.FirstOrDefault(asso => asso.IdAssociado == idAssociado);
            CARGOS cargo = _context.CARGOS.FirstOrDefault(car => car.IdCargo == avalcompetencias.IdCargo);

            AssociadosModel dadosAssociadoUltimaAvaliacao = new AssociadosModel();
            dadosAssociadoUltimaAvaliacao.Nome = associado.Nome;
            dadosAssociadoUltimaAvaliacao.Cargo = cargo.Cargo;
            dadosAssociadoUltimaAvaliacao.IdCargo = cargo.IdCargo;
            dadosAssociadoUltimaAvaliacao.ProximoCargo = cargo.idProximoCargo.HasValue ? _context.CARGOS.FirstOrDefault(x => x.IdCargo == cargo.idProximoCargo).Cargo : string.Empty;
            dadosAssociadoUltimaAvaliacao.Mentor = _context.ASSOCIADOS.FirstOrDefault(x => x.IdAssociado == associado.IdAssociadoMentor).Nome;

            if (dadosAssociadoUltimaAvaliacao != null)
            {
                return dadosAssociadoUltimaAvaliacao;
            }
            else
            {
                return null;
            }
        }

        public AssociadosModel ObterAssociadoCargoUltimaEvolucao(int idAssociado, string TipoAvaliacao, string Escopo)
        {
            DataModel _context = new DataModel();

            EVOLUCAOASSOCIADO evolucao = _context.EVOLUCAOASSOCIADO.OrderByDescending(ev => ev.IdEvolucaoAssociado).
                FirstOrDefault(av => av.IdAssociado == idAssociado && av.TipoAvaliacao == TipoAvaliacao && av.Escopo == Escopo);

            CARGOS cargo = _context.CARGOS.FirstOrDefault(car => car.Cargo == evolucao.Cargo);

            ASSOCIADOS associado = _context.ASSOCIADOS.FirstOrDefault(asso => asso.IdAssociado == idAssociado);
            

            AssociadosModel dadosAssociadoUltimaAvaliacao = new AssociadosModel();
            dadosAssociadoUltimaAvaliacao.Nome = associado.Nome;
            dadosAssociadoUltimaAvaliacao.Cargo = cargo.Cargo;
            dadosAssociadoUltimaAvaliacao.IdCargo = cargo.IdCargo;
            dadosAssociadoUltimaAvaliacao.ProximoCargo = cargo.idProximoCargo.HasValue ? _context.CARGOS.FirstOrDefault(x => x.IdCargo == cargo.idProximoCargo).Cargo : string.Empty;
            dadosAssociadoUltimaAvaliacao.Mentor = _context.ASSOCIADOS.FirstOrDefault(x => x.IdAssociado == associado.IdAssociadoMentor).Nome;

            if (dadosAssociadoUltimaAvaliacao != null)
            {
                return dadosAssociadoUltimaAvaliacao;
            }
            else
            {
                return null;
            }
        }

        public ASSOCIADOS ObterAssociado(int idAssociado)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.FirstOrDefault(a => a.IdAssociado == idAssociado);
        }

        public ASSOCIADOS ObterLiderado(int idAssociado)
        {
            DataModel context = new DataModel();
            var idMentor = WebStorage.GetUsuarioLogado().Id;
            return context.ASSOCIADOS.FirstOrDefault(a => a.IdAssociado == idAssociado && a.IdAssociadoMentor == idMentor);
        }

        public ASSOCIADOS ObterAssociado(string email)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.FirstOrDefault(a => a.Email.Trim().ToUpper() == email.Trim().ToUpper());
        }
        public ASSOCIADOS ObterAssociadoPeloNome(string nome)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.FirstOrDefault(a => a.Nome == nome);
        }
        public ASSOCIADOS ObterAssociadoPeloNomeDaFoto(string fotoNome)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.FirstOrDefault(a => a.FotoNome == fotoNome);
        }
        public ASSOCIADOS ObterGestor(int idProjeto, int idAssociado)
        {
            DataModel context = new DataModel();
            var listaProjetosAssociados = new ProjetosService().ObterListaAssociados(idProjeto, idAssociado);
            if (listaProjetosAssociados.Count > 0)
            {
                var idGestor = listaProjetosAssociados[0].IdGestor;

                return context.ASSOCIADOS.FirstOrDefault(a => a.IdAssociado == idGestor);
            }
            else
            {
                return null;
            }
        }

        public ASSOCIADOS ObterAvaliador(int idProjeto, int idAssociado)
        {
            DataModel context = new DataModel();
            var listaProjetosAssociados = new ProjetosService().ObterListaAssociados(idProjeto, idAssociado);
            var idGestor = listaProjetosAssociados[0].IdGestor;
            if (listaProjetosAssociados.Count > 0)
                return context.ASSOCIADOS.FirstOrDefault(a => a.IdAssociado == idGestor);
            else
                return null;
        }

        public ASSOCIADOS ObterGestor2(int idAssociado)
        {
            DataModel context = new DataModel();
            var listaProjetosAssociados = new ProjetosService().ObterListaAssociados(idAssociado);
            var idGestor = listaProjetosAssociados[0].IdGestor;
            if (listaProjetosAssociados.Count > 0)
                return context.ASSOCIADOS.FirstOrDefault(a => a.IdAssociado == idGestor);
            else
                return null;
        }


        public ASSOCIADOS ObterMentor(int idAssociado)
        {
            DataModel context = new DataModel();
            var associado = context.ASSOCIADOS.FirstOrDefault(a => a.IdAssociado == idAssociado);
            return context.ASSOCIADOS.FirstOrDefault(a => a.IdAssociadoMentor == associado.IdAssociadoMentor);

        }

        public List<ASSOCIADOS> ObterMentorados(int idAssociado)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.Where(a => a.IdAssociadoMentor == idAssociado).ToList();
        }

        public List<ASSOCIADOS> ObterAssociados()
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.OrderBy(a => a.Nome).ToList();
        }



        public List<ASSOCIADOS> ObterAssociados(bool ativos)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Nome).ToList();
        }


        public List<ASSOCIADOS> ObterAssociadosSocios(bool ativos)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.Where(a => a.ATV == (ativos ? 1 : 0) && a.IdPerfil == 3).OrderBy(a => a.Nome).ToList();
        }


        public List<ASSOCIADOS> ObterAssociadosGerentes(bool ativos)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.Where(a => a.ATV == (ativos ? 1 : 0) && a.IdPerfil == 2).OrderBy(a => a.Nome).ToList();
        }


        public List<ASSOCIADOS> ObterAssociadosConsultores(bool ativos)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Nome).ToList();
        }


        public List<ASSOCIADOS> ObterAssociados(PERFIS perfil)
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.Where(a => a.IdPerfil == perfil.IdPerfil).ToList();
        }

        public ASSOCIADOS ObterUltimoAssociado()
        {
            DataModel context = new DataModel();
            return context.ASSOCIADOS.OrderByDescending(a => a.IdAssociado).FirstOrDefault(a => a.ATV == 1);
        }

        public bool InserirAssociado(ASSOCIADOS associado)
        {
            try
            {
                DataModel context = new DataModel();
                context.ASSOCIADOS.Add(associado);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                //throw ex;
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
        }

        public bool ExcluiAssociado(int idAssociado, ASSOCIADOS associado)
        {
            bool OK = false;

            try
            {
                using (var db = new DataModel())
                {
                    var result = db.ASSOCIADOS.SingleOrDefault(c => c.IdAssociado == idAssociado);
                    if (result != null)
                    {
                        //result.Senha = associado.Senha;    
                        result.IdStatus = associado.IdStatus;
                        result.ATV = associado.ATV;
                        OK = db.SaveChanges() >= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return OK;
        }

        public bool AlteraAssociadoSenha(int idAssociado, ASSOCIADOS associado)
        {
            bool OK = false;

            try
            {
                using (var db = new DataModel())
                {
                    var result = db.ASSOCIADOS.SingleOrDefault(c => c.IdAssociado == idAssociado);
                    if (result != null)
                    {
                        result.Senha = associado.Senha;
                        OK = db.SaveChanges() >= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return OK;
        }
        public bool AlteraAssociado(int idAssociado, ASSOCIADOS associado)
        {
            bool OK = true;

            try
            {
                using (var db = new DataModel())
                {
                    var result = db.ASSOCIADOS.SingleOrDefault(c => c.IdAssociado == idAssociado);

                    if (result != null)
                    {
                        result.Nome = associado.Nome;
                        result.Email = associado.Email;
                        result.IdCargo = associado.IdCargo;
                        result.IdAssociadoMentor = associado.IdAssociadoMentor;
                        result.IdPerfil = associado.IdPerfil;
                        result.IdNivel = associado.IdNivel;
                        result.IdStatus = associado.IdStatus;
                        result.ATV = associado.ATV;
                        result.FotoNome = associado.FotoNome;
                        result.DataAdmissao = associado.DataAdmissao;
                        result.Vertical = associado.Vertical;

                        if (associado.Senha != "")
                        {
                            result.Senha = associado.Senha;
                        }

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return OK;
        }

    }
}
