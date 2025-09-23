using Peers.Moderno.Services.DisparoMassivoRH.Common;

namespace Peers.Moderno.Services.DisparoMassivoRH.Common;

public interface IDisparoMassivoRHService
{
    Task<List<DisparoMassivoRHItem>> GetNovosDisparosAsync();
    Task<DisparoEmailResult> DispararTodosAsync(List<DisparoMassivoRHItem> disparos);
    Task<DisparoEmailResult> DispararEmailAsync(DisparoEmailRequest request);
    Task<ConfiguracaoEmail> ObterConfiguracaoEmailAsync(int idEmpresa);
    string ConfigurarCorpoEmail(string template, Models.Associado associado, Models.Associado mentor, string prazoFinal);
}