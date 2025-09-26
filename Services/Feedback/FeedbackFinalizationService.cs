using Peers.Moderno.Services.Feedback.Common;
using Peers.Moderno.Services.Common;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Peers.Moderno.Services.Feedback;

public class FeedbackFinalizationService : IFeedbackFinalizationService
{
    private readonly IFeedbackService _feedbackService;
    private readonly IMessageBoxService _messageBoxService;

    public FeedbackFinalizationService(IFeedbackService feedbackService, IMessageBoxService messageBoxService)
    {
        _feedbackService = feedbackService;
        _messageBoxService = messageBoxService;
    }

    public async Task<FeedbackFinalizationResult> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId)
    {
        var avaliacaoEmail = await _feedbackService.ObterAvaliacaoEmailAsync(idAvaliacao);
        if (avaliacaoEmail == null)
        {
            return new FeedbackFinalizationResult
            {
                Sucesso = false,
                Mensagem = "É obrigatório digitar todas as notas das Avaliações Competência e Performance antes de finalizar a Avaliação"
            };
        }

        string etapaAtual = "Feedback";
        var avaliacoesPerformance = await _feedbackService.ObterAvaliacoesPerformanceAsync(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual, true);
        if (avaliacoesPerformance.Count == 0)
        {
            return new FeedbackFinalizationResult
            {
                Sucesso = false,
                Mensagem = "É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação"
            };
        }
        var avaliacoesCompetencia = await _feedbackService.ObterAvaliacoesCompetenciasAsync(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo, etapaAtual, avaliacaoEmail.TipoAvaliacao, avaliacaoEmail.Escopo, avaliacaoEmail.idAvaliacao);
        if (avaliacoesCompetencia.Count == 0)
        {
            return new FeedbackFinalizationResult
            {
                Sucesso = false,
                Mensagem = "É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação"
            };
        }
        foreach (var avaliacao in avaliacoesPerformance)
        {
            if (avaliacao.IdNotaNivel1Feedback <= 0)
            {
                return new FeedbackFinalizationResult
                {
                    Sucesso = false,
                    Mensagem = "É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação"
                };
            }
        }
        foreach (var avaliacao in avaliacoesCompetencia)
        {
            if (avaliacao.IdNotaNivel1Feedback <= 0 || avaliacao.IdNotaNivel2Feedback <= 0)
            {
                if (avaliacao.COMPETENCIAS?.IdModo == 1)
                {
                    return new FeedbackFinalizationResult
                    {
                        Sucesso = false,
                        Mensagem = "É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação"
                    };
                }
                else if (avaliacao.COMPETENCIAS?.IdModo == 2)
                {
                    avaliacao.IdNotaNivel1Feedback = avaliacao.COMPETENCIAS.IdNotaPadraoNivel1 ?? 5;
                    avaliacao.IdNotaNivel2Feedback = avaliacao.COMPETENCIAS.IdNotaPadraoNivel2 ?? 5;
                }
            }
        }
        DateTime dataHoraFinalizacao = DateTime.Now;
        foreach (var avaliacao in avaliacoesPerformance)
        {
            avaliacao.DataHoraFimFeedback = dataHoraFinalizacao;
            await _feedbackService.AlterarAvaliacaoPerformanceAsync(avaliacao.IdAvaliacaoPerformance, avaliacao);
            await _feedbackService.AvancarProximaEtapaPerformanceAsync(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
        }
        foreach (var avaliacao in avaliacoesCompetencia)
        {
            avaliacao.DataHoraFimFeedback = dataHoraFinalizacao;
            await _feedbackService.AlterarAvaliacaoCompetenciaAsync(avaliacao.IdAvaliacaoCompetencia, avaliacao);
            await _feedbackService.AvancarProximaEtapaCompetenciaAsync(avaliacao, avaliacaoEmail, avaliacaoEmail.TipoAvaliacao);
        }
        await _feedbackService.AvancarProximaEtapaEmailAsync(avaliacaoEmail);
        return new FeedbackFinalizationResult
        {
            Sucesso = true,
            Mensagem = "Avaliação Finalizada com Sucesso",
            RedirectUrl = $"/avalizacao_feedback?IdProjeto={avaliacaoEmail.idProjeto}&IdPeriodo={avaliacaoEmail.idPeriodo}&Finalizou=Y"
        };
    }
}
