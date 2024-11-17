using System;
using mitimco.assessment.Models;

namespace mitimco.assessment.Services
{
    public interface IStockService
    {
        Task<List<StockReturn>> GetStockReturnsAsync(string ticker, string fromDate = null, string toDate = null);
        Task<StockAlpha> GetAlphaAsync(string ticker, string benchmark, string fromDate = null, string toDate = null);
    }
}

