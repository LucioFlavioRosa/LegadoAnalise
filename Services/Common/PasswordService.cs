using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Common;

public interface IPasswordService
{
    Task<PasswordValidationResult> ValidatePasswordAsync(string password, string confirmPassword);
    Task<PasswordChangeResult> ChangePasswordAsync(int userId, string newPassword);
    Task NotifyPasswordChangeAsync(int userId);
    bool IsDefaultPassword(string password);
}

public class PasswordService : IPasswordService
{
    private readonly IAssociadosService _associadosService;
    private readonly ITelemetryService _telemetryService;
    private readonly IUserContextService _userContextService;

    public PasswordService(
        IAssociadosService associadosService,
        ITelemetryService telemetryService,
        IUserContextService userContextService)
    {
        _associadosService = associadosService;
        _telemetryService = telemetryService;
        _userContextService = userContextService;
    }

    public Task<PasswordValidationResult> ValidatePasswordAsync(string password, string confirmPassword)
    {
        var result = new PasswordValidationResult();

        if (string.IsNullOrWhiteSpace(password))
        {
            result.IsValid = false;
            result.ErrorMessage = "Digite a Nova Senha";
            return Task.FromResult(result);
        }

        if (string.IsNullOrWhiteSpace(confirmPassword))
        {
            result.IsValid = false;
            result.ErrorMessage = "Confirme a Nova Senha";
            return Task.FromResult(result);
        }

        if (password != confirmPassword)
        {
            result.IsValid = false;
            result.ErrorMessage = "As Senhas não são iguais";
            return Task.FromResult(result);
        }

        if (password.Length < 6)
        {
            result.IsValid = false;
            result.ErrorMessage = "A senha deve ter pelo menos 6 caracteres";
            return Task.FromResult(result);
        }

        result.IsValid = true;
        return Task.FromResult(result);
    }

    public async Task<PasswordChangeResult> ChangePasswordAsync(int userId, string newPassword)
    {
        try
        {
            var associado = await _associadosService.ObterAssociadoAsync(userId);
            if (associado == null)
            {
                return new PasswordChangeResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Usuário não encontrado"
                };
            }

            associado.Senha = newPassword;
            var success = await _associadosService.AlteraAssociadoSenhaAsync(userId, associado);

            if (success)
            {
                await NotifyPasswordChangeAsync(userId);
                
                var updatedUser = _userContextService.GetUsuarioLogado();
                if (updatedUser != null)
                {
                    updatedUser.Senha = newPassword.GetHashCode().ToString();
                    _userContextService.SetUsuarioLogado(updatedUser);
                }

                return new PasswordChangeResult
                {
                    IsSuccess = true,
                    SuccessMessage = "Senha Alterada com Sucesso"
                };
            }
            else
            {
                return new PasswordChangeResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Falha ao Alterar a Senha"
                };
            }
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "Operation", "ChangePassword" }
            });

            return new PasswordChangeResult
            {
                IsSuccess = false,
                ErrorMessage = "Erro interno ao alterar senha"
            };
        }
    }

    public async Task NotifyPasswordChangeAsync(int userId)
    {
        _telemetryService.TrackEvent("PasswordChanged", new Dictionary<string, string>
        {
            { "UserId", userId.ToString() },
            { "Timestamp", DateTime.UtcNow.ToString() }
        });

        await Task.CompletedTask;
    }

    public bool IsDefaultPassword(string password)
    {
        return password == "avaliacao";
    }
}

public class PasswordValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class PasswordChangeResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;
}