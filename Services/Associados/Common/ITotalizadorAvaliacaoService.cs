using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Associados.Common;

public interface ITotalizadorAvaliacaoService
{
    Task<List<ComboItem>> ObterMentoresAsync();
    Task<List<ComboItem>> ObterCargosAsync();
    Task<List<ComboItem>> ObterPerfisAsync();
    Task<List<ComboItem>> ObterStatusAsync();
    Task<AssociadoFormModel> ObterAssociadoParaEdicaoAsync(int id);
    Task<OperationResult> SalvarAssociadoAsync(AssociadoFormModel model);
    Task<OperationResult> ValidarAssociadoAsync(AssociadoFormModel model);
    Task<bool> ExisteEmailAsync(string email, int? idExcluir = null);
}

public class AssociadoFormModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string IdMentor { get; set; } = string.Empty;
    public string IdCargo { get; set; } = string.Empty;
    public string IdPerfil { get; set; } = string.Empty;
    public string Status { get; set; } = "1";
    public bool IsEdicao => Id > 0;
}

public class OperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new List<string>();

    private OperationResult() { }

    public static OperationResult Success(string message = "Operação realizada com sucesso")
    {
        return new OperationResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static OperationResult Error(string message, List<string>? errors = null)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static OperationResult ValidationError(List<string> errors)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Message = "Erro de validação",
            Errors = errors
        };
    }
}