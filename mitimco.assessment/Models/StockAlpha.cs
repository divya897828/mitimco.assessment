using System;
namespace mitimco.assessment.Models
{
    /// <summary>
    /// Represents the calculated alpha value for a stock compared to a benchmark.
    /// </summary>
    public class StockAlpha
    {
        /// <summary>
        /// The calculated alpha value for the stock, in percentage.<br />
        /// Simplified Alpha Calculation = average stock return - average benchmark return
        /// </summary>
        public decimal Alpha { get; set; }
    }

}

