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

namespace Tests.Services.Resultados
{
    public class ResultadoServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockDb;
        private readonly Mock<ITelemetryService> _mockTelemetryService;
        private readonly Mock<IMessageBoxService> _mockMessageBoxService;
        private readonly Mock<IExportFileService> _mockExportFileService;
        private readonly ResultadoService _service;
        private readonly Mock<DbSet<ResultadoProjetosModel>> _mockResultadoProjetosSet;
        private readonly Mock<DbSet<PERIODOSAVALIACOES>> _mockPeriodosSet;
        private readonly Mock<DbSet<AvaliacaoCompetencia>> _mockAvaliacoesCompetenciasSet;
        private readonly Mock<DbSet<Associado>> _mockAssociadosSet;
        private readonly Mock<DbSet<AvaliacaoCompetenciaNota>> _mockAvaliacoesCompetenciasNotasSet;

        public ResultadoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockDb = new Mock<ApplicationDbContext>(options);
            _mockTelemetryService = new Mock<ITelemetryService>();
            _mockMessageBoxService = new Mock<IMessageBoxService>();
            _mockExportFileService = new Mock<IExportFileService>();

            _mockResultadoProjetosSet = new Mock<DbSet<ResultadoProjetosModel>>();
            _mockPeriodosSet = new Mock<DbSet<PERIODOSAVALIACOES>>();
            _mockAvaliacoesCompetenciasSet = new Mock<DbSet<AvaliacaoCompetencia>>();
            _mockAssociadosSet = new Mock<DbSet<Associado>>();
            _mockAvaliacoesCompetenciasNotasSet = new Mock<DbSet<AvaliacaoCompetenciaNota>>();

            _mockDb.Setup(x => x.ResultadoProjetos).Returns(_mockResultadoProjetosSet.Object);
            _mockDb.Setup(x => x.PeriodosAvaliacoes).Returns(_mockPeriodosSet.Object);
            _mockDb.Setup(x => x.AvaliacoesCompetencias).Returns(_mockAvaliacoesCompetenciasSet.Object);
            _mockDb.Setup(x => x.Associados).Returns(_mockAssociadosSet.Object);
            _mockDb.Setup(x => x.AvaliacoesCompetenciasNotas).Returns(_mockAvaliacoesCompetenciasNotasSet.Object);

