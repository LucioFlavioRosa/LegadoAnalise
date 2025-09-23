using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Cargos.Common;

public class CargoLookupService : ICargoLookupService
{
    private readonly ApplicationDbContext _context;

    public CargoLookupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cargo?> BuscarPorCodigoAsync(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            return null;

        if (!int.TryParse(codigo, out int idCargo))
            return null;

        return await _context.Cargos
            .FirstOrDefaultAsync(c => c.IdCargo == idCargo);
    }
}