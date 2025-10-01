using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Models;
using Services.Auth;
using Business.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Services.Auth
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _cache;
        private readonly AssociadosService _associadosService;

        public UserSessionService(
            IHttpContextAccessor httpContextAccessor,
            IDistributedCache cache,
            AssociadosService associadosService)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _associadosService = associadosService;
        }

        public async Task CreateUserSessionAsync(UserInfo userInfo, ASSOCIADOS associado)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, associado.Email ?? userInfo.Email ?? ""),
                new Claim(ClaimTypes.Name, associado.Nome ?? userInfo.Name ?? ""),
                new Claim("IdAssociado", associado.IdAssociado.ToString()),
                new Claim("IdEmpresa", associado.IdEmpresa.ToString()),
                new Claim("IdCargo", associado.IdCargo.ToString()),
                new Claim("IdPerfil", associado.IdPerfil.ToString()),
                new Claim("IsLogged", "true")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public Task<UsuarioLogado> GetCurrentUserAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
                return Task.FromResult<UsuarioLogado>(null);

            var email = user.FindFirstValue(ClaimTypes.Email);
            var nome = user.FindFirstValue(ClaimTypes.Name);
            var idAssociado = user.FindFirstValue("IdAssociado");
            var idEmpresa = user.FindFirstValue("IdEmpresa");
            var idCargo = user.FindFirstValue("IdCargo");
            var idPerfil = user.FindFirstValue("IdPerfil");

            var usuarioLogado = new UsuarioLogado
            {
                Email = email,
                Nome = nome,
                Id = idAssociado,
                IdEmpresa = idEmpresa,
                IdCargo = idCargo,
                IdPerfil = idPerfil,
                IsLogged = true
            };
            return Task.FromResult(usuarioLogado);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
