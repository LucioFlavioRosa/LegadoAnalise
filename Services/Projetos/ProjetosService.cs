using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Projetos;

public interface IProjetosService
{
    Task<List<Projeto>> ObterProjetosAsync();
    Task<List<Projeto>> ObterProjetosAsync(bool apenasAtivos);
    Task<Projeto?> ObterProjetoAsync(int idProjeto);
    Task<bool> InserirProjetoAsync(Projeto projeto);
    Task<bool> AtualizarProjetoAsync(Projeto projeto);
    Task<bool> RemoverProjetoAsync(int idProjeto);
    Task<bool> InativarProjetoAsync(int idProjeto);
    Task<bool> AtivarProjetoAsync(int idProjeto);
    Task<List<Projeto>> ObterProjetosPorClienteAsync(int idCliente);
    Task<List<Projeto>> ObterProjetosPorResponsavelAsync(int idResponsavel);
    Task<List<Projeto>> ObterProjetosPorGestorAsync(int idGestor);
    Task<bool> AdicionarAssociadoProjetoAsync(int idProjeto, int idAssociado, DateTime dataInicio, DateTime? dataFim = null);
    Task<bool> RemoverAssociadoProjetoAsync(int idProjeto, int idAssociado);
    Task<List<Associado>> ObterAssociadosProjetoAsync(int idProjeto);
    Task<bool> ProjetoExisteAsync(string codigo);
    Task<bool> ValidarPeriodoProjetoAsync(DateTime dataInicio, DateTime dataFim);
}

