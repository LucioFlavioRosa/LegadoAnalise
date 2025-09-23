using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Complexidades.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Complexidades;

public class ComplexidadesService : IComplexidadeService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public ComplexidadesService(ApplicationDbContext context, ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<ComplexidadeDto>> ObterListaComplexidadesAsync()
    {
        try
        {
            var complexidades = await _context.Set<ProjetoComplexidade>()
                .Where(c => c.ATV == 1)
                .OrderBy(c => c.Complexidade)
                .ToListAsync();

            return complexidades.Select(c => new ComplexidadeDto
            {
                IdComplexidade = c.IdComplexidade,
                Complexidade = c.Complexidade ?? string.Empty,
                Codigo = c.Codigo ?? string.Empty,
                ATV = c.ATV,
                Peso = c.Peso,
                PesoPonderado = c.PesoPonderado,
                Ponderacao = c.Ponderacao,
                FaixaInicial = c.FaixaInicial,
                FaixaFinal = c.FaixaFinal,
                SomaMinimaFator = c.SomaMinimaFator ?? 0,
                DHC = c.DHC,
                USR = c.USR ?? string.Empty
            }).ToList();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao obter lista de complexidades");
            throw;
        }
    }

    public async Task<ComplexidadeDto?> ObterComplexidadeAsync(int id)
    {
        try
        {
            var complexidade = await _context.Set<ProjetoComplexidade>()
                .FirstOrDefaultAsync(c => c.IdComplexidade == id);

            if (complexidade == null)
                return null;

            return new ComplexidadeDto
            {
                IdComplexidade = complexidade.IdComplexidade,
                Complexidade = complexidade.Complexidade ?? string.Empty,
                Codigo = complexidade.Codigo ?? string.Empty,
                ATV = complexidade.ATV,
                Peso = complexidade.Peso,
                PesoPonderado = complexidade.PesoPonderado,
                Ponderacao = complexidade.Ponderacao,
                FaixaInicial = complexidade.FaixaInicial,
                FaixaFinal = complexidade.FaixaFinal,
                SomaMinimaFator = complexidade.SomaMinimaFator ?? 0,
                DHC = complexidade.DHC,
                USR = complexidade.USR ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, $"Erro ao obter complexidade {id}");
            throw;
        }
    }

    public async Task<bool> InserirComplexidadeAsync(ComplexidadeDto dto)
    {
        try
        {
            if (!await ValidarComplexidadeAsync(dto))
                return false;

            var complexidade = new ProjetoComplexidade
            {
                Complexidade = dto.Complexidade,
                Codigo = dto.Codigo,
                ATV = dto.ATV,
                Peso = dto.Peso,
                PesoPonderado = dto.PesoPonderado,
                Ponderacao = dto.Ponderacao,
                FaixaInicial = dto.FaixaInicial,
                FaixaFinal = dto.FaixaFinal,
                DHC = DateTime.Now,
                USR = "1"
            };

            _context.Set<ProjetoComplexidade>().Add(complexidade);
            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("ComplexidadeInserida", new Dictionary<string, string>
            {
                { "Complexidade", dto.Complexidade },
                { "Usuario", dto.USR }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, "Erro ao inserir complexidade");
            return false;
        }
    }

    public async Task<bool> AlterarComplexidadeAsync(ComplexidadeDto dto)
    {
        try
        {
            if (!await ValidarComplexidadeAsync(dto))
                return false;

            var complexidade = await _context.Set<ProjetoComplexidade>()
                .FirstOrDefaultAsync(c => c.IdComplexidade == dto.IdComplexidade);

            if (complexidade == null)
                return false;

            complexidade.Complexidade = dto.Complexidade;
            complexidade.Codigo = dto.Codigo;
            complexidade.ATV = dto.ATV;
            complexidade.Peso = dto.Peso;
            complexidade.PesoPonderado = dto.PesoPonderado;
            complexidade.Ponderacao = dto.Ponderacao;
            complexidade.FaixaInicial = dto.FaixaInicial;
            complexidade.FaixaFinal = dto.FaixaFinal;
            complexidade.DHC = DateTime.Now;
            complexidade.USR = "1";

            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("ComplexidadeAlterada", new Dictionary<string, string>
            {
                { "Id", dto.IdComplexidade.ToString() },
                { "Complexidade", dto.Complexidade }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, $"Erro ao alterar complexidade {dto.IdComplexidade}");
            return false;
        }
    }

    public async Task<bool> ExcluirComplexidadeAsync(int id)
    {
        try
        {
            var complexidade = await _context.Set<ProjetoComplexidade>()
                .FirstOrDefaultAsync(c => c.IdComplexidade == id);

            if (complexidade == null)
                return false;

            complexidade.ATV = 0;
            complexidade.DHC = DateTime.Now;
            complexidade.USR = "1";

            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("ComplexidadeInativada", new Dictionary<string, string>
            {
                { "Id", id.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, $"Erro ao inativar complexidade {id}");
            return false;
        }
    }

    public async Task<bool> ValidarComplexidadeAsync(ComplexidadeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Complexidade))
            return false;

        if (dto.Peso <= 0 || dto.PesoPonderado <= 0)
            return false;

        if (dto.Ponderacao <= 0 || dto.FaixaInicial < 0 || dto.FaixaFinal <= 0)
            return false;

        if (dto.FaixaInicial >= dto.FaixaFinal)
            return false;

        var complexidadeExistente = await _context.Set<ProjetoComplexidade>()
            .FirstOrDefaultAsync(c => c.Complexidade == dto.Complexidade && c.IdComplexidade != dto.IdComplexidade && c.ATV == 1);

        return complexidadeExistente == null;
    }
}