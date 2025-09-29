using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Competencias.Common;
using Services.Common;

namespace Services.Competencias;

public interface ICompetenciasService
{
    Task<List<CompetenciaModel>> ObterListaCompetenciasModelAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao, int? idCargo = null, int? idNivel = null, int? idEmpresa = null);
    Task<bool> SalvarAvaliacaoAsync(int idAssociado, int idProjeto, int idPeriodo, int idAvaliacao, string tipoAvaliacao, string escopo, List<AvaliacaoCompetenciaInputModel> avaliacoes, bool finalizarAvaliacao = false);
    Task<AvaliacaoCompetencia?> ObterAvaliacaoCompetenciaAsync(int idAssociado, int idProjeto, int idCompetencia, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao);
}

public class CompetenciasService : ICompetenciasService
{
    private readonly ApplicationDbContext _db;
    private readonly IMessageBoxService _messageBoxService;

    public CompetenciasService(ApplicationDbContext db, IMessageBoxService messageBoxService)
    {
        _db = db;
        _messageBoxService = messageBoxService;
    }

    public async Task<List<CompetenciaModel>> ObterListaCompetenciasModelAsync(
        int idAssociado,
        int idProjeto,
        int idPeriodo,
        string tipoAvaliacao,
        string escopo,
        int idAvaliacao,
        int? idCargo = null,
        int? idNivel = null,
        int? idEmpresa = null)
    {
        // Busca associado e projeto
        var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
        if (associado == null) return new List<CompetenciaModel>();
        var projeto = await _db.Projetos.FirstOrDefaultAsync(p => p.Id == idProjeto);
        if (projeto == null) return new List<CompetenciaModel>();
        var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
        if (periodo == null) return new List<CompetenciaModel>();

        // Busca avaliação email
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail == null) return new List<CompetenciaModel>();

        // Busca competências parametrizadas
        var competencias = await _db.Competencias
            .Where(c => c.IdCargo == (idCargo ?? associado.IdCargo) && c.TipoAvaliacao == tipoAvaliacao && c.Escopo == escopo)
            .OrderBy(c => c.IdCompetencia)
            .ToListAsync();

