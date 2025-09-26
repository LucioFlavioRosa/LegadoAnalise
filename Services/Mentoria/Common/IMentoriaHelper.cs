using System.Collections.Generic;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Mentoria.Common;

public interface IMentoriaHelper
{
    List<PDIPillsModel> MontarPillsModel(List<PERIODOSAVALIACOES> periodos, PERIODOSAVALIACOES ultimoPeriodo, int idMentorado, int idMentor, List<MENTORADORESPOSTAS> respostasMentorado);
    List<MentoradoRespostaPill> MontarRespostaPills(List<MENTORADORESPOSTAS> respostasTodas, List<PERIODOSAVALIACOES> periodos, PERIODOSAVALIACOES ultimoPeriodo, Associado mentor, string fotoMentor, List<MentorPergunta> perguntas, List<MentorNota> notas, bool periodoEnabled);
}
