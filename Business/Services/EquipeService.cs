using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class EquipeService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EquipeService));
        public List<EQUIPE> ObterEquipeTodasAtivas()
        {
            DataModel context = new DataModel();
            return context.EQUIPE.Where(eq => eq.ATV == true).ToList();
        }
        public List<EQUIPE> ObterEquipePorProjeto(int idProjeto)
        {
            DataModel context = new DataModel();
            return context.EQUIPE.Where(eq => eq.ATV == true && eq.idProjeto == idProjeto).ToList();
        }
        public EQUIPE ObterEquipePorProjetoAssociado(int idProjeto, int idAssociado)
        {
            DataModel context = new DataModel();
            return context.EQUIPE.FirstOrDefault(eq => eq.ATV == true && eq.idProjeto == idProjeto && eq.idAssociado == idAssociado);
        }
        public void AdicionarEquipe(EQUIPE equipe)
        {
            try
            {
                DataModel context = new DataModel();
                context.EQUIPE.Add(equipe);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }
        public void AtualizaEquipeProjeto(int idProjeto, Dictionary<int, Dictionary<string, object>> equipe)
        {
            try
            {
                DataModel context = new DataModel();
                List<EQUIPE> listaEquipeAtual = context.EQUIPE.Where(e => e.idProjeto == idProjeto).ToList();
                foreach (var equipeAtual in listaEquipeAtual)
                {
                    context.EQUIPE.RemoveRange(listaEquipeAtual);
                }
                foreach (var slotAssociado in equipe)
                {
                    EQUIPE addEquipe = new EQUIPE();
                    addEquipe.idProjeto = idProjeto;
                    addEquipe.idAssociado = (int)equipe[slotAssociado.Key]["idAssociado"];
                    addEquipe.RespondeAvaliacaoDesempenho = (bool)equipe[slotAssociado.Key]["respondeDesempenho"];
                    addEquipe.RespondeAvaliacaoLideranca = (bool)equipe[slotAssociado.Key]["respondeLideranca"];
                    addEquipe.DHC = DateTime.Now;
                    addEquipe.ATV = true;

                    var checkIdAssociadoHierarquia = equipe[slotAssociado.Key]["associadoHierarquia"];
                    if (checkIdAssociadoHierarquia != null)
                    {
                        addEquipe.idAssociadoHierarquia = int.Parse(checkIdAssociadoHierarquia.ToString());
                    }

                    context.EQUIPE.Add(addEquipe);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }
        public void ResetEquipeProjeto(int idProjeto)
        {
            try
            {
                DataModel context = new DataModel();
                var projetosAssociadosService = new ProjetosService();

                List<EQUIPE> listaEquipeAtual = context.EQUIPE.Where(e => e.idProjeto == idProjeto).ToList();
                var projetosAssociadosProjeto = projetosAssociadosService.ObterListaAssociados(idProjeto);

                context.EQUIPE.RemoveRange(listaEquipeAtual);

                if (projetosAssociadosProjeto.Count > 0)
                {
                    foreach (var projetoAssociado in projetosAssociadosProjeto)
                    {
                        var existeEquipe =
                            projetoAssociado.TipoAvaliacao == "desempenho" ? 
                            ObterEquipePorProjetoAssociado(idProjeto, projetoAssociado.IdAssociado) :
                            ObterEquipePorProjetoAssociado(idProjeto, (int)projetoAssociado.IdGestor);
                        if (existeEquipe == null)
                        {
                            EQUIPE addEquipe = new EQUIPE();
                            addEquipe.idProjeto = idProjeto;
                            addEquipe.idAssociado = projetoAssociado.TipoAvaliacao == "desempenho" ? projetoAssociado.IdAssociado : (int)projetoAssociado.IdGestor;
                            addEquipe.idAssociadoHierarquia = projetoAssociado.TipoAvaliacao == "desempenho" ? projetoAssociado.IdAvaliador : projetoAssociado.IdAssociado;
                            addEquipe.DHC = projetoAssociado.DHC;
                            addEquipe.ATV = true;
                            addEquipe.RespondeAvaliacaoDesempenho = projetoAssociado.TipoAvaliacao == "desempenho" ? true : false;
                            addEquipe.RespondeAvaliacaoLideranca = projetoAssociado.TipoAvaliacao == "lideranca" ? true : false;
                            context.EQUIPE.Add(addEquipe);
                            context.SaveChanges();
                        }
                        else
                        {
                            existeEquipe.RespondeAvaliacaoDesempenho = projetoAssociado.TipoAvaliacao == "desempenho" ? true : existeEquipe.RespondeAvaliacaoDesempenho;
                            existeEquipe.RespondeAvaliacaoLideranca = projetoAssociado.TipoAvaliacao == "lideranca" ? true : existeEquipe.RespondeAvaliacaoLideranca;
                            existeEquipe.idAssociadoHierarquia = projetoAssociado.IdAvaliador;
                            existeEquipe.DHC = projetoAssociado.DHC;
                        }

                        var existeEquipeHierarquia = 
                            projetoAssociado.TipoAvaliacao == "desempenho" ? 
                            ObterEquipePorProjetoAssociado(idProjeto, (int)projetoAssociado.IdAvaliador) :
                            ObterEquipePorProjetoAssociado(idProjeto, (int)projetoAssociado.IdAssociado);
                        if (existeEquipeHierarquia == null)
                        {
                            EQUIPE addEquipe = new EQUIPE();
                            addEquipe.idProjeto = idProjeto;
                            addEquipe.idAssociado = (int)(projetoAssociado.TipoAvaliacao == "desempenho" ? projetoAssociado.IdAvaliador : projetoAssociado.IdAssociado);
                            addEquipe.DHC = projetoAssociado.DHC;
                            addEquipe.ATV = true;
                            addEquipe.RespondeAvaliacaoDesempenho = projetoAssociado.TipoAvaliacao == "desempenho" ? true : false;
                            addEquipe.RespondeAvaliacaoLideranca = projetoAssociado.TipoAvaliacao == "lideranca" ? true : false;
                            context.EQUIPE.Add(addEquipe);
                            context.SaveChanges();
                        }
                        else
                        {
                            existeEquipeHierarquia.RespondeAvaliacaoDesempenho = projetoAssociado.TipoAvaliacao == "desempenho" ? true : existeEquipeHierarquia.RespondeAvaliacaoDesempenho;
                            existeEquipeHierarquia.RespondeAvaliacaoLideranca = projetoAssociado.TipoAvaliacao == "lideranca" ? true : existeEquipeHierarquia.RespondeAvaliacaoLideranca;
                            existeEquipeHierarquia.DHC = projetoAssociado.DHC;
                        }
                    }
                }
                else
                {
                    for (int i = -1; i >= -3; i--)
                    {
                        EQUIPE addEquipe = new EQUIPE();
                        addEquipe.idProjeto = idProjeto;
                        addEquipe.idAssociado = i;
                        addEquipe.RespondeAvaliacaoDesempenho = true;
                        addEquipe.RespondeAvaliacaoLideranca = true;
                        addEquipe.DHC = DateTime.Now;
                        addEquipe.ATV = true;

                        if (i < -1) { addEquipe.idAssociadoHierarquia = i + 1; }

                        context.EQUIPE.Add(addEquipe);
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }
    }
}
