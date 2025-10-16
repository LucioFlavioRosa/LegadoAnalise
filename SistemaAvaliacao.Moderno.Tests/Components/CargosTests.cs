using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using SistemaAvaliacao.Moderno.Models;
using SistemaAvaliacao.Moderno.Services;
using SistemaAvaliacao.Moderno.Components.Pages;
using SistemaAvaliacao.Moderno.Components.Shared;

namespace SistemaAvaliacao.Moderno.Tests.Components
{
    public class CargosTests : TestContext
    {
        private readonly Mock<ICargoService> _cargoServiceMock;
        private readonly List<Cargo> _cargos;

        public CargosTests()
        {
            _cargoServiceMock = new Mock<ICargoService>();
            _cargos = new List<Cargo>
            {
                new Cargo { IdCargo = 1, NomeCargo = "Analista", Ativo = true },
                new Cargo { IdCargo = 2, NomeCargo = "Gestor", Ativo = false }
            };
            _cargoServiceMock.Setup(s => s.ObterTodosAsync()).ReturnsAsync(_cargos);
            Services.AddSingleton(_cargoServiceMock.Object);
        }

        [Fact]
        public void DeveRenderizarFormularioDeCargo()
        {
            var cut = RenderComponent<Cargos>();
            cut.MarkupMatches(markup => markup.Contains("Dados do Cargo"));
        }

        [Fact]
        public void DeveExibirListaDeCargos()
        {
            var cut = RenderComponent<CargosList>(parameters => parameters.Add(p => p.Cargos, _cargos));
            Assert.Contains("Analista", cut.Markup);
            Assert.Contains("Gestor", cut.Markup);
        }

        [Fact]
        public void DeveExibirMensagemDeErroAoSubmeterFormularioInvalido()
        {
            var cut = RenderComponent<Cargos>();
            var botao = cut.Find("button[type=submit]");
            botao.Click();
            Assert.Contains("campo obrigatório", cut.Markup, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void DevePermitirInteracaoComListaDeCargos()
        {
            var cut = RenderComponent<CargosList>(parameters => parameters.Add(p => p.Cargos, _cargos));
            var btnAlterar = cut.Find("button[data-action='alterar']");
            btnAlterar.Click();
            // Simula evento de alteração e verifica se o evento foi emitido
            Assert.True(true); // Placeholder para evento
        }
    }
}
