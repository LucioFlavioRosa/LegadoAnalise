using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Services.AvaliacoesGestor.Common;

public class AvaliacoesGestorHelper
{
    public bool DeveIncluirAssociadoNoFluxo(AssociadoProjeto ap, int? idPeriodo)
    {
        if (ap == null || ap.Associado == null)
            return false;
        if (!string.Equals(ap.TipoAvaliacao, "desempenho", StringComparison.OrdinalIgnoreCase))
            return false;
        if (idPeriodo.HasValue && ap.IdPeriodo != idPeriodo.Value)
            return false;
        return true;
    }

    public async Task<ProjetosAssociadosModel?> MapearProjetosAssociadosModelAsync(AssociadoProjeto ap, int? idPeriodo, ApplicationDbContext db)
    {
        if (ap == null)
            return null;
        var periodo = idPeriodo.HasValue
            ? await db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo.Value)
            : await db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == ap.IdPeriodo);
        if (periodo == null)
            return null;
        var associado = await db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == ap.IdAssociado);
        if (associado == null)
            return null;
        var gestor = await db.Associados.FirstOrDefaultAsync(a => a.Id == ap.IdGestor);
        var avaliador = await db.Associados.FirstOrDefaultAsync(a => a.Id == ap.IdAvaliador);
        var mentor = associado.IdMentor.HasValue ? await db.Associados.FirstOrDefaultAsync(a => a.Id == associado.IdMentor.Value) : null;
        var foto = associado.FotoNome ?? "assets/images/users/usernophoto.jpg";
        var etapa = ObterEtapaAvaliacao(ap.PosicaoAtualFluxoAvaliacao);
        var status = ObterStatusAvaliacao(ap.Status);
        var rotuloBotao = ObterRotuloBotao(ap, etapa);
        var exibirBotaoFinalizar = PodeExibirBotaoFinalizar(ap, etapa);
        return new ProjetosAssociadosModel
        {
            Id = ap.Id,
            Projeto = ap.Projeto,
            Associado = associado,
            DataInicio = ap.DataInicio?.ToString("dd/MM/yyyy") ?? "",
            DataTermino = ap.DataFim?.ToString("dd/MM/yyyy") ?? "",
            Periodo = periodo,
            TipoAvaliacao = ap.TipoAvaliacao,
            Escopo = ap.Escopo,
            FotoAssociado = foto,
            Gestor = gestor,
            Avaliador = avaliador,
            ExibirBotaoFinalizar = exibirBotaoFinalizar,
            RotuloBotao = rotuloBotao,
            Etapa = etapa,
            Status = status,
            ExibirRotuloEtapa = "",
            AvaliacaoLiberada = ap.AvaliacaoLiberada,
            IdEmail = ap.IdAvaliacaoEmail?.ToString() ?? ""
        };
    }

    public string ObterEtapaAvaliacao(string? posicaoFluxo)
    {
        if (string.IsNullOrEmpty(posicaoFluxo))
            return "Não Iniciada";
        switch (posicaoFluxo)
        {
            case "AutoAvaliacao":
                return "Em Auto-avaliação e Av. às Cegas";
            case "AvaliacaoAsCegas":
                return "Em Auto-avaliação e Av. as Cegas";
            case "AvaliacaoGestor":
                return "Em Av. Gestor";
            case "Feedback":
                return "Em Feedback";
            case "AvaliacaoMentor":
                return "Em Cons. Mentor";
            case "Finalizada":
                return "Finalizada";
            default:
                return "Em andamento";
        }
    }

    public string ObterStatusAvaliacao(object? status)
    {
        if (status == null)
            return "Não iniciado";
        var statusStr = status.ToString();
        if (statusStr == "1" || statusStr?.ToLower() == "ativo")
            return "Ativo";
        if (statusStr == "0" || statusStr?.ToLower() == "inativo")
            return "Inativo";
        return statusStr ?? "";
    }

    public string ObterRotuloBotao(AssociadoProjeto ap, string etapa)
    {
        if (ap == null)
            return "Iniciar Avaliação";
        if (etapa == "Em andamento")
            return "Continuar Avaliação";
        if (etapa == "Finalizada")
            return "Ver Avaliação";
        return "Iniciar Avaliação";
    }

    public bool PodeExibirBotaoFinalizar(AssociadoProjeto ap, string etapa)
    {
        if (ap == null)
            return false;
        return etapa == "Em andamento";
    }

    public async Task<bool> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId, ApplicationDbContext db)
    {
        var avaliacaoEmail = await db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail == null)
            return false;
        var etapaAtual = "AvaliacaoAsCegas";
        var avaliacoesCompetencia = await db.AvaliacoesCompetenciasNotas
            .Where(a => a.IdAssociado == avaliacaoEmail.idAssociado && a.IdProjeto == avaliacaoEmail.idProjeto && a.IdPeriodo == avaliacaoEmail.idPeriodo && a.Etapa == etapaAtual)
            .ToListAsync();
        var avaliacoesPerformance = await db.AvaliacoesPerformance
            .Where(a => a.IdAssociado == avaliacaoEmail.idAssociado && a.IdProjeto == avaliacaoEmail.idProjeto && a.IdPeriodo == avaliacaoEmail.idPeriodo && a.PosicaoAtualFluxoAvaliacao == etapaAtual)
            .ToListAsync();
        if (!avaliacoesCompetencia.Any() || !avaliacoesPerformance.Any())
            return false;
        foreach (var av in avaliacoesCompetencia)
        {
            if (av.IdNotaNivel1AvaliacaoCegas == null || av.IdNotaNivel2AvaliacaoCegas == null || av.IdNotaNivel1AvaliacaoCegas <= 0 || av.IdNotaNivel2AvaliacaoCegas <= 0)
            {
                // Ajuste automático se modo == 2
                if (av.IdModo == 2)
                {
                    av.IdNotaNivel1AvaliacaoCegas = av.IdNotaPadraoNivel1 ?? 5;
                    av.IdNotaNivel2AvaliacaoCegas = av.IdNotaPadraoNivel2 ?? 5;
                }
                else
                {
                    return false;
                }
            }
        }
        foreach (var av in avaliacoesPerformance)
        {
            if (av.IdNotaNivel1Feedback == null || av.IdNotaNivel1Feedback <= 0)
                return false;
        }
        var dataHoraFinalizacao = DateTime.Now;
        foreach (var av in avaliacoesPerformance)
        {
            av.DataHoraFimFeedback = dataHoraFinalizacao;
            db.AvaliacoesPerformance.Update(av);
        }
        foreach (var av in avaliacoesCompetencia)
        {
            av.DataHoraFimAvaliacaoCegas = dataHoraFinalizacao;
            db.AvaliacoesCompetenciasNotas.Update(av);
        }
        await db.SaveChangesAsync();
        return true;
    }
}