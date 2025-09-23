using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Eixos;

public interface IEixosService
{
    Task<List<Eixo>> ListAsync();
    Task<Eixo?> GetByIdAsync(int id);
    Task<Eixo> CreateAsync(Eixo eixo);
    Task<Eixo> UpdateAsync(Eixo eixo);
    Task<bool> InactivateAsync(int id);
    Task<byte[]> ExportAsync();
}