using Peers.Moderno.Data;
using Peers.Moderno.Services.Competencias.Common.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Competencias.Common;

public class CompetenciasValidator : ICompetenciasValidator
{
    private readonly ApplicationDbContext _context;

    public CompetenciasValidator(ApplicationDbContext context)
    {
        _context = context;
    }

    public ValidationResult ValidateCompetencia(CompetenciaDto competencia)
    {
        var errors = new List<string>();

        if (competencia.TipoAvaliacao == "desempenho")
        {
            if (competencia.IdCargo <= 0)
                errors.Add("Favor selecionar o Cargo");
            
            if (competencia.IdEixo <= 0)
                errors.Add("Favor selecionar o Eixo");
            
            if (competencia.IdSubCompetencia <= 0)
                errors.Add("Favor selecionar a Sub Competência");
            
            if (competencia.IdDimensao <= 0)
                errors.Add("Favor selecionar a Dimensão");
            
            if (string.IsNullOrWhiteSpace(competencia.DetalhamentoNivelAtual))
                errors.Add("Favor preencher o Detalhamento do Nível Atual");
            
            if (string.IsNullOrWhiteSpace(competencia.NivelAtual))
                errors.Add("Favor preencher o Nivel Atual");
        }
        else if (competencia.TipoAvaliacao == "lideranca")
        {
            if (competencia.IdEixo <= 0)
                errors.Add("Favor selecionar o Pilar");
            
            if (competencia.IdSubCompetencia <= 0)
                errors.Add("Favor selecionar o Título");
            
            if (string.IsNullOrWhiteSpace(competencia.Escopo))
                errors.Add("Favor selecionar o Escopo");
            
            if (string.IsNullOrWhiteSpace(competencia.DetalhamentoLideranca))
                errors.Add("Favor preencher o Detalhamento");
        }
        else
        {
            errors.Add("Selecione o tipo de avaliação");
        }

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }

    public ValidationResult ValidateForInsert(CompetenciaDto competencia)
    {
        var baseValidation = ValidateCompetencia(competencia);
        if (!baseValidation.IsValid)
            return baseValidation;

        return ValidationResult.Success();
    }

    public ValidationResult ValidateForUpdate(int id, CompetenciaDto competencia)
    {
        var baseValidation = ValidateCompetencia(competencia);
        if (!baseValidation.IsValid)
            return baseValidation;

        if (id <= 0)
            return ValidationResult.Failure("ID da competência inválido");

        return ValidationResult.Success();
    }

    public ValidationResult ValidateImportData(CompetenciaImportDto importData)
    {
        var errors = new List<string>();

        if (importData.IdCargo <= 0)
            errors.Add("IdCargo deve ser maior que zero");
        
        if (importData.IdEixo <= 0)
            errors.Add("IdEixo deve ser maior que zero");
        
        if (importData.IdSubCompetencia <= 0)
            errors.Add("IdSubCompetencia deve ser maior que zero");
        
        if (importData.IdDimensao <= 0)
            errors.Add("IdDimensao deve ser maior que zero");
        
        if (string.IsNullOrWhiteSpace(importData.TipoAvaliacao))
            errors.Add("TipoAvaliacao é obrigatório");
        
        if (string.IsNullOrWhiteSpace(importData.Escopo))
            errors.Add("Escopo é obrigatório");

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }
}