using System.Collections.Generic;
using System.Linq;
using Xunit;
using Services.Common;

namespace Tests.Services.Common
{
    public class ComboHelperTests
    {
        [Fact]
        public void GetNotasCompetenciaItems_IncludeSelecionarFalse_DeveNaoIncluirSelecionar()
        {
            var result = ComboHelper.GetNotasCompetenciaItems(false);

            Assert.DoesNotContain(result, item => item.Text == "[Selecionar]");
            Assert.Equal(5, result.Count);
            Assert.Contains(result, item => item.Value == "1" && item.Text == "1");
            Assert.Contains(result, item => item.Value == "5" && item.Text == "N/A");
        }

        [Fact]
        public void GetNotasCompetenciaItems_IncludeSelecionarTrue_DeveIncluirSelecionar()
        {
            var result = ComboHelper.GetNotasCompetenciaItems(true);

            Assert.Contains(result, item => item.Text == "[Selecionar]" && item.IsDisabled);
            Assert.Equal(6, result.Count);
            var selecionarItem = result.First(item => item.Text == "[Selecionar]");
            Assert.True(selecionarItem.IsDisabled);
            Assert.Equal("0", selecionarItem.Value);
        }

        [Fact]
        public void GetNotasPerformanceItems_IncludeSelecionarFalse_DeveNaoIncluirSelecionar()
        {
            var result = ComboHelper.GetNotasPerformanceItems(false);

            Assert.DoesNotContain(result, item => item.Text == "[Selecionar]");
            Assert.Equal(5, result.Count);
            Assert.Contains(result, item => item.Value == "1" && item.Text == "1");
            Assert.Contains(result, item => item.Value == "5" && item.Text == "N/A");
        }

        [Fact]
        public void GetSelectedText_ItemNaoEncontrado_DeveRetornarSelecionar()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Item 1" },
                new ComboItem { Value = "2", Text = "Item 2" }
            };

            var result = ComboHelper.GetSelectedText(items, "999");

            Assert.Equal("[Selecionar]", result);
        }

        [Fact]
        public void GetSelectedText_ValueNuloOuVazio_DeveRetornarSelecionar()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Item 1" }
            };

            var resultNull = ComboHelper.GetSelectedText(items, null);
            var resultEmpty = ComboHelper.GetSelectedText(items, "");

            Assert.Equal("[Selecionar]", resultNull);
            Assert.Equal("[Selecionar]", resultEmpty);
        }

        [Fact]
        public void GetSelectedText_ItemEncontrado_DeveRetornarTextoCorreto()
        {
            var items = new List<ComboItem>
            {
                new ComboItem { Value = "1", Text = "Item 1" },
                new ComboItem { Value = "2", Text = "Item 2" }
            };

            var result = ComboHelper.GetSelectedText(items, "2");

            Assert.Equal("Item 2", result);
        }

        [Fact]
        public void IsValidSelection_ValueValidoOuInvalido_DeveRetornarCorreto()
        {
            Assert.True(ComboHelper.IsValidSelection("1"));
            Assert.True(ComboHelper.IsValidSelection("valid"));
            Assert.False(ComboHelper.IsValidSelection(null));
            Assert.False(ComboHelper.IsValidSelection(""));
            Assert.False(ComboHelper.IsValidSelection(string.Empty));
        }

        [Fact]
        public void GetStatusText_StatusAtivoOuInativo_DeveRetornarTextoCorreto()
        {
            Assert.Equal("Ativo", ComboHelper.GetStatusText(1));
            Assert.Equal("Inativo", ComboHelper.GetStatusText(0));
            Assert.Equal("Inativo", ComboHelper.GetStatusText(-1));
            Assert.Equal("Inativo", ComboHelper.GetStatusText(2));
        }

        [Fact]
        public void GetTiposAvaliacao_DeveRetornarListaCorreta()
        {
            var result = ComboHelper.GetTiposAvaliacao();

            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("desempenho", result);
            Assert.Contains("liderança", result);
        }

        [Fact]
        public void GetEscopos_DeveRetornarListaCorreta()
        {
            var result = ComboHelper.GetEscopos();

            Assert.Equal(3, result.Count);
            Assert.Contains("[Selecionar]", result);
            Assert.Contains("projeto", result);
            Assert.Contains("lider", result);
        }

        [Fact]
        public void GetDefaultSelectionItem_DeveRetornarItemPadrao()
        {
            var result = ComboHelper.GetDefaultSelectionItem();

            Assert.Equal("", result.Value);
            Assert.Equal("[Selecionar]", result.Text);
        }

        [Fact]
        public void GetStatusItems_DeveRetornarItensStatus()
        {
            var result = ComboHelper.GetStatusItems();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, item => item.Value == "1" && item.Text == "Ativo");
            Assert.Contains(result, item => item.Value == "0" && item.Text == "Inativo");
        }

        [Fact]
        public void GetAbrangenciaPerformanceItems_DeveRetornarItensAbrangencia()
        {
            var result = ComboHelper.GetAbrangenciaPerformanceItems();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, item => item.Value == "Individual" && item.Text == "Individual");
            Assert.Contains(result, item => item.Value == "Coletivo" && item.Text == "Coletivo");
        }
    }
}