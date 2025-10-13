using System;
using System.Collections.Generic;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Domain.Validators
{
    /// <summary>
    /// Responsável por validar a entidade Cargo.
    /// </summary>
    public class CargoValidator
    {
        /// <summary>
        /// Valida o modelo Cargo e retorna uma lista de mensagens de erro, se houver.
        /// </summary>
        /// <param name="cargo">Instância de Cargo a ser validada.</param>
        /// <returns>Lista de mensagens de erro. Lista vazia indica validação bem-sucedida.</returns>
        public IList<string> Validar(Cargo cargo)
        {
            var erros = new List<string>();

            if (cargo == null)
            {
                erros.Add("O objeto Cargo não pode ser nulo.");
                return erros;
            }

            // Campo obrigatório: NomeCargo
            if (string.IsNullOrWhiteSpace(cargo.NomeCargo))
                erros.Add("O campo 'Cargo' é obrigatório.");

            // Campo obrigatório: ProximoCargoId (pode ser opcional dependendo da regra de negócio, ajuste se necessário)
            // Exemplo: se não pode ser igual ao próprio IdCargo
            if (cargo.ProximoCargoId.HasValue && cargo.ProximoCargoId == cargo.IdCargo)
                erros.Add("O 'Próximo Cargo' não pode ser o mesmo que o cargo atual.");

            // Campo obrigatório: TempoMinimoPromocao
            if (cargo.TempoMinimoPromocao < 0)
                erros.Add("O tempo mínimo para promoção deve ser maior ou igual a zero.");

            // Campo obrigatório: Funcao
            if (string.IsNullOrWhiteSpace(cargo.Funcao))
                erros.Add("O campo 'Função' é obrigatório.");

            // Campo obrigatório: Autonomia
            if (string.IsNullOrWhiteSpace(cargo.Autonomia))
                erros.Add("O campo 'Autonomia' é obrigatório.");

            // Campo obrigatório: EscopoAtuacao
            if (string.IsNullOrWhiteSpace(cargo.EscopoAtuacao))
                erros.Add("O campo 'Escopo de Atuação' é obrigatório.");

            // Campo obrigatório: NivelInterlocucao
            if (string.IsNullOrWhiteSpace(cargo.NivelInterlocucao))
                erros.Add("O campo 'Nível de interlocução principal no cliente' é obrigatório.");

            // Status deve ser 0 (Inativo) ou 1 (Ativo)
            if (cargo.Status != 0 && cargo.Status != 1)
                erros.Add("O campo 'Status' deve ser 0 (Inativo) ou 1 (Ativo).");

            return erros;
        }
    }
}
