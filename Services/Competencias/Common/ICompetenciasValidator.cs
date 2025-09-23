using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Competencias.Common;

public interface ICompetenciasValidator
{
    ValidationResult ValidateForCreate(Competencia competencia);
    ValidationResult ValidateForUpdate(Competencia competencia);
    ValidationResult ValidateForImport(CompetenciaImportModel competencia);
    ValidationResult ValidateBusinessRules(Competencia competencia);
    ValidationResult ValidateRelacaoCargoSubcompetencia(int idCargo, int idSubcompetencia, string descricao);
    ValidationResult ValidateExportParameters(bool includeInactive = false);
    ValidationResult ValidateAggregationParameters(int idCompetencia, int idPeriodo);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Failure(params string[] errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    public static ValidationResult Warning(string warning, params string[] errors)
    {
        return new ValidationResult
        {
            IsValid = errors.Length == 0,
            Errors = errors.ToList(),
            Warnings = new List<string> { warning }
        };
    }

    public void AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
    }

    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }

    public void Merge(ValidationResult other)
    {
        Errors.AddRange(other.Errors);
        Warnings.AddRange(other.Warnings);
        if (!other.IsValid)
            IsValid = false;
    }
}

public class CompetenciaImportModel
{
    public int? IdCompetencia { get; set; }
    public int IdCargo { get; set; }
    public int IdEixo { get; set; }
    public int IdSubCompetencia { get; set; }
    public int IdDimensao { get; set; }
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int ATV { get; set; }
}