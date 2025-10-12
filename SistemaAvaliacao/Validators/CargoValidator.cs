using System.Collections.Generic;
using SistemaAvaliacao.Models;

namespace SistemaAvaliacao.Validators
{
    public class CargoValidator
    {
        public IList<string> Validate(CargoViewModel cargo)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(cargo.Cargo))
                errors.Add("O campo 'Cargo' é obrigatório.");
            if (cargo.TempoMinimo < 0)
                errors.Add("O tempo mínimo para promoção não pode ser negativo.");
            if (string.IsNullOrWhiteSpace(cargo.Funcao))
                errors.Add("O campo 'Função' é obrigatório.");
            if (string.IsNullOrWhiteSpace(cargo.Autonomia))
                errors.Add("O campo 'Autonomia' é obrigatório.");
            if (string.IsNullOrWhiteSpace(cargo.EscopoAtuacao))
                errors.Add("O campo 'Escopo de Atuação' é obrigatório.");
            if (string.IsNullOrWhiteSpace(cargo.NivelInterlocucao))
                errors.Add("O campo 'Nível de interlocução principal no cliente' é obrigatório.");
            // Adicione outras validações conforme necessário
            return errors;
        }
    }
}
