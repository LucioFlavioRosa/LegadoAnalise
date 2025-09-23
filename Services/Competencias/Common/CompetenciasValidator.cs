using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Competencias.Common;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Competencias.Common;

public class CompetenciasValidator : ICompetenciasValidator
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public CompetenciasValidator(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public ValidationResult ValidateForCreate(Competencia competencia)
    {
        var result = new ValidationResult { IsValid = true };

        ValidateRequiredFields(competencia, result);
        ValidateBusinessRules(competencia, result);
        ValidateDuplicateCompetencia(competencia, result);

        return result;
    }

    public ValidationResult ValidateForUpdate(Competencia competencia)
    {
        var result = new ValidationResult { IsValid = true };

        if (competencia.IdCompetencia <= 0)
        {
            result.AddError("ID da competência é obrigatório para alteração");
            return result;
        }

        ValidateRequiredFields(competencia, result);
        ValidateBusinessRules(competencia, result);
        ValidateCompetenciaExists(competencia.IdCompetencia, result);

        return result;
    }

    public ValidationResult ValidateForImport(CompetenciaImportModel competencia)
    {
        var result = new ValidationResult { IsValid = true };

        if (competencia.IdCargo <= 0)
            result.AddError("ID do cargo é obrigatório");

        if (competencia.IdEixo <= 0)
            result.AddError("ID do eixo é obrigatório");

        if (competencia.IdSubCompetencia <= 0)
            result.AddError("ID da subcompetência é obrigatório");

        if (competencia.IdDimensao <= 0)
            result.AddError("ID da dimensão é obrigatório");

        if (string.IsNullOrWhiteSpace(competencia.TipoAvaliacao))
            result.AddError("Tipo de avaliação é obrigatório");
        else if (!IsValidTipoAvaliacao(competencia.TipoAvaliacao))
            result.AddError("Tipo de avaliação deve ser 'desempenho' ou 'lideranca'");

        if (string.IsNullOrWhiteSpace(competencia.Escopo))
            result.AddError("Escopo é obrigatório");

        if (string.IsNullOrWhiteSpace(competencia.DetalheNivelAtual))
            result.AddError("Detalhamento do nível atual é obrigatório");

        if (string.IsNullOrWhiteSpace(competencia.CompetenciaAtual))
            result.AddError("Competência atual é obrigatória");

        ValidateImportBusinessRules(competencia, result);

        return result;
    }

    public ValidationResult ValidateBusinessRules(Competencia competencia)
    {
        var result = new ValidationResult { IsValid = true };
        ValidateBusinessRules(competencia, result);
        return result;
    }

    public ValidationResult ValidateRelacaoCargoSubcompetencia(int idCargo, int idSubcompetencia, string descricao)
    {
        var result = new ValidationResult { IsValid = true };

        if (idCargo <= 0)
            result.AddError("ID do cargo é obrigatório");

        if (idSubcompetencia <= 0)
            result.AddError("ID da subcompetência é obrigatório");

        if (string.IsNullOrWhiteSpace(descricao))
            result.AddError("Descrição da relação cargo-subcompetência é obrigatória");

        var maxDescricaoLength = _configuration.GetValue<int>("Competencias:MaxDescricaoRelacaoLength", 1000);
        if (!string.IsNullOrWhiteSpace(descricao) && descricao.Length > maxDescricaoLength)
            result.AddError($"Descrição da relação não pode exceder {maxDescricaoLength} caracteres");

        return result;
    }

    public ValidationResult ValidateExportParameters(bool includeInactive = false)
    {
        var result = new ValidationResult { IsValid = true };

        var maxExportRecords = _configuration.GetValue<int>("Competencias:MaxExportRecords", 10000);
        var totalRecords = _context.Competencias.Count(c => includeInactive || c.ATV == 1);

        if (totalRecords > maxExportRecords)
        {
            result.AddWarning($"Exportação contém {totalRecords} registros. Considere filtrar os dados para melhor performance.");
        }

        return result;
    }

    public ValidationResult ValidateAggregationParameters(int idCompetencia, int idPeriodo)
    {
        var result = new ValidationResult { IsValid = true };

        if (idCompetencia <= 0)
            result.AddError("ID da competência é obrigatório para agregação");

        if (idPeriodo <= 0)
            result.AddError("ID do período é obrigatório para agregação");

        if (result.IsValid)
        {
            var competenciaExists = _context.Competencias.Any(c => c.IdCompetencia == idCompetencia && c.ATV == 1);
            if (!competenciaExists)
                result.AddError("Competência não encontrada ou inativa");
        }

        return result;
    }

    private void ValidateRequiredFields(Competencia competencia, ValidationResult result)
    {
        if (competencia.IdCargo <= 0)
            result.AddError("Cargo é obrigatório");

        if (competencia.IdEixo <= 0)
            result.AddError("Eixo é obrigatório");

        if (competencia.IdSubCompetencia <= 0)
            result.AddError("Sub Competência é obrigatória");

        if (competencia.IdDimensao <= 0)
            result.AddError("Dimensão é obrigatória");

        if (string.IsNullOrWhiteSpace(competencia.CompetenciaJRDetalhe))
            result.AddError("Detalhamento do Nível Atual é obrigatório");

        if (string.IsNullOrWhiteSpace(competencia.CompetenciaJR))
            result.AddError("Nível Atual é obrigatório");

        if (string.IsNullOrWhiteSpace(competencia.TipoAvaliacao))
            result.AddError("Tipo de avaliação é obrigatório");
        else if (!IsValidTipoAvaliacao(competencia.TipoAvaliacao))
            result.AddError("Tipo de avaliação deve ser 'desempenho' ou 'lideranca'");

        if (string.IsNullOrWhiteSpace(competencia.Escopo))
            result.AddError("Escopo é obrigatório");
    }

    private void ValidateBusinessRules(Competencia competencia, ValidationResult result)
    {
        if (competencia.TipoAvaliacao == "desempenho")
        {
            ValidateDesempenhoRules(competencia, result);
        }
        else if (competencia.TipoAvaliacao == "lideranca")
        {
            ValidateLiderancaRules(competencia, result);
        }

        ValidateNotasPadrao(competencia, result);
        ValidateConfiguracoesAutoPreenchimento(competencia, result);
    }

    private void ValidateDesempenhoRules(Competencia competencia, ValidationResult result)
    {
        if (competencia.IdCargo == 96)
            result.AddError("Cargo 96 é reservado para avaliações de liderança");

        if (competencia.IdDimensao == 10 && competencia.TipoAvaliacao != "lideranca")
            result.AddError("Dimensão 10 é reservada para avaliações de liderança");

        var maxPalavrasChaveLength = _configuration.GetValue<int>("Competencias:MaxPalavrasChaveLength", 500);
        if (!string.IsNullOrWhiteSpace(competencia.PalavrasChave) && competencia.PalavrasChave.Length > maxPalavrasChaveLength)
            result.AddError($"Palavras-chave não podem exceder {maxPalavrasChaveLength} caracteres");
    }

    private void ValidateLiderancaRules(Competencia competencia, ValidationResult result)
    {
        if (competencia.IdCargo != 96)
            result.AddError("Avaliações de liderança devem usar o cargo 96");

        if (competencia.IdDimensao != 10)
            result.AddError("Avaliações de liderança devem usar a dimensão 10");

        var validEscopos = new[] { "projeto", "líder", "backoffice" };
        if (!validEscopos.Contains(competencia.Escopo?.ToLower()))
            result.AddError("Escopo deve ser 'projeto', 'líder' ou 'backoffice' para avaliações de liderança");
    }

    private void ValidateNotasPadrao(Competencia competencia, ValidationResult result)
    {
        if (competencia.IdNotaPadraoNivel1.HasValue && competencia.IdNotaPadraoNivel1 > 0)
        {
            var notaExists = _context.AvaliacoesCompetenciasNotas.Any(n => n.IdNota == competencia.IdNotaPadraoNivel1 && n.ATV == 1);
            if (!notaExists)
                result.AddError("Nota padrão nível 1 não encontrada ou inativa");
        }

        if (competencia.IdNotaPadraoNivel2.HasValue && competencia.IdNotaPadraoNivel2 > 0)
        {
            var notaExists = _context.AvaliacoesCompetenciasNotas.Any(n => n.IdNota == competencia.IdNotaPadraoNivel2 && n.ATV == 1);
            if (!notaExists)
                result.AddError("Nota padrão nível 2 não encontrada ou inativa");
        }
    }

    private void ValidateConfiguracoesAutoPreenchimento(Competencia competencia, ValidationResult result)
    {
        if (!competencia.InputNivel1 && !competencia.InputNivel2)
            result.AddWarning("Pelo menos um nível de input deve estar habilitado");

        if (!competencia.VisivelNivel1 && !competencia.VisivelNivel2)
            result.AddWarning("Pelo menos um nível deve estar visível");

        if (competencia.InputAutoAvaliacao && !competencia.VisivelAutoAvaliacao)
            result.AddWarning("Auto avaliação está habilitada para input mas não está visível");

        if (competencia.InputAvaliacaoAsCegas && !competencia.VisivelAvaliacaoAsCegas)
            result.AddWarning("Avaliação às cegas está habilitada para input mas não está visível");

        if (competencia.InputAvaliacaoGestor && !competencia.VisivelAvaliacaoGestor)
            result.AddWarning("Avaliação do gestor está habilitada para input mas não está visível");

        if (competencia.InputFeedback && !competencia.VisivelFeedback)
            result.AddWarning("Feedback está habilitado para input mas não está visível");
    }

    private void ValidateDuplicateCompetencia(Competencia competencia, ValidationResult result)
    {
        var exists = _context.Competencias.Any(c =>
            c.IdCargo == competencia.IdCargo &&
            c.IdEixo == competencia.IdEixo &&
            c.IdSubCompetencia == competencia.IdSubCompetencia &&
            c.IdDimensao == competencia.IdDimensao &&
            c.TipoAvaliacao == competencia.TipoAvaliacao &&
            c.Escopo == competencia.Escopo &&
            c.ATV == 1);

        if (exists)
            result.AddError("Já existe uma competência ativa com essa combinação de cargo, eixo, subcompetência, dimensão, tipo de avaliação e escopo");
    }

    private void ValidateCompetenciaExists(int idCompetencia, ValidationResult result)
    {
        var exists = _context.Competencias.Any(c => c.IdCompetencia == idCompetencia);
        if (!exists)
            result.AddError("Competência não encontrada");
    }

    private void ValidateImportBusinessRules(CompetenciaImportModel competencia, ValidationResult result)
    {
        var cargoExists = _context.Cargos.Any(c => c.IdCargo == competencia.IdCargo && c.ATV == 1);
        if (!cargoExists)
            result.AddError($"Cargo com ID {competencia.IdCargo} não encontrado ou inativo");

        var eixoExists = _context.Eixos.Any(e => e.IdEixo == competencia.IdEixo && e.ATV == 1);
        if (!eixoExists)
            result.AddError($"Eixo com ID {competencia.IdEixo} não encontrado ou inativo");

        var subcompetenciaExists = _context.SubCompetencias.Any(s => s.IdSubCompetencia == competencia.IdSubCompetencia && s.ATV == 1);
        if (!subcompetenciaExists)
            result.AddError($"Subcompetência com ID {competencia.IdSubCompetencia} não encontrada ou inativa");

        var dimensaoExists = _context.Dimensoes.Any(d => d.IdDimensao == competencia.IdDimensao && d.ATV == 1);
        if (!dimensaoExists)
            result.AddError($"Dimensão com ID {competencia.IdDimensao} não encontrada ou inativa");

        var maxDetalheLength = _configuration.GetValue<int>("Competencias:MaxDetalheLength", 2000);
        if (!string.IsNullOrWhiteSpace(competencia.DetalheNivelAtual) && competencia.DetalheNivelAtual.Length > maxDetalheLength)
            result.AddError($"Detalhamento não pode exceder {maxDetalheLength} caracteres");

        var maxCompetenciaLength = _configuration.GetValue<int>("Competencias:MaxCompetenciaLength", 500);
        if (!string.IsNullOrWhiteSpace(competencia.CompetenciaAtual) && competencia.CompetenciaAtual.Length > maxCompetenciaLength)
            result.AddError($"Competência atual não pode exceder {maxCompetenciaLength} caracteres");
    }

    private bool IsValidTipoAvaliacao(string tipoAvaliacao)
    {
        var validTipos = new[] { "desempenho", "lideranca" };
        return validTipos.Contains(tipoAvaliacao?.ToLower());
    }
}