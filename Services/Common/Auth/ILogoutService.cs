namespace Peers.Moderno.Services.Common.Auth;

public interface ILogoutService
{
    Task<string> LogoutAsync();
}