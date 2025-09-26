namespace Services.FeedbackPerformance.Common
{
    public class FeedbackPerformanceValidator
    {
        public ValidationResult ValidateIds(int idProjeto, int idAssociado, int idPeriodo)
        {
            if (idProjeto <= 0)
                return ValidationResult.Failure("É obrigatório a seleção de um Projeto.");
            if (idAssociado <= 0)
                return ValidationResult.Failure("É obrigatório a seleção de um Associado.");
            if (idPeriodo <= 0)
                return ValidationResult.Failure("É obrigatório a seleção de um Período.");
            return ValidationResult.Success();
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public static ValidationResult Failure(string message) => new ValidationResult { IsValid = false, Message = message };
        public static ValidationResult Success() => new ValidationResult { IsValid = true };
    }
}
