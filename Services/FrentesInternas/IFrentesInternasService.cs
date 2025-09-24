namespace Peers.Moderno.Services.FrentesInternas;

public interface IFrentesInternasService
{
    Task<IEnumerable<FrenteInternaLider>> ObterLideresFrentesInternasAsync();
    Task<IEnumerable<dynamic>> ObterFrentesInternasAsync();
    Task<dynamic?> ObterFrenteInternaAsync(int id);
    Task<bool> CriarFrenteInternaAsync(dynamic frenteInterna);
    Task<bool> AtualizarFrenteInternaAsync(dynamic frenteInterna);
    Task<bool> ExcluirFrenteInternaAsync(int id);
}

public class FrenteInternaLider
{
    public int IdAssociado { get; set; }
    public string NomeAssociado { get; set; } = string.Empty;
    public int IdFrenteInterna { get; set; }
    public string NomeFrenteInterna { get; set; } = string.Empty;
}