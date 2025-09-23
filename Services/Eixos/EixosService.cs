using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Eixos.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Eixos;

public class EixosService : IEixosService
{
    private readonly ApplicationDbContext _context;
    private readonly IExportFileService _exportFileService;
    private readonly ITelemetryService _telemetryService;

    public EixosService(
        ApplicationDbContext context,
        IExportFileService exportFileService,
        ITelemetryService telemetryService)
    {
        _context = context;
        _exportFileService = exportFileService;
        _telemetryService = telemetryService;
    }

    public async Task<List<Eixo>> ListAsync()
    {
        try
        {
            var eixos = await _context.Eixos
                .OrderBy(e => e.IdEixo)
                .ToListAsync();

            _telemetryService.TrackEvent("EixosService.ListAsync", new Dictionary<string, string>
            {
                { "Count", eixos.Count.ToString() }
            });

            return eixos;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<Eixo?> GetByIdAsync(int id)
    {
        try
        {
            var eixo = await _context.Eixos
                .FirstOrDefaultAsync(e => e.IdEixo == id);

            _telemetryService.TrackEvent("EixosService.GetByIdAsync", new Dictionary<string, string>
            {
                { "Id", id.ToString() },
                { "Found", (eixo != null).ToString() }
            });

            return eixo;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<Eixo> CreateAsync(Eixo eixo)
    {
        try
        {
            eixo.DHC = DateTime.Now;
            eixo.TipoAvaliacao = "desempenho";

            _context.Eixos.Add(eixo);
            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("EixosService.CreateAsync", new Dictionary<string, string>
            {
                { "Id", eixo.IdEixo.ToString() },
                { "Nome", eixo.Nome }
            });

            return eixo;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Nome", eixo.Nome }
            });
            throw;
        }
    }

    public async Task<Eixo> UpdateAsync(Eixo eixo)
    {
        try
        {
            var existingEixo = await _context.Eixos
                .FirstOrDefaultAsync(e => e.IdEixo == eixo.IdEixo);

            if (existingEixo == null)
                throw new InvalidOperationException($"Eixo com ID {eixo.IdEixo} não encontrado");

            existingEixo.Nome = eixo.Nome;
            existingEixo.ATV = eixo.ATV;
            existingEixo.DHC = DateTime.Now;

            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("EixosService.UpdateAsync", new Dictionary<string, string>
            {
                { "Id", eixo.IdEixo.ToString() },
                { "Nome", eixo.Nome }
            });

            return existingEixo;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Id", eixo.IdEixo.ToString() }
            });
            throw;
        }
    }

    public async Task<bool> InactivateAsync(int id)
    {
        try
        {
            var eixo = await _context.Eixos
                .FirstOrDefaultAsync(e => e.IdEixo == id);

            if (eixo == null)
                return false;

            eixo.ATV = 0;
            eixo.DHC = DateTime.Now;

            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("EixosService.InactivateAsync", new Dictionary<string, string>
            {
                { "Id", id.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Id", id.ToString() }
            });
            throw;
        }
    }

    public async Task<byte[]> ExportAsync()
    {
        try
        {
            var eixos = await ListAsync();
            var fileName = $"Eixos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            var exportData = eixos.Select(e => new
            {
                IdEixo = e.IdEixo,
                Eixo = e.Nome,
                TipoAvaliacao = e.TipoAvaliacao,
                ATV = e.ATV
            }).ToList();

            var result = await _exportFileService.GenerateExcelAsync(fileName, exportData);

            _telemetryService.TrackEvent("EixosService.ExportAsync", new Dictionary<string, string>
            {
                { "Count", eixos.Count.ToString() },
                { "FileName", fileName }
            });

            return result;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }
}