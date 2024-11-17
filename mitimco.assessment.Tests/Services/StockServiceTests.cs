using Moq;
using mitimco.assessment.Repositories;
using mitimco.assessment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace mitimco.assessment.Tests.Services
{
    public class StockServiceTests
    {
        private readonly Mock<IStockRepository> _mockStockRepository;
        private readonly StockService _service;

        public StockServiceTests()
        {
            _mockStockRepository = new Mock<IStockRepository>();
            _service = new StockService(_mockStockRepository.Object);
        }

        [Fact]
        public async Task GetStockReturnsAsync_ValidData_ReturnsCalculatedReturns()
        {	
 
            // Arrange
            var ticker = "AAPL";
            var stockData = new Dictionary<DateTime, decimal>
            {
                { new DateTime(2024, 11, 14), 210.00m },
                { new DateTime(2024, 11, 13), 208.00m }
            };
            _mockStockRepository.Setup(r => r.GetStockDataAsync(ticker, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                                .ReturnsAsync(stockData);

            // Act
            var result = await _service.GetStockReturnsAsync(ticker, null, null);

            // Assert
            Assert.Equal(0.9615m, result.Select(x=>x.Return).First(), 4);  // Return between 11/13 and 11/14
        }

        [Fact]
        public async Task GetAlpha_ValidData_ReturnsCalculatedAlpha()
        {
            // Arrange
            var ticker = "AAPL";
            var stockData = new Dictionary<DateTime, decimal>
            {
                { new DateTime(2024, 11, 14), 210.00m },
                { new DateTime(2024, 11, 13), 208.00m }
            };
            var benchmarkData = new Dictionary<DateTime, decimal>
            {
                { new DateTime(2024, 11, 14), 411.00m },
                { new DateTime(2024, 11, 13), 410.00m }
            };

            _mockStockRepository.Setup(r => r.GetStockDataAsync(ticker, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                                .ReturnsAsync(stockData);
            _mockStockRepository.Setup(r => r.GetStockDataAsync("SPY", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                                .ReturnsAsync(benchmarkData);

            // Act
            var result = await _service.GetAlphaAsync(ticker: ticker,benchmark: "SPY", null, null);

            // Assert
            Assert.Equal(0.7176m, result.Alpha, 4);  // Alpha calculation
        }
    }
}
