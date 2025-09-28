using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Avaliacoes.Common;

namespace Services.Avaliacoes;

public class RetrocederAvaliacaoService
{
    private readonly ApplicationDbContext _db;
    private readonly RetrocederAvaliacaoValidator _validator;

    public RetrocederAvaliacaoService(ApplicationDbContext db, RetrocederAvaliacaoValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<List<AvaliacaoEmail>> BuscarAvaliacoesAsync(int? projetoId, int? associadoId, int? periodoId, int? clienteId, string? fase)
    {
        var query = _db.AvaliacoesEmail
            .Include(a => a.ASSOCIADOS)
            .Include(a => a.PROJETOS).ThenInclude(p => p.CLIENTES)
            .Include(a => a.PERIODOSAVALIACOES)
            .Include(a => a.AVALIACOESSTATUS)
            .AsQueryable();

        if (projetoId.HasValue)
            query = query.Where(a => a.PROJETOS.Id == projetoId.Value);
        if (associadoId.HasValue)
            query = query.Where(a => a.ASSOCIADOS.Id == associadoId.Value);
        if (periodoId.HasValue)
            query = query.Where(a => a.PERIODOSAVALIACOES.IdPeriodo == periodoId.Value);
        if (clienteId.HasValue)
            query = query.Where(a => a.PROJETOS.IdCliente == clienteId.Value);
        if (!string.IsNullOrEmpty(fase))
            query = query.Where(a => a.PosicaoAtualFluxoAvaliacao == fase);

        var result = await query.ToListAsync();
        return result;
    }

    public async Task<RetrocederAvaliacaoResult> RetrocederAvaliacaoAsync(int avaliacaoId)
    {
        var avaliacao = await _db.AvaliacoesEmail
            .Include(a => a.ASSOCIADOS)
            .Include(a => a.PROJETOS)
            .FirstOrDefaultAsync(a => a.idAvaliacao == avaliacaoId);
        if (avaliacao == null)
            return RetrocederAvaliacaoResult.Falha("Avaliação não encontrada.");

        var competencias = await _db.AvaliacoesCompetencias
            .Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo)
            .ToListAsync();
        var performances = await _db.AvaliacoesPerformance
            .Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo)
            .ToListAsync();

        var validacao = _validator.ValidarRetrocesso(avaliacao, competencias, performances);
        if (!validacao.Sucesso)
            return RetrocederAvaliacaoResult.Falha(validacao.Mensagem);

        var autoAvaliacaoFinalizada = competencias.Any(c => c.DataHoraFimAutoAvaliacao != null) && performances.Any(c => c.DataHoraFimAutoAvaliacao != null);
        var avaliacaoAsCegasFinalizada = competencias.Any(c => c.DataHoraFimAvaliacaoCegas != null) && performances.Any(c => c.DataHoraFimAvaliacaoCegas != null);

        return RetrocederAvaliacaoResult.Sucesso(autoAvaliacaoFinalizada, avaliacaoAsCegasFinalizada);
    }

    public async Task<EfetuarRetrocessoResult> EfetuarRetrocessoAsync(int avaliacaoId, string novaFase, bool incluirAutoAvaliacao, bool incluirAsCegas)
    {
        var avaliacao = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == avaliacaoId);
        if (avaliacao == null)
            return EfetuarRetrocessoResult.Falha("Avaliação não encontrada.");

        var competencias = await _db.AvaliacoesCompetencias
            .Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo)
            .ToListAsync();
        var performances = await _db.AvaliacoesPerformance
            .Where(x => x.IdAssociado == avaliacao.idAssociado && x.IdProjeto == avaliacao.idProjeto && x.IdPeriodo == avaliacao.idPeriodo)
            .ToListAsync();

        var validacao = _validator.ValidarEfetuarRetrocesso(avaliacao, competencias, performances, novaFase);
        if (!validacao.Sucesso)
            return EfetuarRetrocessoResult.Falha(validacao.Mensagem);

        if (novaFase == RetrocederAvaliacaoValidator.EtapaEmParalelo)
        {
            foreach (var c in competencias)
            {
                c.IdAvaliacaoStatus = 2;
                c.DataHoraFimAutoAvaliacao = incluirAutoAvaliacao ? null : c.DataHoraFimAutoAvaliacao;
                c.DataHoraFimAvaliacaoCegas = incluirAsCegas ? null : c.DataHoraFimAvaliacaoCegas;
                c.PosicaoAtualFluxoAvaliacao = novaFase;
            }
            foreach (var p in performances)
            {
                p.IdAvaliacaoStatus = 2;
                p.DataHoraFimAutoAvaliacao = incluirAutoAvaliacao ? null : p.DataHoraFimAutoAvaliacao;
                p.DataHoraFimAvaliacaoCegas = incluirAsCegas ? null : p.DataHoraFimAvaliacaoCegas;
                p.PosicaoAtualFluxoAvaliacao = novaFase;
            }
            avaliacao.PosicaoAtualFluxoAvaliacao = novaFase;
            avaliacao.idStatus = 2;
        }
        else
        {
            foreach (var c in competencias)
            {
                c.IdAvaliacaoStatus = 2;
                c.PosicaoAtualFluxoAvaliacao = novaFase;
            }
            foreach (var p in performances)
            {
                p.IdAvaliacaoStatus = 2;
                p.PosicaoAtualFluxoAvaliacao = novaFase;
            }
            avaliacao.PosicaoAtualFluxoAvaliacao = novaFase;
            avaliacao.idStatus = 2;
        }
        await _db.SaveChangesAsync();
        return EfetuarRetrocessoResult.Sucesso("Fase da avaliação atualizada com sucesso!");
    }
}

public class RetrocederAvaliacaoResult
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public bool AutoAvaliacaoFinalizada { get; set; }
    public bool AvaliacaoAsCegasFinalizada { get; set; }

    public static RetrocederAvaliacaoResult Sucesso(bool autoAvaliacaoFinalizada, bool avaliacaoAsCegasFinalizada)
    {
        return new RetrocederAvaliacaoResult
        {
            Sucesso = true,
            AutoAvaliacaoFinalizada = autoAvaliacaoFinalizada,
            AvaliacaoAsCegasFinalizada = avaliacaoAsCegasFinalizada
        };
    }
    public static RetrocederAvaliacaoResult Falha(string mensagem)
    {
        return new RetrocederAvaliacaoResult
        {
            Sucesso = false,
            Mensagem = mensagem
        };
    }
}

public class EfetuarRetrocessoResult
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }

    public static EfetuarRetrocessoResult Sucesso(string mensagem)
    {
        return new EfetuarRetrocessoResult
        {
            Sucesso = true,
            Mensagem = mensagem
        };
    }
    public static EfetuarRetrocessoResult Falha(string mensagem)
    {
        return new EfetuarRetrocessoResult
        {
            Sucesso = false,
            Mensagem = mensagem
        };
    }
}
