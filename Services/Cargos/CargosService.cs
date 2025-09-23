using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Cargos;

public interface ICargosService
{
    Task<List<Cargo>> ObterListaCargosAsync(bool apenasAtivos = false);
    Task<Cargo?> ObterCargoAsync(int id);
    Task<bool> InserirCargoAsync(Cargo cargo);
    Task<bool> AlterarCargoAsync(Cargo cargo);
    Task<bool> InativarCargoAsync(int id);
    Task<bool> ExisteCargoComNomeAsync(string nome, int? idExcluir = null);
}

public class CargosService : ICargosService
{
    private readonly ApplicationDbContext _context;

    public CargosService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cargo>> ObterListaCargosAsync(bool apenasAtivos = false)
    {
        var query = _context.Cargos
            .Include(c => c.ProximoCargo)
            .AsQueryable();

        if (apenasAtivos)
        {
            query = query.Where(c => c.ATV == 1);
        }

        return await query
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cargo?> ObterCargoAsync(int id)
    {
        return await _context.Cargos
            .Include(c => c.ProximoCargo)
            .FirstOrDefaultAsync(c => c.IdCargo == id);
    }

    public async Task<bool> InserirCargoAsync(Cargo cargo)
    {
        try
        {
            if (await ExisteCargoComNomeAsync(cargo.Nome))
                return false;

            cargo.DHC = DateTime.Now;
            _context.Cargos.Add(cargo);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AlterarCargoAsync(Cargo cargo)
    {
        try
        {
            if (await ExisteCargoComNomeAsync(cargo.Nome, cargo.IdCargo))
                return false;

            var cargoExistente = await _context.Cargos.FindAsync(cargo.IdCargo);
            if (cargoExistente == null)
                return false;

            cargoExistente.Nome = cargo.Nome;
            cargoExistente.IdProximoCargo = cargo.IdProximoCargo;
            cargoExistente.TempoMinimoPromocao = cargo.TempoMinimoPromocao;
            cargoExistente.Funcao = cargo.Funcao;
            cargoExistente.Autonomia = cargo.Autonomia;
            cargoExistente.EscopoDeAtuacao = cargo.EscopoDeAtuacao;
            cargoExistente.NivelInterlocucao = cargo.NivelInterlocucao;
            cargoExistente.ATV = cargo.ATV;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> InativarCargoAsync(int id)
    {
        try
        {
            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo == null)
                return false;

            cargo.ATV = 0;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExisteCargoComNomeAsync(string nome, int? idExcluir = null)
    {
        var query = _context.Cargos.Where(c => c.Nome.ToLower() == nome.ToLower());
        
        if (idExcluir.HasValue)
        {
            query = query.Where(c => c.IdCargo != idExcluir.Value);
        }

        return await query.AnyAsync();
    }
}