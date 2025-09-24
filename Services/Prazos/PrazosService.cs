using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Prazos.Common;

namespace Peers.Moderno.Services.Prazos;

public interface IPrazosService
{
    Task<List<Prazo>> ObterTodosPrazosAsync();
    Task<Prazo?> ObterPrazoPorIdAsync(int id);
    Task<OperationResult> CadastrarOuAtualizarPrazoAsync(PrazoFormModel model, int userId);
    Task<OperationResult> InativarPrazoAsync(int id);
}

public class PrazosService : IPrazosService
{
    private readonly ApplicationDbContext _context;
    private readonly IPrazosValidationHelper _validationHelper;
    private readonly ITelemetryService _telemetryService;

    public PrazosService(
        ApplicationDbContext context,
        IPrazosValidationHelper validationHelper,
        ITelemetryService telemetryService)
    {
        _context = context;
        _validationHelper = validationHelper;
        _telemetryService = telemetryService;
    }

    public async Task<List<Prazo>> ObterTodosPrazosAsync()
    {
        try
        {
            var prazos = await _context.Set<Prazo>()
                .OrderBy(p => p.NomeDisparo)
                .ToListAsync();

            _telemetryService.TrackEvent("PrazosListLoaded", new Dictionary<string, string>
            {
                { "Count", prazos.Count.ToString() }
            });

            return prazos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterTodosPrazosAsync" },
                { "Component", "PrazosService" }
            });
            return new List<Prazo>();
        }
    }

    public async Task<Prazo?> ObterPrazoPorIdAsync(int id)
    {
        try
        {
            var prazo = await _context.Set<Prazo>()
                .FirstOrDefaultAsync(p => p.IdPrazo == id);

            if (prazo != null)
            {
                _telemetryService.TrackEvent("PrazoLoaded", new Dictionary<string, string>
                {
                    { "PrazoId", id.ToString() }
                });
            }

            return prazo;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPrazoPorIdAsync" },
                { "Component", "PrazosService" },
                { "PrazoId", id.ToString() }
            });
            return null;
        }
    }

    public async Task<OperationResult> CadastrarOuAtualizarPrazoAsync(PrazoFormModel model, int userId)
    {
        try
        {
            var validation = _validationHelper.ValidatePrazo(model);
            if (!validation.IsValid)
            {
                return OperationResult.Error(validation.ErrorMessage);
            }

            var prazo = model.IdPrazo > 0 
                ? await _context.Set<Prazo>().FirstOrDefaultAsync(p => p.IdPrazo == model.IdPrazo)
                : new Prazo();

            if (prazo == null)
            {
                return OperationResult.Error("Prazo não encontrado");
            }

            var isUpdate = model.IdPrazo > 0;

            MapFormModelToPrazo(model, prazo, userId);

            if (!isUpdate)
            {
                _context.Set<Prazo>().Add(prazo);
            }

            await _context.SaveChangesAsync();

            var message = isUpdate ? "Prazo atualizado com sucesso" : "Prazo inserido com sucesso";
            var eventName = isUpdate ? "PrazoUpdated" : "PrazoCreated";

            _telemetryService.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "PrazoId", prazo.IdPrazo.ToString() },
                { "UserId", userId.ToString() }
            });

            return OperationResult.Success(message);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CadastrarOuAtualizarPrazoAsync" },
                { "Component", "PrazosService" },
                { "PrazoId", model.IdPrazo.ToString() },
                { "UserId", userId.ToString() }
            });
            return OperationResult.Error("Erro ao salvar o prazo");
        }
    }

    public async Task<OperationResult> InativarPrazoAsync(int id)
    {
        try
        {
            var prazo = await _context.Set<Prazo>()
                .FirstOrDefaultAsync(p => p.IdPrazo == id);

            if (prazo == null)
            {
                return OperationResult.Error("Prazo não encontrado");
            }

            prazo.ATV = 0;
            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("PrazoInactivated", new Dictionary<string, string>
            {
                { "PrazoId", id.ToString() }
            });

            return OperationResult.Success("Prazo inativado com sucesso");
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InativarPrazoAsync" },
                { "Component", "PrazosService" },
                { "PrazoId", id.ToString() }
            });
            return OperationResult.Error("Erro ao inativar o prazo");
        }
    }

    private void MapFormModelToPrazo(PrazoFormModel model, Prazo prazo, int userId)
    {
        prazo.NomeDisparo = model.NomeDisparo.Trim();
        prazo.ATV = model.Status;
        prazo.USR = userId;
        prazo.DHC = DateTime.Now;

        _validationHelper.TryParseInt(model.DuracaoAutoAvaliacao, out var duracaoAut);
        prazo.DuracaoAutoAvaliacao = duracaoAut;
        prazo.GatilhoAutoAvaliacao = model.GatilhoAutoAvaliacao;
        _validationHelper.TryParseInt(model.CompensacaoAutoAvaliacao, out var compensacaoAut);
        prazo.CompensadorAutoAvaliacao = compensacaoAut;

        _validationHelper.TryParseInt(model.DuracaoAvaliacaoAsCegas, out var duracaoCeg);
        prazo.DuracaoAvaliacaoAsCegas = duracaoCeg;
        prazo.GatilhoAvaliacaoAsCegas = model.GatilhoAvaliacaoAsCegas;
        _validationHelper.TryParseInt(model.CompensacaoAvaliacaoAsCegas, out var compensacaoCeg);
        prazo.CompensadorAvaliacaoAsCegas = compensacaoCeg;

        _validationHelper.TryParseInt(model.DuracaoAvaliacaoGestor, out var duracaoGes);
        prazo.DuracaoAvaliacaoGestor = duracaoGes;
        prazo.GatilhoAvaliacaoGestor = model.GatilhoAvaliacaoGestor;
        _validationHelper.TryParseInt(model.CompensacaoAvaliacaoGestor, out var compensacaoGes);
        prazo.CompensadorAvaliacaoGestor = compensacaoGes;

        _validationHelper.TryParseInt(model.DuracaoFeedback, out var duracaoFee);
        prazo.DuracaoFeedback = duracaoFee;
        prazo.GatilhoFeedback = model.GatilhoFeedback;
        _validationHelper.TryParseInt(model.CompensacaoFeedback, out var compensacaoFee);
        prazo.CompensadorFeedback = compensacaoFee;

        _validationHelper.TryParseInt(model.DuracaoMentor, out var duracaoMen);
        prazo.DuracaoMentor = duracaoMen;
        prazo.GatilhoMentor = model.GatilhoMentor;
        _validationHelper.TryParseInt(model.CompensacaoMentor, out var compensacaoMen);
        prazo.CompensadorMentor = compensacaoMen;
    }
}

public class OperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;

    private OperationResult() { }

    public static OperationResult Success(string message = "")
    {
        return new OperationResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static OperationResult Error(string message)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}