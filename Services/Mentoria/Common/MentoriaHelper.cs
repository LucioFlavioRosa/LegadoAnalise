using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Mentoria.Common
{
    public class MentoriaHelper
    {
        public string GetFotoAssociado(int idAssociado, List<Associado> associados)
        {
            var associado = associados.FirstOrDefault(a => a.Id == idAssociado);
            if (associado == null || string.IsNullOrWhiteSpace(associado.FotoNome))
                return "assets/images/users/usernophoto.jpg";
            return associado.FotoNome;
        }

        public async Task<List<Services.Mentoria.AssociadoProjetoMentoriaModel>> GetAssociadosProjetoMentoriaAsync(ApplicationDbContext db, Projeto projeto, int mentorId, int? periodoId)
        {
            var associadosProjeto = await db.AssociadosProjetos
                .Include(ap => ap.Associado)
                .Include(ap => ap.Gestor)
                .Include(ap => ap.Avaliador)
                .Where(ap => ap.IdProjeto == projeto.Id && ap.Gestor.Id == mentorId)
                .ToListAsync();
            var result = new List<Services.Mentoria.AssociadoProjetoMentoriaModel>();
            foreach (var ap in associadosProjeto)
            {
                var etapa = "";
                var exibirBotaoVerMentor = "";
                var exibirRotuloEtapa = "";
                var feedbackRHVisible = "";
                var rotuloBotao = "Ver Avaliação";
                var status = "Concluído";
                var dataInicio = ap.Projeto?.DataInicio.ToString("dd/MM/yyyy") ?? "";
                var dataTermino = ap.Projeto?.DataFim?.ToString("dd/MM/yyyy") ?? "";
                // Lógica de etapa e botões pode ser expandida conforme regras do negócio
                result.Add(new Services.Mentoria.AssociadoProjetoMentoriaModel
                {
                    IdAssociadoProjeto = ap.Id,
                    Associado = ap.Associado,
                    Cargo = ap.Associado?.Cargo?.Nome ?? "",
                    FotoNome = GetFotoAssociado(ap.Associado?.Id ?? 0, await db.Associados.ToListAsync()),
                    IdPeriodo = periodoId ?? 0,
                    TipoAvaliacao = "desempenho",
                    Escopo = "projeto",
                    Gestor = ap.Gestor,
                    Avaliador = ap.Avaliador,
                    Etapa = etapa,
                    ExibirBotaoVerMentor = exibirBotaoVerMentor,
                    ExibirRotuloEtapa = exibirRotuloEtapa,
                    ExibirBotaoFinalizar = false,
                    RotuloBotao = rotuloBotao,
                    Status = status,
                    FeedbackRHVisible = feedbackRHVisible,
                    DataInicio = dataInicio,
                    DataTermino = dataTermino
                });
            }
            return result;
        }
    }
}
