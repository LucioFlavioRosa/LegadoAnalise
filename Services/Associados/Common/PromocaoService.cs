using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Associados.Common;

public interface IPromocaoService
{
    Task<List<PromocaoHistorico>> ObterHistoricoPromocoes(int associadoId);
    Task<bool> AdicionarPromocaoAsync(int associadoId, int cargoAnteriorId, int cargoNovoId, string comentarios = "Promoção");
    Task<bool> AdicionarPromocaoInicialAsync(int associadoId, int cargoId);
    Task<bool> AlterarComentarioPromocaoAsync(int promocaoId, string comentarios);
}

public class PromocaoService : IPromocaoService
{
    private readonly ApplicationDbContext _context;

    public PromocaoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PromocaoHistorico>> ObterHistoricoPromocoes(int associadoId)
    {
        return await _context.Promocoes
            .Include(p => p.CargoAnterior)
            .Include(p => p.CargoNovo)
            .Where(p => p.IdAssociado == associadoId && p.Ativo)
            .Select(p => new PromocaoHistorico
            {
                Id = p.Id,
                DataPromocao = p.DataPromocao,
                CargoAnterior = p.CargoAnterior != null ? p.CargoAnterior.Nome : string.Empty,
                CargoNovo = p.CargoNovo != null ? p.CargoNovo.Nome : string.Empty,
                Comentarios = p.Comentarios
            })
            .OrderByDescending(p => p.DataPromocao)
            .ToListAsync();
    }

    public async Task<bool> AdicionarPromocaoAsync(int associadoId, int cargoAnteriorId, int cargoNovoId, string comentarios = "Promoção")
    {
        try
        {
            var promocao = new Promocao
            {
                IdAssociado = associadoId,
                IdCargoAnterior = cargoAnteriorId,
                IdCargoNovo = cargoNovoId,
                DataPromocao = DateTime.Now,
                Comentarios = comentarios,
                Ativo = true,
                DataCriacao = DateTime.Now
            };

            _context.Promocoes.Add(promocao);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AdicionarPromocaoInicialAsync(int associadoId, int cargoId)
    {
        return await AdicionarPromocaoAsync(associadoId, cargoId, cargoId, "Contratação");
    }

    public async Task<bool> AlterarComentarioPromocaoAsync(int promocaoId, string comentarios)
    {
        try
        {
            var promocao = await _context.Promocoes.FindAsync(promocaoId);
            if (promocao == null)
                return false;

            promocao.Comentarios = comentarios;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}