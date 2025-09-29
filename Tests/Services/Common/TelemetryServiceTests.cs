using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Moq;
using Xunit;
using Peers.Moderno.Services.Common;

namespace Tests.Services.Common
{
    public class TelemetryServiceTests
    {
        private readonly Mock<TelemetryClient> _mockTelemetryClient;
        private readonly TelemetryService _service;

        public TelemetryServiceTests()
        {
            _mockTelemetryClient = new Mock<TelemetryClient>();
            _service = new TelemetryService(_mockTelemetryClient.Object);
        }

        [Fact]
        public void TrackEvent_DevePassarParametrosCorretamente()
        {
            // Arrange
            var eventName = "TestEvent";
            var properties = new Dictionary<string, string> { { "key1", "value1" } };
            var metrics = new Dictionary<string, double> { { "metric1", 1.5 } };

            // Act
            _service.TrackEvent(eventName, properties, metrics);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackEvent(eventName, properties, metrics), Times.Once);
        }

        [Fact]
        public void TrackEvent_ComParametrosNulos_DevePassarNulos()
        {
            // Arrange
            var eventName = "TestEvent";

            // Act
            _service.TrackEvent(eventName, null, null);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackEvent(eventName, null, null), Times.Once);
        }

        [Fact]
        public void TrackException_DevePassarParametrosCorretamente()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var properties = new Dictionary<string, string> { { "key1", "value1" } };

            // Act
            _service.TrackException(exception, properties);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackException(exception, properties), Times.Once);
        }

        [Fact]
        public void TrackException_ComPropertiesNulas_DevePassarNulo()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act
            _service.TrackException(exception, null);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackException(exception, null), Times.Once);
        }

        [Fact]
        public void TrackDependency_DevePassarParametrosCorretamente()
        {
            // Arrange
            var dependencyTypeName = "HTTP";
            var dependencyName = "API Call";
            var data = "GET /api/test";
            var startTime = DateTimeOffset.Now;
            var duration = TimeSpan.FromMilliseconds(500);
            var success = true;

            // Act
            _service.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success), Times.Once);
        }

        [Fact]
        public void TrackPageView_DevePassarParametrosCorretamente()
        {
            // Arrange
            var pageName = "TestPage";
            var properties = new Dictionary<string, string> { { "userId", "123" } };

            // Act
            _service.TrackPageView(pageName, properties);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackPageView(pageName, properties), Times.Once);
        }

        [Fact]
        public void TrackPageView_ComPropertiesNulas_DevePassarNulo()
        {
            // Arrange
            var pageName = "TestPage";

            // Act
            _service.TrackPageView(pageName, null);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackPageView(pageName, null), Times.Once);
        }

        [Fact]
        public void TrackMetric_DevePassarParametrosCorretamente()
        {
            // Arrange
            var metricName = "ResponseTime";
            var value = 250.5;
            var properties = new Dictionary<string, string> { { "endpoint", "/api/test" } };

            // Act
            _service.TrackMetric(metricName, value, properties);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackMetric(metricName, value, properties), Times.Once);
        }

        [Fact]
        public void TrackMetric_ComPropertiesNulas_DevePassarNulo()
        {
            // Arrange
            var metricName = "ResponseTime";
            var value = 250.5;

            // Act
            _service.TrackMetric(metricName, value, null);

            // Assert
            _mockTelemetryClient.Verify(x => x.TrackMetric(metricName, value, null), Times.Once);
        }
    }
}