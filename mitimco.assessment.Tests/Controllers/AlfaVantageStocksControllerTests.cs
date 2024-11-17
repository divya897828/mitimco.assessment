using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mitimco.assessment.Controllers;
using mitimco.assessment.Services;
using Xunit;
using mitimco.assessment.Repositories;
using mitimco.assessment.Models;

namespace mitimco.assessment.Tests.Controllers
{
    public class AlfaVantageStocksControllerTests
    {
        private readonly Mock<IStockService> _mockStockService;
        private readonly AlfaVantageStocksController _controller;

        public AlfaVantageStocksControllerTests()
        {
            _mockStockService = new Mock<IStockService>();
            _controller = new AlfaVantageStocksController(_mockStockService.Object);
        }

        [Fact]
        public async Task GetReturn_ValidData_ReturnsOk()
        {
            // Arrange
            var ticker = "AAPL";
            var expectedReturns = new List<Models.StockReturn> { new Models.StockReturn() { Return= 0.05m }, new Models.StockReturn() { Return = 0.02m } };
            _mockStockService.Setup(s => s.GetStockReturnsAsync(ticker, It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync(expectedReturns);

            // Act
            var result = await _controller.GetReturn(ticker, null, null);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Models.StockReturn>>(actionResult.Value);
            Assert.Equal(expectedReturns, returnValue);
        }

        [Fact]
        public async Task GetReturn_NoData_ReturnsNotFound()
        {
            // Arrange
            var ticker = "AAPL";
            _mockStockService.Setup(s => s.GetStockReturnsAsync(ticker, It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync((List<StockReturn>)null);

            // Act
            var result = await _controller.GetReturn(ticker, null, null);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAlpha_ValidData_ReturnsOk()
        {
            // Arrange
            var ticker = "AAPL";
            var expectedAlpha = new StockAlpha() { Alpha = 0.03m };
            _mockStockService.Setup(s => s.GetAlphaAsync(ticker, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync(expectedAlpha);

            // Act
            var result = await _controller.GetStockAlpha(ticker, null, null);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var alphaValue = Assert.IsType<StockAlpha>(actionResult.Value);
            Assert.Equal(expectedAlpha.Alpha, alphaValue.Alpha);
        }
    }
}