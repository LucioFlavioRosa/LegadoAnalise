using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Services.AvaliacoesGestor.Common
{
    public class AvaliacoesGestorHelper
    {
        private readonly ApplicationDbContext _db;
        public AvaliacoesGestorHelper(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Competencia>> ObterCompetenciasParaAvaliacaoAsync(Associado associado, Projeto projeto, PERIODOSAVALIACOES periodo, string tipoAvaliacao, string escopo, AvaliacaoEmail avaliacaoEmail)
        {
            var competencias = await _db.Competencias
                .Where(c => c.IdCargo == associado.IdCargo && c.IdNivel == associado.IdNivel)
                .Include(c => c.SubCompetencia)
                .ToListAsync();
            return competencias;
        }

        public async Task<List<CompetenciaGestorDto>> MapearCompetenciasParaDtoAsync(List<Competencia> competencias, Associado associado, Projeto projeto, PERIODOSAVALIACOES periodo, string tipoAvaliacao, string escopo, AvaliacaoEmail avaliacaoEmail)
        {
            var result = new List<CompetenciaGestorDto>();
            foreach (var comp in competencias)
            {
                var avaliacaoComp = await _db.AvaliacoesCompetenciasNotas.FirstOrDefaultAsync(a => a.IdCompetencia == comp.IdCompetencia && a.IdAssociado == associado.Id && a.IdProjeto == projeto.Id && a.IdPeriodo == periodo.IdPeriodo && a.TipoAvaliacao == tipoAvaliacao && a.Escopo == escopo && a.IdAvaliacao == avaliacaoEmail.idAvaliacao);
                result.Add(new CompetenciaGestorDto
                {
                    IdAvaliacaoCompetencia = avaliacaoComp?.IdNota ?? 0,
                    IdCompetencia = comp.IdCompetencia,
                    NomeCompetencia = comp.Nome,
                    SubCompetencia = comp.SubCompetencia?.Nome ?? string.Empty,
                    PalavrasChave = comp.PalavrasChave ?? string.Empty,
                    DetalheNivelAtual = comp.CompetenciaJRDetalhe ?? string.Empty,
                    DetalheProximoNivel = comp.CompetenciaPLDetalhe ?? string.Empty,
                    NotaNivel1Gestor = avaliacaoComp?.IdNotaNivel1AvaliacaoGestor,
                    NotaNivel2Gestor = avaliacaoComp?.IdNotaNivel2AvaliacaoGestor,
                    ConsideracoesGestor = avaliacaoComp?.ComentariosAvaliacaoGestor ?? string.Empty,
                    IdModo = comp.IdModo,
                    InputNivel1 = comp.InputNivel1,
                    InputNivel2 = comp.InputNivel2,
                    VisivelNivel1 = comp.VisivelNivel1,
                    VisivelNivel2 = comp.VisivelNivel2,
                    TextoNotaNivel1 = "",
                    TextoNotaNivel2 = "",
                    ObservacaoAvaliado = avaliacaoComp?.ComentariosAutoAvaliacao ?? string.Empty,
                    ObservacaoCegas = avaliacaoComp?.ComentariosAvaliacaoCegas ?? string.Empty
                });
            }
            return result;
        }

        public bool ValidarCompetencias(List<CompetenciaGestorDto> competencias)
        {
            foreach (var comp in competencias)
            {
                if (comp.InputNivel1 && (!comp.NotaNivel1Gestor.HasValue || comp.NotaNivel1Gestor == 0))
                    return false;
                if (comp.InputNivel2 && (!comp.NotaNivel2Gestor.HasValue || comp.NotaNivel2Gestor == 0))
                    return false;
                if (comp.NotaNivel1Gestor == 5 && comp.NotaNivel2Gestor != 5)
                    return false;
                if (comp.NotaNivel1Gestor > 0 && comp.NotaNivel2Gestor > 0 && comp.NotaNivel2Gestor > comp.NotaNivel1Gestor)
                    return false;
            }
            return true;
        }
    }
}
