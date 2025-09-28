using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Services.Avaliacoes;

public class AvaliacaoEmailFiltro
{
    public int? ProjetoId { get; set; }
    public int? ClienteId { get; set; }
    public int? AssociadoId { get; set; }
    public int? PeriodoId { get; set; }
}

public class AvaliacaoEmailConsultaModel
{
    public int Id { get; set; }
    public Projeto? Projeto { get; set; }
    public Cliente? Cliente { get; set; }
    public Associado? Associado { get; set; }
    public string? Cargo { get; set; }
    public Associado? Gestor { get; set; }
    public PERIODOSAVALIACOES? Periodo { get; set; }
    public string? DataInicio { get; set; }
    public string? DataTermino { get; set; }
    public string? Fase { get; set; }
    public string? Status { get; set; }
}

public interface IConsultaAvaliacaoService
{
    Task<List<AvaliacaoEmailConsultaModel>> BuscarAvaliacoesAsync(AvaliacaoEmailFiltro filtro);
}

public class ConsultaAvaliacaoService : IConsultaAvaliacaoService
{
    private readonly ApplicationDbContext _db;

    public ConsultaAvaliacaoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<AvaliacaoEmailConsultaModel>> BuscarAvaliacoesAsync(AvaliacaoEmailFiltro filtro)
    {
        var query = _db.AvaliacoesEmail
            .Include(a => a.PROJETOS)
                .ThenInclude(p => p.Cliente)
            .Include(a => a.ASSOCIADOS)
                .ThenInclude(ass => ass.Cargo)
            .Include(a => a.PERIODOSAVALIACOES)
            .AsQueryable();

        if (filtro.ProjetoId.HasValue && filtro.ProjetoId.Value > 0)
            query = query.Where(a => a.PROJETOS.Id == filtro.ProjetoId.Value);
        if (filtro.AssociadoId.HasValue && filtro.AssociadoId.Value > 0)
            query = query.Where(a => a.ASSOCIADOS.Id == filtro.AssociadoId.Value);
        if (filtro.PeriodoId.HasValue && filtro.PeriodoId.Value > 0)
            query = query.Where(a => a.PERIODOSAVALIACOES.IdPeriodo == filtro.PeriodoId.Value);
        if (filtro.ClienteId.HasValue && filtro.ClienteId.Value > 0)
            query = query.Where(a => a.PROJETOS.IdCliente == filtro.ClienteId.Value);

        var avaliacoes = await query.ToListAsync();
        var associadosService = new Peers.Moderno.Services.Associados.AssociadosService(_db);
        var avaliacoesService = new Peers.Moderno.Services.Avaliacoes.AvaliacoesService(_db);

        var result = new List<AvaliacaoEmailConsultaModel>();
        foreach (var avaliacao in avaliacoes)
        {
            var model = new AvaliacaoEmailConsultaModel
            {
                Id = avaliacao.idAvaliacao,
                Projeto = avaliacao.PROJETOS,
                Cliente = avaliacao.PROJETOS?.Cliente,
                Associado = avaliacao.ASSOCIADOS,
                Cargo = avaliacao.ASSOCIADOS?.Cargo?.Nome,
                Gestor = await associadosService.ObterGestorAsync(avaliacao.idProjeto, avaliacao.idAssociado),
                Periodo = avaliacao.PERIODOSAVALIACOES,
                DataInicio = avaliacao.DataLiberacao.HasValue ? avaliacao.DataLiberacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataTermino = await avaliacoesService.CalculaDataTerminoAsync(avaliacao, "FED"),
                Fase = MapearFase(avaliacao.PosicaoAtualFluxoAvaliacao),
                Status = avaliacao.AVALIACOESSTATUS
            };
            result.Add(model);
        }
        return result;
    }

    private string MapearFase(string? posicao)
    {
        return posicao switch
        {
            "AVM" => "Não Iniciado",
            "AAV" => "Iniciado",
            "ACE" => "As Cegas",
            "AGE" => "Gestor",
            "FED" => "Feedback",
            "AME" => "Mentor",
            "AFI" => "Finalizada",
            _ => "-"
        };
    }
}
