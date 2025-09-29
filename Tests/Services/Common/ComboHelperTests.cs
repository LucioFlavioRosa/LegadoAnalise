using System.Collections.Generic;
using System.Linq;
using Xunit;
using Services.Common;

namespace Tests.Services.Common
{
    public class ComboHelperTests
    {
        [Fact]
        public void GetNotasCompetenciaItems_QuandoIncludeSelecionarTrue_DeveRetornarComSelecionar()
        {
            // Act
            var result = ComboHelper.GetNotasCompetenciaItems(true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.Equal("0", result.First().Value);
            Assert.True(result.First().IsDisabled);
            Assert.Equal("5", result.Last().Value);
            Assert.Equal("N/A", result.Last().Text);
        }

        [Fact]
        public void GetNotasCompetenciaItems_QuandoIncludeSelecionarFalse_DeveRetornarSemSelecionar()
        {
            // Act
            var result = ComboHelper.GetNotasCompetenciaItems(false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.Equal("1", result.First().Value);
            Assert.Equal("1", result.First().Text);
            Assert.Equal("5", result.Last().Value);
            Assert.Equal("N/A", result.Last().Text);
        }

        [Fact]
        public void GetNotasPerformanceItems_QuandoIncludeSelecionarTrue_DeveRetornarComSelecionar()
        {
            // Act
            var result = ComboHelper.GetNotasPerformanceItems(true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.Equal("0", result.First().Value);
            Assert.True(result.First().IsDisabled);
            Assert.Equal("5", result.Last().Value);
            Assert.Equal("N/A", result.Last().Text);
        }

        [Fact]
        public void GetNotasPerformanceItems_QuandoIncludeSelecionarFalse_DeveRetornarSemSelecionar()
        {
            // Act
            var result = ComboHelper.GetNotasPerformanceItems(false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.Equal("1", result.First().Value);
            Assert.Equal("1", result.First().Text);
            Assert.Equal("5", result.Last().Value);
            Assert.Equal("N/A", result.Last().Text);
        }

        [Fact]
        public void GetSelectedText_QuandoValueExiste_DeveRetornarTextoCorreto()
        {
            // Arrange
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Primeiro" },
                new ComboItem { Value = "2", Text = "Segundo" }
            };

            // Act
            var result = ComboHelper.GetSelectedText(items, "2");

            // Assert
            Assert.Equal("Segundo", result);
        }

        [Fact]
        public void GetSelectedText_QuandoValueNaoExiste_DeveRetornarSelecionarPadrao()
        {
            // Arrange
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Primeiro" },
                new ComboItem { Value = "2", Text = "Segundo" }
            };

            // Act
            var result = ComboHelper.GetSelectedText(items, "3");

            // Assert
            Assert.Equal("[Selecionar]", result);
        }

        [Fact]
        public void GetSelectedText_QuandoValueNuloOuVazio_DeveRetornarSelecionarPadrao()
        {
            // Arrange
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Primeiro" }
            };

            // Act
            var resultNull = ComboHelper.GetSelectedText(items, null);
            var resultEmpty = ComboHelper.GetSelectedText(items, "");

            // Assert
            Assert.Equal("[Selecionar]", resultNull);
            Assert.Equal("[Selecionar]", resultEmpty);
        }

        [Fact]
        public void GetTiposAvaliacao_DeveRetornarListaEsperada()
        {
            // Act
            var result = ComboHelper.GetTiposAvaliacao();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("desempenho", result);
            Assert.Contains("liderança", result);
        }

        [Fact]
        public void GetEscopos_DeveRetornarListaEsperada()
        {
            // Act
            var result = ComboHelper.GetEscopos();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("projeto", result);
            Assert.Contains("lider", result);
        }

        [Fact]
        public void GetTiposAvaliacaoItems_DeveRetornarItensCorretos()
        {
            // Act
            var result = ComboHelper.GetTiposAvaliacaoItems();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.Equal("desempenho", result[1].Value);
            Assert.Equal("liderança", result.Last().Value);
        }

        [Fact]
        public void GetEscoposItems_DeveRetornarItensCorretos()
        {
            // Act
            var result = ComboHelper.GetEscoposItems();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal("", result.First().Value);
            Assert.Equal("[Selecionar]", result.First().Text);
            Assert.Equal("projeto", result[1].Value);
            Assert.Equal("lider", result.Last().Value);
        }

        [Fact]
        public void GetStatusItems_DeveRetornarItensCorretos()
        {
            // Act
            var result = ComboHelper.GetStatusItems();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result.First().Value);
            Assert.Equal("Ativo", result.First().Text);
            Assert.Equal("0", result.Last().Value);
            Assert.Equal("Inativo", result.Last().Text);
        }

        [Fact]
        public void GetAbrangenciaPerformanceItems_DeveRetornarItensCorretos()
        {
            // Act
            var result = ComboHelper.GetAbrangenciaPerformanceItems();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Individual", result.First().Value);
            Assert.Equal("Individual", result.First().Text);
            Assert.Equal("Coletivo", result.Last().Value);
            Assert.Equal("Coletivo", result.Last().Text);
        }

        [Fact]
        public void GetDefaultSelectionItem_DeveRetornarItemPadrao()
        {
            // Act
            var result = ComboHelper.GetDefaultSelectionItem();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result.Value);
            Assert.Equal("[Selecionar]", result.Text);
        }

        [Theory]
        [InlineData("test", true)]
        [InlineData("123", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValidSelection_DeveRetornarResultadoCorreto(string value, bool expected)
        {
            // Act
            var result = ComboHelper.IsValidSelection(value);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, "Ativo")]
        [InlineData(0, "Inativo")]
        [InlineData(2, "Inativo")]
        [InlineData(-1, "Inativo")]
        public void GetStatusText_DeveRetornarTextoCorreto(int status, string expected)
        {
            // Act
            var result = ComboHelper.GetStatusText(status);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}