using Peers.Moderno.Services.Competencias.Common.DTOs;

namespace Peers.Moderno.Services.Competencias.Common;

public interface ICompetenciasValidator
{
    ValidationResult ValidateCompetencia(CompetenciaDto competencia);
    ValidationResult ValidateForInsert(CompetenciaDto competencia);
    ValidationResult ValidateForUpdate(int id, CompetenciaDto competencia);
    ValidationResult ValidateImportData(CompetenciaImportDto importData);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    
    public static ValidationResult Success() => new ValidationResult { IsValid = true };
    public static ValidationResult Failure(params string[] errors) => new ValidationResult { IsValid = false, Errors = errors.ToList() };
    public static ValidationResult Failure(List<string> errors) => new ValidationResult { IsValid = false, Errors = errors };
}