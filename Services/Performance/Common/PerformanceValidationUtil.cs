using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Performance.Common;

public interface IPerformanceValidationUtil
{
    Task<PerformanceValidationResult> ValidatePerformanceAsync(Models.Performance performance);
    Task<PerformanceValidationResult> ValidatePerformanceFieldsAsync(Models.Performance performance);
    Task<PerformanceValidationResult> ValidatePerformanceBusinessRulesAsync(Models.Performance performance);
    Task<bool> ValidateCargoExistsAsync(int idCargo);
    Task<bool> ValidateNotaExistsAsync(int? idNota);
}

public class PerformanceValidationUtil : IPerformanceValidationUtil
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public PerformanceValidationUtil(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<PerformanceValidationResult> ValidatePerformanceAsync(Models.Performance performance)
    {
        try
        {
            var result = new PerformanceValidationResult { IsValid = true };

            var fieldValidation = await ValidatePerformanceFieldsAsync(performance);
            if (!fieldValidation.IsValid)
            {
                result.IsValid = false;
                result.Errors.AddRange(fieldValidation.Errors);
            }

            var businessValidation = await ValidatePerformanceBusinessRulesAsync(performance);
            if (!businessValidation.IsValid)
            {
                result.IsValid = false;
                result.Errors.AddRange(businessValidation.Errors);
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidatePerformanceAsync" },
                { "PerformanceId", performance.IdPerformance.ToString() }
            });
            
            return new PerformanceValidationResult
            {
                IsValid = false,
                Errors = new List<string> { "Erro interno na validação" }
            };
        }
    }

    public async Task<PerformanceValidationResult> ValidatePerformanceFieldsAsync(Models.Performance performance)
    {
        var result = new PerformanceValidationResult { IsValid = true };

        if (performance.IdCargo <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Selecione o campo Cargo");
        }

        if (string.IsNullOrWhiteSpace(performance.Nome))
        {
            result.IsValid = false;
            result.Errors.Add("Preencha o campo Performance");
        }

        if (string.IsNullOrWhiteSpace(performance.PerformanceAbaixo))
        {
            result.IsValid = false;
            result.Errors.Add("Preencha o campo Performance Abaixo");
        }

        if (string.IsNullOrWhiteSpace(performance.PerformanceEsperado))
        {
            result.IsValid = false;
            result.Errors.Add("Preencha o campo Performance Esperado");
        }

        if (string.IsNullOrWhiteSpace(performance.PerformanceAcima))
        {
            result.IsValid = false;
            result.Errors.Add("Preencha o campo Performance Acima");
        }

        if (string.IsNullOrWhiteSpace(performance.Abrangencia))
        {
            result.IsValid = false;
            result.Errors.Add("Selecione o campo Abrangência");
        }

        if (!performance.InputAutoavaliacao && !performance.NotaPadraoAutoAvaliacao.HasValue)
        {
            result.IsValid = false;
            result.Errors.Add("Selecione a nota padrão para auto avaliação");
        }

        if (!performance.InputAvaliacaoAsCegas && !performance.NotaPadraoAvaliacaoAsCegas.HasValue)
        {
            result.IsValid = false;
            result.Errors.Add("Selecione a nota padrão para avaliação às cegas");
        }

        if (!performance.InputAvaliacaoGestor && !performance.NotaPadraoAvaliacaoGestor.HasValue)
        {
            result.IsValid = false;
            result.Errors.Add("Selecione a nota padrão para avaliação do gestor");
        }

        return result;
    }

    public async Task<PerformanceValidationResult> ValidatePerformanceBusinessRulesAsync(Models.Performance performance)
    {
        var result = new PerformanceValidationResult { IsValid = true };

        var cargoExists = await ValidateCargoExistsAsync(performance.IdCargo);
        if (!cargoExists)
        {
            result.IsValid = false;
            result.Errors.Add("Cargo selecionado não existe");
        }

        if (performance.NotaPadraoAutoAvaliacao.HasValue)
        {
            var notaAutoAvaliacaoExists = await ValidateNotaExistsAsync(performance.NotaPadraoAutoAvaliacao);
            if (!notaAutoAvaliacaoExists)
            {
                result.IsValid = false;
                result.Errors.Add("Nota padrão para auto avaliação não existe");
            }
        }

        if (performance.NotaPadraoAvaliacaoAsCegas.HasValue)
        {
            var notaAsCegasExists = await ValidateNotaExistsAsync(performance.NotaPadraoAvaliacaoAsCegas);
            if (!notaAsCegasExists)
            {
                result.IsValid = false;
                result.Errors.Add("Nota padrão para avaliação às cegas não existe");
            }
        }

        if (performance.NotaPadraoAvaliacaoGestor.HasValue)
        {
            var notaGestorExists = await ValidateNotaExistsAsync(performance.NotaPadraoAvaliacaoGestor);
            if (!notaGestorExists)
            {
                result.IsValid = false;
                result.Errors.Add("Nota padrão para avaliação do gestor não existe");
            }
        }

        if (performance.IdPerformance == 0)
        {
            var duplicateExists = await _context.Set<Models.Performance>()
                .AnyAsync(p => p.Nome.ToLower() == performance.Nome.ToLower() && 
                              p.IdCargo == performance.IdCargo && 
                              p.ATV == 1);
            
            if (duplicateExists)
            {
                result.IsValid = false;
                result.Errors.Add("Já existe uma performance ativa com este nome para o cargo selecionado");
            }
        }
        else
        {
            var duplicateExists = await _context.Set<Models.Performance>()
                .AnyAsync(p => p.Nome.ToLower() == performance.Nome.ToLower() && 
                              p.IdCargo == performance.IdCargo && 
                              p.IdPerformance != performance.IdPerformance && 
                              p.ATV == 1);
            
            if (duplicateExists)
            {
                result.IsValid = false;
                result.Errors.Add("Já existe uma performance ativa com este nome para o cargo selecionado");
            }
        }

        return result;
    }

    public async Task<bool> ValidateCargoExistsAsync(int idCargo)
    {
        try
        {
            return await _context.Cargos.AnyAsync(c => c.IdCargo == idCargo && c.Ativo);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidateCargoExistsAsync" },
                { "CargoId", idCargo.ToString() }
            });
            return false;
        }
    }

    public async Task<bool> ValidateNotaExistsAsync(int? idNota)
    {
        try
        {
            if (!idNota.HasValue)
                return true;

            return await _context.Set<AvaliacaoCompetenciaNota>()
                .AnyAsync(n => n.IdNota == idNota.Value);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidateNotaExistsAsync" },
                { "NotaId", idNota?.ToString() ?? "null" }
            });
            return false;
        }
    }
}