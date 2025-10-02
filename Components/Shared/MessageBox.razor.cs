using Microsoft.AspNetCore.Components;

public class MessageBoxBase : ComponentBase
{
    [Parameter] public string Title { get; set; } = "Mensagem";
    [Parameter] public string Message { get; set; }
    [Parameter] public MessageType Type { get; set; } = MessageType.Info;
    [Parameter] public EventCallback OnClose { get; set; }
    public bool Visible { get; set; }

    public void Show(string message, string title = "Mensagem", MessageType type = MessageType.Info)
    {
        Message = message;
        Title = title;
        Type = type;
        Visible = true;
        StateHasChanged();
    }

    public void Close()
    {
        Visible = false;
        OnClose.InvokeAsync();
        StateHasChanged();
    }

    public string GetAlertClass()
    {
        return Type switch
        {
            MessageType.Info => "alert alert-info",
            MessageType.Warning => "alert alert-warning",
            MessageType.Error => "alert alert-danger",
            _ => "alert alert-secondary"
        };
    }
}

public enum MessageType
{
    Info,
    Warning,
    Error
}
