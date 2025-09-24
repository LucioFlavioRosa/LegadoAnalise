using Microsoft.AspNetCore.Http;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Associados;
using System.Text.Json;

namespace Peers.Moderno.Services.Common;

public interface IUserContextService
{
    Task<Associado?> GetUsuarioLogadoAsync();
    Task SetUsuarioLogadoAsync(Associado usuario);
    bool IsAuthenticated();
    Task ClearUserContextAsync();
    Task<bool> ValidateUserSessionAsync();
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAssociadosService _associadosService;
    private readonly ITelemetryService _telemetryService;
    private const string USER_SESSION_KEY = "UsuarioLogado";

    public UserContextService(
        IHttpContextAccessor httpContextAccessor,
        IAssociadosService associadosService,
        ITelemetryService telemetryService)
    {
        _httpContextAccessor = httpContextAccessor;
        _associadosService = associadosService;
        _telemetryService = telemetryService;
    }

    public async Task<Associado?> GetUsuarioLogadoAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Session == null)
                return null;

            var userJson = context.Session.GetString(USER_SESSION_KEY);
            if (string.IsNullOrEmpty(userJson))
                return null;

            var usuario = JsonSerializer.Deserialize<Associado>(userJson);
            if (usuario != null)
            {
                var isValid = await ValidateUserSessionAsync();
                if (!isValid)
                {
                    await ClearUserContextAsync();
                    return null;
                }
            }

            return usuario;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetUsuarioLogadoAsync" },
                { "Component", "UserContextService" }
            });
            return null;
        }
    }

    public async Task SetUsuarioLogadoAsync(Associado usuario)
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Session == null)
                throw new InvalidOperationException("Sessão não disponível");

            var userJson = JsonSerializer.Serialize(usuario);
            context.Session.SetString(USER_SESSION_KEY, userJson);

            _telemetryService.TrackEvent("UserLogin", new Dictionary<string, string>
            {
                { "UserId", usuario.Id.ToString() },
                { "UserName", usuario.Nome },
                { "ProfileId", usuario.IdPerfil.ToString() }
            });

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "SetUsuarioLogadoAsync" },
                { "Component", "UserContextService" },
                { "UserId", usuario?.Id.ToString() ?? "Unknown" }
            });
            throw;
        }
    }

    public bool IsAuthenticated()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Session == null)
                return false;

            var userJson = context.Session.GetString(USER_SESSION_KEY);
            return !string.IsNullOrEmpty(userJson);
        }
        catch
        {
            return false;
        }
    }

    public async Task ClearUserContextAsync()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Session != null)
            {
                context.Session.Remove(USER_SESSION_KEY);
                context.Session.Clear();
            }

            _telemetryService.TrackEvent("UserLogout");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ClearUserContextAsync" },
                { "Component", "UserContextService" }
            });
        }
    }

    public async Task<bool> ValidateUserSessionAsync()
    {
        try
        {
            var usuario = await GetUsuarioLogadoFromSession();
            if (usuario == null)
                return false;

            var currentUser = await _associadosService.ObterAssociadoAsync(usuario.Id);
            return currentUser != null && currentUser.Ativo;
        }
        catch
        {
            return false;
        }
    }

    private async Task<Associado?> GetUsuarioLogadoFromSession()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.Session == null)
            return null;

        var userJson = context.Session.GetString(USER_SESSION_KEY);
        if (string.IsNullOrEmpty(userJson))
            return null;

        return JsonSerializer.Deserialize<Associado>(userJson);
    }
}