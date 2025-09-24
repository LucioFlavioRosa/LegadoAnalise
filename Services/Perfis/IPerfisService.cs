using Peers.Moderno.Services.Perfis.Common;

namespace Peers.Moderno.Services.Perfis;

public interface IPerfisService
{
    Task<List<PerfilDto>> ListarPerfisAsync();
    Task<PerfilDto?> ObterPerfilAsync(int id);
    Task<bool> InserirPerfilAsync(PerfilDto perfil);
    Task<bool> AlterarPerfilAsync(PerfilDto perfil);
    Task<bool> InativarPerfilAsync(int id);
    Task<bool> ExistePerfilAsync(string nome, int? idExcluir = null);
}