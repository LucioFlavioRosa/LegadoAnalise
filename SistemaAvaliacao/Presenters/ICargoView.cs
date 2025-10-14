using System.Collections.Generic;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Presenters
{
    /// <summary>
    /// Interface para interação entre Presenter e View de Cargo.
    /// Facilita testes unitários e desacopla lógica de apresentação.
    /// </summary>
    public interface ICargoView
    {
        /// <summary>
        /// Exibe uma mensagem para o usuário.
        /// </summary>
        /// <param name="mensagem">Mensagem a ser exibida.</param>
        /// <param name="tipo">Tipo da mensagem (ex: sucesso, erro, info).</param>
        void ExibirMensagem(string mensagem, string tipo = "info");

        /// <summary>
        /// Preenche o grid/listagem de cargos na UI.
        /// </summary>
        /// <param name="cargos">Lista de cargos a serem exibidos.</param>
        void PreencherGrid(IEnumerable<Cargo> cargos);

        /// <summary>
        /// Limpa os campos do formulário de cadastro/edição.
        /// </summary>
        void LimparFormulario();

        /// <summary>
        /// Obtém os dados do formulário preenchido pelo usuário.
        /// </summary>
        /// <returns>Instância de Cargo preenchida com os dados da UI.</returns>
        Cargo ObterDadosFormulario();
    }
}
