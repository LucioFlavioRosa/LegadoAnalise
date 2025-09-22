using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Services;

public interface IEmpresasService
{
    Task<List<Empresa>> ObterTodasAsync(bool apenasAtivas = false);
    Task<Empresa?> ObterPorIdAsync(int id);
}