using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Associados.Common;

public interface IExcelService
{
    Task<(byte[] bytes, string fileName)> ExportarAssociadosAsync();
    Task<(byte[] bytes, string fileName)> ExportarPromocoesAsync();
    Task<ImportResult> ImportarAssociadosAsync(IBrowserFile file);
    Task<ImportResult> ImportarPromocoesAsync(IBrowserFile file);
}

public class ExcelService : IExcelService
{
    private readonly ApplicationDbContext _context;

    public ExcelService(ApplicationDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<(byte[] bytes, string fileName)> ExportarAssociadosAsync()
    {
        var associados = await _context.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Perfil)
            .Include(a => a.Mentor)
            .Include(a => a.Vertical)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Associados");

        // Cabeçalhos
        worksheet.Cells[1, 1].Value = "IdAssociado";
        worksheet.Cells[1, 2].Value = "Nome";
        worksheet.Cells[1, 3].Value = "IdCargo";
        worksheet.Cells[1, 4].Value = "Cargo";
        worksheet.Cells[1, 5].Value = "IdPerfil";
        worksheet.Cells[1, 6].Value = "Perfil";
        worksheet.Cells[1, 7].Value = "IdMentor";
        worksheet.Cells[1, 8].Value = "Mentor";
        worksheet.Cells[1, 9].Value = "Email";
        worksheet.Cells[1, 10].Value = "DataAdmissao";
        worksheet.Cells[1, 11].Value = "IdVertical";
        worksheet.Cells[1, 12].Value = "Vertical";
        worksheet.Cells[1, 13].Value = "Ativo";

        // Dados
        for (int i = 0; i < associados.Count; i++)
        {
            var associado = associados[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = associado.Id;
            worksheet.Cells[row, 2].Value = associado.Nome;
            worksheet.Cells[row, 3].Value = associado.IdCargo;
            worksheet.Cells[row, 4].Value = associado.Cargo?.Nome ?? string.Empty;
            worksheet.Cells[row, 5].Value = associado.IdPerfil;
            worksheet.Cells[row, 6].Value = associado.Perfil?.Nome ?? string.Empty;
            worksheet.Cells[row, 7].Value = associado.IdMentor;
            worksheet.Cells[row, 8].Value = associado.Mentor?.Nome ?? string.Empty;
            worksheet.Cells[row, 9].Value = associado.Email;
            worksheet.Cells[row, 10].Value = associado.DataAdmissao?.ToString("dd/MM/yyyy") ?? string.Empty;
            worksheet.Cells[row, 11].Value = associado.IdVertical;
            worksheet.Cells[row, 12].Value = associado.Vertical?.Nome ?? string.Empty;
            worksheet.Cells[row, 13].Value = associado.Ativo;
        }

        worksheet.Cells.AutoFitColumns();
        
        var fileName = $"Associados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return (package.GetAsByteArray(), fileName);
    }

    public async Task<(byte[] bytes, string fileName)> ExportarPromocoesAsync()
    {
        var promocoes = await _context.Promocoes
            .Include(p => p.Associado)
            .Include(p => p.CargoAnterior)
            .Include(p => p.CargoNovo)
            .ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Promocoes");

        // Cabeçalhos
        worksheet.Cells[1, 1].Value = "IdPromocao";
        worksheet.Cells[1, 2].Value = "IdAssociado";
        worksheet.Cells[1, 3].Value = "Associado";
        worksheet.Cells[1, 4].Value = "IdCargoAnterior";
        worksheet.Cells[1, 5].Value = "CargoAnterior";
        worksheet.Cells[1, 6].Value = "IdCargoNovo";
        worksheet.Cells[1, 7].Value = "CargoNovo";
        worksheet.Cells[1, 8].Value = "DataPromocao";
        worksheet.Cells[1, 9].Value = "Comentarios";
        worksheet.Cells[1, 10].Value = "Ativo";

        // Dados
        for (int i = 0; i < promocoes.Count; i++)
        {
            var promocao = promocoes[i];
            var row = i + 2;

            worksheet.Cells[row, 1].Value = promocao.Id;
            worksheet.Cells[row, 2].Value = promocao.IdAssociado;
            worksheet.Cells[row, 3].Value = promocao.Associado?.Nome ?? string.Empty;
            worksheet.Cells[row, 4].Value = promocao.IdCargoAnterior;
            worksheet.Cells[row, 5].Value = promocao.CargoAnterior?.Nome ?? string.Empty;
            worksheet.Cells[row, 6].Value = promocao.IdCargoNovo;
            worksheet.Cells[row, 7].Value = promocao.CargoNovo?.Nome ?? string.Empty;
            worksheet.Cells[row, 8].Value = promocao.DataPromocao.ToString("dd/MM/yyyy");
            worksheet.Cells[row, 9].Value = promocao.Comentarios;
            worksheet.Cells[row, 10].Value = promocao.Ativo;
        }

        worksheet.Cells.AutoFitColumns();
        
        var fileName = $"Promocoes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return (package.GetAsByteArray(), fileName);
    }

    public async Task<ImportResult> ImportarAssociadosAsync(IBrowserFile file)
    {
        var result = new ImportResult();
        
        using var stream = file.OpenReadStream(10 * 1024 * 1024); // 10MB max
        using var package = new ExcelPackage(stream);
        
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
            throw new InvalidOperationException("Planilha não encontrada no arquivo");

        var rowCount = worksheet.Dimension?.End.Row ?? 0;
        
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var idAssociado = worksheet.Cells[row, 1].GetValue<int?>();
                var nome = worksheet.Cells[row, 2].GetValue<string>();
                var idCargo = worksheet.Cells[row, 3].GetValue<int?>();
                var idPerfil = worksheet.Cells[row, 5].GetValue<int?>();
                var idMentor = worksheet.Cells[row, 7].GetValue<int?>();
                var email = worksheet.Cells[row, 9].GetValue<string>();
                var dataAdmissao = worksheet.Cells[row, 10].GetValue<DateTime?>();
                var idVertical = worksheet.Cells[row, 11].GetValue<int?>();
                var ativo = worksheet.Cells[row, 13].GetValue<bool?>();

                if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || 
                    !idCargo.HasValue || !idPerfil.HasValue || !idMentor.HasValue)
                {
                    result.Desconsiderados++;
                    continue;
                }

                var associadoExistente = idAssociado.HasValue && idAssociado > 0 
                    ? await _context.Associados.FindAsync(idAssociado.Value)
                    : null;

                if (associadoExistente == null)
                {
                    var novoAssociado = new Associado
                    {
                        Nome = nome,
                        Email = email,
                        IdCargo = idCargo.Value,
                        IdPerfil = idPerfil.Value,
                        IdMentor = idMentor.Value,
                        IdVertical = idVertical ?? 1,
                        DataAdmissao = dataAdmissao ?? DateTime.Now,
                        Ativo = ativo ?? true,
                        Senha = "avaliacao",
                        DataCriacao = DateTime.Now,
                        DataAlteracao = DateTime.Now
                    };
                    
                    _context.Associados.Add(novoAssociado);
                    result.Inseridos++;
                }
                else
                {
                    associadoExistente.Nome = nome;
                    associadoExistente.Email = email;
                    associadoExistente.IdCargo = idCargo.Value;
                    associadoExistente.IdPerfil = idPerfil.Value;
                    associadoExistente.IdMentor = idMentor.Value;
                    associadoExistente.IdVertical = idVertical ?? associadoExistente.IdVertical;
                    associadoExistente.DataAdmissao = dataAdmissao ?? associadoExistente.DataAdmissao;
                    associadoExistente.Ativo = ativo ?? associadoExistente.Ativo;
                    associadoExistente.DataAlteracao = DateTime.Now;
                    
                    result.Alterados++;
                }
            }
            catch
            {
                result.Desconsiderados++;
            }
        }

        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<ImportResult> ImportarPromocoesAsync(IBrowserFile file)
    {
        var result = new ImportResult();
        
        using var stream = file.OpenReadStream(10 * 1024 * 1024);
        using var package = new ExcelPackage(stream);
        
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
            throw new InvalidOperationException("Planilha não encontrada no arquivo");

        var rowCount = worksheet.Dimension?.End.Row ?? 0;
        
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var idPromocao = worksheet.Cells[row, 1].GetValue<int?>();
                var idAssociado = worksheet.Cells[row, 2].GetValue<int?>();
                var idCargoAnterior = worksheet.Cells[row, 4].GetValue<int?>();
                var idCargoNovo = worksheet.Cells[row, 6].GetValue<int?>();
                var dataPromocao = worksheet.Cells[row, 8].GetValue<DateTime?>();
                var comentarios = worksheet.Cells[row, 9].GetValue<string>() ?? "Import";
                var ativo = worksheet.Cells[row, 10].GetValue<bool?>() ?? true;

                if (!idAssociado.HasValue || !idCargoAnterior.HasValue || 
                    !idCargoNovo.HasValue || !dataPromocao.HasValue)
                {
                    result.Desconsiderados++;
                    continue;
                }

                var promocaoExistente = idPromocao.HasValue && idPromocao > 0 
                    ? await _context.Promocoes.FindAsync(idPromocao.Value)
                    : null;

                if (promocaoExistente == null)
                {
                    var novaPromocao = new Promocao
                    {
                        IdAssociado = idAssociado.Value,
                        IdCargoAnterior = idCargoAnterior.Value,
                        IdCargoNovo = idCargoNovo.Value,
                        DataPromocao = dataPromocao.Value,
                        Comentarios = comentarios,
                        Ativo = ativo,
                        DataCriacao = DateTime.Now
                    };
                    
                    _context.Promocoes.Add(novaPromocao);
                    result.Inseridos++;
                }
                else
                {
                    promocaoExistente.IdAssociado = idAssociado.Value;
                    promocaoExistente.IdCargoAnterior = idCargoAnterior.Value;
                    promocaoExistente.IdCargoNovo = idCargoNovo.Value;
                    promocaoExistente.DataPromocao = dataPromocao.Value;
                    promocaoExistente.Comentarios = comentarios;
                    promocaoExistente.Ativo = ativo;
                    
                    result.Alterados++;
                }
            }
            catch
            {
                result.Desconsiderados++;
            }
        }

        await _context.SaveChangesAsync();
        return result;
    }
}

public class ImportResult
{
    public int Inseridos { get; set; }
    public int Alterados { get; set; }
    public int Desconsiderados { get; set; }
}