using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Services.Resultados;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using MockQueryable.Moq;

namespace Tests.Services.Resultados
{
    public class ResultadoServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<ITelemetryService> _mockTelemetryService;
        private readonly Mock<IMessageBoxService> _mockMessageBoxService;
        private readonly Mock<IExportFileService> _mockExportFileService;
        private readonly ResultadoService _resultadoService;

        public ResultadoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _mockTelemetryService = new Mock<ITelemetryService>();
            _mockMessageBoxService = new Mock<IMessageBoxService>();
            _mockExportFileService = new Mock<IExportFileService>();

            _resultadoService = new ResultadoService(
                _mockDbContext.Object,
                _mockTelemetryService.Object,
                _mockMessageBoxService.Object,
                _mockExportFileService.Object);
        }

        [Fact]
        public async Task ListarResultadosAsync_RetornaResultadosFiltrados()
        {
            var resultados = new List<ResultadoProjetosModel>
            {
                new ResultadoProjetosModel { Id = 1, IdProjeto = 1, IdAssociado = 1, IdPeriodo = 1, TipoAvaliacao = "desempenho" },
                new ResultadoProjetosModel { Id = 2, IdProjeto = 2, IdAssociado = 2, IdPeriodo = 1, TipoAvaliacao = "lideranca" },
                new ResultadoProjetosModel { Id = 3, IdProjeto = 1, IdAssociado = 1, IdPeriodo = 2, TipoAvaliacao = "desempenho" }
            };

            var mockSet = resultados.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(c => c.ResultadoProjetos).Returns(mockSet.Object);

            var result = await _resultadoService.ListarResultadosAsync(1, 1, 1, "desempenho");

            Assert.Single(result);
            Assert.Equal(1, result.First().IdProjeto);
            Assert.Equal(1, result.First().IdAssociado);
            Assert.Equal(1, result.First().IdPeriodo);
            Assert.Equal("desempenho", result.First().TipoAvaliacao);

            _mockTelemetryService.Verify(t => t.TrackEvent(
                "ListarResultados",
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Dictionary<string, double>>()), Times.Once);
        }

        [Fact]
        public async Task ListarResultadosAsync_QuandoDbLancaExcecao_RetornaListaVaziaERegistraErro()
        {
            _mockDbContext.Setup(c => c.ResultadoProjetos)
                .Throws(new Exception("Database error"));

            var result = await _resultadoService.ListarResultadosAsync(1, 1, 1, "desempenho");

            Assert.Empty(result);
            _mockTelemetryService.Verify(t => t.TrackException(
                It.IsAny<Exception>(),
                It.Is<Dictionary<string, string>>(d => d["Method"] == "ListarResultadosAsync")), Times.Once);
            _mockMessageBoxService.Verify(m => m.ShowError("Erro ao listar resultados."), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosLiderancaAsync_QuandoDbLancaExcecao_RetornaArrayVazioERegistraErro()
        {
            _mockDbContext.Setup(c => c.PeriodosAvaliacoes)
                .Throws(new Exception("Database error"));

            var result = await _resultadoService.ExportarResultadosLiderancaAsync(1);

            Assert.Equal(Array.Empty<byte>(), result);
            _mockTelemetryService.Verify(t => t.TrackException(
                It.IsAny<Exception>(),
                It.Is<Dictionary<string, string>>(d => d["Method"] == "ExportarResultadosLiderancaAsync")), Times.Once);
            _mockMessageBoxService.Verify(m => m.ShowError("Erro ao exportar resultados de liderança."), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosLiderancaAsync_ComPeriodoValido_ChamaExportFileServiceComParametrosCorretos()
        {
            var periodos = new List<PERIODOSAVALIACOES>
            {
                new PERIODOSAVALIACOES { IdPeriodo = 1, Nome = "Período 1" }
            }.AsQueryable().BuildMockDbSet();

            var avaliacoes = new List<AvaliacaoCompetencia>
            {
                new AvaliacaoCompetencia { Id = 1, TipoAvaliacao = "lideranca" }
            }.AsQueryable().BuildMockDbSet();

            var associados = new List<Associado>
            {
                new Associado { Id = 1, Nome = "Associado 1" }
            }.AsQueryable().BuildMockDbSet();

            var avaliacoesNotas = new List<AvaliacaoCompetenciaNota>
            {
                new AvaliacaoCompetenciaNota { IdNota = 1 }
            }.AsQueryable().BuildMockDbSet();

            _mockDbContext.Setup(c => c.PeriodosAvaliacoes).Returns(periodos.Object);
            _mockDbContext.Setup(c => c.AvaliacoesCompetencias).Returns(avaliacoes.Object);
            _mockDbContext.Setup(c => c.Associados).Returns(associados.Object);
            _mockDbContext.Setup(c => c.AvaliacoesCompetenciasNotas).Returns(avaliacoesNotas.Object);

            var expectedBytes = new byte[] { 1, 2, 3, 4 };
            _mockExportFileService.Setup(e => e.GerarExcelResultadoLiderancaAsync(
                It.IsAny<List<PERIODOSAVALIACOES>>(),
                It.IsAny<List<AvaliacaoCompetencia>>(),
                It.IsAny<List<Associado>>(),
                It.IsAny<List<AvaliacaoCompetenciaNota>>()))
                .ReturnsAsync(expectedBytes);

            var result = await _resultadoService.ExportarResultadosLiderancaAsync(1);

            Assert.Equal(expectedBytes, result);
            _mockExportFileService.Verify(e => e.GerarExcelResultadoLiderancaAsync(
                It.IsAny<List<PERIODOSAVALIACOES>>(),
                It.IsAny<List<AvaliacaoCompetencia>>(),
                It.IsAny<List<Associado>>(),
                It.IsAny<List<AvaliacaoCompetenciaNota>>()), Times.Once);
            _mockTelemetryService.Verify(t => t.TrackEvent(
                "ExportarResultadosLideranca",
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Dictionary<string, double>>()), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosDesempenhoAsync_QuandoDbLancaExcecao_RetornaArrayVazioERegistraErro()
        {
            _mockDbContext.Setup(c => c.ResultadoProjetos)
                .Throws(new Exception("Database error"));

            var result = await _resultadoService.ExportarResultadosDesempenhoAsync(1);

            Assert.Equal(Array.Empty<byte>(), result);
            _mockTelemetryService.Verify(t => t.TrackException(
                It.IsAny<Exception>(),
                It.Is<Dictionary<string, string>>(d => d["Method"] == "ExportarResultadosDesempenhoAsync")), Times.Once);
            _mockMessageBoxService.Verify(m => m.ShowError("Erro ao exportar resultados de desempenho."), Times.Once);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoPeriodoNaoExiste_RetornaFalse()
        {
            var periodos = new List<PERIODOSAVALIACOES>().AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(c => c.PeriodosAvaliacoes).Returns(periodos.Object);

            var result = await _resultadoService.LiberarLiderancaAsync(999);

            Assert.False(result);
            _mockDbContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoDbLancaExcecao_RetornaFalseERegistraErro()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_lideranca = false };
            var periodos = new List<PERIODOSAVALIACOES> { periodo }.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(c => c.PeriodosAvaliacoes).Returns(periodos.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync()).ThrowsAsync(new Exception("Save error"));

            var result = await _resultadoService.LiberarLiderancaAsync(1);

            Assert.False(result);
            _mockTelemetryService.Verify(t => t.TrackException(
                It.IsAny<Exception>(),
                It.Is<Dictionary<string, string>>(d => d["Method"] == "LiberarLiderancaAsync")), Times.Once);
            _mockMessageBoxService.Verify(m => m.ShowError("Erro ao liberar liderança."), Times.Once);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoPeriodoExiste_AtualizaFlagERetornaTrue()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_lideranca = false };
            var periodos = new List<PERIODOSAVALIACOES> { periodo }.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(c => c.PeriodosAvaliacoes).Returns(periodos.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _resultadoService.LiberarLiderancaAsync(1);

            Assert.True(result);
            Assert.True(periodo.fl_lib_res_lideranca);
            _mockDbContext.Verify(c => c.Update(periodo), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            _mockTelemetryService.Verify(t => t.TrackEvent(
                "LiberarLideranca",
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Dictionary<string, double>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_QuandoPeriodoNaoExiste_RetornaFalse()
        {
            var periodos = new List<PERIODOSAVALIACOES>().AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(c => c.PeriodosAvaliacoes).Returns(periodos.Object);

            var result = await _resultadoService.LiberarMentoriaAsync(999);

            Assert.False(result);
            _mockDbContext.Verify(c => c.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_QuandoDbLancaExcecao_RetornaFalseERegistraErro()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_mentoria = false };
            var periodos = new List<PERIODOSAVALIACOES> { periodo }.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(c => c.PeriodosAvaliacoes).Returns(periodos.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync()).ThrowsAsync(new Exception("Save error"));

            var result = await _resultadoService.LiberarMentoriaAsync(1);

            Assert.False(result);
            _mockTelemetryService.Verify(t => t.TrackException(
                It.IsAny<Exception>(),
                It.Is<Dictionary<string, string>>(d => d["Method"] == "LiberarMentoriaAsync")), Times.Once);
            _mockMessageBoxService.Verify(m => m.ShowError("Erro ao liberar mentoria."), Times.Once);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_QuandoPeriodoExiste_AtualizaFlagERetornaTrue()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_mentoria = false };
            var periodos = new List<PERIODOSAVALIACOES> { periodo }.AsQueryable().BuildMockDbSet();
            _mockDbContext.Setup(c => c.PeriodosAvaliacoes).Returns(periodos.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _resultadoService.LiberarMentoriaAsync(1);

            Assert.True(result);
            Assert.True(periodo.fl_lib_res_mentoria);
            _mockDbContext.Verify(c => c.Update(periodo), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(), Times.Once);
            _mockTelemetryService.Verify(t => t.TrackEvent(
                "LiberarMentoria",
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Dictionary<string, double>>()), Times.Once);
        }
    }
}