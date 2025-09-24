namespace Peers.Moderno.Services.Common;

public static class ValidationHelper
{
    public static ValidationResult ValidateRequired(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Error($"O campo {fieldName} é obrigatório");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationResult.Error("O campo E-mail é obrigatório");
        }

        if (!IsValidEmail(email))
        {
            return ValidationResult.Error("E-mail inválido");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidatePassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ValidationResult.Error("O campo Senha é obrigatório");
        }

        if (password.Length < 6)
        {
            return ValidationResult.Error("A senha deve ter pelo menos 6 caracteres");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateDropdownSelection(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "" || value == "0")
        {
            return ValidationResult.Error($"Selecione uma opção para {fieldName}");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateMaxLength(string? value, int maxLength, string fieldName)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
        {
            return ValidationResult.Error($"O campo {fieldName} deve ter no máximo {maxLength} caracteres");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateMinLength(string? value, int minLength, string fieldName)
    {
        if (!string.IsNullOrEmpty(value) && value.Length < minLength)
        {
            return ValidationResult.Error($"O campo {fieldName} deve ter pelo menos {minLength} caracteres");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateNumericRange(decimal? value, decimal min, decimal max, string fieldName)
    {
        if (value.HasValue && (value < min || value > max))
        {
            return ValidationResult.Error($"O campo {fieldName} deve estar entre {min} e {max}");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateAssociadoForm(string? nome, string? email, string? senha, string? mentor, string? cargo, string? perfil, string? status)
    {
        var results = new List<ValidationResult>
        {
            ValidateRequired(nome, "Nome"),
            ValidateEmail(email),
            ValidatePassword(senha),
            ValidateDropdownSelection(mentor, "Mentor"),
            ValidateDropdownSelection(cargo, "Cargo"),
            ValidateDropdownSelection(perfil, "Perfil de Acesso"),
            ValidateDropdownSelection(status, "Status")
        };

        var errors = results.Where(r => !r.IsValid).Select(r => r.ErrorMessage).ToList();
        
        if (errors.Any())
        {
            return ValidationResult.Error(string.Join("<br>", errors));
        }

        return ValidationResult.Success();
    }

    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidId(string? id)
    {
        return int.TryParse(id, out var result) && result > 0;
    }

    public static bool IsValidStatus(string? status)
    {
        return status == "0" || status == "1";
    }

    public static string SanitizeInput(string? input)
    {
        return input?.Trim() ?? string.Empty;
    }

    public static List<string> GetValidationErrors(params ValidationResult[] results)
    {
        return results.Where(r => !r.IsValid).Select(r => r.ErrorMessage).ToList();
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    private ValidationResult() { }

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Error(string errorMessage)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }
}