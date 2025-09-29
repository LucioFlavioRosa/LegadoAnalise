using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;
using Services.Resultados;

namespace Tests.Services.Resultados
{
    public class ResultadoServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<ITelemetryService> _mockTelemetryService;
        private readonly Mock<IMessageBoxService> _mockMessageBoxService;
        private readonly Mock<IExportFileService> _mockExportFileService;
        private readonly ResultadoService _resultadoService;
        private readonly Mock<DbSet<ResultadoProjetosModel>> _mockResultadoProjetosDbSet;
        private readonly Mock<DbSet<PERIODOSAVALIACOES>> _mockPeriodosDbSet;

        public ResultadoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockDbContext = new Mock<ApplicationDbContext>(options);
            _mockTelemetryService = new Mock<ITelemetryService>();
            _mockMessageBoxService = new Mock<IMessageBoxService>();
            _mockExportFileService = new Mock<IExportFileService>();
            
            _mockResultadoProjetosDbSet = new Mock<DbSet<ResultadoProjetosModel>>();
            _mockPeriodosDbSet = new Mock<DbSet<PERIODOSAVALIACOES>>();
            
            _mockDbContext.Setup(x => x.ResultadoProjetos).Returns(_mockResultadoProjetosDbSet.Object);
            _mockDbContext.Setup(x => x.PeriodosAvaliacoes).Returns(_mockPeriodosDbSet.Object);
            
            _resultadoService = new ResultadoService(
                _mockDbContext.Object,
                _mockTelemetryService.Object,
                _mockMessageBoxService.Object,
                _mockExportFileService.Object);
        }

        [Fact]
        public async Task ListarResultadosAsync_DeveRetornarResultadosFiltrados()
        {
            var resultados = new List<ResultadoProjetosModel>
            {
                new ResultadoProjetosModel { IdProjeto = 1, IdAssociado = 1, IdPeriodo = 1, TipoAvaliacao = "lideranca" },
                new ResultadoProjetosModel { IdProjeto = 2, IdAssociado = 2, IdPeriodo = 1, TipoAvaliacao = "desempenho" },
                new ResultadoProjetosModel { IdProjeto = 1, IdAssociado = 1, IdPeriodo = 2, TipoAvaliacao = "lideranca" }
            };
            
            var queryable = resultados.AsQueryable();
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            var result = await _resultadoService.ListarResultadosAsync(1, 1, 1, "lideranca");

            Assert.Single(result);
            Assert.Equal(1, result.First().IdProjeto);
            Assert.Equal(1, result.First().IdAssociado);
            Assert.Equal(1, result.First().IdPeriodo);
            Assert.Equal("lideranca", result.First().TipoAvaliacao);
            
            _mockTelemetryService.Verify(x => x.TrackEvent("ListarResultados", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ListarResultadosAsync_QuandoDbFalha_DeveRetornarListaVaziaEChamarShowError()
        {
            _mockDbContext.Setup(x => x.ResultadoProjetos).Throws(new Exception("Database error"));

            var result = await _resultadoService.ListarResultadosAsync(1, 1, 1, "lideranca");

            Assert.Empty(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao listar resultados."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosLiderancaAsync_QuandoExportFileFalha_DeveRetornarArrayVazioEChamarShowError()
        {
            var periodos = new List<PERIODOSAVALIACOES> { new PERIODOSAVALIACOES { IdPeriodo = 1 } };
            var queryablePeriodos = periodos.AsQueryable();
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(queryablePeriodos.Provider);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(queryablePeriodos.Expression);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(queryablePeriodos.ElementType);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(queryablePeriodos.GetEnumerator());
            
            _mockDbContext.Setup(x => x.AvaliacoesCompetencias).Returns(Mock.Of<DbSet<AvaliacaoCompetencia>>());
            _mockDbContext.Setup(x => x.Associados).Returns(Mock.Of<DbSet<Associado>>());
            _mockDbContext.Setup(x => x.AvaliacoesCompetenciasNotas).Returns(Mock.Of<DbSet<AvaliacaoCompetenciaNota>>());
            
            _mockExportFileService.Setup(x => x.GerarExcelResultadoLiderancaAsync(
                It.IsAny<List<PERIODOSAVALIACOES>>(),
                It.IsAny<List<AvaliacaoCompetencia>>(),
                It.IsAny<List<Associado>>(),
                It.IsAny<List<AvaliacaoCompetenciaNota>>()))
                .ThrowsAsync(new Exception("Export service error"));

            var result = await _resultadoService.ExportarResultadosLiderancaAsync(1);

            Assert.Empty(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao exportar resultados de liderança."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosDesempenhoAsync_QuandoSucesso_DeveRetornarBytes()
        {
            var resultados = new List<ResultadoProjetosModel>
            {
                new ResultadoProjetosModel { IdPeriodo = 1, TipoAvaliacao = "desempenho" }
            };
            
            var queryable = resultados.AsQueryable();
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            
            var expectedBytes = new byte[] { 1, 2, 3, 4 };
            _mockExportFileService.Setup(x => x.GerarExcelResultadoDesempenhoAsync(It.IsAny<List<ResultadoProjetosModel>>()))
                .ReturnsAsync(expectedBytes);

            var result = await _resultadoService.ExportarResultadosDesempenhoAsync(1);

            Assert.Equal(expectedBytes, result);
            _mockTelemetryService.Verify(x => x.TrackEvent("ExportarResultadosDesempenho", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoPeriodoNaoExiste_DeveRetornarFalse()
        {
            var periodos = new List<PERIODOSAVALIACOES>().AsQueryable();
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(periodos.Provider);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(periodos.Expression);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(periodos.ElementType);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(periodos.GetEnumerator());

            var result = await _resultadoService.LiberarLiderancaAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoDbFalha_DeveRetornarFalseEChamarShowError()
        {
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new Exception("Database save error"));
            
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1 };
            var periodos = new List<PERIODOSAVALIACOES> { periodo }.AsQueryable();
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(periodos.Provider);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(periodos.Expression);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(periodos.ElementType);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(periodos.GetEnumerator());

            var result = await _resultadoService.LiberarLiderancaAsync(1);

            Assert.False(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao liberar liderança."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoSucesso_DeveRetornarTrueEAtualizarPeriodo()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_lideranca = false };
            var periodos = new List<PERIODOSAVALIACOES> { periodo }.AsQueryable();
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(periodos.Provider);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(periodos.Expression);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(periodos.ElementType);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(periodos.GetEnumerator());
            
            _mockDbContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _resultadoService.LiberarLiderancaAsync(1);

            Assert.True(result);
            Assert.True(periodo.fl_lib_res_lideranca);
            _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackEvent("LiberarLideranca", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_QuandoSucesso_DeveRetornarTrueEAtualizarPeriodo()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_mentoria = false };
            var periodos = new List<PERIODOSAVALIACOES> { periodo }.AsQueryable();
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(periodos.Provider);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(periodos.Expression);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(periodos.ElementType);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(periodos.GetEnumerator());
            
            _mockDbContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await _resultadoService.LiberarMentoriaAsync(1);

            Assert.True(result);
            Assert.True(periodo.fl_lib_res_mentoria);
            _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackEvent("LiberarMentoria", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}