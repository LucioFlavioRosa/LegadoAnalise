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
        public void TrackEvent_DeveChamarTelemetryClient()
        {
            var eventName = "TestEvent";
            var properties = new Dictionary<string, string> { { "key1", "value1" } };
            var metrics = new Dictionary<string, double> { { "metric1", 1.0 } };

            _service.TrackEvent(eventName, properties, metrics);

            _mockTelemetryClient.Verify(x => x.TrackEvent(eventName, properties, metrics), Times.Once);
        }

        [Fact]
        public void TrackEvent_SemParametrosOpcionais_DeveChamarComNulls()
        {
            var eventName = "SimpleEvent";

            _service.TrackEvent(eventName);

            _mockTelemetryClient.Verify(x => x.TrackEvent(eventName, null, null), Times.Once);
        }

        [Fact]
        public void TrackException_DeveChamarTelemetryClient()
        {
            var exception = new Exception("Test exception");
            var properties = new Dictionary<string, string> { { "context", "test" } };

            _service.TrackException(exception, properties);

            _mockTelemetryClient.Verify(x => x.TrackException(exception, properties), Times.Once);
        }

        [Fact]
        public void TrackException_SemProperties_DeveChamarComNull()
        {
            var exception = new Exception("Simple exception");

            _service.TrackException(exception);

            _mockTelemetryClient.Verify(x => x.TrackException(exception, null), Times.Once);
        }

        [Fact]
        public void TrackDependency_DeveChamarTelemetryClient()
        {
            var dependencyTypeName = "HTTP";
            var dependencyName = "API Call";
            var data = "GET /api/test";
            var startTime = DateTimeOffset.Now;
            var duration = TimeSpan.FromMilliseconds(500);
            var success = true;

            _service.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success);

            _mockTelemetryClient.Verify(x => x.TrackDependency(dependencyTypeName, dependencyName, data, startTime, duration, success), Times.Once);
        }

        [Fact]
        public void TrackPageView_DeveChamarTelemetryClient()
        {
            var pageName = "TestPage";
            var properties = new Dictionary<string, string> { { "userId", "123" } };

            _service.TrackPageView(pageName, properties);

            _mockTelemetryClient.Verify(x => x.TrackPageView(pageName, properties), Times.Once);
        }

        [Fact]
        public void TrackPageView_SemProperties_DeveChamarComNull()
        {
            var pageName = "SimplePage";

            _service.TrackPageView(pageName);

            _mockTelemetryClient.Verify(x => x.TrackPageView(pageName, null), Times.Once);
        }

        [Fact]
        public void TrackMetric_DeveChamarTelemetryClient()
        {
            var metricName = "ResponseTime";
            var value = 250.5;
            var properties = new Dictionary<string, string> { { "endpoint", "/api/test" } };

            _service.TrackMetric(metricName, value, properties);

            _mockTelemetryClient.Verify(x => x.TrackMetric(metricName, value, properties), Times.Once);
        }

        [Fact]
        public void TrackMetric_SemProperties_DeveChamarComNull()
        {
            var metricName = "SimpleMetric";
            var value = 100.0;

            _service.TrackMetric(metricName, value);

            _mockTelemetryClient.Verify(x => x.TrackMetric(metricName, value, null), Times.Once);
        }
    }
}