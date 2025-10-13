using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using SistemaAvaliacao.Services;
using SistemaAvaliacao.Domain.Models;
using SistemaAvaliacao.Repositories;
using SistemaAvaliacao.Infrastructure.Validators;

namespace SistemaAvaliacao.Tests.Services
{
    public class CargoServiceTests
    {
        private readonly Mock<ICargoRepository> _cargoRepositoryMock;
        private readonly Mock<ICargoValidator> _cargoValidatorMock;
        private readonly CargoService _cargoService;

        public CargoServiceTests()
        {
            _cargoRepositoryMock = new Mock<ICargoRepository>();
            _cargoValidatorMock = new Mock<ICargoValidator>();
            _cargoService = new CargoService(_cargoRepositoryMock.Object, _cargoValidatorMock.Object);
        }

        [Fact]
        public void AddCargo_ValidCargo_CallsRepositoryAndReturnsCargo()
        {
            // Arrange
            var cargo = new Cargo
            {
                IdCargo = 0,
                NomeCargo = "Analista",
                ProximoCargoId = 2,
                TempoMinimoPromocao = 12,
                Funcao = "Análise de sistemas",
                Autonomia = "Média",
                EscopoAtuacao = "Projetos",
                NivelInterlocucao = "Gerente",
                Status = 1
            };
            _cargoValidatorMock.Setup(v => v.Validate(cargo)).Returns(new List<string>());
            _cargoRepositoryMock.Setup(r => r.AddCargo(cargo)).Returns(1);

            // Act
            var result = _cargoService.AddCargo(cargo);

            // Assert
            _cargoValidatorMock.Verify(v => v.Validate(cargo), Times.Once);
            _cargoRepositoryMock.Verify(r => r.AddCargo(cargo), Times.Once);
            Assert.Equal(1, result);
        }

        [Fact]
        public void AddCargo_InvalidCargo_ThrowsExceptionAndDoesNotCallRepository()
        {
            // Arrange
            var cargo = new Cargo { NomeCargo = "" };
            var errors = new List<string> { "Nome do cargo é obrigatório." };
            _cargoValidatorMock.Setup(v => v.Validate(cargo)).Returns(errors);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _cargoService.AddCargo(cargo));
            Assert.Contains("Nome do cargo é obrigatório.", ex.Message);
            _cargoRepositoryMock.Verify(r => r.AddCargo(It.IsAny<Cargo>()), Times.Never);
        }

        [Fact]
        public void GetCargoById_ExistingCargo_ReturnsCargo()
        {
            // Arrange
            var cargo = new Cargo { IdCargo = 1, NomeCargo = "Analista" };
            _cargoRepositoryMock.Setup(r => r.GetCargoById(1)).Returns(cargo);

            // Act
            var result = _cargoService.GetCargoById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.IdCargo);
            Assert.Equal("Analista", result.NomeCargo);
        }

        [Fact]
        public void GetCargoById_NonExistingCargo_ReturnsNull()
        {
            // Arrange
            _cargoRepositoryMock.Setup(r => r.GetCargoById(99)).Returns((Cargo)null);

            // Act
            var result = _cargoService.GetCargoById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateCargo_ValidCargo_CallsRepository()
        {
            // Arrange
            var cargo = new Cargo { IdCargo = 1, NomeCargo = "Analista" };
            _cargoValidatorMock.Setup(v => v.Validate(cargo)).Returns(new List<string>());
            _cargoRepositoryMock.Setup(r => r.UpdateCargo(cargo)).Returns(true);

            // Act
            var result = _cargoService.UpdateCargo(cargo);

            // Assert
            _cargoValidatorMock.Verify(v => v.Validate(cargo), Times.Once);
            _cargoRepositoryMock.Verify(r => r.UpdateCargo(cargo), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public void UpdateCargo_InvalidCargo_ThrowsException()
        {
            // Arrange
            var cargo = new Cargo { IdCargo = 1, NomeCargo = "" };
            var errors = new List<string> { "Nome do cargo é obrigatório." };
            _cargoValidatorMock.Setup(v => v.Validate(cargo)).Returns(errors);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _cargoService.UpdateCargo(cargo));
            Assert.Contains("Nome do cargo é obrigatório.", ex.Message);
            _cargoRepositoryMock.Verify(r => r.UpdateCargo(It.IsAny<Cargo>()), Times.Never);
        }

        [Fact]
        public void DeleteCargo_ValidId_CallsRepository()
        {
            // Arrange
            _cargoRepositoryMock.Setup(r => r.DeleteCargo(1)).Returns(true);

            // Act
            var result = _cargoService.DeleteCargo(1);

            // Assert
            _cargoRepositoryMock.Verify(r => r.DeleteCargo(1), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public void GetAllCargos_ReturnsListOfCargos()
        {
            // Arrange
            var cargos = new List<Cargo>
            {
                new Cargo { IdCargo = 1, NomeCargo = "Analista" },
                new Cargo { IdCargo = 2, NomeCargo = "Coordenador" }
            };
            _cargoRepositoryMock.Setup(r => r.GetAllCargos()).Returns(cargos);

            // Act
            var result = _cargoService.GetAllCargos();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
