namespace Peers.Moderno.Services.Feedback.Common;

public interface IFeedbackValidator
{
    bool ValidarNotas(int notaNivel1, int notaNivel2);
    bool ValidarNotaRange(int nota, int notaMinima, int notaMaxima);
    bool ValidarNotasObrigatorias(int[] notas, int notaMinima, int notaMaxima);
}