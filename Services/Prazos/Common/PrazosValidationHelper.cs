using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Prazos.Common;

public interface IPrazosValidationHelper
{
    ValidationResult ValidatePrazo(PrazoFormModel model);
    bool TryParseInt(string value, out int result);
    List<GatilhoOption> GetGatilhoOptions(TipoGatilho tipo);
}

public class PrazosValidationHelper : IPrazosValidationHelper
{
    public ValidationResult ValidatePrazo(PrazoFormModel model)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(model.NomeDisparo))
            errors.Add("Nome do Disparo é obrigatório");

        if (!TryParseInt(model.DuracaoAutoAvaliacao, out _))
            errors.Add("Duração Auto Avaliação deve ser um número válido");

        if (!TryParseInt(model.CompensacaoAutoAvaliacao, out _))
            errors.Add("Compensação Auto Avaliação deve ser um número válido");

        if (!TryParseInt(model.DuracaoAvaliacaoAsCegas, out _))
            errors.Add("Duração Avaliação às Cegas deve ser um número válido");

        if (!TryParseInt(model.CompensacaoAvaliacaoAsCegas, out _))
            errors.Add("Compensação Avaliação às Cegas deve ser um número válido");

        if (!TryParseInt(model.DuracaoAvaliacaoGestor, out _))
            errors.Add("Duração Avaliação do Gestor deve ser um número válido");

        if (!TryParseInt(model.CompensacaoAvaliacaoGestor, out _))
            errors.Add("Compensação Avaliação do Gestor deve ser um número válido");

        if (!TryParseInt(model.DuracaoFeedback, out _))
            errors.Add("Duração Feedback deve ser um número válido");

        if (!TryParseInt(model.CompensacaoFeedback, out _))
            errors.Add("Compensação Feedback deve ser um número válido");

        if (!TryParseInt(model.DuracaoMentor, out _))
            errors.Add("Duração Consolidação do Mentor deve ser um número válido");

        if (!TryParseInt(model.CompensacaoMentor, out _))
            errors.Add("Compensação Consolidação do Mentor deve ser um número válido");

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }

    public bool TryParseInt(string value, out int result)
    {
        result = 0;
        return !string.IsNullOrWhiteSpace(value) && int.TryParse(value.Trim(), out result);
    }

    public List<GatilhoOption> GetGatilhoOptions(TipoGatilho tipo)
    {
        var baseOptions = new List<GatilhoOption>
        {
            new GatilhoOption { Value = 0, Text = "Ao disparo da auto-avaliação" }
        };

        switch (tipo)
        {
            case TipoGatilho.AutoAvaliacao:
                return baseOptions;
                
            case TipoGatilho.AvaliacaoAsCegas:
                baseOptions.Add(new GatilhoOption { Value = 1, Text = "Ao encerrar a auto-avaliação" });
                return baseOptions;
                
            case TipoGatilho.AvaliacaoGestor:
                baseOptions.Add(new GatilhoOption { Value = 1, Text = "Ao encerrar a auto-avaliação" });
                baseOptions.Add(new GatilhoOption { Value = 2, Text = "Ao encerrar a avaliação às cegas" });
                return baseOptions;
                
            case TipoGatilho.Feedback:
                baseOptions.Add(new GatilhoOption { Value = 1, Text = "Ao encerrar a auto-avaliação" });
                baseOptions.Add(new GatilhoOption { Value = 2, Text = "Ao encerrar a avaliação às cegas" });
                baseOptions.Add(new GatilhoOption { Value = 3, Text = "Ao encerrar a avaliação do gestor" });
                return baseOptions;
                
            case TipoGatilho.Mentor:
                baseOptions.Add(new GatilhoOption { Value = 1, Text = "Ao encerrar a auto-avaliação" });
                baseOptions.Add(new GatilhoOption { Value = 2, Text = "Ao encerrar a avaliação às cegas" });
                baseOptions.Add(new GatilhoOption { Value = 3, Text = "Ao encerrar a avaliação do gestor" });
                baseOptions.Add(new GatilhoOption { Value = 4, Text = "Ao encerrar o feedback" });
                return baseOptions;
                
            default:
                return baseOptions;
        }
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public string ErrorMessage => string.Join("; ", Errors);
}

public class PrazoFormModel
{
    public int IdPrazo { get; set; }
    public string NomeDisparo { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    
    public string DuracaoAutoAvaliacao { get; set; } = string.Empty;
    public int GatilhoAutoAvaliacao { get; set; }
    public string CompensacaoAutoAvaliacao { get; set; } = string.Empty;
    
    public string DuracaoAvaliacaoAsCegas { get; set; } = string.Empty;
    public int GatilhoAvaliacaoAsCegas { get; set; }
    public string CompensacaoAvaliacaoAsCegas { get; set; } = string.Empty;
    
    public string DuracaoAvaliacaoGestor { get; set; } = string.Empty;
    public int GatilhoAvaliacaoGestor { get; set; }
    public string CompensacaoAvaliacaoGestor { get; set; } = string.Empty;
    
    public string DuracaoFeedback { get; set; } = string.Empty;
    public int GatilhoFeedback { get; set; }
    public string CompensacaoFeedback { get; set; } = string.Empty;
    
    public string DuracaoMentor { get; set; } = string.Empty;
    public int GatilhoMentor { get; set; }
    public string CompensacaoMentor { get; set; } = string.Empty;
}

public class GatilhoOption
{
    public int Value { get; set; }
    public string Text { get; set; } = string.Empty;
}

public enum TipoGatilho
{
    AutoAvaliacao,
    AvaliacaoAsCegas,
    AvaliacaoGestor,
    Feedback,
    Mentor
}