using System.Collections.Generic;
using System.Linq;
using Xunit;
using Services.Common;

namespace Tests.Services.Common
{
    public class ComboHelperTests
    {
        [Fact]
        public void GetNotasCompetenciaItems_QuandoIncludeSelecionarTrue_DeveIncluirItemSelecionar()
        {
            var result = ComboHelper.GetNotasCompetenciaItems(true);

            Assert.Equal(6, result.Count);
            Assert.Equal("0", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.True(result.First().IsDisabled);
            Assert.Equal(0, result.First().AdditionalData["Peso"]);
        }

        [Fact]
        public void GetNotasCompetenciaItems_QuandoIncludeSelecionarFalse_NaoDeveIncluirItemSelecionar()
        {
            var result = ComboHelper.GetNotasCompetenciaItems(false);

            Assert.Equal(5, result.Count);
            Assert.Equal("1", result.First().Value);
            Assert.Equal("1", result.First().Text);
            Assert.False(result.First().IsDisabled);
        }

        [Fact]
        public void GetNotasCompetenciaItems_DeveConterTodosOsItensEsperados()
        {
            var result = ComboHelper.GetNotasCompetenciaItems(false);

            var expectedItems = new[]
            {
                new { Value = "1", Text = "1", Peso = 1 },
                new { Value = "2", Text = "2", Peso = 2 },
                new { Value = "3", Text = "3", Peso = 3 },
                new { Value = "4", Text = "4", Peso = 4 },
                new { Value = "5", Text = "N/A", Peso = 5 }
            };

            for (int i = 0; i < expectedItems.Length; i++)
            {
                Assert.Equal(expectedItems[i].Value, result[i].Value);
                Assert.Equal(expectedItems[i].Text, result[i].Text);
                Assert.Equal(expectedItems[i].Peso, result[i].AdditionalData["Peso"]);
            }
        }

        [Fact]
        public void GetNotasPerformanceItems_QuandoIncludeSelecionarTrue_DeveIncluirItemSelecionar()
        {
            var result = ComboHelper.GetNotasPerformanceItems(true);

            Assert.Equal(6, result.Count);
            Assert.Equal("0", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.True(result.First().IsDisabled);
        }

        [Fact]
        public void GetNotasPerformanceItems_QuandoIncludeSelecionarFalse_NaoDeveIncluirItemSelecionar()
        {
            var result = ComboHelper.GetNotasPerformanceItems(false);

            Assert.Equal(5, result.Count);
            Assert.NotEqual("[Selecionar]", result.First().Text);
        }

        [Fact]
        public void GetSelectedText_QuandoValueExiste_DeveRetornarTextoCorreto()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Primeiro" },
                new ComboItem { Value = "2", Text = "Segundo" }
            };

            var result = ComboHelper.GetSelectedText(items, "2");

            Assert.Equal("Segundo", result);
        }

        [Fact]
        public void GetSelectedText_QuandoValueNaoExiste_DeveRetornarSelecionar()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Primeiro" }
            };

            var result = ComboHelper.GetSelectedText(items, "999");

            Assert.Equal("[Selecionar]", result);
        }

        [Fact]
        public void GetSelectedText_QuandoValueNuloOuVazio_DeveRetornarSelecionar()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Primeiro" }
            };

            var resultNull = ComboHelper.GetSelectedText(items, null);
            var resultEmpty = ComboHelper.GetSelectedText(items, "");

            Assert.Equal("[Selecionar]", resultNull);
            Assert.Equal("[Selecionar]", resultEmpty);
        }

        [Fact]
        public void GetTiposAvaliacao_DeveRetornarListaEsperada()
        {
            var result = ComboHelper.GetTiposAvaliacao();

            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("desempenho", result);
            Assert.Contains("liderança", result);
        }

        [Fact]
        public void GetEscopos_DeveRetornarListaEsperada()
        {
            var result = ComboHelper.GetEscopos();

            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("projeto", result);
            Assert.Contains("lider", result);
        }

        [Fact]
        public void GetStatusItems_DeveRetornarItensCorretos()
        {
            var result = ComboHelper.GetStatusItems();

            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0].Value);
            Assert.Equal("Ativo", result[0].Text);
            Assert.Equal("0", result[1].Value);
            Assert.Equal("Inativo", result[1].Text);
        }

        [Fact]
        public void GetStatusText_DeveRetornarTextoCorreto()
        {
            Assert.Equal("Ativo", ComboHelper.GetStatusText(1));
            Assert.Equal("Inativo", ComboHelper.GetStatusText(0));
            Assert.Equal("Inativo", ComboHelper.GetStatusText(-1));
        }

        [Fact]
        public void IsValidSelection_DeveValidarCorretamente()
        {
            Assert.True(ComboHelper.IsValidSelection("1"));
            Assert.True(ComboHelper.IsValidSelection("test"));
            Assert.False(ComboHelper.IsValidSelection(""));
            Assert.False(ComboHelper.IsValidSelection(null));
        }

        [Fact]
        public void GetDefaultSelectionItem_DeveRetornarItemPadrao()
        {
            var result = ComboHelper.GetDefaultSelectionItem();

            Assert.Equal("", result.Value);
            Assert.Equal("[Selecionar]", result.Text);
        }

        [Fact]
        public void GetAbrangenciaPerformanceItems_DeveRetornarItensCorretos()
        {
            var result = ComboHelper.GetAbrangenciaPerformanceItems();

            Assert.Equal(2, result.Count);
            Assert.Equal("Individual", result[0].Value);
            Assert.Equal("Individual", result[0].Text);
            Assert.Equal("Coletivo", result[1].Value);
            Assert.Equal("Coletivo", result[1].Text);
        }
    }
}