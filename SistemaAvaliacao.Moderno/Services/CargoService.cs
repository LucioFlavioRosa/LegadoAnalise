using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Models;
using SistemaAvaliacao.Moderno.Data;
using Microsoft.Extensions.Caching.Memory;

namespace SistemaAvaliacao.Moderno.Services
{
    public class CargoService : ICargoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private static readonly string CargosAtivosCacheKey = "CargosAtivosCacheKey";
        private static readonly TimeSpan CargosAtivosCacheDuration = TimeSpan.FromMinutes(5);

        public CargoService(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<List<Cargo>> ObterTodosAsync()
        {
            return await _context.Cargos.Include(c => c.ProximoCargo).ToListAsync();
        }

        public async Task<Cargo?> ObterPorIdAsync(int id)
        {
            return await _context.Cargos.Include(c => c.ProximoCargo).FirstOrDefaultAsync(c => c.IdCargo == id);
        }

        public async Task<bool> CriarAsync(Cargo cargo)
        {
            _context.Cargos.Add(cargo);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                _memoryCache.Remove(CargosAtivosCacheKey);
            }
            return result;
        }

        public async Task<bool> AtualizarAsync(Cargo cargo)
        {
            _context.Cargos.Update(cargo);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                _memoryCache.Remove(CargosAtivosCacheKey);
            }
            return result;
        }

        public async Task<bool> InativarAsync(int id)
        {
            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo == null) return false;
            cargo.Ativo = false;
            _context.Cargos.Update(cargo);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                _memoryCache.Remove(CargosAtivosCacheKey);
            }
            return result;
        }

        public async Task<List<Cargo>> ObterCargosAtivosAsync()
        {
            return await _context.Cargos
                .Include(c => c.ProximoCargo)
                .Where(c => c.Ativo)
                .ToListAsync();
        }

        // Passo 29: Método com cache para lista de cargos ativos
        public async Task<List<Cargo>> ObterCargosAtivosComCacheAsync()
        {
            if (!_memoryCache.TryGetValue(CargosAtivosCacheKey, out List<Cargo> cargosAtivos))
            {
                cargosAtivos = await _context.Cargos
                    .Include(c => c.ProximoCargo)
                    .Where(c => c.Ativo)
                    .ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(CargosAtivosCacheDuration);
                _memoryCache.Set(CargosAtivosCacheKey, cargosAtivos, cacheEntryOptions);
            }
            return cargosAtivos;
        }
    }
}
