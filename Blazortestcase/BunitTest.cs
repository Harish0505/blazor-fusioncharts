using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Xunit;

namespace Microsoft.FusionChartsInterop.Tests
{
    public class MockJSRuntime : IJSRuntime
    {
        public List<Tuple<string, object[]>> Invocations { get; } = new List<Tuple<string, object[]>>();

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args)
        {
            Invocations.Add(Tuple.Create(identifier, args));
            return new ValueTask<TValue>(default(TValue));
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args)
        {
            Invocations.Add(Tuple.Create(identifier, args));
            return new ValueTask<TValue>(default(TValue));
        }
    }

    public class FusionChartsServiceTests
    {
        [Fact]
        public async Task RenderChart_CallsInvokeVoidAsyncWithCorrectArguments()
        {
            // Arrange
            var mockJsRuntime = new MockJSRuntime();
            var fusionChartsService = new FusionChartsService(mockJsRuntime);
            var chartConfig = "someChartConfig";

            // Act
            await fusionChartsService.renderChart(chartConfig);

            // Assert
            Assert.Single(mockJsRuntime.Invocations);
            Assert.Equal("FusionCharts.renderChart", mockJsRuntime.Invocations[0].Item1);
            Assert.Equal(new object[] { chartConfig }, mockJsRuntime.Invocations[0].Item2);
        }

        [Fact]
        public async Task ActivateLicense_CallsInvokeVoidAsyncWithCorrectArguments()
        {
            // Arrange
            var mockJsRuntime = new MockJSRuntime();
            var fusionChartsService = new FusionChartsService(mockJsRuntime);
            var licenseKey = "someLicenseKey";

            // Act
            await fusionChartsService.activateLicense(licenseKey);

            // Assert
            Assert.Single(mockJsRuntime.Invocations);
            Assert.Equal("FusionCharts.options.license", mockJsRuntime.Invocations[0].Item1);
            var licenseObject = mockJsRuntime.Invocations[0].Item2[0] as object;
            Assert.NotNull(licenseObject);

            var key = licenseObject.GetType().GetProperty("key")?.GetValue(licenseObject)?.ToString();
            var creditLabel = (bool)licenseObject.GetType().GetProperty("creditLabel")?.GetValue(licenseObject);

            Assert.Equal(licenseKey, key);
            Assert.False(creditLabel);
        }
    }
}
