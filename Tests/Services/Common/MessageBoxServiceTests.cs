using System;
using Xunit;
using Peers.Moderno.Services.Common;

namespace Tests.Services.Common
{
    public class MessageBoxServiceTests
    {
        private readonly MessageBoxService _service;

        public MessageBoxServiceTests()
        {
            _service = new MessageBoxService();
        }

        [Fact]
        public void ShowSuccess_DeveDispararEventoComTipoCorreto()
        {
            // Arrange
            MessageBoxEventArgs capturedArgs = null;
            _service.OnMessageReceived += (args) => capturedArgs = args;

            // Act
            _service.ShowSuccess("Sucesso", "Título", 5000);

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal("Sucesso", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Success, capturedArgs.Type);
            Assert.Equal("Título", capturedArgs.Title);
            Assert.Equal(5000, capturedArgs.Delay);
        }

        [Fact]
        public void ShowError_DeveDispararEventoComTipoCorreto()
        {
            // Arrange
            MessageBoxEventArgs capturedArgs = null;
            _service.OnMessageReceived += (args) => capturedArgs = args;

            // Act
            _service.ShowError("Erro", "Título Erro", 3000);

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal("Erro", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Error, capturedArgs.Type);
            Assert.Equal("Título Erro", capturedArgs.Title);
            Assert.Equal(3000, capturedArgs.Delay);
        }

        [Fact]
        public void ShowInfo_DeveDispararEventoComTipoCorreto()
        {
            // Arrange
            MessageBoxEventArgs capturedArgs = null;
            _service.OnMessageReceived += (args) => capturedArgs = args;

            // Act
            _service.ShowInfo("Informação");

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal("Informação", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Info, capturedArgs.Type);
            Assert.Null(capturedArgs.Title);
            Assert.Null(capturedArgs.Delay);
        }

        [Fact]
        public void ShowWarning_DeveDispararEventoComTipoCorreto()
        {
            // Arrange
            MessageBoxEventArgs capturedArgs = null;
            _service.OnMessageReceived += (args) => capturedArgs = args;

            // Act
            _service.ShowWarning("Aviso", "Título Aviso");

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal("Aviso", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Warning, capturedArgs.Type);
            Assert.Equal("Título Aviso", capturedArgs.Title);
            Assert.Null(capturedArgs.Delay);
        }

        [Fact]
        public void Show_DeveDispararEventoComParametrosCorretos()
        {
            // Arrange
            MessageBoxEventArgs capturedArgs = null;
            _service.OnMessageReceived += (args) => capturedArgs = args;

            // Act
            _service.Show("Mensagem customizada", MessageBoxType.Warning, "Título customizado", 2000);

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal("Mensagem customizada", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Warning, capturedArgs.Type);
            Assert.Equal("Título customizado", capturedArgs.Title);
            Assert.Equal(2000, capturedArgs.Delay);
        }

        [Fact]
        public void Show_SemParametrosOpcionais_DeveUsarPadroes()
        {
            // Arrange
            MessageBoxEventArgs capturedArgs = null;
            _service.OnMessageReceived += (args) => capturedArgs = args;

            // Act
            _service.Show("Mensagem simples");

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal("Mensagem simples", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Info, capturedArgs.Type);
            Assert.Null(capturedArgs.Title);
            Assert.Null(capturedArgs.Delay);
        }

        [Fact]
        public void OnMessageReceived_SemSubscribers_NaoDeveGerarExcecao()
        {
            // Arrange
            var serviceWithoutSubscribers = new MessageBoxService();

            // Act & Assert
            var exception = Record.Exception(() => serviceWithoutSubscribers.ShowSuccess("Teste"));
            Assert.Null(exception);
        }
    }
}