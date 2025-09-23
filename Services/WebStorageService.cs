using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class WebStorageService : IWebStorageService
{
    public UsuarioLogado GetUsuarioLogado()
    {
        return new UsuarioLogado
        {
            Id = 1,
            IdCargo = 1,
            Nome = "Usuário Logado"
        };
    }
}