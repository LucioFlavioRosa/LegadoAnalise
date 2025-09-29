using System;
using Xunit;
using Peers.Moderno.Services.Common;

namespace Tests.Services.Common
{
    public class MessageBoxServiceTests
    {
        [Fact]
        public void ShowError_DisparaEventoComTipoError()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowError("Erro de teste", "Título de Erro", 5000);

            Assert.NotNull(capturedArgs);
            Assert.Equal("Erro de teste", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Error, capturedArgs.Type);
            Assert.Equal("Título de Erro", capturedArgs.Title);
            Assert.Equal(5000, capturedArgs.Delay);
        }

        [Fact]
        public void ShowSuccess_DisparaEventoComTipoSuccess()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowSuccess("Sucesso de teste");

            Assert.NotNull(capturedArgs);
            Assert.Equal("Sucesso de teste", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Success, capturedArgs.Type);
            Assert.Null(capturedArgs.Title);
            Assert.Null(capturedArgs.Delay);
        }

        [Fact]
        public void ShowInfo_DisparaEventoComTipoInfo()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowInfo("Info de teste", "Título Info");

            Assert.NotNull(capturedArgs);
            Assert.Equal("Info de teste", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Info, capturedArgs.Type);
            Assert.Equal("Título Info", capturedArgs.Title);
        }

        [Fact]
        public void ShowWarning_DisparaEventoComTipoWarning()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowWarning("Warning de teste");

            Assert.NotNull(capturedArgs);
            Assert.Equal("Warning de teste", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Warning, capturedArgs.Type);
        }

        [Fact]
        public void Show_DisparaEventoComParametrosPersonalizados()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.Show("Mensagem personalizada", MessageBoxType.Error, "Título Personalizado", 3000);

            Assert.NotNull(capturedArgs);
            Assert.Equal("Mensagem personalizada", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Error, capturedArgs.Type);
            Assert.Equal("Título Personalizado", capturedArgs.Title);
            Assert.Equal(3000, capturedArgs.Delay);
        }

        [Fact]
        public void Show_SemParametrosOpcionais_UsaValoresPadrao()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.Show("Mensagem simples");

            Assert.NotNull(capturedArgs);
            Assert.Equal("Mensagem simples", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Info, capturedArgs.Type);
            Assert.Null(capturedArgs.Title);
            Assert.Null(capturedArgs.Delay);
        }

        [Fact]
        public void MultipleSubscribers_TodosRecebemEvento()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs1 = null;
            MessageBoxEventArgs capturedArgs2 = null;
            
            service.OnMessageReceived += (args) => capturedArgs1 = args;
            service.OnMessageReceived += (args) => capturedArgs2 = args;

            service.ShowError("Erro para múltiplos");

            Assert.NotNull(capturedArgs1);
            Assert.NotNull(capturedArgs2);
            Assert.Equal("Erro para múltiplos", capturedArgs1.Message);
            Assert.Equal("Erro para múltiplos", capturedArgs2.Message);
            Assert.Equal(MessageBoxType.Error, capturedArgs1.Type);
            Assert.Equal(MessageBoxType.Error, capturedArgs2.Type);
        }

        [Fact]
        public void SemSubscribers_NaoLancaExcecao()
        {
            var service = new MessageBoxService();

            var exception = Record.Exception(() => service.ShowError("Erro sem subscribers"));

            Assert.Null(exception);
        }
    }

    public class MessageBoxEventArgsTests
    {
        [Fact]
        public void Constructor_ComTodosParametros_InicializaCorretamente()
        {
            var args = new MessageBoxEventArgs("Mensagem teste", MessageBoxType.Warning, "Título teste", 2000);

            Assert.Equal("Mensagem teste", args.Message);
            Assert.Equal(MessageBoxType.Warning, args.Type);
            Assert.Equal("Título teste", args.Title);
            Assert.Equal(2000, args.Delay);
        }

        [Fact]
        public void Constructor_SemParametrosOpcionais_InicializaComValoresNulos()
        {
            var args = new MessageBoxEventArgs("Mensagem", MessageBoxType.Info);

            Assert.Equal("Mensagem", args.Message);
            Assert.Equal(MessageBoxType.Info, args.Type);
            Assert.Null(args.Title);
            Assert.Null(args.Delay);
        }
    }
}