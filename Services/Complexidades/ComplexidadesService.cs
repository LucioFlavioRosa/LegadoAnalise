using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Complexidades.Common;

namespace Peers.Moderno.Services.Complexidades;

public class ComplexidadesService : IComplexidadeService
{
    private readonly ApplicationDbContext _context;

    public ComplexidadesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComplexidadeDto>> ObterListaComplexidadesAsync()
    {
        var complexidades = await _context.Set<ProjetoComplexidade>()
            .AsNoTracking()
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
            SomaMinimaFator = c.SomaMinimaFator,
            DHC = c.DHC,
            USR = c.USR ?? string.Empty
        }).ToList();
    }

    public async Task<ComplexidadeDto?> ObterComplexidadeAsync(int id)
    {
        var complexidade = await _context.Set<ProjetoComplexidade>()
            .AsNoTracking()
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
            SomaMinimaFator = complexidade.SomaMinimaFator,
            DHC = complexidade.DHC,
            USR = complexidade.USR ?? string.Empty
        };
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
                SomaMinimaFator = dto.SomaMinimaFator,
                DHC = DateTime.Now,
                USR = "1"
            };

            _context.Set<ProjetoComplexidade>().Add(complexidade);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
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
            complexidade.SomaMinimaFator = dto.SomaMinimaFator;
            complexidade.DHC = DateTime.Now;
            complexidade.USR = "1";

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
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
            return true;
        }
        catch
        {
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

        var existeComplexidade = await _context.Set<ProjetoComplexidade>()
            .AnyAsync(c => c.Complexidade == dto.Complexidade && c.IdComplexidade != dto.IdComplexidade);

        return !existeComplexidade;
    }
}