public class ProjetosService : IProjetosService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IUserContextService _userContextService;

    public ProjetosService(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService,
        IUserContextService userContextService)
    {
        _context = context;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
        _userContextService = userContextService;
    }

    public async Task<List<Projeto>> ObterProjetosAsync()
    {
        return await ObterProjetosAsync(false);
    }

    public async Task<List<Projeto>> ObterProjetosAsync(bool apenasAtivos)
    {
        try
        {
            var query = _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.AssociadoResponsavel)
                .Include(p => p.AssociadoGestor)
                .Include(p => p.TipoProjeto)
                .Include(p => p.Complexidade)
                .AsQueryable();

            if (apenasAtivos)
            {
                query = query.Where(p => p.Ativo);
            }

            var projetos = await query.OrderBy(p => p.Nome).ToListAsync();

            _telemetryService.TrackEvent("ProjetosListados", new Dictionary<string, string>
            {
                { "TotalProjetos", projetos.Count.ToString() },
                { "ApenasAtivos", apenasAtivos.ToString() }
            });

            return projetos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterProjetosAsync" },
                { "Component", "ProjetosService" }
            });
            return new List<Projeto>();
        }
    }

    public async Task<Projeto?> ObterProjetoAsync(int idProjeto)
    {
        try
        {
            var projeto = await _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.AssociadoResponsavel)
                .Include(p => p.AssociadoGestor)
                .Include(p => p.TipoProjeto)
                .Include(p => p.Complexidade)
                .Include(p => p.AssociadosProjeto)
                    .ThenInclude(ap => ap.Associado)
                .FirstOrDefaultAsync(p => p.Id == idProjeto);

            if (projeto != null)
            {
                _telemetryService.TrackEvent("ProjetoObtido", new Dictionary<string, string>
                {
                    { "ProjetoId", idProjeto.ToString() },
                    { "ProjetoNome", projeto.Nome }
                });
            }

            return projeto;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoId", idProjeto.ToString() }
            });
            return null;
        }
    }

    public async Task<bool> InserirProjetoAsync(Projeto projeto)
    {
        try
        {
            if (!await ValidarProjetoAsync(projeto))
            {
                return false;
            }

            var usuario = await _userContextService.GetUsuarioLogadoAsync();
            if (usuario != null)
            {
                projeto.UsuarioCriacao = usuario.Id;
                projeto.DataCriacao = DateTime.Now;
            }

            _context.Projetos.Add(projeto);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _messageBoxService.ShowSuccess("Projeto cadastrado com sucesso!");
                _telemetryService.TrackEvent("ProjetoInserido", new Dictionary<string, string>
                {
                    { "ProjetoId", projeto.Id.ToString() },
                    { "ProjetoNome", projeto.Nome },
                    { "UsuarioId", usuario?.Id.ToString() ?? "Unknown" }
                });
            }
            else
            {
                _messageBoxService.ShowError("Erro ao cadastrar projeto");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InserirProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoNome", projeto?.Nome ?? "Unknown" }
            });
            _messageBoxService.ShowError("Erro interno ao cadastrar projeto");
            return false;
        }
    }

    public async Task<bool> AtualizarProjetoAsync(Projeto projeto)
    {
        try
        {
            if (!await ValidarProjetoAsync(projeto))
            {
                return false;
            }

            var projetoExistente = await _context.Projetos.FindAsync(projeto.Id);
            if (projetoExistente == null)
            {
                _messageBoxService.ShowError("Projeto não encontrado");
                return false;
            }

            var usuario = await _userContextService.GetUsuarioLogadoAsync();
            if (usuario != null)
            {
                projeto.UsuarioAlteracao = usuario.Id;
                projeto.DataAlteracao = DateTime.Now;
            }

            _context.Entry(projetoExistente).CurrentValues.SetValues(projeto);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _messageBoxService.ShowSuccess("Projeto atualizado com sucesso!");
                _telemetryService.TrackEvent("ProjetoAtualizado", new Dictionary<string, string>
                {
                    { "ProjetoId", projeto.Id.ToString() },
                    { "ProjetoNome", projeto.Nome },
                    { "UsuarioId", usuario?.Id.ToString() ?? "Unknown" }
                });
            }
            else
            {
                _messageBoxService.ShowError("Erro ao atualizar projeto");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AtualizarProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoId", projeto?.Id.ToString() ?? "Unknown" }
            });
            _messageBoxService.ShowError("Erro interno ao atualizar projeto");
            return false;
        }
    }

    public async Task<bool> RemoverProjetoAsync(int idProjeto)
    {
        try
        {
            var projeto = await _context.Projetos.FindAsync(idProjeto);
            if (projeto == null)
            {
                _messageBoxService.ShowError("Projeto não encontrado");
                return false;
            }

            _context.Projetos.Remove(projeto);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _messageBoxService.ShowSuccess("Projeto removido com sucesso!");
                _telemetryService.TrackEvent("ProjetoRemovido", new Dictionary<string, string>
                {
                    { "ProjetoId", idProjeto.ToString() },
                    { "ProjetoNome", projeto.Nome }
                });
            }
            else
            {
                _messageBoxService.ShowError("Erro ao remover projeto");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "RemoverProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoId", idProjeto.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao remover projeto");
            return false;
        }
    }

    public async Task<bool> InativarProjetoAsync(int idProjeto)
    {
        return await AlterarStatusProjetoAsync(idProjeto, false);
    }

    public async Task<bool> AtivarProjetoAsync(int idProjeto)
    {
        return await AlterarStatusProjetoAsync(idProjeto, true);
    }

    public async Task<List<Projeto>> ObterProjetosPorClienteAsync(int idCliente)
    {
        try
        {
            return await _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.AssociadoResponsavel)
                .Include(p => p.AssociadoGestor)
                .Where(p => p.IdCliente == idCliente)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterProjetosPorClienteAsync" },
                { "Component", "ProjetosService" },
                { "ClienteId", idCliente.ToString() }
            });
            return new List<Projeto>();
        }
    }

    public async Task<List<Projeto>> ObterProjetosPorResponsavelAsync(int idResponsavel)
    {
        try
        {
            return await _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.AssociadoResponsavel)
                .Include(p => p.AssociadoGestor)
                .Where(p => p.IdAssociadoResponsavel == idResponsavel)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterProjetosPorResponsavelAsync" },
                { "Component", "ProjetosService" },
                { "ResponsavelId", idResponsavel.ToString() }
            });
            return new List<Projeto>();
        }
    }

    public async Task<List<Projeto>> ObterProjetosPorGestorAsync(int idGestor)
    {
        try
        {
            return await _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.AssociadoResponsavel)
                .Include(p => p.AssociadoGestor)
                .Where(p => p.IdAssociadoGestor == idGestor)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterProjetosPorGestorAsync" },
                { "Component", "ProjetosService" },
                { "GestorId", idGestor.ToString() }
            });
            return new List<Projeto>();
        }
    }

    public async Task<bool> AdicionarAssociadoProjetoAsync(int idProjeto, int idAssociado, DateTime dataInicio, DateTime? dataFim = null)
    {
        try
        {
            var associadoProjeto = new AssociadoProjeto
            {
                IdProjeto = idProjeto,
                IdAssociado = idAssociado,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Ativo = true
            };

            _context.AssociadosProjetos.Add(associadoProjeto);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _messageBoxService.ShowSuccess("Associado adicionado ao projeto com sucesso!");
                _telemetryService.TrackEvent("AssociadoAdicionadoProjeto", new Dictionary<string, string>
                {
                    { "ProjetoId", idProjeto.ToString() },
                    { "AssociadoId", idAssociado.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AdicionarAssociadoProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoId", idProjeto.ToString() },
                { "AssociadoId", idAssociado.ToString() }
            });
            _messageBoxService.ShowError("Erro ao adicionar associado ao projeto");
            return false;
        }
    }

    public async Task<bool> RemoverAssociadoProjetoAsync(int idProjeto, int idAssociado)
    {
        try
        {
            var associadoProjeto = await _context.AssociadosProjetos
                .FirstOrDefaultAsync(ap => ap.IdProjeto == idProjeto && ap.IdAssociado == idAssociado);

            if (associadoProjeto == null)
            {
                _messageBoxService.ShowError("Associação não encontrada");
                return false;
            }

            _context.AssociadosProjetos.Remove(associadoProjeto);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _messageBoxService.ShowSuccess("Associado removido do projeto com sucesso!");
                _telemetryService.TrackEvent("AssociadoRemovidoProjeto", new Dictionary<string, string>
                {
                    { "ProjetoId", idProjeto.ToString() },
                    { "AssociadoId", idAssociado.ToString() }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "RemoverAssociadoProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoId", idProjeto.ToString() },
                { "AssociadoId", idAssociado.ToString() }
            });
            _messageBoxService.ShowError("Erro ao remover associado do projeto");
            return false;
        }
    }

    public async Task<List<Associado>> ObterAssociadosProjetoAsync(int idProjeto)
    {
        try
        {
            return await _context.AssociadosProjetos
                .Include(ap => ap.Associado)
                    .ThenInclude(a => a.Cargo)
                .Where(ap => ap.IdProjeto == idProjeto && ap.Ativo)
                .Select(ap => ap.Associado)
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterAssociadosProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoId", idProjeto.ToString() }
            });
            return new List<Associado>();
        }
    }

    public async Task<bool> ProjetoExisteAsync(string codigo)
    {
        try
        {
            return await _context.Projetos.AnyAsync(p => p.Codigo == codigo);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ProjetoExisteAsync" },
                { "Component", "ProjetosService" },
                { "Codigo", codigo }
            });
            return false;
        }
    }

    public async Task<bool> ValidarPeriodoProjetoAsync(DateTime dataInicio, DateTime dataFim)
    {
        if (dataInicio >= dataFim)
        {
            _messageBoxService.ShowWarning("A data de início deve ser anterior à data de término");
            return false;
        }

        if (dataInicio < DateTime.Today.AddYears(-10))
        {
            _messageBoxService.ShowWarning("A data de início não pode ser muito antiga");
            return false;
        }

        if (dataFim > DateTime.Today.AddYears(10))
        {
            _messageBoxService.ShowWarning("A data de término não pode ser muito distante");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidarProjetoAsync(Projeto projeto)
    {
        if (string.IsNullOrWhiteSpace(projeto.Nome))
        {
            _messageBoxService.ShowWarning("O nome do projeto é obrigatório");
            return false;
        }

        if (string.IsNullOrWhiteSpace(projeto.Codigo))
        {
            _messageBoxService.ShowWarning("O código do projeto é obrigatório");
            return false;
        }

        if (projeto.Id == 0 && await ProjetoExisteAsync(projeto.Codigo))
        {
            _messageBoxService.ShowWarning("Já existe um projeto com este código");
            return false;
        }

        if (projeto.DataInicio.HasValue && projeto.DataFim.HasValue)
        {
            return await ValidarPeriodoProjetoAsync(projeto.DataInicio.Value, projeto.DataFim.Value);
        }

        return true;
    }

    private async Task<bool> AlterarStatusProjetoAsync(int idProjeto, bool ativo)
    {
        try
        {
            var projeto = await _context.Projetos.FindAsync(idProjeto);
            if (projeto == null)
            {
                _messageBoxService.ShowError("Projeto não encontrado");
                return false;
            }

            projeto.Ativo = ativo;
            var usuario = await _userContextService.GetUsuarioLogadoAsync();
            if (usuario != null)
            {
                projeto.UsuarioAlteracao = usuario.Id;
                projeto.DataAlteracao = DateTime.Now;
            }

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                var statusTexto = ativo ? "ativado" : "inativado";
                _messageBoxService.ShowSuccess($"Projeto {statusTexto} com sucesso!");
                _telemetryService.TrackEvent("ProjetoStatusAlterado", new Dictionary<string, string>
                {
                    { "ProjetoId", idProjeto.ToString() },
                    { "NovoStatus", ativo.ToString() },
                    { "UsuarioId", usuario?.Id.ToString() ?? "Unknown" }
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarStatusProjetoAsync" },
                { "Component", "ProjetosService" },
                { "ProjetoId", idProjeto.ToString() },
                { "NovoStatus", ativo.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao alterar status do projeto");
            return false;
        }
    }
}