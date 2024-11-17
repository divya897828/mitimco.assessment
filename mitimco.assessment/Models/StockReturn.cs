using System;
namespace mitimco.assessment.Models
{
    /// <summary>
    /// Represents the calculated stock return for a given period.
    /// </summary>
    public class StockReturn
    {
        /// <summary>
        /// The date for which the return is calculated.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The calculated return value for the stock on that date, in percentage.<br/>
        /// Calculated Return = (((Price at TimeT) − (Price at TimeT−1))/( Price at TimeT−1 )) x 100 == return in percentage <br />                     
        ///Price at Time T = The price of the asset at the current period(e.g., today's closing price).<br />
        ///Price at Time T - 1 The price of the asset at the previous period (e.g., the previous day's closing price).<br />
        /// </summary>
        public decimal Return { get; set; }
    }
}

