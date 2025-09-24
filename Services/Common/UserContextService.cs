using Peers.Moderno.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Peers.Moderno.Services.Common;

public interface IUserContextService
{
    Associado? GetUsuarioLogado();
    void SetUsuarioLogado(Associado user);
    bool IsAuthenticated();
    void ClearUsuarioLogado();
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string USER_SESSION_KEY = "UsuarioLogado";

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Associado? GetUsuarioLogado()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return null;

        var userJson = session.GetString(USER_SESSION_KEY);
        if (string.IsNullOrEmpty(userJson)) return null;

        return JsonConvert.DeserializeObject<Associado>(userJson);
    }

    public void SetUsuarioLogado(Associado user)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null) return;

        var userJson = JsonConvert.SerializeObject(user);
        session.SetString(USER_SESSION_KEY, userJson);
    }

    public bool IsAuthenticated()
    {
        return GetUsuarioLogado() != null;
    }

    public void ClearUsuarioLogado()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.Remove(USER_SESSION_KEY);
    }
}