using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class CompetenciasService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CompetenciasService));
        public bool InserirCompetencia(COMPETENCIAS competencias)
        {
            try
            {
                DataModel context = new DataModel();
                context.COMPETENCIAS.Add(competencias);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        // Exemplo de modificação do método ObterCompetencia para incluir SUBCOMPETENCIAS
        public COMPETENCIAS ObterCompetencia(int idCompetencia)
        {
            using (DataModel context = new DataModel())
            {
                // Corrigindo o erro CS1660 ao usar o método Include corretamente.
                return context.COMPETENCIAS
                    .Include("SUBCOMPETENCIAS")
                    .FirstOrDefault(c => c.IdCompetencia == idCompetencia);
            }
        }

        public List<COMPETENCIAS> ObterListaCompetencias(int idEmpresa, int idCargo, int idNivel, string TipoAvaliacao, string Escopo, List<int> listaIdCompetencias)
        {
            DataModel context = new DataModel();
            var returnLista = new List<COMPETENCIAS>();
            if (TipoAvaliacao == "desempenho")
            {
                returnLista = context.COMPETENCIAS.Where(c => c.IdEmpresa == idEmpresa && c.IdCargo == idCargo && c.IdNivel == idNivel
                && c.TipoAvaliacao == TipoAvaliacao && c.Escopo == Escopo).OrderBy(o => o.IdEixo).ThenBy(o => o.IdSubCompetencia).ThenBy(o => o.IdDimensao).ToList();
            }
            else
            {
                returnLista = context.COMPETENCIAS.Where(c => c.IdEmpresa == idEmpresa
                && c.TipoAvaliacao == TipoAvaliacao && c.Escopo == Escopo).OrderBy(o => o.IdEixo).ThenBy(o => o.IdSubCompetencia).ThenBy(o => o.IdDimensao).ToList();
            }

            // ATUALIZAÇÃO - COMPETENCIAS INATIVADAS
            if (listaIdCompetencias != null)
            {
                returnLista = returnLista.Where(comp => listaIdCompetencias.Contains(comp.IdCompetencia)).ToList();
            }
            else
            {
                returnLista = returnLista.Where(comp => comp.ATV == 1).ToList();
            }

            return returnLista;
        }

        public COMPETENCIAS ObterCompetencia(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalheNivelAtual)
        {
            DataModel context = new DataModel();
            return context.COMPETENCIAS.FirstOrDefault(c => c.IdEmpresa == idEmpresa && 
            (c.IdCargo == idCargo && c.CompetenciaJRDetalhe == detalheNivelAtual) && c.IdNivel == idNivel && c.IdEixo == idEixo &&
            c.IdSubCompetencia == idSubCompetencia && c.IdDimensao == idDimensao);
        }

        public COMPETENCIAS ObterCompetenciaCargoSub(int idCargo, int idSubCompetencia, string PalavrasChave)
        {
            DataModel context = new DataModel();

            var returnCompetencia = context.COMPETENCIAS.FirstOrDefault(c => c.IdCargo == idCargo && c.IdSubCompetencia == idSubCompetencia && c.PalavrasChave.ToLower() == PalavrasChave.ToLower() && c.ATV == 1);
            if (returnCompetencia == null)
            {
                returnCompetencia = context.COMPETENCIAS.FirstOrDefault(c => c.IdCargo == idCargo && c.IdSubCompetencia == idSubCompetencia && c.PalavrasChave == PalavrasChave && c.ATV == 1);

                if (idCargo == 63)
                {

                }
            }
            if (returnCompetencia == null)
            {
                //returnCompetencia = context.COMPETENCIAS.FirstOrDefault(c => c.IdCargo == idCargo && c.IdSubCompetencia == idSubCompetencia && c.ATV == 1);
            }
            return returnCompetencia;
        }

        public List<COMPETENCIAS> ObterCompetenciasDeCargoENivel(int idEmpresa, int idCargo, int idNivel)
        {
            //xxxxx
            DataModel context = new DataModel();
            return context.COMPETENCIAS.Where(c => c.IdEmpresa == idEmpresa &&
            c.IdCargo == idCargo && c.IdNivel == idNivel && c.ATV == 1).ToList();
        }

        public List<COMPETENCIAS> ObterCompetenciasDeCargo(int idCargo)
        {
            //xxxxx
            DataModel context = new DataModel();
            return context.COMPETENCIAS.Where(c => c.IdCargo == idCargo && c.ATV == 1).ToList();
        }


        public List<COMPETENCIAS> ObterListaCompetencias()
        {
            DataModel context = new DataModel();
            return context.COMPETENCIAS.ToList();
        }


        public List<COMPETENCIAS> ObterListaCompetencias(bool ativos)
        {
            DataModel context = new DataModel();
            return context.COMPETENCIAS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.DIMENSOES.Dimensao).ToList();
        }

        public int ObterQtdeSubCompetencias(int idCompetencia)
        {
            DataModel context = new DataModel();

            var competenciaService = new CompetenciasService();
            var subcompetenciaService = new SubCompetenciasService();
            var competencia = competenciaService.ObterCompetencia(idCompetencia);
            var idEixo = competencia.IdEixo;
            var idCargo = competencia.IdCargo;
            var idNivel = competencia.IdNivel;
            var TipoAvaliacao = competencia.TipoAvaliacao;
            var Escopo = competencia.Escopo;
            var competenciasLista = competenciaService.ObterListaCompetencias(1, idCargo, idNivel, TipoAvaliacao, Escopo, null);
            var competenciasEixo = competenciasLista.Where(x => x.IdEixo == idEixo).ToList();
            var subcompetenciaCount = competenciasEixo.Select(x => x.IdSubCompetencia).Distinct().Count();

            //var result = context.PROC_QTDE_SUB_COMPETENCIAS(idCompetencia).FirstOrDefault();
            //return result.QtdeSubCompetencias.Value;
            
            return subcompetenciaCount;
        }



        public bool AlterarCompetencia(COMPETENCIAS competencia)
        {
            try
            {
                DataModel context = new DataModel();
                COMPETENCIAS item = new COMPETENCIAS();
                item = context.COMPETENCIAS.First(a => a.IdCompetencia == competencia.IdCompetencia);

                if (item != null)
                {

                    context.Entry(item).CurrentValues.SetValues(competencia);

                    //competenciaAtual.IdCargo = competencia.IdCargo;
                    //competenciaAtual.IdNivel = competencia.IdNivel;
                    //competenciaAtual.IdEixo = competencia.IdEixo;
                    //competenciaAtual.IdSubCompetencia = competencia.IdSubCompetencia;
                    //competenciaAtual.IdDimensao = competencia.IdDimensao;
                    //competenciaAtual.CompetenciaJR = competencia.CompetenciaJR;
                    //competenciaAtual.CompetenciaJRDetalhe = competencia.CompetenciaJRDetalhe;
                    //competenciaAtual.CompetenciaPL = competencia.CompetenciaPL;
                    //competenciaAtual.CompetenciaPLDetalhe = competencia.CompetenciaPLDetalhe;
                    //competenciaAtual.CompetenciaSR = competencia.CompetenciaSR;
                    //competenciaAtual.CompetenciaSRDetalhe = competencia.CompetenciaSRDetalhe;
                    //competenciaAtual.Escopo = competencia.Escopo;
                    //competenciaAtual.PalavrasChave = competencia.PalavrasChave;
                    //competenciaAtual.ATV = competencia.ATV;
                    //competenciaAtual.DHC = competencia.DHC;
                    //competenciaAtual.USR = competencia.USR;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }


        public bool ExcluirCompetencia(int idCompetencia)
        {
            try
            {
                DataModel context = new DataModel();
                COMPETENCIAS competenciaAtual = new COMPETENCIAS();
                competenciaAtual = context.COMPETENCIAS.First(a => a.IdCompetencia == idCompetencia);

                if (competenciaAtual != null)
                {
                    competenciaAtual.ATV = 0; 
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }



    }
}
