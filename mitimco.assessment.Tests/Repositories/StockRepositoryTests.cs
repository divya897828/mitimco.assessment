using Moq;
using Moq.Contrib.HttpClient;
using mitimco.assessment.Models;
using mitimco.assessment.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace mitimco.assessment.Tests.Repositories
{
    public class StockRepositoryTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly StockRepository _repository;

        public StockRepositoryTests()
        {
            // Initialize mock HttpMessageHandler and HttpClient
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            var mockMemoryCache = new Mock<IMemoryCache>();

            
            _repository = new StockRepository(_httpClient, new Microsoft.Extensions.Options.OptionsWrapper<ApiSettings>(new ApiSettings
            {
                BaseUrl = "https://www.alphavantage.co/query",  // Base URL for the API
                ApiKey = "dummy_key"  // Dummy API key for testing
            }),
            mockMemoryCache.Object
           );
        }

        [Fact]
        public async Task GetStockDataAsync_ValidData_ReturnsFilteredData()
        {
            // Arrange: Prepare mocked stock data
            var stockData = new AlphaVantageResponse
            {
                TimeSeriesDaily = new Dictionary<string, TimeSeriesDaily>
                {
                    { "2024-11-14", new TimeSeriesDaily { Close = "210.00" } },
                    { "2024-11-13", new TimeSeriesDaily { Close = "208.00" } }
                }
            };

            // Serialize the mock data into a JSON response
            var jsonResponse = JsonConvert.SerializeObject(stockData);

            // Construct the full URL to mock, matching the repository's generated URL
            var mockUrl = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=AAPL&apikey=dummy_key&datatype=json";

            // Set up the mock HttpMessageHandler to return the response for the correct URL
            _mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, mockUrl)
                .ReturnsResponse(System.Net.HttpStatusCode.OK, jsonResponse);

            // Act: Call the method under test
            var result = await _repository.GetStockDataAsync("AAPL", new DateTime(2024, 11, 13), new DateTime(2024, 11, 14));

            // Assert: Verify the returned data
            Assert.Equal(2, result.Count);  // Should return two days of data
            Assert.Equal(210.00m, result[new DateTime(2024, 11, 14)]);  // Verify the value returned for November 14th
        }

        [Fact]
        public async Task GetStockDataAsync_InvalidData_ThrowsException()
        {
            // Arrange: Mock a "NotFound" (404) response for the API
            _mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=AAPL&apikey=dummy_key&datatype=json")
                .ReturnsResponse(System.Net.HttpStatusCode.NotFound);  // Mocking NotFound (404) status

            // Act & Assert: Assert that an InvalidOperationException is thrown
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.GetStockDataAsync("AAPL", new DateTime(2024, 11, 13), new DateTime(2024, 11, 14))
            );

            // Assert: Check that the exception message matches the expected error message
            Assert.Equal("Error fetching data: NotFound", exception.Message);
        }

    }
}
