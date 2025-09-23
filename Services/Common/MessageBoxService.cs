namespace Peers.Moderno.Services.Common;

public interface IMessageBoxService
{
    Task ExibirMensagemAsync(string mensagem, TipoMensagem tipo = TipoMensagem.Info);
    Task<bool> ConfirmarAsync(string mensagem);
    event Action<string, TipoMensagem>? OnMensagemExibida;
}

public class MessageBoxService : IMessageBoxService
{
    public event Action<string, TipoMensagem>? OnMensagemExibida;

    public Task ExibirMensagemAsync(string mensagem, TipoMensagem tipo = TipoMensagem.Info)
    {
        OnMensagemExibida?.Invoke(mensagem, tipo);
        return Task.CompletedTask;
    }

    public Task<bool> ConfirmarAsync(string mensagem)
    {
        // Esta implementação será expandida futuramente para usar modais personalizados
        // Por enquanto, retorna true para manter compatibilidade
        return Task.FromResult(true);
    }
}

public enum TipoMensagem
{
    Info,
    Success,
    Warning,
    Error
}