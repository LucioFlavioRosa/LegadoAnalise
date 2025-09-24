using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Cargos;
using System.Data;

namespace Peers.Moderno.Services.Performance;

public interface IPerformanceService
{
    Task<List<Models.Performance>> ObterListaPerformancesAsync();
    Task<Models.Performance?> ObterPerformanceAsync(int id);
    Task<bool> InserirPerformanceAsync(Models.Performance performance);
    Task<bool> AlterarPerformanceAsync(Models.Performance performance);
    Task<bool> ExcluirPerformanceAsync(int id);
    Task<PerformanceValidationResult> ValidarPerformanceAsync(Models.Performance performance);
    Task<byte[]> ExportarPerformancesAsync();
    Task<PerformanceImportResult> ImportarPerformancesAsync(Stream fileStream, int userId);
    Task<List<Models.Performance>> ObterPerformancesPorCargoAsync(int idCargo);
    Task<bool> PerformanceExisteAsync(int idCargo, string nomePerformance, int? idPerformanceExcluir = null);
}

public class PerformanceService : IPerformanceService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;
    private readonly ICargosService _cargosService;
    private readonly IMessageBoxService _messageBoxService;

    public PerformanceService(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        ICargosService cargosService,
        IMessageBoxService messageBoxService)
    {
        _context = context;
        _telemetryService = telemetryService;
        _cargosService = cargosService;
        _messageBoxService = messageBoxService;
    }

    public async Task<List<Models.Performance>> ObterListaPerformancesAsync()
    {
        try
        {
            var performances = await _context.Set<Models.Performance>()
                .Include(p => p.Cargo)
                .Include(p => p.CargoNivel)
                .OrderBy(p => p.IdPerformance)
                .ToListAsync();

            _telemetryService.TrackEvent("PerformancesListLoaded", new Dictionary<string, string>
            {
                { "Count", performances.Count.ToString() }
            });

            return performances;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterListaPerformancesAsync" },
                { "Component", "PerformanceService" }
            });
            throw;
        }
    }

    public async Task<Models.Performance?> ObterPerformanceAsync(int id)
    {
        try
        {
            var performance = await _context.Set<Models.Performance>()
                .Include(p => p.Cargo)
                .Include(p => p.CargoNivel)
                .Include(p => p.NotaAutoAvaliacao)
                .Include(p => p.NotaAvaliacaoAsCegas)
                .Include(p => p.NotaAvaliacaoGestor)
                .FirstOrDefaultAsync(p => p.IdPerformance == id);

            if (performance != null)
            {
                _telemetryService.TrackEvent("PerformanceLoaded", new Dictionary<string, string>
                {
                    { "PerformanceId", id.ToString() }
                });
            }

            return performance;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPerformanceAsync" },
                { "Component", "PerformanceService" },
                { "PerformanceId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InserirPerformanceAsync(Models.Performance performance)
    {
        try
        {
            var validationResult = await ValidarPerformanceAsync(performance);
            if (!validationResult.IsValid)
            {
                _messageBoxService.ShowWarning(validationResult.ErrorMessage);
                return false;
            }

            performance.DHC = DateTime.Now;
            _context.Set<Models.Performance>().Add(performance);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerformanceInserted", new Dictionary<string, string>
                {
                    { "PerformanceId", performance.IdPerformance.ToString() },
                    { "CargoId", performance.IdCargo.ToString() },
                    { "UserId", performance.USR.ToString() }
                });
                _messageBoxService.ShowSuccess("Performance inserida com sucesso!");
            }
            else
            {
                _messageBoxService.ShowError("Erro ao inserir performance");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "InserirPerformanceAsync" },
                { "Component", "PerformanceService" }
            });
            _messageBoxService.ShowError("Erro interno ao inserir performance");
            return false;
        }
    }

    public async Task<bool> AlterarPerformanceAsync(Models.Performance performance)
    {
        try
        {
            var validationResult = await ValidarPerformanceAsync(performance);
            if (!validationResult.IsValid)
            {
                _messageBoxService.ShowWarning(validationResult.ErrorMessage);
                return false;
            }

            var existingPerformance = await _context.Set<Models.Performance>()
                .FirstOrDefaultAsync(p => p.IdPerformance == performance.IdPerformance);

            if (existingPerformance == null)
            {
                _messageBoxService.ShowError("Performance não encontrada");
                return false;
            }

            // Atualizar propriedades
            existingPerformance.IdCargo = performance.IdCargo;
            existingPerformance.IdNivel = performance.IdNivel;
            existingPerformance.PerformanceNome = performance.PerformanceNome;
            existingPerformance.PerformanceAbaixo = performance.PerformanceAbaixo;
            existingPerformance.PerformanceEsperado = performance.PerformanceEsperado;
            existingPerformance.PerformanceAcima = performance.PerformanceAcima;
            existingPerformance.Abrangencia = performance.Abrangencia;
            existingPerformance.InputAutoavaliacao = performance.InputAutoavaliacao;
            existingPerformance.NotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao;
            existingPerformance.InputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas;
            existingPerformance.NotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas;
            existingPerformance.InputAvaliacaoGestor = performance.InputAvaliacaoGestor;
            existingPerformance.NotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor;
            existingPerformance.ATV = performance.ATV;
            existingPerformance.USR = performance.USR;
            existingPerformance.DHC = DateTime.Now;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerformanceUpdated", new Dictionary<string, string>
                {
                    { "PerformanceId", performance.IdPerformance.ToString() },
                    { "CargoId", performance.IdCargo.ToString() },
                    { "UserId", performance.USR.ToString() }
                });
                _messageBoxService.ShowSuccess("Performance alterada com sucesso!");
            }
            else
            {
                _messageBoxService.ShowError("Erro ao alterar performance");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "AlterarPerformanceAsync" },
                { "Component", "PerformanceService" },
                { "PerformanceId", performance.IdPerformance.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao alterar performance");
            return false;
        }
    }

    public async Task<bool> ExcluirPerformanceAsync(int id)
    {
        try
        {
            var performance = await _context.Set<Models.Performance>()
                .FirstOrDefaultAsync(p => p.IdPerformance == id);

            if (performance == null)
            {
                _messageBoxService.ShowError("Performance não encontrada");
                return false;
            }

            // Inativar ao invés de excluir fisicamente
            performance.ATV = 0;
            performance.DHC = DateTime.Now;

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                _telemetryService.TrackEvent("PerformanceInactivated", new Dictionary<string, string>
                {
                    { "PerformanceId", id.ToString() }
                });
                _messageBoxService.ShowSuccess("Performance inativada com sucesso!");
            }
            else
            {
                _messageBoxService.ShowError("Erro ao inativar performance");
            }

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExcluirPerformanceAsync" },
                { "Component", "PerformanceService" },
                { "PerformanceId", id.ToString() }
            });
            _messageBoxService.ShowError("Erro interno ao inativar performance");
            return false;
        }
    }

    public async Task<PerformanceValidationResult> ValidarPerformanceAsync(Models.Performance performance)
    {
        var errors = new List<string>();

        try
        {
            if (performance.IdCargo <= 0)
                errors.Add("Selecione o campo Cargo");

            if (string.IsNullOrWhiteSpace(performance.PerformanceNome))
                errors.Add("Preencha o campo Performance");

            if (string.IsNullOrWhiteSpace(performance.PerformanceAbaixo))
                errors.Add("Preencha o campo Performance Abaixo");

            if (string.IsNullOrWhiteSpace(performance.PerformanceEsperado))
                errors.Add("Preencha o campo Performance Esperado");

            if (string.IsNullOrWhiteSpace(performance.PerformanceAcima))
                errors.Add("Preencha o campo Performance Acima");

            // Validar notas padrão quando input está desabilitado
            if (!performance.InputAutoavaliacao && !performance.NotaPadraoAutoAvaliacao.HasValue)
                errors.Add("Selecione a nota padrão para auto avaliação");

            if (!performance.InputAvaliacaoAsCegas && !performance.NotaPadraoAvaliacaoAsCegas.HasValue)
                errors.Add("Selecione a nota padrão para avaliação às cegas");

            if (!performance.InputAvaliacaoGestor && !performance.NotaPadraoAvaliacaoGestor.HasValue)
                errors.Add("Selecione a nota padrão para avaliação do gestor");

            // Verificar duplicidade
            if (await PerformanceExisteAsync(performance.IdCargo, performance.PerformanceNome, performance.IdPerformance))
                errors.Add("Já existe uma performance com este nome para o cargo selecionado");

            return errors.Any() 
                ? PerformanceValidationResult.Failure(errors.ToArray())
                : PerformanceValidationResult.Success();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPerformanceAsync" },
                { "Component", "PerformanceService" }
            });
            return PerformanceValidationResult.Failure("Erro interno na validação");
        }
    }

    public async Task<byte[]> ExportarPerformancesAsync()
    {
        try
        {
            var performances = await ObterListaPerformancesAsync();
            var exportData = new List<PerformanceExportModel>();

            foreach (var performance in performances)
            {
                var cargo = await _cargosService.ObterCargoAsync(performance.IdCargo);
                var notaAutoAvaliacao = performance.NotaPadraoAutoAvaliacao.HasValue 
                    ? await ObterNotaPerformanceAsync(performance.NotaPadraoAutoAvaliacao.Value) 
                    : null;
                var notaAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas.HasValue 
                    ? await ObterNotaPerformanceAsync(performance.NotaPadraoAvaliacaoAsCegas.Value) 
                    : null;
                var notaAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor.HasValue 
                    ? await ObterNotaPerformanceAsync(performance.NotaPadraoAvaliacaoGestor.Value) 
                    : null;

                exportData.Add(new PerformanceExportModel
                {
                    IdPerformance = performance.IdPerformance,
                    IdCargo = performance.IdCargo,
                    Cargo = cargo?.Nome ?? "",
                    Performance = performance.PerformanceNome,
                    DescricaoAbaixo = performance.PerformanceAbaixo,
                    DescricaoEsperado = performance.PerformanceEsperado,
                    DescricaoAcima = performance.PerformanceAcima,
                    ATV = performance.ATV,
                    Abrangencia = performance.Abrangencia,
                    InputAutoAvaliacao = performance.InputAutoavaliacao ? 1 : 0,
                    IdNotaPadraoAutoAvaliacao = performance.NotaPadraoAutoAvaliacao,
                    NotaPadraoAutoAvaliacao = notaAutoAvaliacao?.CodigoNota ?? "",
                    InputAvaliacaoAsCegas = performance.InputAvaliacaoAsCegas ? 1 : 0,
                    IdNotaPadraoAvaliacaoAsCegas = performance.NotaPadraoAvaliacaoAsCegas,
                    NotaPadraoAvaliacaoAsCegas = notaAvaliacaoAsCegas?.CodigoNota ?? "",
                    InputAvaliacaoGestor = performance.InputAvaliacaoGestor ? 1 : 0,
                    IdNotaPadraoAvaliacaoGestor = performance.NotaPadraoAvaliacaoGestor,
                    NotaPadraoAvaliacaoGestor = notaAvaliacaoGestor?.CodigoNota ?? ""
                });
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Performances");

            // Headers
            var headers = new string[]
            {
                "IdPerformance", "IdCargo", "Cargo", "Performance", "DescricaoAbaixo",
                "DescricaoEsperado", "DescricaoAcima", "ATV", "Abrangencia",
                "InputAutoAvaliacao", "IdNotaPadraoAutoAvaliacao", "NotaPadraoAutoAvaliacao",
                "InputAvaliacaoAsCegas", "IdNotaPadraoAvaliacaoAsCegas", "NotaPadraoAvaliacaoAsCegas",
                "InputAvaliacaoGestor", "IdNotaPadraoAvaliacaoGestor", "NotaPadraoAvaliacaoGestor"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // Data
            for (int i = 0; i < exportData.Count; i++)
            {
                var item = exportData[i];
                worksheet.Cells[i + 2, 1].Value = item.IdPerformance;
                worksheet.Cells[i + 2, 2].Value = item.IdCargo;
                worksheet.Cells[i + 2, 3].Value = item.Cargo;
                worksheet.Cells[i + 2, 4].Value = item.Performance;
                worksheet.Cells[i + 2, 5].Value = item.DescricaoAbaixo;
                worksheet.Cells[i + 2, 6].Value = item.DescricaoEsperado;
                worksheet.Cells[i + 2, 7].Value = item.DescricaoAcima;
                worksheet.Cells[i + 2, 8].Value = item.ATV;
                worksheet.Cells[i + 2, 9].Value = item.Abrangencia;
                worksheet.Cells[i + 2, 10].Value = item.InputAutoAvaliacao;
                worksheet.Cells[i + 2, 11].Value = item.IdNotaPadraoAutoAvaliacao;
                worksheet.Cells[i + 2, 12].Value = item.NotaPadraoAutoAvaliacao;
                worksheet.Cells[i + 2, 13].Value = item.InputAvaliacaoAsCegas;
                worksheet.Cells[i + 2, 14].Value = item.IdNotaPadraoAvaliacaoAsCegas;
                worksheet.Cells[i + 2, 15].Value = item.NotaPadraoAvaliacaoAsCegas;
                worksheet.Cells[i + 2, 16].Value = item.InputAvaliacaoGestor;
                worksheet.Cells[i + 2, 17].Value = item.IdNotaPadraoAvaliacaoGestor;
                worksheet.Cells[i + 2, 18].Value = item.NotaPadraoAvaliacaoGestor;
            }

            worksheet.Cells.AutoFitColumns();

            _telemetryService.TrackEvent("PerformancesExported", new Dictionary<string, string>
            {
                { "Count", exportData.Count.ToString() }
            });

            return package.GetAsByteArray();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ExportarPerformancesAsync" },
                { "Component", "PerformanceService" }
            });
            throw;
        }
    }

    public async Task<PerformanceImportResult> ImportarPerformancesAsync(Stream fileStream, int userId)
    {
        var result = new PerformanceImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
            {
                result.Erros.Add("Planilha não encontrada");
                return result;
            }

            var dt = new DataTable();
            bool hasHeader = true;
            
            // Criar colunas
            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                dt.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
            }

            // Ler dados
            var startRow = hasHeader ? 2 : 1;
            for (int rowNum = startRow; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                DataRow row = dt.NewRow();
                foreach (var cell in wsRow)
                {
                    row[cell.Start.Column - 1] = cell.Text;
                }
                dt.Rows.Add(row);
            }

            // Processar cada linha
            foreach (DataRow item in dt.Rows)
            {
                try
                {
                    var performance = await ProcessarLinhaImportacaoAsync(item, userId);
                    if (performance != null)
                    {
                        var idPerformance = !string.IsNullOrWhiteSpace(item[0]?.ToString()) ? Convert.ToInt32(item[0]) : 0;
                        
                        if (idPerformance == 0)
                        {
                            await InserirPerformanceAsync(performance);
                            result.LinhasInseridas++;
                        }
                        else
                        {
                            performance.IdPerformance = idPerformance;
                            await AlterarPerformanceAsync(performance);
                            result.LinhasAlteradas++;
                        }
                    }
                    else
                    {
                        result.LinhasDesconsideradas++;
                    }
                }
                catch (Exception ex)
                {
                    result.Erros.Add($"Erro na linha: {ex.Message}");
                    result.LinhasDesconsideradas++;
                }
            }

            _telemetryService.TrackEvent("PerformancesImported", new Dictionary<string, string>
            {
                { "LinhasInseridas", result.LinhasInseridas.ToString() },
                { "LinhasAlteradas", result.LinhasAlteradas.ToString() },
                { "LinhasDesconsideradas", result.LinhasDesconsideradas.ToString() },
                { "UserId", userId.ToString() }
            });

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ImportarPerformancesAsync" },
                { "Component", "PerformanceService" },
                { "UserId", userId.ToString() }
            });
            result.Erros.Add($"Erro geral na importação: {ex.Message}");
            return result;
        }
    }

    public async Task<List<Models.Performance>> ObterPerformancesPorCargoAsync(int idCargo)
    {
        try
        {
            return await _context.Set<Models.Performance>()
                .Where(p => p.IdCargo == idCargo && p.ATV == 1)
                .Include(p => p.Cargo)
                .Include(p => p.CargoNivel)
                .OrderBy(p => p.PerformanceNome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterPerformancesPorCargoAsync" },
                { "Component", "PerformanceService" },
                { "CargoId", idCargo.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> PerformanceExisteAsync(int idCargo, string nomePerformance, int? idPerformanceExcluir = null)
    {
        try
        {
            var query = _context.Set<Models.Performance>()
                .Where(p => p.IdCargo == idCargo && 
                           p.PerformanceNome.ToLower() == nomePerformance.ToLower() && 
                           p.ATV == 1);

            if (idPerformanceExcluir.HasValue)
            {
                query = query.Where(p => p.IdPerformance != idPerformanceExcluir.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "PerformanceExisteAsync" },
                { "Component", "PerformanceService" },
                { "CargoId", idCargo.ToString() },
                { "NomePerformance", nomePerformance }
            });
            return false;
        }
    }

    private async Task<Models.Performance?> ProcessarLinhaImportacaoAsync(DataRow item, int userId)
    {
        try
        {
            int IdCargo = !string.IsNullOrWhiteSpace(item[1]?.ToString()) ? Convert.ToInt32(item[1]) : 0;
            string Performance = !string.IsNullOrWhiteSpace(item[3]?.ToString()) ? item[3].ToString() : "-";
            string DescricaoAbaixo = !string.IsNullOrWhiteSpace(item[4]?.ToString()) ? item[4].ToString() : "-";
            string DescricaoEsperado = !string.IsNullOrWhiteSpace(item[5]?.ToString()) ? item[5].ToString() : "-";
            string DescricaoAcima = !string.IsNullOrWhiteSpace(item[6]?.ToString()) ? item[6].ToString() : "-";
            int ATV = !string.IsNullOrWhiteSpace(item[7]?.ToString()) ? Convert.ToInt32(item[7]) : 1;
            string Abrangencia = !string.IsNullOrWhiteSpace(item[8]?.ToString()) ? item[8].ToString() : "-";
            int InputAutoAvaliacao = !string.IsNullOrWhiteSpace(item[9]?.ToString()) ? Convert.ToInt32(item[9]) : 1;
            int? IdNotaPadraoAutoAvaliacao = !string.IsNullOrWhiteSpace(item[10]?.ToString()) ? (int?)Convert.ToInt32(item[10]) : null;
            int InputAvaliacaoAsCegas = !string.IsNullOrWhiteSpace(item[12]?.ToString()) ? Convert.ToInt32(item[12]) : 1;
            int? IdNotaAvaliacaoAsCegas = !string.IsNullOrWhiteSpace(item[13]?.ToString()) ? (int?)Convert.ToInt32(item[13]) : null;
            int InputAvaliacaoGestor = !string.IsNullOrWhiteSpace(item[15]?.ToString()) ? Convert.ToInt32(item[15]) : 1;
            int? IdNotaPadraoAvaliacaoGestor = !string.IsNullOrWhiteSpace(item[16]?.ToString()) ? (int?)Convert.ToInt32(item[16]) : null;

            if (Performance != "-" && IdCargo > 0 && DescricaoAbaixo != "-" && DescricaoEsperado != "-" && DescricaoAcima != "-" && Abrangencia != "-")
            {
                return new Models.Performance
                {
                    IdEmpresa = 1,
                    IdCargo = IdCargo,
                    IdNivel = 1,
                    PerformanceNome = Performance,
                    PerformanceAbaixo = DescricaoAbaixo,
                    PerformanceEsperado = DescricaoEsperado,
                    PerformanceAcima = DescricaoAcima,
                    Abrangencia = Abrangencia,
                    InputAutoavaliacao = InputAutoAvaliacao == 1,
                    NotaPadraoAutoAvaliacao = IdNotaPadraoAutoAvaliacao,
                    InputAvaliacaoAsCegas = InputAvaliacaoAsCegas == 1,
                    NotaPadraoAvaliacaoAsCegas = IdNotaAvaliacaoAsCegas,
                    InputAvaliacaoGestor = InputAvaliacaoGestor == 1,
                    NotaPadraoAvaliacaoGestor = IdNotaPadraoAvaliacaoGestor,
                    ATV = ATV,
                    USR = userId,
                    DHC = DateTime.Now
                };
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<AvaliacaoCompetenciaNota?> ObterNotaPerformanceAsync(int idNota)
    {
        try
        {
            return await _context.AvaliacoesCompetenciasNotas
                .FirstOrDefaultAsync(n => n.IdNota == idNota);
        }
        catch
        {
            return null;
        }
    }
}