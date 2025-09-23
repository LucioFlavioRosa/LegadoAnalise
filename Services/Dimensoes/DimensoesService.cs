using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Dimensoes.Common;

namespace Peers.Moderno.Services.Dimensoes;

public class DimensoesService : IDimensoesService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public DimensoesService(ApplicationDbContext context, ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<Dimensao>> ListarAsync()
    {
        try
        {
            var startTime = DateTimeOffset.UtcNow;
            
            var dimensoes = await _context.Dimensoes
                .OrderBy(d => d.Nome)
                .ToListAsync();
            
            var duration = DateTimeOffset.UtcNow - startTime;
            _telemetryService.TrackDependency("Database", "ListarDimensoes", "SELECT", startTime, duration, true);
            
            return dimensoes;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ListarDimensoes" }
            });
            throw;
        }
    }

    public async Task<Dimensao?> ObterPorIdAsync(int id)
    {
        try
        {
            var startTime = DateTimeOffset.UtcNow;
            
            var dimensao = await _context.Dimensoes
                .FirstOrDefaultAsync(d => d.IdDimensao == id);
            
            var duration = DateTimeOffset.UtcNow - startTime;
            _telemetryService.TrackDependency("Database", "ObterDimensaoPorId", $"SELECT WHERE Id={id}", startTime, duration, dimensao != null);
            
            return dimensao;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ObterDimensaoPorId" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<Dimensao> InserirAsync(Dimensao dimensao)
    {
        try
        {
            if (await ExisteNomeAsync(dimensao.Nome))
            {
                throw new InvalidOperationException($"Já existe uma dimensão com o nome '{dimensao.Nome}'.");
            }
            
            var startTime = DateTimeOffset.UtcNow;
            
            dimensao.DataCriacao = DateTime.Now;
            dimensao.DataAlteracao = DateTime.Now;
            
            _context.Dimensoes.Add(dimensao);
            await _context.SaveChangesAsync();
            
            var duration = DateTimeOffset.UtcNow - startTime;
            _telemetryService.TrackDependency("Database", "InserirDimensao", "INSERT", startTime, duration, true);
            
            _telemetryService.TrackEvent("Dimensao_Inserida", new Dictionary<string, string>
            {
                { "Id", dimensao.IdDimensao.ToString() },
                { "Nome", dimensao.Nome }
            });
            
            return dimensao;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "InserirDimensao" },
                { "Nome", dimensao.Nome }
            });
            throw;
        }
    }

    public async Task<Dimensao> AlterarAsync(Dimensao dimensao)
    {
        try
        {
            var dimensaoExistente = await ObterPorIdAsync(dimensao.IdDimensao);
            if (dimensaoExistente == null)
            {
                throw new InvalidOperationException($"Dimensão com ID {dimensao.IdDimensao} não encontrada.");
            }
            
            if (await ExisteNomeAsync(dimensao.Nome, dimensao.IdDimensao))
            {
                throw new InvalidOperationException($"Já existe uma dimensão com o nome '{dimensao.Nome}'.");
            }
            
            var startTime = DateTimeOffset.UtcNow;
            
            dimensaoExistente.Nome = dimensao.Nome;
            dimensaoExistente.Ativo = dimensao.Ativo;
            dimensaoExistente.TipoAvaliacao = dimensao.TipoAvaliacao;
            dimensaoExistente.DataAlteracao = DateTime.Now;
            
            _context.Dimensoes.Update(dimensaoExistente);
            await _context.SaveChangesAsync();
            
            var duration = DateTimeOffset.UtcNow - startTime;
            _telemetryService.TrackDependency("Database", "AlterarDimensao", "UPDATE", startTime, duration, true);
            
            _telemetryService.TrackEvent("Dimensao_Alterada", new Dictionary<string, string>
            {
                { "Id", dimensaoExistente.IdDimensao.ToString() },
                { "Nome", dimensaoExistente.Nome }
            });
            
            return dimensaoExistente;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "AlterarDimensao" },
                { "Id", dimensao.IdDimensao.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InativarAsync(int id)
    {
        try
        {
            var dimensao = await ObterPorIdAsync(id);
            if (dimensao == null)
            {
                return false;
            }
            
            var startTime = DateTimeOffset.UtcNow;
            
            dimensao.Ativo = false;
            dimensao.DataAlteracao = DateTime.Now;
            
            _context.Dimensoes.Update(dimensao);
            await _context.SaveChangesAsync();
            
            var duration = DateTimeOffset.UtcNow - startTime;
            _telemetryService.TrackDependency("Database", "InativarDimensao", "UPDATE", startTime, duration, true);
            
            _telemetryService.TrackEvent("Dimensao_Inativada", new Dictionary<string, string>
            {
                { "Id", id.ToString() },
                { "Nome", dimensao.Nome }
            });
            
            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "InativarDimensao" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> ExisteAsync(int id)
    {
        try
        {
            return await _context.Dimensoes.AnyAsync(d => d.IdDimensao == id);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ExisteDimensao" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null)
    {
        try
        {
            var query = _context.Dimensoes.Where(d => d.Nome.ToLower() == nome.ToLower());
            
            if (idExcluir.HasValue)
            {
                query = query.Where(d => d.IdDimensao != idExcluir.Value);
            }
            
            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Operacao", "ExisteNomeDimensao" },
                { "Nome", nome },
                { "IdExcluir", idExcluir?.ToString() ?? "null" }
            });
            throw;
        }
    }
}