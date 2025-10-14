using System;
using System.Collections.Generic;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Domain.Validators
{
    /// <summary>
    /// Responsável por validar regras de negócio do domínio Cargo.
    /// </summary>
    public class CargoValidator
    {
        /// <summary>
        /// Valida uma instância de Cargo e retorna os erros encontrados.
        /// </summary>
        /// <param name="cargo">Instância de Cargo a ser validada.</param>
        /// <returns>Lista de mensagens de erro. Vazia se válido.</returns>
        public IList<string> Validar(Cargo cargo)
        {
            var erros = new List<string>();

            if (cargo == null)
            {
                erros.Add("Cargo não pode ser nulo.");
                return erros;
            }

            if (string.IsNullOrWhiteSpace(cargo.NomeCargo))
                erros.Add("O campo 'Cargo' é obrigatório.");

            if (cargo.TempoMinimoPromocao < 0)
                erros.Add("O tempo mínimo para promoção deve ser maior ou igual a zero.");

            if (cargo.ProximoCargoId.HasValue && cargo.ProximoCargoId.Value == cargo.IdCargo)
                erros.Add("O próximo cargo não pode ser igual ao cargo atual.");

            // Outras regras de negócio podem ser adicionadas aqui

            return erros;
        }
    }
}
