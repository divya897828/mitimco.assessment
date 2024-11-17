using Microsoft.AspNetCore.Mvc;
using mitimco.assessment.Models;
using mitimco.assessment.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace mitimco.assessment.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AlfaVantageStocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public AlfaVantageStocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        /// <summary>
        /// Gets the stock returns for the specified ticker symbol within a date range.
        /// </summary>
        /// <param name="ticker">The stock ticker symbol (e.g., "AAPL")</param>
        /// <param name="fromDate">The start date for the period in yyyy-MM-dd format (optional, default is year-to-date)</param>
        /// <param name="toDate">The end date for the period in yyyy-MM-dd format (optional, default is today's date)</param>
        /// <returns>The calculated stock returns for the specified period</returns>
        /// <response code="200">Returns the calculated stock returns in percentage</response>
        /// <response code="404">If stock data is not found or the calculation fails</response>
        [HttpGet("GetReturn")]
        [ProducesResponseType(typeof(IEnumerable<StockReturn>), 200)] // Response Type for Stock Returns
        [ProducesResponseType(404)] // If not found
        public async Task<IActionResult> GetReturn(string ticker, [FromQuery] string fromDate = null, [FromQuery] string toDate = null)
        {
            var result = await _stockService.GetStockReturnsAsync(ticker, fromDate, toDate);
            if (result == null)
                return NotFound("Stock data not found.");

            return Ok(result);
        }

        /// <summary>
        /// Gets the alpha value for the stock compared to a benchmark
        /// </summary>
        /// <param name="ticker">The stock ticker symbol (e.g., "AAPL")</param>
        /// <param name="benchmark">The benchmark symbol (optional, defaults to "SPY")</param>
        /// <param name="fromDate">The start date for the period (optional, default is year-to-date) in yyyy-MM-dd format</param>
        /// <param name="toDate">The end date for the period (optional, default is today's date) in yyyy-MM-dd format</param>
        /// <returns>The calculated alpha value for the stock</returns>
        /// <response code="200">Returns the calculated alpha value in percentage</response>
        /// <response code="404">If the alpha calculation fails or no data is available</response>
        [HttpGet("GetStockAlpha")]
        [ProducesResponseType(typeof(StockAlpha), 200)] // Response Type for Stock Alpha
        [ProducesResponseType(404)] // If not found
        public async Task<IActionResult> GetStockAlpha(string ticker, string benchmark = "SPY", [FromQuery] string fromDate = null, [FromQuery] string toDate = null)
        {
            var result = await _stockService.GetAlphaAsync(ticker, benchmark, fromDate, toDate);
            if (result == null)
                return NotFound("Alpha calculation failed.");

            return Ok(result);
        }
    }
}