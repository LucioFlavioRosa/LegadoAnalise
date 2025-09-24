using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados.Common;

namespace Peers.Moderno.Services.Associados.Common;

public class TotalizadorAvaliacaoService : ITotalizadorAvaliacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;
    private readonly IAssociadosService _associadosService;

    public TotalizadorAvaliacaoService(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        IAssociadosService associadosService)
    {
        _context = context;
        _telemetryService = telemetryService;
        _associadosService = associadosService;
    }

    public async Task<List<ComboItem>> ObterMentoresAsync()
    {
        try
        {
            var mentores = await _context.Associados
                .Where(a => a.Ativo)
                .OrderBy(a => a.Nome)
                .Select(a => new ComboItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Nome
                })
                .ToListAsync();

            mentores.Insert(0, ComboHelper.GetDefaultSelectionItem());
            return mentores;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterMentoresAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
        }
    }

    public async Task<List<ComboItem>> ObterCargosAsync()
    {
        try
        {
            var cargos = await _context.Cargos
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new ComboItem
                {
                    Value = c.IdCargo.ToString(),
                    Text = c.Nome
                })
                .ToListAsync();

            cargos.Insert(0, ComboHelper.GetDefaultSelectionItem());
            return cargos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterCargosAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
        }
    }

    public async Task<List<ComboItem>> ObterPerfisAsync()
    {
        try
        {
            var perfis = await _context.Perfis
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .Select(p => new ComboItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                })
                .ToListAsync();

            perfis.Insert(0, ComboHelper.GetDefaultSelectionItem());
            return perfis;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPerfisAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
        }
    }

    public async Task<List<ComboItem>> ObterStatusAsync()
    {
        return await Task.FromResult(ComboHelper.GetStatusItems());
    }

    public async Task<ValidationResult> ValidarAssociadoAsync(AssociadoFormModel model)
    {
        try
        {
            var validation = ValidationHelper.ValidateAssociado(
                model.Nome,
                model.Email,
                model.Senha,
                model.IdMentor,
                model.IdCargo,
                model.IdPerfil,
                model.Status
            );

            if (!validation.IsValid)
                return validation;

            var emailExists = await _context.Associados
                .AnyAsync(a => a.Email == model.Email && a.Id != model.Id);

            if (emailExists)
            {
                return ValidationResult.Error("Já existe um associado com este e-mail");
            }

            return ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarAssociadoAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return ValidationResult.Error("Erro interno na validação");
        }
    }

    public async Task<OperationResult> CadastrarAssociadoAsync(AssociadoFormModel model)
    {
        try
        {
            var validation = await ValidarAssociadoAsync(model);
            if (!validation.IsValid)
            {
                return OperationResult.Error(validation.ErrorMessage);
            }

            var associado = new Associado
            {
                Nome = ValidationHelper.SanitizeInput(model.Nome),
                Email = ValidationHelper.SanitizeInput(model.Email),
                Senha = ValidationHelper.SanitizeInput(model.Senha),
                IdMentor = int.Parse(model.IdMentor),
                IdCargo = int.Parse(model.IdCargo),
                IdPerfil = int.Parse(model.IdPerfil),
                Ativo = model.Ativo,
                DataCriacao = DateTime.UtcNow
            };

            _context.Associados.Add(associado);
            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("AssociadoCadastrado", new Dictionary<string, string>
            {
                { "AssociadoId", associado.Id.ToString() },
                { "Nome", associado.Nome }
            });

            return OperationResult.Success("Associado cadastrado com sucesso", associado);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CadastrarAssociadoAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return OperationResult.Error("Erro ao cadastrar associado");
        }
    }

    public async Task<OperationResult> AtualizarAssociadoAsync(int id, AssociadoFormModel model)
    {
        try
        {
            model.Id = id;
            var validation = await ValidarAssociadoAsync(model);
            if (!validation.IsValid)
            {
                return OperationResult.Error(validation.ErrorMessage);
            }

            var associado = await _context.Associados.FindAsync(id);
            if (associado == null)
            {
                return OperationResult.Error("Associado não encontrado");
            }

            associado.Nome = ValidationHelper.SanitizeInput(model.Nome);
            associado.Email = ValidationHelper.SanitizeInput(model.Email);
            associado.Senha = ValidationHelper.SanitizeInput(model.Senha);
            associado.IdMentor = int.Parse(model.IdMentor);
            associado.IdCargo = int.Parse(model.IdCargo);
            associado.IdPerfil = int.Parse(model.IdPerfil);
            associado.Ativo = model.Ativo;
            associado.DataAtualizacao = DateTime.UtcNow;

            _context.Associados.Update(associado);
            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("AssociadoAtualizado", new Dictionary<string, string>
            {
                { "AssociadoId", associado.Id.ToString() },
                { "Nome", associado.Nome }
            });

            return OperationResult.Success("Associado atualizado com sucesso", associado);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AtualizarAssociadoAsync" },
                { "Component", "TotalizadorAvaliacaoService" },
                { "AssociadoId", id.ToString() }
            });
            return OperationResult.Error("Erro ao atualizar associado");
        }
    }

    public async Task<AssociadoFormModel?> ObterAssociadoParaEdicaoAsync(int id)
    {
        try
        {
            var associado = await _context.Associados
                .FirstOrDefaultAsync(a => a.Id == id);

            if (associado == null)
                return null;

            return new AssociadoFormModel
            {
                Id = associado.Id,
                Nome = associado.Nome,
                Email = associado.Email,
                Senha = associado.Senha,
                IdMentor = associado.IdMentor?.ToString() ?? "",
                IdCargo = associado.IdCargo.ToString(),
                IdPerfil = associado.IdPerfil.ToString(),
                Status = associado.Ativo ? "1" : "0"
            };
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterAssociadoParaEdicaoAsync" },
                { "Component", "TotalizadorAvaliacaoService" },
                { "AssociadoId", id.ToString() }
            });
            return null;
        }
    }

    public async Task<List<Associado>> ListarAssociadosAsync()
    {
        try
        {
            return await _context.Associados
                .Include(a => a.Cargo)
                .Include(a => a.Perfil)
                .Include(a => a.Mentor)
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ListarAssociadosAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return new List<Associado>();
        }
    }
}