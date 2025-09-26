namespace Peers.Moderno.Services.Feedback.Common;

public interface IFeedbackValidator
{
    bool ValidarNotas(int notaNivel1, int notaNivel2);
}

public class FeedbackValidator : IFeedbackValidator
{
    public bool ValidarNotas(int notaNivel1, int notaNivel2)
    {
        if (notaNivel1 == 5 && notaNivel2 != 5)
            return false;
        if ((notaNivel1 > 0) && (notaNivel2 > 0) && (notaNivel2 > notaNivel1))
            return false;
        return true;
    }
}
