using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Avaliacoes;

public interface IAvaliacaoCompetenciaService
{
    Task<AvaliacaoCompetenciaDto?> ObterAvaliacaoCompetenciaAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor);
    Task<List<CompetenciaLinhaDto>> ObterLinhasCompetenciaAsync(AvaliacaoCompetenciaDto avaliacao);
    Task<bool> ValidarAvaliacaoAsync(AvaliacaoCompetenciaDto avaliacao, List<CompetenciaLinhaDto> linhas, out string mensagemErro);
    Task<bool> SalvarAvaliacaoAsync(AvaliacaoCompetenciaDto avaliacao, List<CompetenciaLinhaDto> linhas, bool finalizar = false);
}

public class AvaliacaoCompetenciaService : IAvaliacaoCompetenciaService
{
    private readonly ApplicationDbContext _db;

    public AvaliacaoCompetenciaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<AvaliacaoCompetenciaDto?> ObterAvaliacaoCompetenciaAsync(int idProjeto, int idAssociado, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor)
    {
        var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
        var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
        var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
        var gestor = await _db.Associados.FirstOrDefaultAsync(a => a.Id == idGestor);
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(ae => ae.IdProjeto == idProjeto && ae.IdAssociado == idAssociado && ae.IdPeriodo == idPeriodo && ae.TipoAvaliacao == tipoAvaliacao && ae.Escopo == escopo && ae.IdGestor == idGestor);
        if (projeto == null || associado == null || periodo == null || gestor == null || avaliacaoEmail == null)
            return null;
        return new AvaliacaoCompetenciaDto
        {
            Projeto = projeto,
            Associado = associado,
            Periodo = periodo,
            Gestor = gestor,
            AvaliacaoEmail = avaliacaoEmail
        };
    }

    public async Task<List<CompetenciaLinhaDto>> ObterLinhasCompetenciaAsync(AvaliacaoCompetenciaDto avaliacao)
    {
        var competencias = await _db.Competencias
            .Include(c => c.SubCompetencia)
            .Include(c => c.Eixo)
            .Where(c => c.IdCargo == avaliacao.Associado.IdCargo)
            .ToListAsync();
        var linhas = new List<CompetenciaLinhaDto>();
        foreach (var comp in competencias)
        {
            var notaNivel1 = await _db.AvaliacoesCompetenciasNotas.FirstOrDefaultAsync(n => n.IdNota == comp.IdNotaPadraoNivel1);
            var notaNivel2 = await _db.AvaliacoesCompetenciasNotas.FirstOrDefaultAsync(n => n.IdNota == comp.IdNotaPadraoNivel2);
            linhas.Add(new CompetenciaLinhaDto
            {
                IdCompetencia = comp.IdCompetencia,
                Nome = comp.SubCompetencia?.Nome ?? string.Empty,
                PalavrasChave = comp.PalavrasChave ?? string.Empty,
                DetalheNivelAtual = comp.CompetenciaJRDetalhe ?? string.Empty,
                DetalheProximoNivel = comp.CompetenciaPLDetalhe ?? string.Empty,
                NotaNivel1 = notaNivel1?.DescricaoNota ?? "",
                NotaNivel2 = notaNivel2?.DescricaoNota ?? "",
                IdNotaNivel1 = comp.IdNotaPadraoNivel1,
                IdNotaNivel2 = comp.IdNotaPadraoNivel2
            });
        }
        return linhas;
    }

    public async Task<bool> ValidarAvaliacaoAsync(AvaliacaoCompetenciaDto avaliacao, List<CompetenciaLinhaDto> linhas, out string mensagemErro)
    {
        mensagemErro = string.Empty;
        var statusList = ComboHelper.GetNotasCompetenciaItems(false);
        foreach (var linha in linhas)
        {
            if ((linha.IdNotaNivel1 == 0 && linha.IdNotaNivel2 > 0) || (linha.IdNotaNivel1 > 0 && linha.IdNotaNivel2 == 0))
            {
                mensagemErro = "É obrigatório selecionar uma nota para cada nível.";
                return false;
            }
            if ((linha.IdNotaNivel1 > 0) && (linha.IdNotaNivel2 > 0))
            {
                var pesoDdl1 = statusList.FirstOrDefault(x => x.Value == linha.IdNotaNivel1.ToString())?.AdditionalData["Peso"];
                var pesoDdl2 = statusList.FirstOrDefault(x => x.Value == linha.IdNotaNivel2.ToString())?.AdditionalData["Peso"];
                if (pesoDdl1 != null && pesoDdl2 != null && Convert.ToInt32(pesoDdl2) > Convert.ToInt32(pesoDdl1))
                {
                    mensagemErro = "A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).";
                    return false;
                }
            }
        }
        return true;
    }

    public async Task<bool> SalvarAvaliacaoAsync(AvaliacaoCompetenciaDto avaliacao, List<CompetenciaLinhaDto> linhas, bool finalizar = false)
    {
        foreach (var linha in linhas)
        {
            var competencia = await _db.Competencias.FirstOrDefaultAsync(c => c.IdCompetencia == linha.IdCompetencia);
            if (competencia == null) continue;
            competencia.IdNotaPadraoNivel1 = linha.IdNotaNivel1;
            competencia.IdNotaPadraoNivel2 = linha.IdNotaNivel2;
        }
        if (finalizar)
        {
            avaliacao.AvaliacaoEmail.PosicaoAtualFluxoAvaliacao = "Finalizada";
        }
        await _db.SaveChangesAsync();
        return true;
    }
}

public class AvaliacaoCompetenciaDto
{
    public Projeto Projeto { get; set; } = null!;
    public Associado Associado { get; set; } = null!;
    public PERIODOSAVALIACOES Periodo { get; set; } = null!;
    public Associado Gestor { get; set; } = null!;
    public AvaliacaoEmail AvaliacaoEmail { get; set; } = null!;
}

public class CompetenciaLinhaDto
{
    public int IdCompetencia { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string DetalheProximoNivel { get; set; } = string.Empty;
    public string NotaNivel1 { get; set; } = string.Empty;
    public string NotaNivel2 { get; set; } = string.Empty;
    public int? IdNotaNivel1 { get; set; }
    public int? IdNotaNivel2 { get; set; }
}
