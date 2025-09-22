using OfficeOpenXml;
using SistemaAvaliacao.Moderno.Models;
using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Data;

namespace SistemaAvaliacao.Moderno.Services;

public class ExcelService : IExcelService
{
    private readonly ApplicationDbContext _context;
    private readonly IAssociadosService _associadosService;
    private readonly IPromocoesService _promocoesService;
    
    public ExcelService(ApplicationDbContext context, IAssociadosService associadosService, IPromocoesService promocoesService)
    {
        _context = context;
        _associadosService = associadosService;
        _promocoesService = promocoesService;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    
    public async Task<byte[]> ExportarAssociadosAsync()
    {
        var associados = await _associadosService.ObterTodosAsync();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Associados");
        
        worksheet.Cells[1, 1].Value = "IdAssociado";
        worksheet.Cells[1, 2].Value = "Nome";
        worksheet.Cells[1, 3].Value = "IdCargo";
        worksheet.Cells[1, 4].Value = "Cargo";
        worksheet.Cells[1, 5].Value = "IdEmpresa";
        worksheet.Cells[1, 6].Value = "Empresa";
        worksheet.Cells[1, 7].Value = "IdPerfil";
        worksheet.Cells[1, 8].Value = "Perfil";
        worksheet.Cells[1, 9].Value = "IdMentor";
        worksheet.Cells[1, 10].Value = "Mentor";
        worksheet.Cells[1, 11].Value = "Email";
        worksheet.Cells[1, 12].Value = "DataAdmissao";
        worksheet.Cells[1, 13].Value = "Vertical";
        worksheet.Cells[1, 14].Value = "IdVertical";
        worksheet.Cells[1, 15].Value = "ATV";
        
        for (int i = 0; i < associados.Count; i++)
        {
            var associado = associados[i];
            int row = i + 2;
            
            worksheet.Cells[row, 1].Value = associado.IdAssociado;
            worksheet.Cells[row, 2].Value = associado.Nome;
            worksheet.Cells[row, 3].Value = associado.IdCargo;
            worksheet.Cells[row, 4].Value = associado.Cargo?.Nome ?? "";
            worksheet.Cells[row, 5].Value = associado.IdEmpresa;
            worksheet.Cells[row, 6].Value = "Empresa";
            worksheet.Cells[row, 7].Value = associado.IdPerfil;
            worksheet.Cells[row, 8].Value = associado.Perfil?.Nome ?? "";
            worksheet.Cells[row, 9].Value = associado.IdAssociadoMentor;
            worksheet.Cells[row, 10].Value = associado.Mentor?.Nome ?? "";
            worksheet.Cells[row, 11].Value = associado.Email;
            worksheet.Cells[row, 12].Value = associado.DataAdmissao?.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cells[row, 13].Value = associado.Vertical ?? "";
            worksheet.Cells[row, 14].Value = associado.IdVertical;
            worksheet.Cells[row, 15].Value = associado.ATV;
        }
        
        return package.GetAsByteArray();
    }
    
    public async Task<byte[]> ExportarPromocoesAsync()
    {
        var promocoes = await _promocoesService.ObterTodasAsync();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Promocoes");
        
        worksheet.Cells[1, 1].Value = "IdPromocao";
        worksheet.Cells[1, 2].Value = "IdAssociado";
        worksheet.Cells[1, 3].Value = "Associado";
        worksheet.Cells[1, 4].Value = "IdCargoAnterior";
        worksheet.Cells[1, 5].Value = "CargoAnterior";
        worksheet.Cells[1, 6].Value = "IdCargoNovo";
        worksheet.Cells[1, 7].Value = "CargoNovo";
        worksheet.Cells[1, 8].Value = "DataPromocao";
        worksheet.Cells[1, 9].Value = "Comentarios";
        worksheet.Cells[1, 10].Value = "ATV";
        
        for (int i = 0; i < promocoes.Count; i++)
        {
            var promocao = promocoes[i];
            int row = i + 2;
            
            worksheet.Cells[row, 1].Value = promocao.IdPromocao;
            worksheet.Cells[row, 2].Value = promocao.IdAssociado;
            worksheet.Cells[row, 3].Value = promocao.Associado?.Nome ?? "";
            worksheet.Cells[row, 4].Value = promocao.IdCargoAnterior;
            worksheet.Cells[row, 5].Value = promocao.CargoAnterior?.Nome ?? "";
            worksheet.Cells[row, 6].Value = promocao.IdCargoNovo;
            worksheet.Cells[row, 7].Value = promocao.CargoNovo?.Nome ?? "";
            worksheet.Cells[row, 8].Value = promocao.DataPromocao?.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cells[row, 9].Value = promocao.Comentarios ?? "";
            worksheet.Cells[row, 10].Value = promocao.ATV;
        }
        
        return package.GetAsByteArray();
    }
    
    public async Task<(int inseridos, int alterados, int desconsiderados)> ImportarAssociadosAsync(Stream fileStream)
    {
        int inseridos = 0, alterados = 0, desconsiderados = 0;
        
        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        
        if (worksheet == null) return (0, 0, 0);
        
        int rowCount = worksheet.Dimension.End.Row;
        
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                int idAssociado = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                string nome = worksheet.Cells[row, 2].GetValue<string>() ?? "";
                int idCargo = worksheet.Cells[row, 3].GetValue<int?>() ?? 0;
                int idEmpresa = worksheet.Cells[row, 5].GetValue<int?>() ?? 1;
                int idPerfil = worksheet.Cells[row, 7].GetValue<int?>() ?? 0;
                int idMentor = worksheet.Cells[row, 9].GetValue<int?>() ?? 0;
                string email = worksheet.Cells[row, 11].GetValue<string>() ?? "";
                DateTime? dataAdmissao = worksheet.Cells[row, 12].GetValue<DateTime?>();
                string vertical = worksheet.Cells[row, 13].GetValue<string>() ?? "";
                int idVertical = worksheet.Cells[row, 14].GetValue<int?>() ?? 1;
                int atv = worksheet.Cells[row, 15].GetValue<int?>() ?? 0;
                
                if (string.IsNullOrEmpty(nome) || idCargo == 0 || idPerfil == 0 || string.IsNullOrEmpty(email))
                {
                    desconsiderados++;
                    continue;
                }
                
                var associado = new Associado
                {
                    IdAssociado = idAssociado,
                    Nome = nome,
                    IdCargo = idCargo,
                    IdEmpresa = idEmpresa,
                    IdPerfil = idPerfil,
                    IdAssociadoMentor = idMentor > 0 ? idMentor : null,
                    Email = email,
                    DataAdmissao = dataAdmissao,
                    Vertical = vertical,
                    IdVertical = idVertical,
                    ATV = atv,
                    Senha = "avaliacao",
                    IdNivel = 1,
                    IdStatus = 1,
                    DHC = DateTime.Now,
                    USR = 1
                };
                
                if (idAssociado == 0)
                {
                    await _associadosService.InserirAsync(associado);
                    inseridos++;
                }
                else
                {
                    var existente = await _associadosService.ObterPorIdAsync(idAssociado);
                    if (existente != null)
                    {
                        associado.Senha = existente.Senha;
                        await _associadosService.AtualizarAsync(associado);
                        alterados++;
                    }
                    else
                    {
                        await _associadosService.InserirAsync(associado);
                        inseridos++;
                    }
                }
            }
            catch
            {
                desconsiderados++;
            }
        }
        
