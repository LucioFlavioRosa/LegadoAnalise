using Peers.Moderno.Services.Complexidades.Common;

namespace Peers.Moderno.Services.Complexidades.Common;

public interface IComplexidadeService
{
    Task<List<ComplexidadeDto>> ObterListaComplexidadesAsync();
    Task<ComplexidadeDto?> ObterComplexidadeAsync(int id);
    Task<bool> InserirComplexidadeAsync(ComplexidadeDto dto);
    Task<bool> AlterarComplexidadeAsync(ComplexidadeDto dto);
    Task<bool> ExcluirComplexidadeAsync(int id);
    Task<bool> ValidarComplexidadeAsync(ComplexidadeDto dto);
}