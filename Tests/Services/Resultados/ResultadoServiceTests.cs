using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Services.Resultados;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Common;

namespace Tests.Services.Resultados
{
    public class ResultadoServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockDbContext;
        private readonly Mock<ITelemetryService> _mockTelemetryService;
        private readonly Mock<IMessageBoxService> _mockMessageBoxService;
        private readonly Mock<IExportFileService> _mockExportFileService;
        private readonly Mock<DbSet<ResultadoProjetosModel>> _mockResultadoProjetosDbSet;
        private readonly Mock<DbSet<PERIODOSAVALIACOES>> _mockPeriodosDbSet;
        private readonly Mock<DbSet<AvaliacaoCompetencia>> _mockAvaliacoesDbSet;
        private readonly Mock<DbSet<Associado>> _mockAssociadosDbSet;
        private readonly Mock<DbSet<AvaliacaoCompetenciaNota>> _mockAvaliacoesNotasDbSet;
        private readonly ResultadoService _service;

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
            _mockAvaliacoesDbSet = new Mock<DbSet<AvaliacaoCompetencia>>();
            _mockAssociadosDbSet = new Mock<DbSet<Associado>>();
            _mockAvaliacoesNotasDbSet = new Mock<DbSet<AvaliacaoCompetenciaNota>>();

            _mockDbContext.Setup(x => x.ResultadoProjetos).Returns(_mockResultadoProjetosDbSet.Object);
            _mockDbContext.Setup(x => x.PeriodosAvaliacoes).Returns(_mockPeriodosDbSet.Object);
            _mockDbContext.Setup(x => x.AvaliacoesCompetencias).Returns(_mockAvaliacoesDbSet.Object);
            _mockDbContext.Setup(x => x.Associados).Returns(_mockAssociadosDbSet.Object);
            _mockDbContext.Setup(x => x.AvaliacoesCompetenciasNotas).Returns(_mockAvaliacoesNotasDbSet.Object);

