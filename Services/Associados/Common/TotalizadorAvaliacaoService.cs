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
    private readonly IMessageBoxService _messageBoxService;

    public TotalizadorAvaliacaoService(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService)
    {
        _context = context;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
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

            return ComboHelper.CreateComboFromList(mentores, m => m.Value, m => m.Text, true);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterMentoresAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return ComboHelper.CreateComboFromList(new List<ComboItem>(), m => m.Value, m => m.Text, true);
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

            return ComboHelper.CreateComboFromList(cargos, c => c.Value, c => c.Text, true);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterCargosAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return ComboHelper.CreateComboFromList(new List<ComboItem>(), c => c.Value, c => c.Text, true);
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

            return ComboHelper.CreateComboFromList(perfis, p => p.Value, p => p.Text, true);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPerfisAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return ComboHelper.CreateComboFromList(new List<ComboItem>(), p => p.Value, p => p.Text, true);
        }
    }

    public async Task<List<ComboItem>> ObterStatusAsync()
    {
        return await Task.FromResult(ComboHelper.GetStatusItems());
    }

    public async Task<AssociadoFormModel> ObterAssociadoParaEdicaoAsync(int id)
    {
        try
        {
            var associado = await _context.Associados
                .FirstOrDefaultAsync(a => a.Id == id);

            if (associado == null)
            {
                return new AssociadoFormModel();
            }

            return new AssociadoFormModel
            {
                Id = associado.Id,
                Nome = associado.Nome,
                Email = associado.Email,
                Senha = associado.Senha,
                IdMentor = associado.IdMentor?.ToString() ?? string.Empty,
                IdCargo = associado.IdCargo?.ToString() ?? string.Empty,
                IdPerfil = associado.IdPerfil?.ToString() ?? string.Empty,
                Status = associado.Ativo ? "1" : "0"
            };
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterAssociadoParaEdicaoAsync" },
                { "Component", "TotalizadorAvaliacaoService" },
                { "Id", id.ToString() }
            });
            return new AssociadoFormModel();
        }
    }

    public async Task<OperationResult> ValidarAssociadoAsync(AssociadoFormModel model)
    {
        try
        {
            var validationResult = ValidationHelper.ValidateAssociadoForm(
                model.Nome,
                model.Email,
                model.Senha,
                model.IdMentor,
                model.IdCargo,
                model.IdPerfil,
                model.Status
            );

            if (!validationResult.IsValid)
            {
                return OperationResult.ValidationError(new List<string> { validationResult.ErrorMessage });
            }

            var emailExiste = await ExisteEmailAsync(model.Email, model.IsEdicao ? model.Id : null);
            if (emailExiste)
            {
                return OperationResult.ValidationError(new List<string> { "Já existe um associado com este e-mail" });
            }

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarAssociadoAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return OperationResult.Error("Erro interno na validação");
        }
    }

    public async Task<OperationResult> SalvarAssociadoAsync(AssociadoFormModel model)
    {
        try
        {
            var validationResult = await ValidarAssociadoAsync(model);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            if (model.IsEdicao)
            {
                return await AtualizarAssociadoAsync(model);
            }
            else
            {
                return await CriarAssociadoAsync(model);
            }
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SalvarAssociadoAsync" },
                { "Component", "TotalizadorAvaliacaoService" }
            });
            return OperationResult.Error("Erro interno ao salvar associado");
        }
    }

    public async Task<bool> ExisteEmailAsync(string email, int? idExcluir = null)
    {
        try
        {
            var query = _context.Associados.Where(a => a.Email.ToLower() == email.ToLower());
            
            if (idExcluir.HasValue)
            {
                query = query.Where(a => a.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExisteEmailAsync" },
                { "Component", "TotalizadorAvaliacaoService" },
                { "Email", email }
            });
            return false;
        }
    }

    private async Task<OperationResult> CriarAssociadoAsync(AssociadoFormModel model)
    {
        var associado = new Associado
        {
            Nome = ValidationHelper.SanitizeInput(model.Nome),
            Email = ValidationHelper.SanitizeInput(model.Email),
            Senha = ValidationHelper.SanitizeInput(model.Senha),
            IdMentor = ValidationHelper.IsValidId(model.IdMentor) ? int.Parse(model.IdMentor) : null,
            IdCargo = ValidationHelper.IsValidId(model.IdCargo) ? int.Parse(model.IdCargo) : null,
            IdPerfil = ValidationHelper.IsValidId(model.IdPerfil) ? int.Parse(model.IdPerfil) : null,
            Ativo = model.Status == "1",
            DataCriacao = DateTime.UtcNow
        };

        _context.Associados.Add(associado);
        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            _telemetryService.TrackEvent("AssociadoCriado", new Dictionary<string, string>
            {
                { "Id", associado.Id.ToString() },
                { "Nome", associado.Nome }
            });
            return OperationResult.Success("Associado cadastrado com sucesso");
        }

        return OperationResult.Error("Erro ao cadastrar associado");
    }

    private async Task<OperationResult> AtualizarAssociadoAsync(AssociadoFormModel model)
    {
        var associado = await _context.Associados.FirstOrDefaultAsync(a => a.Id == model.Id);
        if (associado == null)
        {
            return OperationResult.Error("Associado não encontrado");
        }

        associado.Nome = ValidationHelper.SanitizeInput(model.Nome);
        associado.Email = ValidationHelper.SanitizeInput(model.Email);
        associado.Senha = ValidationHelper.SanitizeInput(model.Senha);
        associado.IdMentor = ValidationHelper.IsValidId(model.IdMentor) ? int.Parse(model.IdMentor) : null;
        associado.IdCargo = ValidationHelper.IsValidId(model.IdCargo) ? int.Parse(model.IdCargo) : null;
        associado.IdPerfil = ValidationHelper.IsValidId(model.IdPerfil) ? int.Parse(model.IdPerfil) : null;
        associado.Ativo = model.Status == "1";
        associado.DataAtualizacao = DateTime.UtcNow;

        _context.Associados.Update(associado);
        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            _telemetryService.TrackEvent("AssociadoAtualizado", new Dictionary<string, string>
            {
                { "Id", associado.Id.ToString() },
                { "Nome", associado.Nome }
            });
            return OperationResult.Success("Associado alterado com sucesso");
        }

        return OperationResult.Error("Erro ao alterar associado");
    }
}