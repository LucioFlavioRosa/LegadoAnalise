using System.Collections.Generic;
using System.Linq;
using Xunit;
using Services.Common;

namespace Tests.Services.Common
{
    public class ComboHelperTests
    {
        [Fact]
        public void GetNotasCompetenciaItems_ComSelecionar_RetornaItemSelecionarENotas()
        {
            var result = ComboHelper.GetNotasCompetenciaItems(includeSelecionar: true);

            Assert.Equal(6, result.Count);
            Assert.Equal("0", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.True(result.First().IsDisabled);
            Assert.Equal(0, result.First().AdditionalData["Peso"]);
            
            var notaUm = result.Skip(1).First();
            Assert.Equal("1", notaUm.Value);
            Assert.Equal("1", notaUm.Text);
            Assert.Equal(1, notaUm.AdditionalData["Peso"]);
            
            var notaNa = result.Last();
            Assert.Equal("5", notaNa.Value);
            Assert.Equal("N/A", notaNa.Text);
            Assert.Equal(5, notaNa.AdditionalData["Peso"]);
        }

        [Fact]
        public void GetNotasCompetenciaItems_SemSelecionar_RetornaApenasNotas()
        {
            var result = ComboHelper.GetNotasCompetenciaItems(includeSelecionar: false);

            Assert.Equal(5, result.Count);
            Assert.DoesNotContain(result, item => item.Text == "[Selecionar]");
            Assert.Equal("1", result.First().Value);
            Assert.Equal("1", result.First().Text);
            Assert.Equal(1, result.First().AdditionalData["Peso"]);
        }

        [Fact]
        public void GetNotasPerformanceItems_ComSelecionar_RetornaItemSelecionarENotas()
        {
            var result = ComboHelper.GetNotasPerformanceItems(includeSelecionar: true);

            Assert.Equal(6, result.Count);
            Assert.Equal("0", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.True(result.First().IsDisabled);
            Assert.Equal(0, result.First().AdditionalData["Peso"]);
        }

        [Fact]
        public void GetNotasPerformanceItems_SemSelecionar_RetornaApenasNotas()
        {
            var result = ComboHelper.GetNotasPerformanceItems(includeSelecionar: false);

            Assert.Equal(5, result.Count);
            Assert.DoesNotContain(result, item => item.Text == "[Selecionar]");
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", true)]
        [InlineData("valor", true)]
        [InlineData("0", true)]
        public void IsValidSelection_ParaValoresNulosOuVazios_RetornaResultadoEsperado(string value, bool expected)
        {
            var result = ComboHelper.IsValidSelection(value);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetSelectedText_ParaValorExistente_RetornaTextoCorreto()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Opção 1" },
                new ComboItem { Value = "2", Text = "Opção 2" }
            };

            var result = ComboHelper.GetSelectedText(items, "1");

            Assert.Equal("Opção 1", result);
        }

        [Fact]
        public void GetSelectedText_ParaValorNaoExistente_RetornaSelecionarPadrao()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Opção 1" },
                new ComboItem { Value = "2", Text = "Opção 2" }
            };

            var result = ComboHelper.GetSelectedText(items, "999");

            Assert.Equal("[Selecionar]", result);
        }

        [Fact]
        public void GetSelectedText_ParaValorNulo_RetornaSelecionarPadrao()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Opção 1" }
            };

            var result = ComboHelper.GetSelectedText(items, null);

            Assert.Equal("[Selecionar]", result);
        }

        [Fact]
        public void GetSelectedText_ParaValorVazio_RetornaSelecionarPadrao()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Opção 1" }
            };

            var result = ComboHelper.GetSelectedText(items, "");

            Assert.Equal("[Selecionar]", result);
        }

        [Theory]
        [InlineData(1, "Ativo")]
        [InlineData(0, "Inativo")]
        [InlineData(999, "Inativo")]
        [InlineData(-1, "Inativo")]
        public void GetStatusText_ParaDiferentesValores_RetornaTextoCorreto(int status, string expected)
        {
            var result = ComboHelper.GetStatusText(status);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetTiposAvaliacao_RetornaListaCorreta()
        {
            var result = ComboHelper.GetTiposAvaliacao();

            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("desempenho", result);
            Assert.Contains("liderança", result);
        }

        [Fact]
        public void GetEscopos_RetornaListaCorreta()
        {
            var result = ComboHelper.GetEscopos();

            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("projeto", result);
            Assert.Contains("lider", result);
        }

        [Fact]
        public void GetTiposAvaliacaoItems_RetornaItensCorretos()
        {
            var result = ComboHelper.GetTiposAvaliacaoItems();

            Assert.Equal(3, result.Count);
            Assert.Equal("", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.Equal("desempenho", result.Skip(1).First().Value);
            Assert.Equal("liderança", result.Last().Value);
        }

        [Fact]
        public void GetEscoposItems_RetornaItensCorretos()
        {
            var result = ComboHelper.GetEscoposItems();

            Assert.Equal(3, result.Count);
            Assert.Equal("", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.Equal("projeto", result.Skip(1).First().Value);
            Assert.Equal("lider", result.Last().Value);
        }

        [Fact]
        public void GetStatusItems_RetornaItensCorretos()
        {
            var result = ComboHelper.GetStatusItems();

            Assert.Equal(2, result.Count);
            Assert.Equal("1", result.First().Value);
            Assert.Equal("Ativo", result.First().Text);
            Assert.Equal("0", result.Last().Value);
            Assert.Equal("Inativo", result.Last().Text);
        }

        [Fact]
        public void GetAbrangenciaPerformanceItems_RetornaItensCorretos()
        {
            var result = ComboHelper.GetAbrangenciaPerformanceItems();

            Assert.Equal(2, result.Count);
            Assert.Equal("Individual", result.First().Value);
            Assert.Equal("Individual", result.First().Text);
            Assert.Equal("Coletivo", result.Last().Value);
            Assert.Equal("Coletivo", result.Last().Text);
        }

        [Fact]
        public void GetDefaultSelectionItem_RetornaItemPadrao()
        {
            var result = ComboHelper.GetDefaultSelectionItem();

            Assert.Equal("", result.Value);
            Assert.Equal("[Selecionar]", result.Text);
        }
    }
}