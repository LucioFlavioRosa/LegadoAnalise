using System.Collections.Generic;
using System.Linq;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Mentoria.Common;

namespace Peers.Moderno.Services.Mentoria;

public class MentoriaHelper : IMentoriaHelper
{
    public List<PDIPillsModel> MontarPillsModel(List<PERIODOSAVALIACOES> periodos, PERIODOSAVALIACOES ultimoPeriodo, int idMentorado, int idMentor, List<MENTORADORESPOSTAS> respostasMentorado)
    {
        var pdiPillsModel = new List<PDIPillsModel>();
        foreach (var periodo in periodos)
        {
            var hasRespostas = respostasMentorado.Any(r => r.idPeriodo == periodo.IdPeriodo);
            if (hasRespostas)
            {
                var addPill = new PDIPillsModel
                {
                    id = $"tab-{periodo.IdPeriodo}-tab",
                    href = $"#tab-{periodo.IdPeriodo}",
                    ariacontrols = $"tab-{periodo.IdPeriodo}",
                    ariaselected = periodo.IdPeriodo == ultimoPeriodo.IdPeriodo ? "true" : "false",
                    active = periodo.IdPeriodo == ultimoPeriodo.IdPeriodo ? "active" : string.Empty,
                    classe = periodo.IdPeriodo == ultimoPeriodo.IdPeriodo ? "btn btn-danger" : "btn btn-facebook",
                    Periodo = periodo.Periodo
                };
                pdiPillsModel.Add(addPill);
            }
        }
        return pdiPillsModel;
    }

    public List<MentoradoRespostaPill> MontarRespostaPills(List<MENTORADORESPOSTAS> respostasTodas, List<PERIODOSAVALIACOES> periodos, PERIODOSAVALIACOES ultimoPeriodo, Associado mentor, string fotoMentor, List<MentorPergunta> perguntas, List<MentorNota> notas, bool periodoEnabled)
    {
        var pillModel = new List<MentoradoRespostaPill>();
        var periodosTodos = respostasTodas.Select(x => x.idPeriodo).Distinct().OrderBy(x => x).ToList();
        foreach (var periodo in periodosTodos)
        {
            var addPeriodo = new MentoradoRespostaPill
            {
                idPeriodo = periodo,
                Periodo = periodos.FirstOrDefault(p => p.IdPeriodo == periodo)?.Periodo ?? string.Empty,
                id = $"tab-{periodo}",
                active = periodo == ultimoPeriodo.IdPeriodo ? "active in show" : string.Empty,
                arialabelled = $"tab-{periodo}-tab",
                respostas = new List<MentoradoRespostasModel>(),
                MentorNome = mentor.Nome,
                MentorFoto = fotoMentor
            };
            var isPeriodoEnabled = periodo == ultimoPeriodo.IdPeriodo;
            var unselectColor = isPeriodoEnabled ? "#FFFFFF" : "#C0C4CF";
            foreach (var pergunta in perguntas)
            {
                var item = respostasTodas.FirstOrDefault(x => x.idPeriodo == periodo && x.idPergunta == pergunta.idMentorPergunta);
                var addResposta = new MentoradoRespostasModel
                {
                    idPergunta = pergunta.idMentorPergunta,
                    Pergunta = pergunta.Descricao,
                    idModo = pergunta.idModo,
                    idResposta = item?.idResposta ?? 0,
                    Comentarios = item?.Comentario ?? string.Empty,
                    ComentarioColor = unselectColor,
                    Enabled = isPeriodoEnabled,
                    notas = new List<MentoradoRespostasNotasModel>()
                };
                foreach (var nota in notas)
                {
                    var addNota = new MentoradoRespostasNotasModel
                    {
                        BackgroundColor = unselectColor,
                        idNota = nota.idNota,
                        Escala = -1,
                        NotaTexto = nota.Descricao,
                        Icone = nota.Icone,
                        HideIcone = "hidden",
                        Enabled = isPeriodoEnabled,
                        idResposta = item?.idResposta ?? 0
                    };
                    if (item != null && item.idNota == nota.idNota)
                    {
                        addNota.BackgroundColor = "#E5F419";
                    }
                    addResposta.notas.Add(addNota);
                }
                addPeriodo.respostas.Add(addResposta);
            }
            pillModel.Add(addPeriodo);
        }
        return pillModel;
    }
}
