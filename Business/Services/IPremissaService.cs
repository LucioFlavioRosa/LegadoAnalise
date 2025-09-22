using Business.Model;

namespace Business.Services
{
    public interface IPremissaService
    {
        Task<Premissa?> ObterPremissaPorCardoAsync(int idCargo);
    }
}