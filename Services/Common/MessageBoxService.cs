namespace Peers.Moderno.Services.Common;

public interface IMessageBoxService
{
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowInfo(string message);
    void ShowWarning(string message);
    event Action<MessageBoxEventArgs>? OnMessageReceived;
}

public class MessageBoxService : IMessageBoxService
{
    public event Action<MessageBoxEventArgs>? OnMessageReceived;

    public void ShowSuccess(string message)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Success));
    }

    public void ShowError(string message)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Error));
    }

    public void ShowInfo(string message)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Info));
    }

    public void ShowWarning(string message)
    {
        OnMessageReceived?.Invoke(new MessageBoxEventArgs(message, MessageBoxType.Warning));
    }
}

public class MessageBoxEventArgs
{
    public string Message { get; }
    public MessageBoxType Type { get; }

    public MessageBoxEventArgs(string message, MessageBoxType type)
    {
        Message = message;
        Type = type;
    }
}

public enum MessageBoxType
{
    Success,
    Error,
    Info,
    Warning
}
