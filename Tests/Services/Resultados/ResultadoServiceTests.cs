using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly Mock<DbSet<ResultadoProjetosModel>> _mockDbSet;
        private readonly ResultadoService _service;
        private readonly List<ResultadoProjetosModel> _testData;

        public ResultadoServiceTests()
        {
            _mockDbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockTelemetryService = new Mock<ITelemetryService>();
            _mockMessageBoxService = new Mock<IMessageBoxService>();
            _mockExportFileService = new Mock<IExportFileService>();
            _mockDbSet = new Mock<DbSet<ResultadoProjetosModel>>();

            _testData = new List<ResultadoProjetosModel>
            {
                new ResultadoProjetosModel
                {
                    Id = 1,
                    IdProjeto = 100,
                    IdAssociado = 200,
                    IdPeriodo = 300,
                    TipoAvaliacao = "lideranca"
                },
                new ResultadoProjetosModel
                {
                    Id = 2,
                    IdProjeto = 101,
                    IdAssociado = 201,
                    IdPeriodo = 301,
                    TipoAvaliacao = "desempenho"
                },
                new ResultadoProjetosModel
                {
                    Id = 3,
                    IdProjeto = 100,
                    IdAssociado = 200,
                    IdPeriodo = 300,
                    TipoAvaliacao = "lideranca"
                },
                new ResultadoProjetosModel
                {
                    Id = 4,
                    IdProjeto = 102,
                    IdAssociado = 202,
                    IdPeriodo = 302,
                    TipoAvaliacao = "mentoria"
                }
            };

            var queryable = _testData.AsQueryable();
            _mockDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockDbSet.As<IQueryable<ResultadoProjetosModel>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            _mockDbContext.Setup(c => c.ResultadoProjetos).Returns(_mockDbSet.Object);

            _service = new ResultadoService(
                _mockDbContext.Object,
                _mockTelemetryService.Object,
                _mockMessageBoxService.Object,
                _mockExportFileService.Object);
        }

        [Fact]
        public async Task ListarResultadosAsync_FiltrosAplicados_RetornaResultadosFiltrados()
        {
            // Arrange
            int? idProjeto = 100;
            int? idAssociado = 200;
            int? idPeriodo = 300;
            string tipoAvaliacao = "lideranca";

            // Act
            var resultado = await _service.ListarResultadosAsync(idProjeto, idAssociado, idPeriodo, tipoAvaliacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, r => Assert.Equal(idProjeto.Value, r.IdProjeto));
            Assert.All(resultado, r => Assert.Equal(idAssociado.Value, r.IdAssociado));
            Assert.All(resultado, r => Assert.Equal(idPeriodo.Value, r.IdPeriodo));
            Assert.All(resultado, r => Assert.Equal(tipoAvaliacao, r.TipoAvaliacao));

            _mockTelemetryService.Verify(t => t.TrackEvent(
                "ListarResultados",
                It.Is<Dictionary<string, string>>(d =>
                    d["IdProjeto"] == idProjeto.ToString() &&
                    d["IdAssociado"] == idAssociado.ToString() &&
                    d["IdPeriodo"] == idPeriodo.ToString() &&
                    d["TipoAvaliacao"] == tipoAvaliacao &&
                    d["Count"] == "2"),
                null), Times.Once);
        }

        [Fact]
        public async Task ListarResultadosAsync_FiltroIdProjeto_RetornaApenasProjetosFiltrados()
        {
            // Arrange
            int? idProjeto = 101;
            int? idAssociado = null;
            int? idPeriodo = null;
            string tipoAvaliacao = null;

            // Act
            var resultado = await _service.ListarResultadosAsync(idProjeto, idAssociado, idPeriodo, tipoAvaliacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(idProjeto.Value, resultado.First().IdProjeto);
            Assert.Equal(201, resultado.First().IdAssociado);
            Assert.Equal("desempenho", resultado.First().TipoAvaliacao);
        }

        [Fact]
        public async Task ListarResultadosAsync_FiltroTipoAvaliacao_RetornaApenasTiposFiltrados()
        {
            // Arrange
            int? idProjeto = null;
            int? idAssociado = null;
            int? idPeriodo = null;
            string tipoAvaliacao = "lideranca";

            // Act
            var resultado = await _service.ListarResultadosAsync(idProjeto, idAssociado, idPeriodo, tipoAvaliacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, r => Assert.Equal(tipoAvaliacao, r.TipoAvaliacao));
        }

        [Fact]
        public async Task ListarResultadosAsync_SemFiltros_RetornaTodosResultados()
        {
            // Arrange
            int? idProjeto = null;
            int? idAssociado = null;
            int? idPeriodo = null;
            string tipoAvaliacao = null;

            // Act
            var resultado = await _service.ListarResultadosAsync(idProjeto, idAssociado, idPeriodo, tipoAvaliacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(4, resultado.Count);

            _mockTelemetryService.Verify(t => t.TrackEvent(
                "ListarResultados",
                It.Is<Dictionary<string, string>>(d =>
                    d["IdProjeto"] == "" &&
                    d["IdAssociado"] == "" &&
                    d["IdPeriodo"] == "" &&
                    d["TipoAvaliacao"] == "" &&
                    d["Count"] == "4"),
                null), Times.Once);
        }

        [Fact]
        public async Task ListarResultadosAsync_FiltrosComValoresZero_NaoAplicaFiltro()
        {
            // Arrange
            int? idProjeto = 0;
            int? idAssociado = 0;
            int? idPeriodo = 0;
            string tipoAvaliacao = "";

            // Act
            var resultado = await _service.ListarResultadosAsync(idProjeto, idAssociado, idPeriodo, tipoAvaliacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(4, resultado.Count);
        }

        [Fact]
        public async Task ListarResultadosAsync_FiltrosSemCorrespondencia_RetornaListaVazia()
        {
            // Arrange
            int? idProjeto = 999;
            int? idAssociado = null;
            int? idPeriodo = null;
            string tipoAvaliacao = null;

            // Act
            var resultado = await _service.ListarResultadosAsync(idProjeto, idAssociado, idPeriodo, tipoAvaliacao);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            _mockTelemetryService.Verify(t => t.TrackEvent(
                "ListarResultados",
                It.Is<Dictionary<string, string>>(d => d["Count"] == "0"),
                null), Times.Once);
        }
    }
}