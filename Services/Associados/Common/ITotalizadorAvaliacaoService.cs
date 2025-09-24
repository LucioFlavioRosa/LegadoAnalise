using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Associados.Common;

public interface ITotalizadorAvaliacaoService
{
    Task<List<ComboItem>> ObterMentoresAsync();
    Task<List<ComboItem>> ObterCargosAsync();
    Task<List<ComboItem>> ObterPerfisAsync();
    Task<List<ComboItem>> ObterStatusAsync();
    Task<ValidationResult> ValidarAssociadoAsync(AssociadoFormModel model);
    Task<OperationResult> CadastrarAssociadoAsync(AssociadoFormModel model);
    Task<OperationResult> AtualizarAssociadoAsync(int id, AssociadoFormModel model);
    Task<AssociadoFormModel?> ObterAssociadoParaEdicaoAsync(int id);
    Task<List<Associado>> ListarAssociadosAsync();
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
    public string Status { get; set; } = string.Empty;
    public bool Ativo => Status == "1";
}

public class OperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string ErrorMessage { get; private set; } = string.Empty;
    public object? Data { get; private set; }

    private OperationResult() { }

    public static OperationResult Success(string message = "Operação realizada com sucesso", object? data = null)
    {
        return new OperationResult
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static OperationResult Error(string errorMessage)
    {
        return new OperationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}