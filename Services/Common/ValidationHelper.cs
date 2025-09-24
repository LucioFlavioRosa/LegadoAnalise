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
            return ValidationResult.Error($"Selecione um valor para {fieldName}");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateMultiple(params ValidationResult[] results)
    {
        var errors = results.Where(r => !r.IsValid).Select(r => r.ErrorMessage).ToList();
        
        if (errors.Any())
        {
            return ValidationResult.Error(string.Join("; ", errors));
        }
        
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateStringLength(string? value, string fieldName, int maxLength, int minLength = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (minLength > 0)
            {
                return ValidationResult.Error($"O campo {fieldName} é obrigatório");
            }
            return ValidationResult.Success();
        }

        if (value.Length < minLength)
        {
            return ValidationResult.Error($"O campo {fieldName} deve ter pelo menos {minLength} caracteres");
        }

        if (value.Length > maxLength)
        {
            return ValidationResult.Error($"O campo {fieldName} deve ter no máximo {maxLength} caracteres");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateNumericRange(int? value, string fieldName, int min, int max)
    {
        if (!value.HasValue)
        {
            return ValidationResult.Error($"O campo {fieldName} é obrigatório");
        }

        if (value < min || value > max)
        {
            return ValidationResult.Error($"O campo {fieldName} deve estar entre {min} e {max}");
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

    public static string SanitizeInput(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return input.Trim();
    }

    public static ValidationResult ValidateAssociado(string? nome, string? email, string? senha, string? mentor, string? cargo, string? perfil, string? status)
    {
        var validations = new List<ValidationResult>
        {
            ValidateRequired(nome, "Nome"),
            ValidateEmail(email),
            ValidatePassword(senha),
            ValidateDropdownSelection(mentor, "Mentor"),
            ValidateDropdownSelection(cargo, "Cargo"),
            ValidateDropdownSelection(perfil, "Perfil de Acesso"),
            ValidateDropdownSelection(status, "Status")
        };

        return ValidateMultiple(validations.ToArray());
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<string> ErrorMessages { get; private set; } = new List<string>();

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
            ErrorMessage = errorMessage,
            ErrorMessages = new List<string> { errorMessage }
        };
    }

    public static ValidationResult Error(List<string> errorMessages)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = string.Join("; ", errorMessages),
            ErrorMessages = errorMessages
        };
    }
}