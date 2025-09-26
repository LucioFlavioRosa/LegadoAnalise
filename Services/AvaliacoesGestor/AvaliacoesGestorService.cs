using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.AvaliacoesGestor.Common;
using Peers.Moderno.Services.Common;

namespace Services.AvaliacoesGestor
{
    public interface IAvaliacoesGestorService
    {
        Task<AvaliacaoGestorDto> CarregarAvaliacaoGestorAsync(int idProjeto, int idAssociado, int idPeriodo, int idGestor, string tipoAvaliacao, string escopo);
        Task<bool> SalvarAvaliacaoCompetenciasAsync(AvaliacaoGestorDto avaliacao, bool finalizar = false);
        Task<bool> ValidarAvaliacaoAsync(AvaliacaoGestorDto avaliacao);
    }

    public class AvaliacoesGestorService : IAvaliacoesGestorService
    {
        private readonly ApplicationDbContext _db;
        private readonly AvaliacoesGestorHelper _helper;
        private readonly IValidationHelper _validationHelper;
        private readonly IMessageBoxService _messageBoxService;

        public AvaliacoesGestorService(
            ApplicationDbContext db,
            AvaliacoesGestorHelper helper,
            IValidationHelper validationHelper,
            IMessageBoxService messageBoxService)
        {
            _db = db;
            _helper = helper;
            _validationHelper = validationHelper;
            _messageBoxService = messageBoxService;
        }

        public async Task<AvaliacaoGestorDto> CarregarAvaliacaoGestorAsync(int idProjeto, int idAssociado, int idPeriodo, int idGestor, string tipoAvaliacao, string escopo)
        {
            var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
            var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
            var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
            var gestor = await _db.Associados.FirstOrDefaultAsync(a => a.Id == idGestor);

            var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo && a.TipoAvaliacao == tipoAvaliacao && a.Escopo == escopo && a.IdGestor == idGestor);
            if (avaliacaoEmail == null)
                throw new Exception("Avaliação não encontrada");

            var competencias = await _helper.ObterCompetenciasParaAvaliacaoAsync(associado, projeto, periodo, tipoAvaliacao, escopo, avaliacaoEmail);
            var competenciasDto = await _helper.MapearCompetenciasParaDtoAsync(competencias, associado, projeto, periodo, tipoAvaliacao, escopo, avaliacaoEmail);

            return new AvaliacaoGestorDto
            {
                Projeto = projeto,
                Associado = associado,
                Gestor = gestor,
                Periodo = periodo,
                AvaliacaoEmail = avaliacaoEmail,
                Competencias = competenciasDto,
                TipoAvaliacao = tipoAvaliacao,
                Escopo = escopo
            };
        }

        public async Task<bool> SalvarAvaliacaoCompetenciasAsync(AvaliacaoGestorDto avaliacao, bool finalizar = false)
        {
            foreach (var comp in avaliacao.Competencias)
            {
                var avaliacaoComp = await _db.AvaliacoesCompetenciasNotas.FirstOrDefaultAsync(a => a.IdNota == comp.IdAvaliacaoCompetencia);
                if (avaliacaoComp == null)
                    continue;
                avaliacaoComp.IdNotaNivel1AvaliacaoGestor = comp.NotaNivel1Gestor;
                avaliacaoComp.IdNotaNivel2AvaliacaoGestor = comp.NotaNivel2Gestor;
                avaliacaoComp.ComentariosAvaliacaoGestor = comp.ConsideracoesGestor;
                avaliacaoComp.DHCAvaliacaoGestor = DateTime.UtcNow;
                if (finalizar)
                    avaliacaoComp.DataHoraFimAvaliacaoGestor = DateTime.UtcNow;
                _db.AvaliacoesCompetenciasNotas.Update(avaliacaoComp);
            }
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidarAvaliacaoAsync(AvaliacaoGestorDto avaliacao)
        {
            return _helper.ValidarCompetencias(avaliacao.Competencias);
        }
    }

    public class AvaliacaoGestorDto
    {
        public Projeto Projeto { get; set; }
        public Associado Associado { get; set; }
        public Associado Gestor { get; set; }
        public PERIODOSAVALIACOES Periodo { get; set; }
        public AvaliacaoEmail AvaliacaoEmail { get; set; }
        public List<CompetenciaGestorDto> Competencias { get; set; } = new();
        public string TipoAvaliacao { get; set; } = string.Empty;
        public string Escopo { get; set; } = string.Empty;
    }

    public class CompetenciaGestorDto
    {
        public int IdAvaliacaoCompetencia { get; set; }
        public int IdCompetencia { get; set; }
        public string NomeCompetencia { get; set; } = string.Empty;
        public string SubCompetencia { get; set; } = string.Empty;
        public string PalavrasChave { get; set; } = string.Empty;
        public string DetalheNivelAtual { get; set; } = string.Empty;
        public string DetalheProximoNivel { get; set; } = string.Empty;
        public int? NotaNivel1Gestor { get; set; }
        public int? NotaNivel2Gestor { get; set; }
        public string ConsideracoesGestor { get; set; } = string.Empty;
        public int IdModo { get; set; }
        public bool InputNivel1 { get; set; }
        public bool InputNivel2 { get; set; }
        public bool VisivelNivel1 { get; set; }
        public bool VisivelNivel2 { get; set; }
        public string TextoNotaNivel1 { get; set; } = string.Empty;
        public string TextoNotaNivel2 { get; set; } = string.Empty;
        public string ObservacaoAvaliado { get; set; } = string.Empty;
        public string ObservacaoCegas { get; set; } = string.Empty;
    }
}
