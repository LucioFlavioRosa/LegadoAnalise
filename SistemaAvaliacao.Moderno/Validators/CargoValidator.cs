using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Validators
{
    public class CargoValidator : AbstractValidator<Cargo>
    {
        public CargoValidator()
        {
            RuleFor(c => c.NomeCargo)
                .NotEmpty().WithMessage("O campo 'Cargo' é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do cargo deve ter até 100 caracteres.");

            RuleFor(c => c.TempoMinimoPromocao)
                .GreaterThan(0).WithMessage("O tempo mínimo para promoção deve ser maior que zero.");

            RuleFor(c => c.Funcao)
                .NotEmpty().WithMessage("O campo 'Função' é obrigatório.")
                .MaximumLength(500).WithMessage("A função deve ter até 500 caracteres.");

            RuleFor(c => c.Autonomia)
                .NotEmpty().WithMessage("O campo 'Autonomia' é obrigatório.")
                .MaximumLength(500).WithMessage("A autonomia deve ter até 500 caracteres.");

            RuleFor(c => c.EscopoAtuacao)
                .NotEmpty().WithMessage("O campo 'Escopo de Atuação' é obrigatório.")
                .MaximumLength(500).WithMessage("O escopo de atuação deve ter até 500 caracteres.");

            RuleFor(c => c.NivelInterlocucao)
                .NotEmpty().WithMessage("O campo 'Nível de interlocução principal no cliente' é obrigatório.")
                .MaximumLength(200).WithMessage("O nível de interlocução deve ter até 200 caracteres.");

            RuleFor(c => c)
                .Must((cargo, context) => !CriaCiclo(cargo, context.RootContextData))
                .WithMessage("O próximo cargo selecionado cria um ciclo de promoção, o que não é permitido.");
        }

        /// <summary>
        /// Verifica se a seleção de Próximo Cargo cria um ciclo na hierarquia.
        /// O contexto deve fornecer a lista de todos os cargos como RootContextData["Cargos"]
        /// </summary>
        private bool CriaCiclo(Cargo cargo, IDictionary<string, object> contextData)
        {
            if (cargo.ProximoCargoId == null || cargo.ProximoCargoId == 0)
                return false;

            if (contextData == null || !contextData.ContainsKey("Cargos"))
                return false; // Não é possível validar sem a lista

            var cargos = contextData["Cargos"] as List<Cargo>;
            if (cargos == null)
                return false;

            var visitados = new HashSet<int> { cargo.IdCargo };
            int? proximoId = cargo.ProximoCargoId;
            while (proximoId.HasValue)
            {
                if (visitados.Contains(proximoId.Value))
                    return true; // ciclo detectado
                visitados.Add(proximoId.Value);
                var proximo = cargos.FirstOrDefault(c => c.IdCargo == proximoId.Value);
                if (proximo == null)
                    break;
                proximoId = proximo.ProximoCargoId;
            }
            return false;
        }
    }
}
