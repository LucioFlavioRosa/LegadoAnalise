using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using SistemaAvaliacao.Moderno.Models;
using SistemaAvaliacao.Moderno.Data;
using SistemaAvaliacao.Moderno.Services;
using Microsoft.Extensions.Logging;

namespace SistemaAvaliacao.Moderno.Tests.Services
{
    public class CargoServiceTests
    {
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly Mock<ILogger<CargoService>> _loggerMock;
        private readonly CargoService _service;

        public CargoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var dbContext = new ApplicationDbContext(options);
            _dbContextMock = new Mock<ApplicationDbContext>(options);
            _loggerMock = new Mock<ILogger<CargoService>>();
            _service = new CargoService(dbContext, _loggerMock.Object, null);
        }

        [Fact]
        public async Task CriarAsync_DeveCriarCargoComSucesso()
        {
            var cargo = new Cargo
            {
                NomeCargo = "Analista",
                TempoMinimoPromocao = 12,
                Ativo = true
            };

            var result = await _service.CriarAsync(cargo);

            Assert.True(result);
            var cargos = await _service.ObterTodosAsync();
            Assert.Contains(cargos, c => c.NomeCargo == "Analista");
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarCargoComSucesso()
        {
            var cargo = new Cargo { NomeCargo = "Dev", TempoMinimoPromocao = 6, Ativo = true };
            await _service.CriarAsync(cargo);
            var cargos = await _service.ObterTodosAsync();
            var cargoDb = cargos[0];
            cargoDb.NomeCargo = "Desenvolvedor";

            var result = await _service.AtualizarAsync(cargoDb);

            Assert.True(result);
            var atualizado = await _service.ObterPorIdAsync(cargoDb.IdCargo);
            Assert.Equal("Desenvolvedor", atualizado.NomeCargo);
        }

        [Fact]
        public async Task InativarAsync_DeveInativarCargo()
        {
            var cargo = new Cargo { NomeCargo = "Gestor", TempoMinimoPromocao = 24, Ativo = true };
            await _service.CriarAsync(cargo);
            var cargos = await _service.ObterTodosAsync();
            var cargoDb = cargos[0];

            var result = await _service.InativarAsync(cargoDb.IdCargo);

            Assert.True(result);
            var atualizado = await _service.ObterPorIdAsync(cargoDb.IdCargo);
            Assert.False(atualizado.Ativo);
        }

        [Fact]
        public async Task CriarAsync_DeveFalharComDadosInvalidos()
        {
            var cargo = new Cargo { NomeCargo = "", TempoMinimoPromocao = 0, Ativo = true };

            var result = await _service.CriarAsync(cargo);

            Assert.False(result);
        }

        [Fact]
        public async Task CriarAsync_DeveTratarExcecaoDeBanco()
        {
            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(db => db.AddAsync(It.IsAny<Cargo>(), default))
                .ThrowsAsync(new Exception("Erro de banco"));
            var service = new CargoService(dbContextMock.Object, _loggerMock.Object, null);
            var cargo = new Cargo { NomeCargo = "Teste", TempoMinimoPromocao = 1, Ativo = true };

            var result = await service.CriarAsync(cargo);

            Assert.False(result);
        }
    }
}
