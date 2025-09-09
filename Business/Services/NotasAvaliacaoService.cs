using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class NotasAvaliacaoService
    {
        public List<AVALIACOESCOMPETENCIASNOTAS> ListaNotasCompetencias(bool enableAll = false)
        {
            DataModel context = new DataModel();
            if (enableAll)
                return context.AVALIACOESCOMPETENCIASNOTAS.ToList();
            else
                return context.AVALIACOESCOMPETENCIASNOTAS.Where(n => n.ATV == 1).ToList();
        }

        public List<AVALIACOESCOMPETENCIASNOTAS> ListaNotasCompetencias(bool indFeedback, bool enableAll = false)
        {
            DataModel context = new DataModel();
            if (enableAll)
                return context.AVALIACOESCOMPETENCIASNOTAS.Where(n => n.IndFeedback == indFeedback).ToList();
            else
                return context.AVALIACOESCOMPETENCIASNOTAS.Where(n => n.ATV == 1 && n.IndFeedback == indFeedback).ToList();

        }

        public List<AVALIACOESPERFORMANCESNOTAS> ListaNotasPerformances()
        {
            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCESNOTAS.Where(n => n.ATV == 1).ToList();
        }

        public List<AVALIACOESPERFORMANCESNOTAS> ListaNotasPerformances(bool indFeedback)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCESNOTAS.Where(n => n.ATV == 1 && n.IndFeedback == indFeedback).ToList();
        }

        public AVALIACOESCOMPETENCIASNOTAS ObterNotaCompetencia(int idNota)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIASNOTAS.FirstOrDefault(a => a.IdNota == idNota);
        }

        public AVALIACOESCOMPETENCIASNOTAS ObterNotaCompetencia(string descricao)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIASNOTAS.FirstOrDefault(a => a.CodigoNota.ToLower().Trim() == descricao.ToLower().Trim());
        }

        public AVALIACOESPERFORMANCESNOTAS ObterNotaPerformance(int idNota)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCESNOTAS.FirstOrDefault(a => a.IdNota == idNota);
        }

    }
}
