using Peers.Moderno.Models;
using Peers.Moderno.Services.Associados;

namespace Peers.Moderno.Services.Common;

public interface IPasswordService
{
    Task<PasswordValidationResult> ValidatePasswordAsync(string password, string confirmPassword);
    Task<PasswordChangeResult> ChangePasswordAsync(int userId, string newPassword);
    Task NotifyPasswordChangeAsync(int userId);
    bool IsDefaultPassword(string password);
    Task<bool> RequiresPasswordChangeAsync(int userId);
}

public class PasswordService : IPasswordService
{
    private readonly IAssociadosService _associadosService;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;
    private const string DEFAULT_PASSWORD = "avaliacao";

    public PasswordService(
        IAssociadosService associadosService,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService)
    {
        _associadosService = associadosService;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
    }

    public async Task<PasswordValidationResult> ValidatePasswordAsync(string password, string confirmPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return PasswordValidationResult.Error("Digite a Nova Senha");
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                return PasswordValidationResult.Error("Confirme a Nova Senha");
            }

            if (password.Trim() != confirmPassword.Trim())
            {
                return PasswordValidationResult.Error("As Senhas não são iguais");
            }

            if (password.Length < 6)
            {
                return PasswordValidationResult.Error("A senha deve ter pelo menos 6 caracteres");
            }

            if (IsDefaultPassword(password))
            {
                return PasswordValidationResult.Error("A nova senha não pode ser igual à senha padrão");
            }

            return PasswordValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidatePasswordAsync" },
                { "Component", "PasswordService" }
            });
            return PasswordValidationResult.Error("Erro interno ao validar senha");
        }
    }

    public async Task<PasswordChangeResult> ChangePasswordAsync(int userId, string newPassword)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(userId);
            if (associado == null)
            {
                return PasswordChangeResult.Error("Usuário não encontrado");
            }

            associado.Senha = newPassword.Trim();
            var success = await _associadosService.AlteraAssociadoSenhaAsync(userId, associado);

            if (success)
            {
                await NotifyPasswordChangeAsync(userId);
                _messageBoxService.ShowSuccess("Senha alterada com sucesso");
                return PasswordChangeResult.Success();
            }
            else
            {
                _messageBoxService.ShowError("Falha ao alterar a senha");
                return PasswordChangeResult.Error("Falha ao alterar a senha");
            }
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ChangePasswordAsync" },
                { "Component", "PasswordService" },
                { "UserId", userId.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao alterar senha");
            return PasswordChangeResult.Error("Erro interno ao alterar senha");
        }
    }

    public async Task NotifyPasswordChangeAsync(int userId)
    {
        try
        {
            _telemetryService.TrackEvent("PasswordChanged", new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
            });

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "NotifyPasswordChangeAsync" },
                { "Component", "PasswordService" },
                { "UserId", userId.ToString() }
            });
        }
    }

    public bool IsDefaultPassword(string password)
    {
        return string.Equals(password?.Trim(), DEFAULT_PASSWORD, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> RequiresPasswordChangeAsync(int userId)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(userId);
            return associado != null && IsDefaultPassword(associado.Senha);
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "RequiresPasswordChangeAsync" },
                { "Component", "PasswordService" },
                { "UserId", userId.ToString() }
            });
            return false;
        }
    }
}

public class PasswordValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private PasswordValidationResult() { }

    public static PasswordValidationResult Success()
    {
        return new PasswordValidationResult { IsValid = true };
    }

    public static PasswordValidationResult Error(string errorMessage)
    {
        return new PasswordValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }
}

public class PasswordChangeResult
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private PasswordChangeResult() { }

    public static PasswordChangeResult Success()
    {
        return new PasswordChangeResult { IsSuccess = true };
    }

    public static PasswordChangeResult Error(string errorMessage)
    {
        return new PasswordChangeResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}