            _service = new ResultadoService(
                _mockDb.Object,
                _mockTelemetryService.Object,
                _mockMessageBoxService.Object,
                _mockExportFileService.Object);
        }

        [Fact]
        public async Task ListarResultadosAsync_FiltrosAplicados_DeveFiltrarCorretamente()
        {
            var resultados = new List<ResultadoProjetosModel>
            {
                new ResultadoProjetosModel { IdProjeto = 1, IdAssociado = 1, IdPeriodo = 1, TipoAvaliacao = "desempenho" },
                new ResultadoProjetosModel { IdProjeto = 2, IdAssociado = 2, IdPeriodo = 2, TipoAvaliacao = "lideranca" },
                new ResultadoProjetosModel { IdProjeto = 1, IdAssociado = 1, IdPeriodo = 1, TipoAvaliacao = "lideranca" }
            }.AsQueryable();

            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Returns(resultados.Provider);
            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Expression).Returns(resultados.Expression);
            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.ElementType).Returns(resultados.ElementType);
            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.GetEnumerator()).Returns(resultados.GetEnumerator());

            var result = await _service.ListarResultadosAsync(1, 1, 1, "desempenho");

            Assert.Single(result);
            Assert.Equal(1, result.First().IdProjeto);
            Assert.Equal("desempenho", result.First().TipoAvaliacao);
            _mockTelemetryService.Verify(x => x.TrackEvent("ListarResultados", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ListarResultadosAsync_ExcecaoBanco_DeveTratarELogar()
        {
            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Throws(new Exception("Database error"));

            var result = await _service.ListarResultadosAsync(1, 1, 1, "desempenho");

            Assert.Empty(result);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao listar resultados."), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosLiderancaAsync_PeriodoNuloOuInexistente_DeveExportarSemFiltro()
        {
            var periodos = new List<PERIODOSAVALIACOES>
            {
                new PERIODOSAVALIACOES { IdPeriodo = 1 },
                new PERIODOSAVALIACOES { IdPeriodo = 2 }
            }.AsQueryable();

            var avaliacoes = new List<AvaliacaoCompetencia>().AsQueryable();
            var associados = new List<Associado>().AsQueryable();
            var avaliacoesNotas = new List<AvaliacaoCompetenciaNota>().AsQueryable();

            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(periodos.Provider);
            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(periodos.Expression);
            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(periodos.ElementType);
            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(periodos.GetEnumerator());

            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.Provider).Returns(avaliacoes.Provider);
            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.Expression).Returns(avaliacoes.Expression);
            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.ElementType).Returns(avaliacoes.ElementType);
            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.GetEnumerator()).Returns(avaliacoes.GetEnumerator());

            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.Provider).Returns(associados.Provider);
            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.Expression).Returns(associados.Expression);
            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.ElementType).Returns(associados.ElementType);
            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.GetEnumerator()).Returns(associados.GetEnumerator());

            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.Provider).Returns(avaliacoesNotas.Provider);
            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.Expression).Returns(avaliacoesNotas.Expression);
            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.ElementType).Returns(avaliacoesNotas.ElementType);
            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.GetEnumerator()).Returns(avaliacoesNotas.GetEnumerator());

            var expectedBytes = new byte[] { 1, 2, 3 };
            _mockExportFileService.Setup(x => x.GerarExcelResultadoLiderancaAsync(
                It.IsAny<List<PERIODOSAVALIACOES>>(),
                It.IsAny<List<AvaliacaoCompetencia>>(),
                It.IsAny<List<Associado>>(),
                It.IsAny<List<AvaliacaoCompetenciaNota>>()
            )).ReturnsAsync(expectedBytes);

            var result = await _service.ExportarResultadosLiderancaAsync(null);

            Assert.Equal(expectedBytes, result);
            _mockExportFileService.Verify(x => x.GerarExcelResultadoLiderancaAsync(
                It.Is<List<PERIODOSAVALIACOES>>(p => p.Count == 2),
                It.IsAny<List<AvaliacaoCompetencia>>(),
                It.IsAny<List<Associado>>(),
                It.IsAny<List<AvaliacaoCompetenciaNota>>()
            ), Times.Once);
            _mockTelemetryService.Verify(x => x.TrackEvent("ExportarResultadosLideranca", It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosLiderancaAsync_ExcecaoExportacao_DeveTratarELogar()
        {
            var periodos = new List<PERIODOSAVALIACOES>().AsQueryable();
            var avaliacoes = new List<AvaliacaoCompetencia>().AsQueryable();
            var associados = new List<Associado>().AsQueryable();
            var avaliacoesNotas = new List<AvaliacaoCompetenciaNota>().AsQueryable();

            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Provider).Returns(periodos.Provider);
            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.Expression).Returns(periodos.Expression);
            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.ElementType).Returns(periodos.ElementType);
            _mockPeriodosSet.As<IQueryable<PERIODOSAVALIACOES>>().Setup(m => m.GetEnumerator()).Returns(periodos.GetEnumerator());

            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.Provider).Returns(avaliacoes.Provider);
            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.Expression).Returns(avaliacoes.Expression);
            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.ElementType).Returns(avaliacoes.ElementType);
            _mockAvaliacoesCompetenciasSet.As<IQueryable<AvaliacaoCompetencia>>().Setup(m => m.GetEnumerator()).Returns(avaliacoes.GetEnumerator());

            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.Provider).Returns(associados.Provider);
            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.Expression).Returns(associados.Expression);
            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.ElementType).Returns(associados.ElementType);
            _mockAssociadosSet.As<IQueryable<Associado>>().Setup(m => m.GetEnumerator()).Returns(associados.GetEnumerator());

            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.Provider).Returns(avaliacoesNotas.Provider);
            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.Expression).Returns(avaliacoesNotas.Expression);
            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.ElementType).Returns(avaliacoesNotas.ElementType);
            _mockAvaliacoesCompetenciasNotasSet.As<IQueryable<AvaliacaoCompetenciaNota>>().Setup(m => m.GetEnumerator()).Returns(avaliacoesNotas.GetEnumerator());

            _mockExportFileService.Setup(x => x.GerarExcelResultadoLiderancaAsync(
                It.IsAny<List<PERIODOSAVALIACOES>>(),
                It.IsAny<List<AvaliacaoCompetencia>>(),
                It.IsAny<List<Associado>>(),
                It.IsAny<List<AvaliacaoCompetenciaNota>>()
            )).ThrowsAsync(new Exception("Export error"));

            var result = await _service.ExportarResultadosLiderancaAsync(1);

            Assert.Empty(result);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao exportar resultados de liderança."), Times.Once);
        }

        [Fact]
        public async Task ExportarResultadosDesempenhoAsync_IdPeriodoInvalido_DeveRetornarArrayVazio()
        {
            var resultados = new List<ResultadoProjetosModel>().AsQueryable();

            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Returns(resultados.Provider);
            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Expression).Returns(resultados.Expression);
            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.ElementType).Returns(resultados.ElementType);
            _mockResultadoProjetosSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.GetEnumerator()).Returns(resultados.GetEnumerator());

            var expectedBytes = new byte[0];
            _mockExportFileService.Setup(x => x.GerarExcelResultadoDesempenhoAsync(It.IsAny<List<ResultadoProjetosModel>>()))
                .ReturnsAsync(expectedBytes);

            var result = await _service.ExportarResultadosDesempenhoAsync(999);

            Assert.Empty(result);
            _mockExportFileService.Verify(x => x.GerarExcelResultadoDesempenhoAsync(It.Is<List<ResultadoProjetosModel>>(r => r.Count == 0)), Times.Once);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_PeriodoNaoEncontrado_DeveRetornarFalse()
        {
            _mockPeriodosSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync((PERIODOSAVALIACOES)null);

            var result = await _service.LiberarLiderancaAsync(999);

            Assert.False(result);
            _mockDb.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task LiberarLiderancaAsync_ExcecaoAoSalvar_DeveTratarELogar()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_lideranca = false };
            _mockPeriodosSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync(periodo);
            _mockDb.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new Exception("Save error"));

            var result = await _service.LiberarLiderancaAsync(1);

            Assert.False(result);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao liberar liderança."), Times.Once);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_PeriodoNaoEncontrado_DeveRetornarFalse()
        {
            _mockPeriodosSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync((PERIODOSAVALIACOES)null);

            var result = await _service.LiberarMentoriaAsync(999);

            Assert.False(result);
            _mockDb.Verify(x => x.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task LiberarMentoriaAsync_ExcecaoAoSalvar_DeveTratarELogar()
        {
            var periodo = new PERIODOSAVALIACOES { IdPeriodo = 1, fl_lib_res_mentoria = false };
            _mockPeriodosSet.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PERIODOSAVALIACOES, bool>>>(), default))
                .ReturnsAsync(periodo);
            _mockDb.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new Exception("Save error"));

            var result = await _service.LiberarMentoriaAsync(1);

            Assert.False(result);
            _mockTelemetryService.Verify(x => x.TrackException(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
            _mockMessageBoxService.Verify(x => x.ShowError("Erro ao liberar mentoria."), Times.Once);
        }
    }
}