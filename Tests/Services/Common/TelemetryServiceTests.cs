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
        private readonly TelemetryService _telemetryService;

        public TelemetryServiceTests()
        {
            _mockTelemetryClient = new Mock<TelemetryClient>();
            _telemetryService = new TelemetryService(_mockTelemetryClient.Object);
        }

        [Fact]
        public void TrackEvent_ChamaTelemetryClientCorretamente()
        {
            var eventName = "TestEvent";
            var properties = new Dictionary<string, string> { { "key1", "value1" } };
            var metrics = new Dictionary<string, double> { { "metric1", 1.5 } };

            _telemetryService.TrackEvent(eventName, properties, metrics);

            _mockTelemetryClient.Verify(t => t.TrackEvent(eventName, properties, metrics), Times.Once);
        }

        [Fact]
        public void TrackEvent_ComParametrosNulos_ChamaTelemetryClientCorretamente()
        {
            var eventName = "TestEvent";

            _telemetryService.TrackEvent(eventName, null, null);

            _mockTelemetryClient.Verify(t => t.TrackEvent(eventName, null, null), Times.Once);
        }

        [Fact]
        public void TrackEvent_SemParametrosOpcionais_ChamaTelemetryClientCorretamente()
        {
            var eventName = "TestEvent";

            _telemetryService.TrackEvent(eventName);

            _mockTelemetryClient.Verify(t => t.TrackEvent(eventName, null, null), Times.Once);
        }

        [Fact]
        public void TrackException_ChamaTelemetryClientCorretamente()
        {
            var exception = new Exception("Test exception");
            var properties = new Dictionary<string, string> { { "context", "test" } };

            _telemetryService.TrackException(exception, properties);

            _mockTelemetryClient.Verify(t => t.TrackException(exception, properties), Times.Once);
        }

        [Fact]
        public void TrackException_ComPropertiesNulas_ChamaTelemetryClientCorretamente()
        {
            var exception = new Exception("Test exception");

            _telemetryService.TrackException(exception, null);

            _mockTelemetryClient.Verify(t => t.TrackException(exception, null), Times.Once);
        }

        [Fact]
        public void TrackException_SemParametrosOpcionais_ChamaTelemetryClientCorretamente()
        {
            var exception = new Exception("Test exception");

            _telemetryService.TrackException(exception);

            _mockTelemetryClient.Verify(t => t.TrackException(exception, null), Times.Once);
        }

        [Fact]
        public void TrackDependency_ChamaTelemetryClientCorretamente()
        {
            var dependencyTypeName = "SQL";
            var dependencyName = "GetUsers";
            var data = "SELECT * FROM Users";
            var startTime = DateTimeOffset.Now;
            var duration = TimeSpan.FromMilliseconds(500);
            var success = true;

            _telemetryService.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success);

            _mockTelemetryClient.Verify(t => t.TrackDependency(
                dependencyTypeName, dependencyName, data, startTime, duration, success), Times.Once);
        }

        [Fact]
        public void TrackPageView_ChamaTelemetryClientCorretamente()
        {
            var pageName = "HomePage";
            var properties = new Dictionary<string, string> { { "user", "testuser" } };

            _telemetryService.TrackPageView(pageName, properties);

            _mockTelemetryClient.Verify(t => t.TrackPageView(pageName, properties), Times.Once);
        }

        [Fact]
        public void TrackPageView_ComPropertiesNulas_ChamaTelemetryClientCorretamente()
        {
            var pageName = "HomePage";

            _telemetryService.TrackPageView(pageName, null);

            _mockTelemetryClient.Verify(t => t.TrackPageView(pageName, null), Times.Once);
        }

        [Fact]
        public void TrackPageView_SemParametrosOpcionais_ChamaTelemetryClientCorretamente()
        {
            var pageName = "HomePage";

            _telemetryService.TrackPageView(pageName);

            _mockTelemetryClient.Verify(t => t.TrackPageView(pageName, null), Times.Once);
        }

        [Fact]
        public void TrackMetric_ChamaTelemetryClientCorretamente()
        {
            var metricName = "ResponseTime";
            var value = 250.5;
            var properties = new Dictionary<string, string> { { "endpoint", "/api/users" } };

            _telemetryService.TrackMetric(metricName, value, properties);

            _mockTelemetryClient.Verify(t => t.TrackMetric(metricName, value, properties), Times.Once);
        }

        [Fact]
        public void TrackMetric_ComPropertiesNulas_ChamaTelemetryClientCorretamente()
        {
            var metricName = "ResponseTime";
            var value = 250.5;

            _telemetryService.TrackMetric(metricName, value, null);

            _mockTelemetryClient.Verify(t => t.TrackMetric(metricName, value, null), Times.Once);
        }

        [Fact]
        public void TrackMetric_SemParametrosOpcionais_ChamaTelemetryClientCorretamente()
        {
            var metricName = "ResponseTime";
            var value = 250.5;

            _telemetryService.TrackMetric(metricName, value);

            _mockTelemetryClient.Verify(t => t.TrackMetric(metricName, value, null), Times.Once);
        }

        [Fact]
        public void MultipleOperations_ChamaTelemetryClientMultiplasVezes()
        {
            var eventName = "Event1";
            var exception = new Exception("Error1");
            var pageName = "Page1";

            _telemetryService.TrackEvent(eventName);
            _telemetryService.TrackException(exception);
            _telemetryService.TrackPageView(pageName);

            _mockTelemetryClient.Verify(t => t.TrackEvent(eventName, null, null), Times.Once);
            _mockTelemetryClient.Verify(t => t.TrackException(exception, null), Times.Once);
            _mockTelemetryClient.Verify(t => t.TrackPageView(pageName, null), Times.Once);
        }
    }
}