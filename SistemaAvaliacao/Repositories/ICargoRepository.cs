using System.Collections.Generic;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Repositories
{
    public interface ICargoRepository
    {
        Cargo GetById(int id);
        IEnumerable<Cargo> GetAll();
        void Add(Cargo cargo);
        void Update(Cargo cargo);
        void Delete(int id);
        IEnumerable<Cargo> GetCargosForExport();
    }
}
