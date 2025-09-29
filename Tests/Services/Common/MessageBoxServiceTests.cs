using System;
using Xunit;
using Peers.Moderno.Services.Common;

namespace Tests.Services.Common
{
    public class MessageBoxServiceTests
    {
        [Fact]
        public void ShowError_DeveDispararEventoComTipoError()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowError("Test error message", "Error Title", 5000);

            Assert.NotNull(capturedArgs);
            Assert.Equal("Test error message", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Error, capturedArgs.Type);
            Assert.Equal("Error Title", capturedArgs.Title);
            Assert.Equal(5000, capturedArgs.Delay);
        }

        [Fact]
        public void ShowSuccess_DeveDispararEventoComTipoSuccess()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowSuccess("Success message");

            Assert.NotNull(capturedArgs);
            Assert.Equal("Success message", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Success, capturedArgs.Type);
            Assert.Null(capturedArgs.Title);
            Assert.Null(capturedArgs.Delay);
        }

        [Fact]
        public void ShowInfo_DeveDispararEventoComTipoInfo()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowInfo("Info message", "Info Title");

            Assert.NotNull(capturedArgs);
            Assert.Equal("Info message", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Info, capturedArgs.Type);
            Assert.Equal("Info Title", capturedArgs.Title);
        }

        [Fact]
        public void ShowWarning_DeveDispararEventoComTipoWarning()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.ShowWarning("Warning message", "Warning Title", 3000);

            Assert.NotNull(capturedArgs);
            Assert.Equal("Warning message", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Warning, capturedArgs.Type);
            Assert.Equal("Warning Title", capturedArgs.Title);
            Assert.Equal(3000, capturedArgs.Delay);
        }

        [Fact]
        public void Show_DeveDispararEventoComParametrosCorretos()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.Show("Generic message", MessageBoxType.Warning, "Generic Title", 2000);

            Assert.NotNull(capturedArgs);
            Assert.Equal("Generic message", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Warning, capturedArgs.Type);
            Assert.Equal("Generic Title", capturedArgs.Title);
            Assert.Equal(2000, capturedArgs.Delay);
        }

        [Fact]
        public void Show_SemParametrosOpcionais_DeveUsarPadroes()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs = null;
            service.OnMessageReceived += (args) => capturedArgs = args;

            service.Show("Default message");

            Assert.NotNull(capturedArgs);
            Assert.Equal("Default message", capturedArgs.Message);
            Assert.Equal(MessageBoxType.Info, capturedArgs.Type);
            Assert.Null(capturedArgs.Title);
            Assert.Null(capturedArgs.Delay);
        }

        [Fact]
        public void MultipleSubscribers_DevemReceberEventos()
        {
            var service = new MessageBoxService();
            MessageBoxEventArgs capturedArgs1 = null;
            MessageBoxEventArgs capturedArgs2 = null;
            
            service.OnMessageReceived += (args) => capturedArgs1 = args;
            service.OnMessageReceived += (args) => capturedArgs2 = args;

            service.ShowError("Multi subscriber test");

            Assert.NotNull(capturedArgs1);
            Assert.NotNull(capturedArgs2);
            Assert.Equal("Multi subscriber test", capturedArgs1.Message);
            Assert.Equal("Multi subscriber test", capturedArgs2.Message);
            Assert.Equal(MessageBoxType.Error, capturedArgs1.Type);
            Assert.Equal(MessageBoxType.Error, capturedArgs2.Type);
        }
    }
}