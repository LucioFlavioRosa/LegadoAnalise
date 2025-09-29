using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Avaliacoes.Common;

public class EvolucaoAssociadoViewService : IEvolucaoAssociadoViewService
{
    private readonly ApplicationDbContext _db;

    public EvolucaoAssociadoViewService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<EvolucaoAssociadoViewModel> ObterEvolucaoAssociadoAsync(int associadoId, string tipoAvaliacao, string escopo)
    {
        var viewModel = new EvolucaoAssociadoViewModel();
        try
        {
            viewModel.AssociadoInfo = await ObterInfoAssociadoAsync(associadoId);
            var projetos = await _db.Avaliacoes
                .Where(a => a.IdAssociado == associadoId && a.TipoAvaliacao == tipoAvaliacao && a.Escopo == escopo)
                .Include(a => a.Periodo)
                .Include(a => a.Cargo)
                .Select(a => new EvolucaoAssociadoProjeto
                {
                    Cargo = a.Cargo.Nome,
                    Periodo = a.Periodo.Nome,
                    IdPeriodo = a.IdPeriodo,
                    TipoAvaliacao = a.TipoAvaliacao,
                    NotaCompetencia = a.NotaCompetencia ?? 0,
                    NotaPerformance = a.NotaPerformance,
                    RatingPerformance = a.RatingPerformance ?? string.Empty
                })
                .ToListAsync();
            viewModel.Projetos = projetos;
            viewModel.HasData = projetos.Any();
            if (viewModel.HasData)
            {
                var idCargo = await _db.Associados
                    .Where(a => a.Id == associadoId)
                    .Select(a => a.IdCargo)
                    .FirstOrDefaultAsync();
                viewModel.JsonRadar = await MontarJsonRadarAsync(projetos, idCargo);
            }
        }
        catch (Exception ex)
        {
            viewModel.ErrorMessage = $"Erro ao obter evolução: {ex.Message}";
        }
        return viewModel;
    }

    public async Task<string> MontarJsonRadarAsync(List<EvolucaoAssociadoProjeto> projetos, int idCargo)
    {
        var radarData = new
        {
            labels = projetos.Select(p => p.Periodo).ToArray(),
            datasets = new[]
            {
                new
                {
                    label = "Competência",
                    data = projetos.Select(p => p.NotaCompetencia).ToArray()
                },
                new
                {
                    label = "Performance",
                    data = projetos.Select(p => p.NotaPerformance ?? 0).ToArray()
                }
            }
        };
        return await Task.FromResult(JsonSerializer.Serialize(radarData));
    }

    public async Task<AssociadoInfoViewModel> ObterInfoAssociadoAsync(int associadoId)
    {
        var associado = await _db.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Mentor)
            .FirstOrDefaultAsync(a => a.Id == associadoId);
        if (associado == null)
        {
            return new AssociadoInfoViewModel();
        }
        return new AssociadoInfoViewModel
        {
            Nome = associado.Nome,
            Mentor = associado.Mentor?.Nome ?? "Sem mentor",
            Cargo = associado.Cargo?.Nome ?? "Sem cargo",
            ProximoCargo = associado.Cargo?.ProximoCargo ?? "N/A"
        };
    }
}
