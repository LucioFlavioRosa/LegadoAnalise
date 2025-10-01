using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Models;

namespace Services.Common
{
    public class UserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UsuarioLogado GetUsuarioLogado()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
                return null;

            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("preferred_username")?.Value;
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var nome = user.FindFirst(ClaimTypes.Name)?.Value;
            var idEmpresaStr = user.FindFirst("IdEmpresa")?.Value;
            var idCargoStr = user.FindFirst("IdCargo")?.Value;
            var idPerfilStr = user.FindFirst("IdPerfil")?.Value;

            int.TryParse(idEmpresaStr, out int idEmpresa);
            int.TryParse(idCargoStr, out int idCargo);
            int.TryParse(idPerfilStr, out int idPerfil);

            return new UsuarioLogado
            {
                Email = email,
                Id = id,
                Nome = nome,
                IdEmpresa = idEmpresa,
                IdCargo = idCargo,
                IdPerfil = idPerfil,
                IsLogged = true
            };
        }
    }
}