using System;
using System.Collections.Generic;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Infrastructure.Validators
{
    public interface ICargoValidator
    {
        IEnumerable<string> Validate(Cargo cargo);
        bool IsValid(Cargo cargo);
    }

    public class CargoValidator : ICargoValidator
    {
        public IEnumerable<string> Validate(Cargo cargo)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(cargo.NomeCargo))
                errors.Add("O campo 'Cargo' é obrigatório.");
            if (!cargo.TempoMinimoPromocao.HasValue || cargo.TempoMinimoPromocao.Value < 0)
                errors.Add("O campo 'Tempo mínimo para promoção' deve ser um número positivo.");
            // Outras validações específicas podem ser adicionadas aqui
            return errors;
        }

        public bool IsValid(Cargo cargo)
        {
            return Validate(cargo) == null || ((List<string>)Validate(cargo)).Count == 0;
        }
    }
}
