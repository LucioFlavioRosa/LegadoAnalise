using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Performance.Common;

namespace Services.Performance
{
    public class PerformanceService : IPerformanceService
    {
        private readonly ApplicationDbContext _dbContext;

        public PerformanceService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PerformanceModel>> GetMentorPerformanceAsync(int idAssociado, int idProjeto, int idPeriodo)
        {
            var avaliacaoService = _dbContext.AvaliacoesPerformance
                .Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo)
                .AsNoTracking();

            var listaPerformances = await avaliacaoService.ToListAsync();
            var listaIdPerformances = listaPerformances.Select(perf => perf.IdPerformance).ToList();

            var associado = await _dbContext.Associados.FirstOrDefaultAsync(a => a.Id == idAssociado);
            var projeto = await _dbContext.Projetos.FirstOrDefaultAsync(p => p.Id == idProjeto);
            var periodo = await _dbContext.PERIODOSAVALIACOES.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);

            var cargoNaAvaliacao = listaPerformances.FirstOrDefault()?.IdCargo ?? associado?.IdCargo ?? 0;
            var nivelNaAvaliacao = listaPerformances.FirstOrDefault()?.IdNivel ?? 0;

            var performances = await GetPerformanceListAsync(associado?.IdEmpresa ?? 0, cargoNaAvaliacao, nivelNaAvaliacao, listaIdPerformances);
            var organized = OrganizeByAbrangencia(performances);
            return organized;
        }

        public async Task<List<PerformanceModel>> GetPerformanceListAsync(int idEmpresa, int idCargo, int idNivel, List<int> listaIdPerformances)
        {
            var query = _dbContext.Performances
                .Where(p => p.Cargo.IdEmpresa == idEmpresa && p.IdCargo == idCargo && p.IdNivel == idNivel && listaIdPerformances.Contains(p.IdPerformance))
                .AsNoTracking();

            var performances = await query.ToListAsync();
            var result = new List<PerformanceModel>();
            foreach (var item in performances)
            {
                var perfModel = new PerformanceModel
                {
                    IdPerformance = item.IdPerformance,
                    Descricao = item.Descricao,
                    Abaixo = item.PerformanceAbaixo,
                    Esperado = item.PerformanceEsperado,
                    Acima = item.PerformanceAcima,
                    Abrangencia = item.Abrangencia != null ? item.Abrangencia.ToUpper() : string.Empty,
                    SeparadorAbrangencia = ""
                };
                result.Add(perfModel);
            }
            return result;
        }

        public string TruncateText(string texto, int maxLength)
        {
            return PerformanceHelpers.TruncateText(texto, maxLength);
        }

        public List<PerformanceModel> OrganizeByAbrangencia(List<PerformanceModel> performances)
        {
            return PerformanceHelpers.OrganizeByAbrangencia(performances);
        }
    }
}
