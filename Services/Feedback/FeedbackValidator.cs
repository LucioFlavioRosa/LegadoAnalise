using Peers.Moderno.Services.Feedback.Common;

public class FeedbackValidator : IFeedbackValidator
{
    public bool ValidarNotas(int notaNivel1, int notaNivel2)
    {
        // Regra: nota do nível 2 não pode ser maior que a do nível 1
        return notaNivel2 <= notaNivel1;
    }

    public bool ValidarNotaRange(int nota, int notaMinima, int notaMaxima)
    {
        return nota >= notaMinima && nota <= notaMaxima;
    }

    public bool ValidarNotasObrigatorias(int[] notas, int notaMinima, int notaMaxima)
    {
        if (notas == null || notas.Length == 0)
            return false;
        foreach (var nota in notas)
        {
            if (!ValidarNotaRange(nota, notaMinima, notaMaxima))
                return false;
        }
        return true;
    }
}