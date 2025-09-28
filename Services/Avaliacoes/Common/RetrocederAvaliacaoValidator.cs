using System;
using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;

namespace Services.Avaliacoes.Common;

public class RetrocederAvaliacaoValidator
{
    // Constantes das etapas
    public const string EtapaNaoIniciada = "AVM";
    public const string EtapaAutoAvaliacao = "AAV";
    public const string EtapaAvaliacaoAsCegas = "ACE";
    public const string EtapaEmParalelo = "AEP";
    public const string EtapaAvaliacaoGestor = "AGE";
    public const string EtapaFeedback = "FED";
    public const string EtapaAvaliacaoMentor = "AME";
    public const string EtapaAvaliacaoFinalizada = "AFI";

    public RetrocederAvaliacaoValidacaoResult ValidarRetrocesso(AvaliacaoEmail avaliacao, List<AvaliacaoCompetencia> competencias, List<AvaliacaoPerformance> performances)
    {
        if (avaliacao == null)
            return RetrocederAvaliacaoValidacaoResult.Falha("Avaliação não encontrada.");

        bool competenciasAutoAvaliacaoFinalizada = competencias.Any(c => c.DataHoraFimAutoAvaliacao != null);
        bool performancesAutoAvaliacaoFinalizada = performances.Any(c => c.DataHoraFimAutoAvaliacao != null);
        bool competenciasAsCegasFinalizada = competencias.Any(c => c.DataHoraFimAvaliacaoCegas != null);
        bool performancesAsCegasFinalizada = performances.Any(c => c.DataHoraFimAvaliacaoCegas != null);

        bool autoAvaliacaoFinalizada = competenciasAutoAvaliacaoFinalizada && performancesAutoAvaliacaoFinalizada;
        bool avaliacaoAsCegasFinalizada = competenciasAsCegasFinalizada && performancesAsCegasFinalizada;

        if (!autoAvaliacaoFinalizada && !avaliacaoAsCegasFinalizada)
        {
            return RetrocederAvaliacaoValidacaoResult.Falha("Não é possível retroceder a avaliação. Auto avaliação e Avaliação às Cegas não concluídas");
        }
        return RetrocederAvaliacaoValidacaoResult.Sucesso();
    }

    public RetrocederAvaliacaoValidacaoResult ValidarEfetuarRetrocesso(AvaliacaoEmail avaliacao, List<AvaliacaoCompetencia> competencias, List<AvaliacaoPerformance> performances, string novaFase)
    {
        if (avaliacao == null)
            return RetrocederAvaliacaoValidacaoResult.Falha("Avaliação não encontrada.");

        var etapasPermitidas = new[] { EtapaAvaliacaoAsCegas, EtapaAutoAvaliacao, EtapaNaoIniciada, EtapaEmParalelo };
        if (!etapasPermitidas.Contains(avaliacao.PosicaoAtualFluxoAvaliacao))
            return RetrocederAvaliacaoValidacaoResult.Falha("A avaliação não está em uma etapa permitida para retrocesso.");

        // Validações específicas por fase
        if (novaFase == EtapaAvaliacaoGestor)
        {
            var competenciasAutoCegasEstaoRespondidas = competencias.All(c =>
                c.IdNotaNivel1AutoAvaliacao != null &&
                c.IdNotaNivel2AutoAvaliacao != null &&
                c.IdNotaNivel1AvaliacaoCegas != null &&
                c.IdNotaNivel2AvaliacaoCegas != null
            );
            var performancesEstaoAutoCegasRespondidas = performances.All(c =>
                c.IdNotaNivel1AutoAvaliacao != null &&
                c.IdNotaNivel1AvaliacaoCegas != null
            );
            if (!competenciasAutoCegasEstaoRespondidas || !performancesEstaoAutoCegasRespondidas)
            {
                return RetrocederAvaliacaoValidacaoResult.Falha("Não é possível avançar a etapa da avaliação. Auto avaliação e/ou Avaliação às Cegas não concluídas");
            }
        }
        if (novaFase == EtapaFeedback)
        {
            var competenciasGestorEstaoRespondidas = competencias.All(c =>
                c.IdNotaNivel1AvaliacaoGestor != null &&
                c.IdNotaNivel2AvaliacaoGestor != null
            );
            var performancesGestorEstaoRespondidas = performances.All(c =>
                c.IdNotaNivel1AvaliacaoGestor != null
            );
            if (!competenciasGestorEstaoRespondidas || !performancesGestorEstaoRespondidas)
            {
                return RetrocederAvaliacaoValidacaoResult.Falha("Não é possível avançar a etapa da avaliação. Avaliação do Gestor não concluída");
            }
        }
        if (novaFase == EtapaAvaliacaoMentor)
        {
            var competenciasFeedbackEstaoRespondidas = competencias.All(c =>
                c.IdNotaNivel1Feedback != null &&
                c.IdNotaNivel2Feedback != null
            );
            var performancesFeedbackEstaoRespondidas = performances.All(c =>
                c.IdNotaNivel1Feedback != null
            );
            if (!competenciasFeedbackEstaoRespondidas || !performancesFeedbackEstaoRespondidas)
            {
                return RetrocederAvaliacaoValidacaoResult.Falha("Não é possível avançar a etapa da avaliação. Feedback não concluído");
            }
        }
        return RetrocederAvaliacaoValidacaoResult.Sucesso();
    }
}

public class RetrocederAvaliacaoValidacaoResult
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }

    public static RetrocederAvaliacaoValidacaoResult Sucesso()
    {
        return new RetrocederAvaliacaoValidacaoResult { Sucesso = true };
    }
    public static RetrocederAvaliacaoValidacaoResult Falha(string mensagem)
    {
        return new RetrocederAvaliacaoValidacaoResult { Sucesso = false, Mensagem = mensagem };
    }
}
