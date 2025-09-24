using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Common;

public interface IPerguntasEncerramentoService
{
    Task<List<PerguntaEncerramento>> ListarAsync();
    Task<List<PerguntaEncerramento>> ListarAtivasAsync();
    Task<PerguntaEncerramento?> ObterPorIdAsync(int id);
    Task<PerguntaEncerramento?> ObterPorCodigoAsync(string codigo);
    Task<bool> AdicionarAsync(PerguntaEncerramento pergunta);
    Task<bool> AtualizarAsync(PerguntaEncerramento pergunta);
    Task<bool> RemoverAsync(int id);
    Task<bool> InativarAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo, int? idExcluir = null);
}

public class PerguntasEncerramentoService : IPerguntasEncerramentoService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;

    public PerguntasEncerramentoService(
        ApplicationDbContext context,
        ITelemetryService telemetryService)
    {
        _context = context;
        _telemetryService = telemetryService;
    }

    public async Task<List<PerguntaEncerramento>> ListarAsync()
    {
        try
        {
            var perguntas = await _context.PerguntasEncerramento
                .OrderBy(p => p.Codigo)
                .ToListAsync();

            _telemetryService.TrackEvent("PerguntasEncerramentoListadas", new Dictionary<string, string>
            {
                { "Count", perguntas.Count.ToString() }
            });

            return perguntas;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarAsync" },
                { "Component", "PerguntasEncerramentoService" }
            });
            throw;
        }
    }

    public async Task<List<PerguntaEncerramento>> ListarAtivasAsync()
    {
        try
        {
            var perguntas = await _context.PerguntasEncerramento
                .Where(p => p.Ativo)
                .OrderBy(p => p.Codigo)
                .ToListAsync();

            return perguntas;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarAtivasAsync" },
                { "Component", "PerguntasEncerramentoService" }
            });
            throw;
        }
    }

    public async Task<PerguntaEncerramento?> ObterPorIdAsync(int id)
    {
        try
        {
            return await _context.PerguntasEncerramento
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPorIdAsync" },
                { "Component", "PerguntasEncerramentoService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<PerguntaEncerramento?> ObterPorCodigoAsync(string codigo)
    {
        try
        {
            return await _context.PerguntasEncerramento
                .FirstOrDefaultAsync(p => p.Codigo == codigo);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPorCodigoAsync" },
                { "Component", "PerguntasEncerramentoService" },
                { "Codigo", codigo }
            });
            throw;
        }
    }

    public async Task<bool> AdicionarAsync(PerguntaEncerramento pergunta)
    {
        try
        {
            pergunta.DataCriacao = DateTime.UtcNow;
            pergunta.Ativo = true;

            _context.PerguntasEncerramento.Add(pergunta);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerguntaEncerramentoAdicionada", new Dictionary<string, string>
                {
                    { "Id", pergunta.Id.ToString() },
                    { "Codigo", pergunta.Codigo }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AdicionarAsync" },
                { "Component", "PerguntasEncerramentoService" },
                { "Codigo", pergunta?.Codigo ?? "Unknown" }
            });
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(PerguntaEncerramento pergunta)
    {
        try
        {
            var perguntaExistente = await ObterPorIdAsync(pergunta.Id);
            if (perguntaExistente == null)
                return false;

            perguntaExistente.Codigo = pergunta.Codigo;
            perguntaExistente.Descricao = pergunta.Descricao;
            perguntaExistente.PossuiComentario = pergunta.PossuiComentario;
            perguntaExistente.DataAtualizacao = DateTime.UtcNow;

            _context.PerguntasEncerramento.Update(perguntaExistente);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerguntaEncerramentoAtualizada", new Dictionary<string, string>
                {
                    { "Id", pergunta.Id.ToString() },
                    { "Codigo", pergunta.Codigo }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AtualizarAsync" },
                { "Component", "PerguntasEncerramentoService" },
                { "Id", pergunta?.Id.ToString() ?? "Unknown" }
            });
            throw;
        }
    }

    public async Task<bool> RemoverAsync(int id)
    {
        try
        {
            var pergunta = await ObterPorIdAsync(id);
            if (pergunta == null)
                return false;

            _context.PerguntasEncerramento.Remove(pergunta);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerguntaEncerramentoRemovida", new Dictionary<string, string>
                {
                    { "Id", id.ToString() },
                    { "Codigo", pergunta.Codigo }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "RemoverAsync" },
                { "Component", "PerguntasEncerramentoService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InativarAsync(int id)
    {
        try
        {
            var pergunta = await ObterPorIdAsync(id);
            if (pergunta == null)
                return false;

            pergunta.Ativo = false;
            pergunta.DataAtualizacao = DateTime.UtcNow;

            _context.PerguntasEncerramento.Update(pergunta);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerguntaEncerramentoInativada", new Dictionary<string, string>
                {
                    { "Id", id.ToString() },
                    { "Codigo", pergunta.Codigo }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarAsync" },
                { "Component", "PerguntasEncerramentoService" },
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? idExcluir = null)
    {
        try
        {
            var query = _context.PerguntasEncerramento.Where(p => p.Codigo == codigo);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(p => p.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExisteCodigoAsync" },
                { "Component", "PerguntasEncerramentoService" },
                { "Codigo", codigo }
            });
            throw;
        }
    }
}