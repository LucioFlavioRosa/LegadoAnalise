using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.FrentesInternas.Common;

namespace Peers.Moderno.Services.FrentesInternas;

public class FrentesInternasService : IFrentesInternasService
{
    private readonly ApplicationDbContext _context;
    private readonly ITelemetryService _telemetryService;
    private readonly IStatusHelper _statusHelper;

    public FrentesInternasService(
        ApplicationDbContext context,
        ITelemetryService telemetryService,
        IStatusHelper statusHelper)
    {
        _context = context;
        _telemetryService = telemetryService;
        _statusHelper = statusHelper;
    }

    public async Task<List<FrenteInternaModel>> ObterFrentesInternasAsync()
    {
        try
        {
            var frentes = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        fi.idFrenteInterna,
                        fi.FrenteInterna,
                        fi.ATV,
                        fi.DHC,
                        fi.USR,
                        COUNT(DISTINCT lfi.idLiderFrenteInterna) as TotalLideres,
                        STRING_AGG(a.Nome, '<br/>') as Lideres
                    FROM FRENTEINTERNA fi
                    LEFT JOIN LIDERESFRENTEINTERNA lfi ON fi.idFrenteInterna = lfi.idFrenteInterna AND lfi.ATV = 1
                    LEFT JOIN ASSOCIADOS a ON lfi.idAssociado = a.IdAssociado
                    GROUP BY fi.idFrenteInterna, fi.FrenteInterna, fi.ATV, fi.DHC, fi.USR
                    ORDER BY fi.FrenteInterna")
                .ToListAsync();

            var resultado = frentes.Select(f => new FrenteInternaModel
            {
                IdFrenteInterna = f.idFrenteInterna,
                FrenteInterna = f.FrenteInterna ?? string.Empty,
                ATV = f.ATV ? 1 : 0,
                TotalLideres = f.TotalLideres,
                Lideres = f.Lideres ?? string.Empty,
                DHC = f.DHC,
                USR = f.USR
            }).ToList();

            _telemetryService.TrackEvent("FrentesInternas_ObterLista", new Dictionary<string, string>
            {
                { "TotalFrentes", resultado.Count.ToString() }
            });

            return resultado;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<FrenteInternaModel?> ObterFrenteInternaAsync(int id)
    {
        try
        {
            var frente = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        idFrenteInterna,
                        FrenteInterna,
                        ATV,
                        DHC,
                        USR
                    FROM FRENTEINTERNA 
                    WHERE idFrenteInterna = {0}", id)
                .FirstOrDefaultAsync();

            if (frente == null)
                return null;

            var resultado = new FrenteInternaModel
            {
                IdFrenteInterna = frente.idFrenteInterna,
                FrenteInterna = frente.FrenteInterna ?? string.Empty,
                ATV = frente.ATV ? 1 : 0,
                DHC = frente.DHC,
                USR = frente.USR
            };

            resultado.LideresLista = await ObterLideresFrenteInternaAsync(id);
            resultado.ParticipantesLista = await ObterParticipantesFrenteInternaAsync(id);

            return resultado;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<bool> GerirFrenteInternaAsync(FrenteInternaModel frenteInterna)
    {
        try
        {
            var isUpdate = frenteInterna.IdFrenteInterna > 0;
            
            if (isUpdate)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE FRENTEINTERNA 
                    SET FrenteInterna = {0}, ATV = {1}, DHC = {2}, USR = {3}
                    WHERE idFrenteInterna = {4}",
                    frenteInterna.FrenteInterna,
                    frenteInterna.Ativo,
                    DateTime.Now,
                    frenteInterna.USR,
                    frenteInterna.IdFrenteInterna);
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO FRENTEINTERNA (FrenteInterna, ATV, DHC, USR)
                    VALUES ({0}, {1}, {2}, {3})",
                    frenteInterna.FrenteInterna,
                    frenteInterna.Ativo,
                    DateTime.Now,
                    frenteInterna.USR);
            }

            _telemetryService.TrackEvent("FrentesInternas_Gerir", new Dictionary<string, string>
            {
                { "Operacao", isUpdate ? "Update" : "Insert" },
                { "IdFrenteInterna", frenteInterna.IdFrenteInterna.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }

    public async Task<bool> ExcluirFrenteInternaAsync(int id)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE FRENTEINTERNA 
                SET ATV = 0
                WHERE idFrenteInterna = {0}", id);

            _telemetryService.TrackEvent("FrentesInternas_Excluir", new Dictionary<string, string>
            {
                { "IdFrenteInterna", id.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }

    public async Task<List<LiderFrenteInternaModel>> ObterLideresFrenteInternaAsync(int idFrenteInterna)
    {
        try
        {
            var lideres = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        lfi.idLiderFrenteInterna,
                        lfi.idFrenteInterna,
                        lfi.idAssociado,
                        a.Nome as NomeAssociado,
                        lfi.ATV,
                        lfi.DHC,
                        lfi.USR
                    FROM LIDERESFRENTEINTERNA lfi
                    INNER JOIN ASSOCIADOS a ON lfi.idAssociado = a.IdAssociado
                    WHERE lfi.idFrenteInterna = {0}
                    ORDER BY a.Nome", idFrenteInterna)
                .ToListAsync();

            return lideres.Select(l => new LiderFrenteInternaModel
            {
                IdLiderFrenteInterna = l.idLiderFrenteInterna,
                IdFrenteInterna = l.idFrenteInterna,
                IdAssociado = l.idAssociado,
                NomeAssociado = l.NomeAssociado ?? string.Empty,
                ATV = l.ATV,
                DHC = l.DHC,
                USR = l.USR
            }).ToList();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<bool> GerirLiderFrenteInternaAsync(LiderFrenteInternaModel lider)
    {
        try
        {
            var isUpdate = lider.IdLiderFrenteInterna > 0;
            
            if (isUpdate)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE LIDERESFRENTEINTERNA 
                    SET ATV = {0}, DHC = {1}, USR = {2}
                    WHERE idLiderFrenteInterna = {3}",
                    lider.ATV,
                    DateTime.Now,
                    lider.USR,
                    lider.IdLiderFrenteInterna);
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO LIDERESFRENTEINTERNA (idFrenteInterna, idAssociado, ATV, DHC, USR)
                    VALUES ({0}, {1}, {2}, {3}, {4})",
                    lider.IdFrenteInterna,
                    lider.IdAssociado,
                    lider.ATV,
                    DateTime.Now,
                    lider.USR);
            }

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }

    public async Task<bool> ExcluirLiderFrenteInternaAsync(int id)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE LIDERESFRENTEINTERNA 
                SET ATV = 0
                WHERE idLiderFrenteInterna = {0}", id);

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }

    public async Task<List<ParticipanteFrenteInternaModel>> ObterParticipantesFrenteInternaAsync(int idFrenteInterna)
    {
        try
        {
            var participantes = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        pfi.idParticipanteFrenteInterna,
                        pfi.idFrenteInterna,
                        pfi.idAssociado,
                        a.Nome as NomeAssociado,
                        pfi.ATV,
                        pfi.DHC,
                        pfi.USR
                    FROM PARTICIPANTESFRENTEINTERNA pfi
                    INNER JOIN ASSOCIADOS a ON pfi.idAssociado = a.IdAssociado
                    WHERE pfi.idFrenteInterna = {0}
                    ORDER BY a.Nome", idFrenteInterna)
                .ToListAsync();

            return participantes.Select(p => new ParticipanteFrenteInternaModel
            {
                IdParticipanteFrenteInterna = p.idParticipanteFrenteInterna,
                IdFrenteInterna = p.idFrenteInterna,
                IdAssociado = p.idAssociado,
                NomeAssociado = p.NomeAssociado ?? string.Empty,
                ATV = p.ATV,
                DHC = p.DHC,
                USR = p.USR
            }).ToList();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<bool> GerirParticipanteFrenteInternaAsync(ParticipanteFrenteInternaModel participante)
    {
        try
        {
            var isUpdate = participante.IdParticipanteFrenteInterna > 0;
            
            if (isUpdate)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE PARTICIPANTESFRENTEINTERNA 
                    SET ATV = {0}, DHC = {1}, USR = {2}
                    WHERE idParticipanteFrenteInterna = {3}",
                    participante.ATV,
                    DateTime.Now,
                    participante.USR,
                    participante.IdParticipanteFrenteInterna);
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO PARTICIPANTESFRENTEINTERNA (idFrenteInterna, idAssociado, ATV, DHC, USR)
                    VALUES ({0}, {1}, {2}, {3}, {4})",
                    participante.IdFrenteInterna,
                    participante.IdAssociado,
                    participante.ATV,
                    DateTime.Now,
                    participante.USR);
            }

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }

    public async Task<bool> ExcluirParticipanteFrenteInternaAsync(int id)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE PARTICIPANTESFRENTEINTERNA 
                SET ATV = 0
                WHERE idParticipanteFrenteInterna = {0}", id);

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }

    public async Task<List<AlocacaoExportModel>> ObterAvaliacoesAlocacaoAsync()
    {
        try
        {
            var avaliacoes = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT 
                        aai.idAvaliacaoAlocacaoInterna,
                        fi.idFrenteInterna,
                        fi.FrenteInterna,
                        aai.idPeriodo,
                        pa.Periodo,
                        aai.idAvaliador,
                        av.Nome as NomeAvaliador,
                        aai.idAssociado,
                        a.Nome as NomeAvaliado,
                        aai.idNota,
                        nai.Descricao as DescricaoNota,
                        aai.Comentarios,
                        aai.DHC,
                        aai.ValidadoMD,
                        aai.DHCValidadoMD
                    FROM AVALIACOESALOCACOESINTERNAS aai
                    INNER JOIN FRENTEINTERNA fi ON aai.idFrenteInterna = fi.idFrenteInterna
                    INNER JOIN PERIODOSAVALIACOES pa ON aai.idPeriodo = pa.idPeriodo
                    INNER JOIN ASSOCIADOS av ON aai.idAvaliador = av.IdAssociado
                    INNER JOIN ASSOCIADOS a ON aai.idAssociado = a.IdAssociado
                    INNER JOIN NOTASALOCACOESINTERNAS nai ON aai.idNota = nai.idNota
                    ORDER BY aai.idPeriodo, fi.FrenteInterna, a.Nome")
                .ToListAsync();

            return avaliacoes.Select(a => new AlocacaoExportModel
            {
                IdAvaliacaoAlocacao = a.idAvaliacaoAlocacaoInterna,
                IdAlocacaoInterna = a.idFrenteInterna,
                AlocacaoInterna = a.FrenteInterna ?? string.Empty,
                IdPeriodo = a.idPeriodo,
                Periodo = a.Periodo ?? string.Empty,
                IdLiderAlocacao = a.idAvaliador,
                LiderAlocacao = a.NomeAvaliador ?? string.Empty,
                IdAvaliado = a.idAssociado,
                Avaliado = a.NomeAvaliado ?? string.Empty,
                IdNota = a.idNota,
                Nota = a.DescricaoNota ?? string.Empty,
                Comentarios = a.Comentarios ?? string.Empty,
                DHCNota = a.DHC.ToString(),
                ValidadoMD = a.ValidadoMD ? "Validado" : "Pendente",
                DHCValidadoMD = a.DHCValidadoMD?.ToString() ?? string.Empty
            }).ToList();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<List<int>> ObterFrentesLideradasPorAssociadoAsync(int idAssociado)
    {
        try
        {
            var frentes = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT DISTINCT idFrenteInterna
                    FROM LIDERESFRENTEINTERNA
                    WHERE idAssociado = {0} AND ATV = 1", idAssociado)
                .ToListAsync();

            return frentes.Select(f => (int)f.idFrenteInterna).ToList();
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            throw;
        }
    }

    public async Task<bool> ValidarPermissaoEdicaoAsync(int idAssociado, int? idFrenteInterna = null)
    {
        try
        {
            var perfilAssociado = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT IdPerfil
                    FROM ASSOCIADOS
                    WHERE IdAssociado = {0}", idAssociado)
                .FirstOrDefaultAsync();

            if (perfilAssociado == null)
                return false;

            if (perfilAssociado.IdPerfil >= 3)
                return true;

            if (idFrenteInterna.HasValue)
            {
                var frentesLideradas = await ObterFrentesLideradasPorAssociadoAsync(idAssociado);
                return frentesLideradas.Contains(idFrenteInterna.Value);
            }

            var temFrentesLideradas = await _context.Set<dynamic>()
                .FromSqlRaw(@"
                    SELECT COUNT(*) as Total
                    FROM LIDERESFRENTEINTERNA
                    WHERE idAssociado = {0} AND ATV = 1", idAssociado)
                .FirstOrDefaultAsync();

            return temFrentesLideradas?.Total > 0;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex);
            return false;
        }
    }
}