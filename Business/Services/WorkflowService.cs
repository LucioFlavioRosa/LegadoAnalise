using Business.DataAccess;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class WorkflowService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(WorkflowService));
        public bool Inserir(WORKFLOW workflow)
        {
            try
            {
                var userLogado = WebStorage.GetUsuarioLogado();
                
                //Padrão na Inclusão
                workflow.ATV = 1;
                workflow.DHC = DateTime.Now;
                workflow.IdEmpresa = userLogado.IdEmpresa;
                workflow.USR = userLogado.Id;

                DataModel context = new DataModel();
                context.WORKFLOW.Add(workflow);
                return context.SaveChanges() >= 1;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
        }

        public bool Alterar(int idWorkflow, WORKFLOW workflow)
        {
            bool OK = false;

            try
            {
                using (var db = new DataModel())
                {
                    var result = db.WORKFLOW.SingleOrDefault(c => c.IdWorkflow == idWorkflow);
                    if (result != null)
                    {
                        result.ATV = workflow.ATV;
                        result.DataInicio = workflow.DataInicio;
                        result.DHC = workflow.DHC;
                        result.DiasAutoAvaliacao = workflow.DiasAutoAvaliacao;
                        result.DiasAvaliacaoCegas = workflow.DiasAvaliacaoCegas;
                        result.DiasAvaliacaoGestor = workflow.DiasAvaliacaoGestor;
                        result.DiasFeedback = workflow.DiasFeedback;
                        result.IdPeriodo = workflow.IdPeriodo;
                        result.USR = workflow.USR;
                        
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

        public bool Deletar(int idWorkflow)
        {
            try
            {
                using (var db = new DataModel())
                {
                    var result = db.WORKFLOW.SingleOrDefault(c => c.IdWorkflow == idWorkflow);
                    if (result != null)
                    {
                        db.WORKFLOW.Remove(result);
                        return db.SaveChanges() >= 1;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
        }

        public WORKFLOW Obter(int idWorkflow)
        {
            DataModel context = new DataModel();
            return context.WORKFLOW.FirstOrDefault(a => a.IdWorkflow == idWorkflow);
        }

        public WORKFLOW ObterByPeriodo(int idPeriodo)
        {
            DataModel context = new DataModel();
            return context.WORKFLOW.FirstOrDefault(a => a.IdPeriodo == idPeriodo);
        }

        public List<WORKFLOW> ObterLista()
        {
            var userLogado = WebStorage.GetUsuarioLogado();

            DataModel context = new DataModel();
            return context.WORKFLOW.Where(w => w.IdEmpresa == userLogado.IdEmpresa).OrderByDescending(o => o.DataInicio).ToList();
        }

        public List<WORKFLOW> ObterListaAtivos()
        {
            var userLogado = WebStorage.GetUsuarioLogado();

            DataModel context = new DataModel();
            return context.WORKFLOW.Where(w => w.IdEmpresa == userLogado.IdEmpresa && w.ATV == 1).OrderByDescending(o => o.DataInicio).ToList();
        }

        // PRAZOS
        public List<PRAZOS> ObterTodosPrazos()
        {
            DataModel context = new DataModel();
            return context.PRAZOS.ToList();
        }
        public List<PRAZOS> ObterTodosPrazosAtivos()
        {
            DataModel context = new DataModel();
            return context.PRAZOS.Where(p => p.ATV == 1).ToList();
        }
        public PRAZOS ObterPrazo(int IdPrazo)
        {
            DataModel context = new DataModel();
            return context.PRAZOS.FirstOrDefault(p => p.IdPrazo == IdPrazo);
        }
        public int CadastrarPrazo(PRAZOS addPrazo)
        {
            try
            {
                DataModel context = new DataModel();
                bool insercao = true;

                // INSERIR
                if (addPrazo.IdPrazo <= 0)
                {
                    context.PRAZOS.Add(addPrazo);
                    insercao = true;
                }
                // ATUALIZAR
                else
                {
                    var getPrazo = context.PRAZOS.FirstOrDefault(p => p.IdPrazo == addPrazo.IdPrazo);
                    getPrazo.NomeDisparo = addPrazo.NomeDisparo;
                    getPrazo.DuracaoAutoAvaliacao = addPrazo.DuracaoAutoAvaliacao;
                    getPrazo.GatilhoAutoAvaliacao = addPrazo.GatilhoAutoAvaliacao;
                    getPrazo.CompensadorAutoAvaliacao = addPrazo.CompensadorAutoAvaliacao;
                    getPrazo.DuracaoAvaliacaoAsCegas = addPrazo.DuracaoAvaliacaoAsCegas;
                    getPrazo.GatilhoAvaliacaoAsCegas = addPrazo.GatilhoAvaliacaoAsCegas;
                    getPrazo.CompensadorAvaliacaoAsCegas = addPrazo.CompensadorAvaliacaoAsCegas;
                    getPrazo.DuracaoAvaliacaoGestor = addPrazo.DuracaoAvaliacaoGestor;
                    getPrazo.GatilhoAvaliacaoGestor = addPrazo.GatilhoAvaliacaoGestor;
                    getPrazo.CompensadorAvaliacaoGestor = addPrazo.CompensadorAvaliacaoGestor;
                    getPrazo.DuracaoFeedback = addPrazo.DuracaoFeedback;
                    getPrazo.GatilhoFeedback = addPrazo.GatilhoFeedback;
                    getPrazo.CompensadorFeedback = addPrazo.CompensadorFeedback;
                    getPrazo.DuracaoMentor = addPrazo.DuracaoMentor;
                    getPrazo.GatilhoMentor = addPrazo.GatilhoMentor;
                    getPrazo.CompensadorMentor = addPrazo.CompensadorMentor;
                    getPrazo.ATV = addPrazo.ATV;
                    getPrazo.DHC = addPrazo.DHC;
                    getPrazo.USR = addPrazo.USR;
                    insercao = false;
                }

                context.SaveChanges();
                return insercao ? 1 : 2;
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return 0;
            }
        }
    }
}