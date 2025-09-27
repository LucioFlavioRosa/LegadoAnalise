using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.AvaliacaoMentorCompetencia.Common;
using Microsoft.EntityFrameworkCore;

namespace Services.AvaliacaoMentorCompetencia
{
    public class AvaliacaoMentorCompetenciaService : IAvaliacaoMentorCompetenciaService
    {
        private readonly ApplicationDbContext _db;
        private readonly AvaliacaoMentorCompetenciaHelper _helper;

        public AvaliacaoMentorCompetenciaService(ApplicationDbContext db, AvaliacaoMentorCompetenciaHelper helper)
        {
            _db = db;
            _helper = helper;
        }

        public async Task<AvaliacaoMentorCompetenciaDto> ObterAvaliacaoMentorCompetenciaAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor)
        {
            var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
            var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
            var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
            var gestor = await _db.Associados.FirstOrDefaultAsync(g => g.Id == idGestor);

            var competencias = await _db.Competencias
                .Where(c => c.IdCargo == associado.IdCargo && c.IdNivel == associado.IdNivel && c.TipoAvaliacao == tipoAvaliacao && c.Escopo == escopo)
                .Include(c => c.Eixo)
                .Include(c => c.SubCompetencia)
                .Include(c => c.Dimensao)
                .ToListAsync();

            var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(ae => ae.IdAssociado == idAssociado && ae.IdProjeto == idProjeto && ae.IdPeriodo == idPeriodo && ae.TipoAvaliacao == tipoAvaliacao && ae.Escopo == escopo && ae.IdGestor == idGestor);

            var radar = await ObterRadarAsync(idAssociado, idProjeto, idPeriodo, associado.IdCargo);

            return new AvaliacaoMentorCompetenciaDto
            {
                Associado = associado,
                Periodo = periodo,
                Projeto = projeto,
                Gestor = gestor,
                Competencias = competencias,
                AvaliacaoEmail = avaliacaoEmail,
                RadarJson = radar
            };
        }

        public async Task<string> ObterRadarAsync(int idAssociado, int idProjeto, int idPeriodo, int idCargo)
        {
            var radarData = await _db.PremissasRadar
                .Where(r => r.IdCargo == idCargo)
                .Include(r => r.Eixo)
                .ToListAsync();

            var labels = new List<string>();
            var datasetAvaliado = new List<decimal>();
            var datasetGestor = new List<decimal>();
            var datasetAtual = new List<decimal>();
            var datasetProximoNivel = new List<decimal>();

            foreach (var item in radarData)
            {
                labels.Add(item.Eixo.Nome);
                datasetGestor.Add(item.PercentualGestor);
                datasetAvaliado.Add(item.PercentualAvaliado);
                datasetAtual.Add(100);
                datasetProximoNivel.Add(200);
            }

            var radarObj = new
            {
                labels,
                datasetavaliado = datasetAvaliado,
                datasetgestor = datasetGestor,
                datasetatual = datasetAtual,
                datasetproximonivel = datasetProximoNivel
            };

            return System.Text.Json.JsonSerializer.Serialize(new[] { radarObj });
        }

        public async Task<bool> SalvarConsideracoesMentorAsync(ConsideracoesMentorInput input)
        {
            var consideracao = await _db.Set<ConsideracoesMentor>().FirstOrDefaultAsync(c => c.Id == input.Id);
            if (consideracao == null)
                return false;

            consideracao.ElegivelPromocao = input.ElegivelPromocao;
            consideracao.InputPromocao = input.InputPromocao;
            consideracao.TrajetoriaAssociado = input.TrajetoriaAssociado;
            consideracao.PontosFortes = input.PontosFortes;
            consideracao.PontosFracos = input.PontosFracos;
            consideracao.MentoriaRealizada = input.MentoriaRealizada;
            consideracao.DataMentoriaRealizada = input.MentoriaRealizada ? DateTime.Today : (DateTime?)null;

            await _db.SaveChangesAsync();
            return true;
        }

        public string FormatPercentagem(decimal nota)
        {
            return _helper.FormatPercentagem(nota);
        }

        public string FormatDecimal(decimal nota)
        {
            return _helper.FormatDecimal(nota);
        }

        public string TruncarTexto(string texto, int qtdCaracteres)
        {
            return _helper.TruncarTexto(texto, qtdCaracteres);
        }
    }

    public class AvaliacaoMentorCompetenciaDto
    {
        public Associado Associado { get; set; }
        public PERIODOSAVALIACOES Periodo { get; set; }
        public Projeto Projeto { get; set; }
        public Associado Gestor { get; set; }
        public List<Competencia> Competencias { get; set; }
        public AvaliacaoEmail AvaliacaoEmail { get; set; }
        public string RadarJson { get; set; }
    }

    public class ConsideracoesMentorInput
    {
        public int Id { get; set; }
        public bool ElegivelPromocao { get; set; }
        public bool InputPromocao { get; set; }
        public string TrajetoriaAssociado { get; set; }
        public string PontosFortes { get; set; }
        public string PontosFracos { get; set; }
        public bool MentoriaRealizada { get; set; }
    }

    public class ConsideracoesMentor
    {
        public int Id { get; set; }
        public bool ElegivelPromocao { get; set; }
        public bool InputPromocao { get; set; }
        public string TrajetoriaAssociado { get; set; }
        public string PontosFortes { get; set; }
        public string PontosFracos { get; set; }
        public bool MentoriaRealizada { get; set; }
        public DateTime? DataMentoriaRealizada { get; set; }
    }
}
