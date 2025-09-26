using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Mentoria.Common;
using Services.Mentoria.Common.ComboHelperMentoria;

namespace Services.Mentoria
{
    public interface IMentoriaService
    {
        Task<List<ComboItem>> GetProjetosComboAsync(int mentorId, int? projetoId = null, int? status = null, int? periodoId = null, int? clienteId = null);
        Task<List<ComboItem>> GetClientesComboAsync();
        Task<List<ComboItem>> GetPeriodosComboAsync(int empresaId);
        Task<List<ComboItem>> GetStatusComboAsync();
        Task<List<MentoradoModel>> GetMentoradosAsync(int mentorId, int empresaId);
        Task<List<ProjetoMentoriaModel>> GetProjetosMentoriaAsync(int mentorId, int? projetoId = null, int? status = null, int? periodoId = null, int? clienteId = null);
    }

    public class MentoriaService : IMentoriaService
    {
        private readonly ApplicationDbContext _db;
        private readonly MentoriaHelper _helper;
        public MentoriaService(ApplicationDbContext db, MentoriaHelper helper)
        {
            _db = db;
            _helper = helper;
        }

        public async Task<List<ComboItem>> GetProjetosComboAsync(int mentorId, int? projetoId = null, int? status = null, int? periodoId = null, int? clienteId = null)
        {
            return await ComboHelperMentoria.GetProjetosComboAsync(_db, mentorId, clienteId, status, periodoId);
        }

        public async Task<List<ComboItem>> GetClientesComboAsync()
        {
            return await ComboHelperMentoria.GetClientesComboAsync(_db);
        }

        public async Task<List<ComboItem>> GetPeriodosComboAsync(int empresaId)
        {
            return await ComboHelperMentoria.GetPeriodosComboAsync(_db, empresaId);
        }

        public async Task<List<ComboItem>> GetStatusComboAsync()
        {
            return await ComboHelperMentoria.GetStatusComboAsync(_db);
        }

        public async Task<List<MentoradoModel>> GetMentoradosAsync(int mentorId, int empresaId)
        {
            var ultimoPeriodo = await _db.PeriodosAvaliacoes.Where(p => p.IdEmpresa == empresaId).OrderByDescending(p => p.IdPeriodo).FirstOrDefaultAsync();
            if (ultimoPeriodo == null) return new List<MentoradoModel>();
            var mentorados = await _db.Associados
                .Include(a => a.Cargo)
                .Include(a => a.Mentor)
                .Where(a => a.Ativo && a.IdMentor == mentorId)
                .ToListAsync();
            var fotos = await _db.Associados.ToListAsync(); // Supondo que fotos estão em Associados (ajustar se necessário)
            var result = mentorados.Select(item => new MentoradoModel
            {
                IdAssociado = item.Id,
                Associado = item.Nome,
                IdMentor = item.IdMentor,
                Mentor = item.Mentor?.Nome ?? string.Empty,
                IdCargo = item.IdCargo,
                Cargo = item.Cargo?.Nome ?? string.Empty,
                FotoNome = _helper.GetFotoAssociado(item.Id, fotos),
                IdProjeto = -1,
                IdGestor = -1,
                IdPeriodo = ultimoPeriodo.IdPeriodo,
                TipoAvaliacao = "desempenho",
                Escopo = "projeto"
            }).ToList();
            return result;
        }

        public async Task<List<ProjetoMentoriaModel>> GetProjetosMentoriaAsync(int mentorId, int? projetoId = null, int? status = null, int? periodoId = null, int? clienteId = null)
        {
            var projetosQuery = _db.Projetos.AsQueryable();
            if (projetoId.HasValue)
                projetosQuery = projetosQuery.Where(p => p.Id == projetoId.Value);
            if (clienteId.HasValue)
                projetosQuery = projetosQuery.Where(p => p.IdCliente == clienteId.Value);
            if (status.HasValue)
                projetosQuery = projetosQuery.Where(p => p.Status == status.Value);
            if (mentorId > 0)
                projetosQuery = projetosQuery.Where(p => p.AssociadoGestor.Id == mentorId);
            if (periodoId.HasValue)
                projetosQuery = projetosQuery.Where(p => p.AssociadosProjeto.Any(ap => ap.IdPeriodo == periodoId.Value));
            var projetos = await projetosQuery.Include(p => p.Cliente).Include(p => p.AssociadoGestor).Include(p => p.AssociadoResponsavel).ToListAsync();
            var result = new List<ProjetoMentoriaModel>();
            foreach (var projeto in projetos)
            {
                var model = new ProjetoMentoriaModel
                {
                    Id = projeto.Id,
                    Nome = projeto.Nome,
                    Cliente = projeto.Cliente,
                    Gestor = projeto.AssociadoGestor,
                    Responsavel = projeto.AssociadoResponsavel,
                    Status = projeto.Status,
                    DataInicio = projeto.DataInicio,
                    DataTermino = projeto.DataFim,
                    Associados = await _helper.GetAssociadosProjetoMentoriaAsync(_db, projeto, mentorId, periodoId)
                };
                result.Add(model);
            }
            return result;
        }
    }

    public class MentoradoModel
    {
        public int IdAssociado { get; set; }
        public string Associado { get; set; } = string.Empty;
        public int? IdMentor { get; set; }
        public string Mentor { get; set; } = string.Empty;
        public int? IdCargo { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string FotoNome { get; set; } = string.Empty;
        public int IdProjeto { get; set; }
        public int IdGestor { get; set; }
        public int IdPeriodo { get; set; }
        public string TipoAvaliacao { get; set; } = string.Empty;
        public string Escopo { get; set; } = string.Empty;
    }

    public class ProjetoMentoriaModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public Cliente? Cliente { get; set; }
        public Associado? Gestor { get; set; }
        public Associado? Responsavel { get; set; }
        public int Status { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }
        public List<AssociadoProjetoMentoriaModel> Associados { get; set; } = new List<AssociadoProjetoMentoriaModel>();
    }

    public class AssociadoProjetoMentoriaModel
    {
        public int IdAssociadoProjeto { get; set; }
        public Associado? Associado { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string FotoNome { get; set; } = string.Empty;
        public int IdPeriodo { get; set; }
        public string TipoAvaliacao { get; set; } = string.Empty;
        public string Escopo { get; set; } = string.Empty;
        public Associado? Gestor { get; set; }
        public Associado? Avaliador { get; set; }
        public string Etapa { get; set; } = string.Empty;
        public string ExibirBotaoVerMentor { get; set; } = string.Empty;
        public string ExibirRotuloEtapa { get; set; } = string.Empty;
        public bool ExibirBotaoFinalizar { get; set; }
        public string RotuloBotao { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string FeedbackRHVisible { get; set; } = string.Empty;
        public string DataInicio { get; set; } = string.Empty;
        public string DataTermino { get; set; } = string.Empty;
    }
}
