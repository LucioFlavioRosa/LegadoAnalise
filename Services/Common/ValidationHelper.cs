using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Common;

public static class ValidationHelper
{
    public static ValidationResult ValidateAssociado(Associado associado, string status)
    {
        if (associado == null)
        {
            return ValidationResult.Error("Dados do associado são obrigatórios");
        }

        if (string.IsNullOrWhiteSpace(associado.Nome))
        {
            return ValidationResult.Error("Nome é obrigatório");
        }

        if (string.IsNullOrWhiteSpace(associado.Email))
        {
            return ValidationResult.Error("E-mail é obrigatório");
        }

        if (!IsValidEmail(associado.Email))
        {
            return ValidationResult.Error("E-mail inválido");
        }

        if (string.IsNullOrWhiteSpace(associado.Senha))
        {
            return ValidationResult.Error("Senha é obrigatória");
        }

        if (associado.Senha.Length < 6)
        {
            return ValidationResult.Error("Senha deve ter pelo menos 6 caracteres");
        }

        if (associado.IdCargo <= 0)
        {
            return ValidationResult.Error("Selecione um cargo");
        }

        if (associado.IdPerfil <= 0)
        {
            return ValidationResult.Error("Selecione um perfil de acesso");
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            return ValidationResult.Error("Selecione o status");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Error($"{fieldName} é obrigatório");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationResult.Error("E-mail é obrigatório");
        }

        if (!IsValidEmail(email))
        {
            return ValidationResult.Error("E-mail inválido");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ValidationResult.Error("Senha é obrigatória");
        }

        if (password.Length < 6)
        {
            return ValidationResult.Error("Senha deve ter pelo menos 6 caracteres");
        }

        return ValidationResult.Success();
    }

    public static ValidationResult ValidateNumericRange(int value, int min, int max, string fieldName)
    {
        if (value < min || value > max)
        {
            return ValidationResult.Error($"{fieldName} deve estar entre {min} e {max}");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateStringLength(string value, int maxLength, string fieldName)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
        {
            return ValidationResult.Error($"{fieldName} deve ter no máximo {maxLength} caracteres");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateSelection(int selectedValue, string fieldName)
    {
        if (selectedValue <= 0)
        {
            return ValidationResult.Error($"Selecione {fieldName}");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateSelectionString(string selectedValue, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(selectedValue) || selectedValue == "" || selectedValue == "0")
        {
            return ValidationResult.Error($"Selecione {fieldName}");
        }
        return ValidationResult.Success();
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

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

    public static bool IsValidCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = cpf.Replace(".", "").Replace("-", "");

        if (cpf.Length != 11)
            return false;

        if (cpf.All(c => c == cpf[0]))
            return false;

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != digit1)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == digit2;
    }

    public static ValidationResult ValidateCPF(string cpf, string fieldName = "CPF")
    {
        if (!IsValidCPF(cpf))
        {
            return ValidationResult.Error($"{fieldName} inválido");
        }
        return ValidationResult.Success();
    }

    public static ValidationResult ValidateMultiple(params ValidationResult[] validations)
    {
        var errors = validations.Where(v => !v.IsValid).Select(v => v.ErrorMessage).ToList();
        
        if (errors.Any())
        {
            return ValidationResult.Error(string.Join("; ", errors));
        }
        
        return ValidationResult.Success();
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

    public ValidationResult AddError(string errorMessage)
    {
        if (IsValid)
        {
            IsValid = false;
            ErrorMessages = new List<string>();
        }
        
        ErrorMessages.Add(errorMessage);
        ErrorMessage = string.Join("; ", ErrorMessages);
        
        return this;
    }
}