            _service = new ResultadoService(
                _mockDbContext.Object,
                _mockTelemetryService.Object,
                _mockMessageBoxService.Object,
                _mockExportFileService.Object);
        }

        [Fact]
        public async Task ListarResultadosAsync_DeveRetornarResultadosFiltrados()
        {
            // Arrange
            var resultados = new List<ResultadoProjetosModel>
            {
                new ResultadoProjetosModel { IdProjeto = 1, IdAssociado = 1, IdPeriodo = 1, TipoAvaliacao = "desempenho" },
                new ResultadoProjetosModel { IdProjeto = 2, IdAssociado = 2, IdPeriodo = 1, TipoAvaliacao = "lideranca" },
                new ResultadoProjetosModel { IdProjeto = 1, IdAssociado = 1, IdPeriodo = 2, TipoAvaliacao = "desempenho" }
            }.AsQueryable();

            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Returns(resultados.Provider);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Expression).Returns(resultados.Expression);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.ElementType).Returns(resultados.ElementType);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.GetEnumerator()).Returns(resultados.GetEnumerator());

            // Act
            var result = await _service.ListarResultadosAsync(1, 1, 1, "desempenho");

            // Assert
            Assert.NotNull(result);
            _mockTelemetryService.Verify(x => x.TrackEvent("ListarResultados", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ListarResultadosAsync_QuandoDbFalha_DeveRetornarListaVaziaEChamarShowError()
        {
            // Arrange
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider)
                .Throws(new Exception("Database error"));

            // Act
            var result = await _service.ListarResultadosAsync(1, 1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao listar resultados."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosLiderancaAsync_QuandoExportFileFalha_DeveRetornarArrayVazioEChamarShowError()
        {
            // Arrange
            var periodos = new List<PERIODOSAVALIACOES>().AsQueryable();
            var avaliacoes = new List<AvaliacaoCompetencia>().AsQueryable();
            var associados = new List<Associado>().AsQueryable();
            var avaliacoesNotas = new List<AvaliacaoCompetenciaNota>().AsQueryable();

            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(periodos.Provider);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(periodos.Expression);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(periodos.ElementType);
            _mockPeriodosDbSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(periodos.GetEnumerator());

            _mockAvaliacoesDbSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.Provider).Returns(avaliacoes.Provider);
            _mockAvaliacoesDbSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.Expression).Returns(avaliacoes.Expression);
            _mockAvaliacoesDbSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.ElementType).Returns(avaliacoes.ElementType);
            _mockAvaliacoesDbSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.GetEnumerator()).Returns(avaliacoes.GetEnumerator());

            _mockAssociadosDbSet.As<IQueryable<Associado>>().Setup(m => m.Provider).Returns(associados.Provider);
            _mockAssociadosDbSet.As<IQueryable<Associado>>().Setup(m => m.Expression).Returns(associados.Expression);
            _mockAssociadosDbSet.As<IQueryable<Associado>>().Setup(m => m.ElementType).Returns(associados.ElementType);
            _mockAssociadosDbSet.As<IQueryable<Associado>>().Setup(m => m.GetEnumerator()).Returns(associados.GetEnumerator());

            _mockAvaliacoesNotasDbSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.Provider).Returns(avaliacoesNotas.Provider);
            _mockAvaliacoesNotasDbSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.Expression).Returns(avaliacoesNotas.Expression);
            _mockAvaliacoesNotasDbSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.ElementType).Returns(avaliacoesNotas.ElementType);
            _mockAvaliacoesNotasDbSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.GetEnumerator()).Returns(avaliacoesNotas.GetEnumerator());

            _mockExportFileService.Setup(x => x.GerarExcelResultadoLiderancaAsync(It.IsAny<List<PERIODOSAVALIACOES>>(), It.IsAny<List<AvaliacaoCompetencia>>(), It.IsAny<List<Associado>>(), It.IsAny<List<AvaliacaoCompetenciaNota>>()))
                .ThrowsAsync(new Exception("Export service error"));

            // Act
            var result = await _service.ExportarResultadosLiderancaAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao exportar resultados de liderança."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoPeriodoNaoExiste_DeveRetornarFalse()
        {
            // Arrange
            _mockPeriodosDbSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync((PERIODOSAVALIACOES)null);

            // Act
            var result = await _service.LiberarLiderancaAsync(1);

            // Assert
            Assert.False(result);
            _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoDbFalha_DeveRetornarFalseEChamarShowError()
        {
            // Arrange
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1 };
            _mockPeriodosDbSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync(periodo);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new Exception("Database save error"));

            // Act
            var result = await _service.LiberarLiderancaAsync(1);

            // Assert
            Assert.False(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao liberar liderança."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_QuandoSucesso_DeveRetornarTrueEAtualizarPeriodo()
        {
            // Arrange
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_lideranca = false };
            _mockPeriodosDbSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync(periodo);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _service.LiberarLiderancaAsync(1);

            // Assert
            Assert.True(result);
            Assert.True(periodo.fl_lib_res_lideranca);
            _mockPeriodosDbSet.Verify(x => x.Update(periodo), Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackEvent("LiberarLideranca", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_QuandoPeriodoNaoExiste_DeveRetornarFalse()
        {
            // Arrange
            _mockPeriodosDbSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync((PERIODOSAVALIACOES)null);

            // Act
            var result = await _service.LiberarMentoriaAsync(1);

            // Assert
            Assert.False(result);
            _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_QuandoDbFalha_DeveRetornarFalseEChamarShowError()
        {
            // Arrange
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1 };
            _mockPeriodosDbSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync(periodo);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new Exception("Database save error"));

            // Act
            var result = await _service.LiberarMentoriaAsync(1);

            // Assert
            Assert.False(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao liberar mentoria."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_QuandoSucesso_DeveRetornarTrueEAtualizarPeriodo()
        {
            // Arrange
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_mentoria = false };
            _mockPeriodosDbSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync(periodo);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _service.LiberarMentoriaAsync(1);

            // Assert
            Assert.True(result);
            Assert.True(periodo.fl_lib_res_mentoria);
            _mockPeriodosDbSet.Verify(x => x.Update(periodo), Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackEvent("LiberarMentoria", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosDesempenhoAsync_QuandoSucesso_DeveRetornarBytes()
        {
            // Arrange
            var resultados = new List<ResultadoProjetosModel>
            {
                new ResultadoProjetosModel { IdPeriodo = 1, TipoAvaliacao = "desempenho" }
            }.AsQueryable();

            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Returns(resultados.Provider);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Expression).Returns(resultados.Expression);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.ElementType).Returns(resultados.ElementType);
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.GetEnumerator()).Returns(resultados.GetEnumerator());

            var expectedBytes = new byte[] { 1, 2, 3, 4 };
            _mockExportFileService.Setup(x => x.GerarExcelResultadoDesempenhoAsync(It.IsAny<List<ResultadoProjetosModel>>()))
                .ReturnsAsync(expectedBytes);

            // Act
            var result = await _service.ExportarResultadosDesempenhoAsync(1);

            // Assert
            Assert.Equal(expectedBytes, result);
            _mockTelemetryService.Verify(x => x.TrackEvent("ExportarResultadosDesempenho", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosDesempenhoAsync_QuandoFalha_DeveRetornarArrayVazio()
        {
            // Arrange
            _mockResultadoProjetosDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider)
                .Throws(new Exception("Database error"));

            // Act
            var result = await _service.ExportarResultadosDesempenhoAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao exportar resultados de desempenho."), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}