        var listaCompetenciasModel = new List<CompetenciaModel>();
        int cont = 0;
        foreach (var item in competencias)
        {
            var linhaCompetencia = new CompetenciaModel
            {
                IdCompetencia = item.IdCompetencia,
                PalavrasChave = item.PalavrasChave,
                Eixo = item.Eixo?.Nome ?? string.Empty,
                SubCompetencia = item.SubCompetencia?.Nome ?? string.Empty,
                Dimensao = item.Dimensao?.Nome ?? string.Empty
            };

            // Detalhamento e títulos por nível
            switch (idNivel ?? associado.IdNivel)
            {
                case 1: // Junior
                    linhaCompetencia.DetalheNivelAtual = item.CompetenciaJRDetalhe;
                    linhaCompetencia.CompetenciaAtual = item.CompetenciaJR;
                    linhaCompetencia.CompetenciaProximo = item.CompetenciaPL;
                    linhaCompetencia.DetalheProximoNivel = item.CompetenciaPLDetalhe;
                    linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "hidden";
                    break;
                case 2: // Pleno
                    linhaCompetencia.DetalheNivelAtual = item.CompetenciaPLDetalhe;
                    linhaCompetencia.CompetenciaAtual = item.CompetenciaPL;
                    linhaCompetencia.CompetenciaProximo = item.CompetenciaSR;
                    linhaCompetencia.DetalheProximoNivel = item.CompetenciaSRDetalhe;
                    linhaCompetencia.IsHiddenDetalhamentoProximoNivel = "hidden";
                    break;
                case 3: // Senior
                    linhaCompetencia.DetalheNivelAtual = item.CompetenciaSRDetalhe;
                    linhaCompetencia.CompetenciaAtual = item.CompetenciaSR;
                    linhaCompetencia.CompetenciaProximo = "Não existe parametrização para o próximo nível";
                    linhaCompetencia.DetalheProximoNivel = "Não existe parametrização para o próximo nível";
                    linhaCompetencia.IsHiddenDetalhamentoProximoNivel = string.Empty;
                    break;
                default:
                    linhaCompetencia.DetalheNivelAtual = string.Empty;
                    linhaCompetencia.CompetenciaAtual = string.Empty;
                    linhaCompetencia.CompetenciaProximo = string.Empty;
                    linhaCompetencia.DetalheProximoNivel = string.Empty;
                    linhaCompetencia.IsHiddenDetalhamentoProximoNivel = string.Empty;
                    break;
            }
            listaCompetenciasModel.Add(linhaCompetencia);
            cont++;
        }
        return listaCompetenciasModel;
    }

    public async Task<AvaliacaoCompetencia?> ObterAvaliacaoCompetenciaAsync(int idAssociado, int idProjeto, int idCompetencia, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao)
    {
        return await _db.AvaliacoesCompetencias.FirstOrDefaultAsync(a =>
            a.IdAssociado == idAssociado &&
            a.IdProjeto == idProjeto &&
            a.IdCompetencia == idCompetencia &&
            a.IdPeriodo == idPeriodo &&
            a.TipoAvaliacao == tipoAvaliacao &&
            a.Escopo == escopo &&
            a.idAvaliacao == idAvaliacao
        );
    }

    public async Task<bool> SalvarAvaliacaoAsync(
        int idAssociado,
        int idProjeto,
        int idPeriodo,
        int idAvaliacao,
        string tipoAvaliacao,
        string escopo,
        List<AvaliacaoCompetenciaInputModel> avaliacoes,
        bool finalizarAvaliacao = false)
    {
        var associado = await _db.Associados.FirstOrDefaultAsync(a => a.Id == idAssociado);
        if (associado == null) return false;
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail == null) return false;
        foreach (var input in avaliacoes)
        {
            var avaliacao = await ObterAvaliacaoCompetenciaAsync(idAssociado, idProjeto, input.IdCompetencia, idPeriodo, tipoAvaliacao, escopo, idAvaliacao);
            if (avaliacao == null)
            {
                avaliacao = new AvaliacaoCompetencia
                {
                    IdEmpresa = associado.IdEmpresa,
                    IdAssociado = idAssociado,
                    USRAutoAvaliacao = idAssociado,
                    IdCargo = associado.IdCargo,
                    IdNivel = associado.IdNivel,
                    IdProjeto = idProjeto,
                    IdPeriodo = idPeriodo,
                    IdCompetencia = input.IdCompetencia,
                    IdAvaliacaoStatus = 2, // Em Andamento
                    PosicaoAtualFluxoAvaliacao = "Em Auto-avaliação",
                    DataHoraInicio = DateTime.Now,
                    DHCAutoAvaliacao = DateTime.Now,
                    USR = idAssociado,
                    DHC = DateTime.Now,
                    ATV = true,
                    IdNotaNivel1AutoAvaliacao = input.IdNotaNivel1,
                    IdNotaNivel2AutoAvaliacao = input.IdNotaNivel2,
                    ComentariosAutoAvaliacao = input.Comentarios?.Trim() ?? string.Empty,
                    DataHoraInicioAutoAvaliacao = DateTime.Now,
                    TipoAvaliacao = tipoAvaliacao,
                    Escopo = escopo,
                    idAvaliacao = idAvaliacao
                };
                await _db.AvaliacoesCompetencias.AddAsync(avaliacao);
            }
            else
            {
                avaliacao.IdNotaNivel1AutoAvaliacao = input.IdNotaNivel1;
                avaliacao.IdNotaNivel2AutoAvaliacao = input.IdNotaNivel2;
                avaliacao.ComentariosAutoAvaliacao = input.Comentarios?.Trim() ?? string.Empty;
                if (avaliacao.IdAvaliacaoStatus == 1)
                {
                    avaliacao.IdAvaliacaoStatus = 2;
                    avaliacao.PosicaoAtualFluxoAvaliacao = "Em Auto-avaliação";
                    if (avaliacao.DataHoraInicio == DateTime.MinValue)
                        avaliacao.DataHoraInicio = DateTime.Now;
                    avaliacao.DataHoraInicioAutoAvaliacao = DateTime.Now;
                }
            }
            if (finalizarAvaliacao)
                avaliacao.DataHoraFimAutoAvaliacao = DateTime.Now;
        }
        await _db.SaveChangesAsync();
        return true;
    }
}

public class CompetenciaModel
{
    public int IdCompetencia { get; set; }
    public string PalavrasChave { get; set; } = string.Empty;
    public string Eixo { get; set; } = string.Empty;
    public string SubCompetencia { get; set; } = string.Empty;
    public string Dimensao { get; set; } = string.Empty;
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public string CompetenciaProximo { get; set; } = string.Empty;
    public string DetalheProximoNivel { get; set; } = string.Empty;
    public string IsHiddenDetalhamentoProximoNivel { get; set; } = string.Empty;
}

public class AvaliacaoCompetenciaInputModel
{
    public int IdCompetencia { get; set; }
    public int IdNotaNivel1 { get; set; }
    public int IdNotaNivel2 { get; set; }
    public string? Comentarios { get; set; }
}
