using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.FeedbackPerformance;

namespace Services.FeedbackPerformance.Common
{
    public class FeedbackPerformanceHelper
    {
        public string CalculaTempoRestante(string dataFinal, int compensadorFeedback)
        {
            if (string.IsNullOrWhiteSpace(dataFinal))
                return string.Empty;
            var dataFinalDate = DateTime.ParseExact(dataFinal, "dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"));
            if (DateTime.Today >= dataFinalDate)
                return DateTime.Today.AddDays(compensadorFeedback).ToString("dd/MM/yyyy");
            return dataFinal;
        }

        public async Task<List<Performance>> ObterListaPerformancesAsync(ApplicationDbContext db, int idEmpresa, int idCargo, int idNivel, List<int>? listaIdPerformances)
        {
            var query = db.Performances.AsQueryable();
            query = query.Where(p => p.IdEmpresa == idEmpresa && p.IdCargo == idCargo && p.IdNivel == idNivel);
            if (listaIdPerformances != null && listaIdPerformances.Count > 0)
                query = query.Where(p => listaIdPerformances.Contains(p.IdPerformance));
            return await query.ToListAsync();
        }

        public List<FeedbackPerformanceService.PerformanceModel> OrganizarPerformances(List<Performance> performances)
        {
            var listaPerformancesModel = new List<FeedbackPerformanceService.PerformanceModel>();
            var abrangenciasContadas = new List<string>();
            string setAbrangencia = string.Empty;
            bool novaAbrangencia = true, addItem = false, headerSeparador = false;
            while (novaAbrangencia)
            {
                novaAbrangencia = false;
                setAbrangencia = string.Empty;
                foreach (var item in performances)
                {
                    addItem = false;
                    if (!string.IsNullOrEmpty(setAbrangencia))
                    {
                        if (item.Abrangencia == setAbrangencia)
                        {
                            addItem = true;
                            headerSeparador = false;
                        }
                    }
                    else if (!abrangenciasContadas.Contains(item.Abrangencia))
                    {
                        abrangenciasContadas.Add(item.Abrangencia);
                        setAbrangencia = item.Abrangencia;
                        novaAbrangencia = true;
                        headerSeparador = true;
                        addItem = true;
                    }
                    if (addItem)
                    {
                        var linhaPerformance = new FeedbackPerformanceService.PerformanceModel
                        {
                            IdPerformance = item.IdPerformance,
                            Descricao = item.Descricao,
                            Abaixo = item.PerformanceAbaixo,
                            Esperado = item.PerformanceEsperado,
                            Acima = item.PerformanceAcima,
                            Abrangencia = item.Abrangencia?.ToUpper() ?? string.Empty,
                            SeparadorAbrangencia = headerSeparador ? string.Empty : "hidden",
                            Input = item.InputAvaliacaoGestor ? string.Empty : string.Empty,
                            DisclaimerInput = item.InputAvaliacaoGestor ? string.Empty : "Esta nota não requer preenchimento em feedback"
                        };
                        listaPerformancesModel.Add(linhaPerformance);
                    }
                }
            }
            return listaPerformancesModel;
        }

        public string TruncaTexto(string texto, int qtdCaracter)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;
            if (texto.Length > qtdCaracter)
                return texto.Substring(0, qtdCaracter) + "...";
            return texto;
        }
    }

    // Mock de entidade Performance (substitua pela real do projeto)
    public class Performance
    {
        public int IdPerformance { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? PerformanceAbaixo { get; set; }
        public string? PerformanceEsperado { get; set; }
        public string? PerformanceAcima { get; set; }
        public string? Abrangencia { get; set; }
        public bool InputAvaliacaoGestor { get; set; }
    }
}
