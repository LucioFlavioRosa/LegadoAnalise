using System.Threading.Tasks;
using Models;
using Services.Auth;

namespace Services.Auth
{
    public interface IUserSessionService
    {
        Task CreateUserSessionAsync(UserInfo userInfo, ASSOCIADOS associado);
        Task<UsuarioLogado> GetCurrentUserAsync();
        Task SignOutAsync();
    }
}
