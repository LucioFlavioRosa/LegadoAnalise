using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Services.Common;

public class ConsultaAvancadaService : IConsultaAvancadaService
{
    private readonly ApplicationDbContext _db;
    private readonly ComboHelper _comboHelper;

    public ConsultaAvancadaService(ApplicationDbContext db, ComboHelper comboHelper)
    {
        _db = db;
        _comboHelper = comboHelper;
    }

    public async Task<List<ComboItem>> CarregarProjetosAsync(int? gestorId = null, int? clienteId = null, int? status = null, int? periodoId = null)
    {
        return await ComboHelper.GetProjetosComboAsync(_db, gestorId, clienteId, status, periodoId);
    }

    public async Task<List<ComboItem>> CarregarClientesAsync()
    {
        return await ComboHelper.GetClientesComboAsync(_db);
    }

    public async Task<List<ComboItem>> CarregarProfissionaisAsync(bool apenasAtivos = true)
    {
        var query = _db.Associados.AsQueryable();
        if (apenasAtivos)
            query = query.Where(a => a.Ativo);
        var profissionais = await query.OrderBy(a => a.Nome).ToListAsync();
        var items = new List<ComboItem> { new ComboItem { Value = "", Text = "[Selecionar]" } };
        items.AddRange(profissionais.Select(p => new ComboItem { Value = p.Id.ToString(), Text = p.Nome }));
        return items;
    }

    public async Task<List<ComboItem>> CarregarPeriodosAsync(int? empresaId = null)
    {
        return await ComboHelper.GetPeriodosComboAsync(_db, empresaId);
    }

    public async Task<List<AvaliacaoEmailModel>> ConsultarAvaliacoesAsync(
        int? projetoId,
        int? associadoId,
        int? periodoId,
        int? clienteId,
        string? fase
    )
    {
        var query = _db.AvaliacoesEmail
            .Include(a => a.PROJETOS)
                .ThenInclude(p => p.CLIENTE)
            .Include(a => a.ASSOCIADOS)
                .ThenInclude(ass => ass.Cargo)
            .Include(a => a.PERIODOSAVALIACOES)
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

        var avaliacoes = await query.ToListAsync();

        var listaAvaliacoes = new List<AvaliacaoEmailModel>();
        foreach (var avaliacao in avaliacoes)
        {
            var model = new AvaliacaoEmailModel
            {
                Id = avaliacao.idAvaliacao,
                Projeto = avaliacao.PROJETOS,
                Cliente = avaliacao.PROJETOS.CLIENTE,
                Associado = avaliacao.ASSOCIADOS,
                Gestor = null, // Preencher conforme regra de negócio se necessário
                Periodo = avaliacao.PERIODOSAVALIACOES,
                TipoAvaliacao = avaliacao.TipoAvaliacao,
                Escopo = avaliacao.Escopo,
                DataInicio = avaliacao.DataLiberacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                DataTermino = string.Empty, // Preencher conforme regra de negócio se necessário
                Status = avaliacao.AVALIACOESSTATUS,
                Fase = avaliacao.PosicaoAtualFluxoAvaliacao
            };
            listaAvaliacoes.Add(model);
        }
        return listaAvaliacoes;
    }
}
