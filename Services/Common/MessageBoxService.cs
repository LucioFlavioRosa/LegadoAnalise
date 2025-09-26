namespace Peers.Moderno.Services.Common;

public interface IMessageBoxService
{
    void ShowSuccess(string message, string? title = null, int? delay = null);
    void ShowError(string message, string? title = null, int? delay = null);
    void ShowInfo(string message, string? title = null, int? delay = null);
    void ShowWarning(string message, string? title = null, int? delay = null);
    event Action<MessageBoxEventArgs>? OnMessageReceived;
}

public class MessageBoxService : IMessageBoxService
{
    public event Action<MessageBoxEventArgs>? OnMessageReceived;

    public void ShowSuccess(string message, string? title = null, int? delay = null)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Success, title, delay));
    }

    public void ShowError(string message, string? title = null, int? delay = null)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Error, title, delay));
    }

    public void ShowInfo(string message, string? title = null, int? delay = null)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Info, title, delay));
    }

    public void ShowWarning(string message, string? title = null, int? delay = null)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Warning, title, delay));
    }
}

public class MessageBoxEventArgs
{
    public string Message { get; }
    public MessageBoxType Type { get; }
    public string? Title { get; }
    public int? Delay { get; }

    public MessageBoxEventArgs(string message, MessageBoxType type, string? title = null, int? delay = null)
    {
        Message = message;
        Type = type;
        Title = title;
        Delay = delay;
    }
}

public enum MessageBoxType
{
    Success,
    Error,
    Info,
    Warning
}
