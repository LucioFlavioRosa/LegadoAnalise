using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Services.AvaliacoesGestor.Common;

namespace Services.AvaliacoesGestor;

public interface IAvaliacoesGestorService
{
    Task<List<ProjetoModel>> BuscarAvaliacoesGestorAsync(AvaliacaoGestorFiltro filtro, int userId, int userProfileId);
    Task FinalizarAvaliacaoAsync(int idAvaliacao, int userId);
    Task LiberarVisualizacaoLiderAsync(int idAvaliacao, int userId);
    Task<List<ComboItem>> CarregarProjetosAsync(int userId);
    Task<List<ComboItem>> CarregarClientesAsync();
    Task<List<ComboItem>> CarregarPeriodosAsync(int empresaId);
    Task<List<ComboItem>> CarregarStatusAsync();
    List<ComboItem> CarregarEtapas();
}

public class AvaliacoesGestorService : IAvaliacoesGestorService
{
    private readonly ApplicationDbContext _db;
    private readonly IMessageBoxService _messageBoxService;
    public AvaliacoesGestorService(ApplicationDbContext db, IMessageBoxService messageBoxService)
    {
        _db = db;
        _messageBoxService = messageBoxService;
    }

    public async Task<List<ComboItem>> CarregarProjetosAsync(int userId)
    {
        var projetos = await _db.Projetos
            .Where(p => p.AssociadoGestor.Id == userId && p.Ativo)
            .ToListAsync();
        return AvaliacoesGestorHelper.GetProjetosCombo(projetos);
    }

    public async Task<List<ComboItem>> CarregarClientesAsync()
    {
        var clientes = await _db.Clientes.Where(c => c.Ativo).ToListAsync();
        return AvaliacoesGestorHelper.GetClientesCombo(clientes);
    }

    public async Task<List<ComboItem>> CarregarPeriodosAsync(int empresaId)
    {
        var periodos = await _db.PeriodosAvaliacoes
            .Where(p => p.IdEmpresa == empresaId)
            .OrderByDescending(p => p.IdPeriodo)
            .ToListAsync();
        return AvaliacoesGestorHelper.GetPeriodosCombo(periodos);
    }

    public async Task<List<ComboItem>> CarregarStatusAsync()
    {
        var statusList = await _db.Set<PROJETOSSTATUS>().ToListAsync();
        return AvaliacoesGestorHelper.GetStatusCombo(statusList);
    }

    public List<ComboItem> CarregarEtapas()
    {
        return AvaliacoesGestorHelper.GetEtapasAvaliacaoItems();
    }

    public async Task<List<ProjetoModel>> BuscarAvaliacoesGestorAsync(AvaliacaoGestorFiltro filtro, int userId, int userProfileId)
    {
        var projetosQuery = _db.Projetos
            .Include(p => p.Cliente)
            .Include(p => p.AssociadoGestor)
            .Include(p => p.AssociadoResponsavel)
            .Where(p => p.Ativo);

        if (AvaliacoesGestorHelper.IsProjetoSelecionado(filtro.IdProjeto))
        {
            int idProjeto = int.Parse(filtro.IdProjeto);
            projetosQuery = projetosQuery.Where(p => p.Id == idProjeto);
        }
        if (AvaliacoesGestorHelper.IsClienteSelecionado(filtro.IdCliente))
        {
            int idCliente = int.Parse(filtro.IdCliente);
            projetosQuery = projetosQuery.Where(p => p.Cliente.IdCliente == idCliente);
        }
        if (AvaliacoesGestorHelper.IsStatusSelecionado(filtro.IdStatus))
        {
            int idStatus = int.Parse(filtro.IdStatus);
            projetosQuery = projetosQuery.Where(p => p.Status.IdStatus == idStatus);
        }

        var projetos = await projetosQuery.ToListAsync();
        var periodos = await _db.PeriodosAvaliacoes.ToListAsync();

        var listaProjetosAvaliacoes = new List<ProjetoModel>();

        foreach (var projeto in projetos)
        {
            var projetoModel = new ProjetoModel
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                DataInicio = AvaliacoesGestorHelper.FormatData(projeto.DataInicio),
                DataTermino = AvaliacoesGestorHelper.FormatData(projeto.DataFim),
                Gestor = projeto.AssociadoGestor,
                Responsavel = projeto.AssociadoResponsavel,
                Status = projeto.Status,
                Cliente = projeto.Cliente,
                Associados = new List<ProjetosAssociadosModel>(),
                Lideres = new List<ProjetosAssociadosModel>()
            };

            // Simulação: buscar associados do projeto e avaliações (deve ser adaptado conforme modelo real)
            var associadosProjeto = await _db.AssociadosProjetos
                .Include(ap => ap.Associado)
                .Where(ap => ap.IdProjeto == projeto.Id)
                .ToListAsync();

            foreach (var ap in associadosProjeto)
            {
                var associado = ap.Associado;
                var associadoModel = new ProjetosAssociadosModel
                {
                    Id = ap.Id,
                    Projeto = projeto,
                    Associado = associado,
                    DataInicio = AvaliacoesGestorHelper.FormatData(ap.DataInicio),
                    DataTermino = AvaliacoesGestorHelper.FormatData(ap.DataFim),
                    Periodo = periodos.FirstOrDefault(),
                    TipoAvaliacao = "desempenho",
                    Escopo = "projeto",
                    FotoAssociado = AvaliacoesGestorHelper.GetFotoAssociado(associado),
                    Gestor = projeto.AssociadoGestor,
                    Avaliador = projeto.AssociadoGestor,
                    Status = projeto.Status,
                    RotuloBotao = "Iniciar Avaliação",
                    ExibirBotaoFinalizar = false,
                    ExibirBotaoLiberarLider = false,
                    ExibirRotuloEtapa = "",
                    Etapa = "Não Iniciada",
                    AvaliacaoLiberada = false,
                    IdEmail = ""
                };
                projetoModel.Associados.Add(associadoModel);
            }
            if (projetoModel.Associados.Count > 0 || projetoModel.Lideres.Count > 0)
                listaProjetosAvaliacoes.Add(projetoModel);
        }
        return listaProjetosAvaliacoes;
    }

    public async Task FinalizarAvaliacaoAsync(int idAvaliacao, int userId)
    {
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail == null)
        {
            _messageBoxService.ShowWarning("Avaliação não encontrada");
            return;
        }
        // Simulação: lógica de finalização (deve ser adaptada para lógica real)
        avaliacaoEmail.PosicaoAtualFluxoAvaliacao = "Finalizada";
        avaliacaoEmail.DataLiberacao = DateTime.Now;
        await _db.SaveChangesAsync();
        _messageBoxService.ShowSuccess("Avaliação Finalizada com Sucesso");
    }

    public async Task LiberarVisualizacaoLiderAsync(int idAvaliacao, int userId)
    {
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail == null)
        {
            _messageBoxService.ShowWarning("Avaliação não encontrada");
            return;
        }
        // Simulação: lógica de liberação (deve ser adaptada para lógica real)
        avaliacaoEmail.Liberado = true;
        await _db.SaveChangesAsync();
        _messageBoxService.ShowSuccess("Visualização liberada ao líder");
    }
}

public class AvaliacaoGestorFiltro
{
    public string? IdProjeto { get; set; }
    public string? IdCliente { get; set; }
    public string? IdPeriodo { get; set; }
    public string? IdStatus { get; set; }
    public string? Etapa { get; set; }
}