        return (inseridos, alterados, desconsiderados);
    }
    
    public async Task<(int inseridos, int alterados, int desconsiderados)> ImportarPromocoesAsync(Stream fileStream)
    {
        int inseridos = 0, alterados = 0, desconsiderados = 0;
        
        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        
        if (worksheet == null) return (0, 0, 0);
        
        int rowCount = worksheet.Dimension.End.Row;
        
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                int idPromocao = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                int idAssociado = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                int idCargoAnterior = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                int idCargoNovo = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                DateTime? dataPromocao = worksheet.Cells[row, 8].GetValue<DateTime?>();
                string comentarios = worksheet.Cells[row, 9].GetValue<string>() ?? "Import";
                bool atv = worksheet.Cells[row, 10].GetValue<bool?>() ?? true;
                
                if (idAssociado == 0 || idCargoAnterior == 0 || idCargoNovo == 0 || dataPromocao == null)
                {
                    desconsiderados++;
                    continue;
                }
                
                var promocao = new Promocao
                {
                    IdPromocao = idPromocao,
                    IdAssociado = idAssociado,
                    IdCargoAnterior = idCargoAnterior,
                    IdCargoNovo = idCargoNovo,
                    DataPromocao = dataPromocao,
                    Comentarios = comentarios,
                    ATV = atv,
                    DHC = DateTime.Now
                };
                
                if (idPromocao == 0)
                {
                    await _promocoesService.InserirAsync(promocao);
                    inseridos++;
                }
                else
                {
                    var existente = await _promocoesService.ObterPorIdAsync(idPromocao);
                    if (existente != null)
                    {
                        await _promocoesService.AtualizarAsync(promocao);
                        alterados++;
                    }
                    else
                    {
                        await _promocoesService.InserirAsync(promocao);
                        inseridos++;
                    }
                }
            }
            catch
            {
                desconsiderados++;
            }
        }
        
        return (inseridos, alterados, desconsiderados);
    }
}