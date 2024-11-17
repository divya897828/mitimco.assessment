using System;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using mitimco.assessment.Models;
using mitimco.assessment.Repositories;

namespace mitimco.assessment.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<List<StockReturn>> GetStockReturnsAsync(string ticker, string fromDate = null, string toDate = null)
        {
            DateTime startDate = string.IsNullOrEmpty(fromDate) ? DateTime.Now.AddYears(-1) : DateTime.Parse(fromDate);
            DateTime endDate = string.IsNullOrEmpty(toDate) ? DateTime.Now : DateTime.Parse(toDate);

            if (endDate < startDate)
                throw new ArgumentException("ToDate must be greater than or equal to FromDate");

            var stockData = await _stockRepository.GetStockDataAsync(ticker, startDate, endDate);
            if (stockData == null)
                return null;
            
            return CalculateDailyReturns(stockData);
        }

        //Simplified Alpha Calculation = average stock return - average benchmark return
        public async Task<StockAlpha> GetAlphaAsync(string ticker, string benchmark, string fromDate = null, string toDate = null)
        {
            DateTime startDate = string.IsNullOrEmpty(fromDate) ? DateTime.Now.AddYears(-1) : DateTime.Parse(fromDate);
            DateTime endDate = string.IsNullOrEmpty(toDate) ? DateTime.Now : DateTime.Parse(toDate);

            var stockData = await _stockRepository.GetStockDataAsync(ticker, startDate, endDate);
            var benchmarkData = await _stockRepository.GetStockDataAsync(benchmark, startDate, endDate); // Using SPY as benchmark

            if (stockData == null || benchmarkData == null)
                return new StockAlpha() { Alpha = 0 }; // Handle error or return default alpha value

            var stockReturns = CalculateDailyReturns(stockData);
            var benchmarkReturns = CalculateDailyReturns(benchmarkData);

            return CalculateAlpha(stockReturns, benchmarkReturns);
        }

        //Calculated Return = (Price at TimeT) − (Price at TimeT−1)
        //                     ------------------------------------- x 100 == return in percentage
        //                       ( Price at TimeT−1 )
        //Price at Time T = The price of the asset at the current period(e.g., today's closing price).
        //Price at Time T - 1 The price of the asset at the previous period (e.g., the previous day's closing price).​
        private List<StockReturn> CalculateDailyReturns(Dictionary<DateTime, decimal> stockData)
        {
            var returns = new List<StockReturn>();
            var stockPrices = stockData.OrderBy(d => d.Key).ToList();

            for (int i = 1; i < stockPrices.Count; i++)
            {
                decimal returnValue = (stockPrices[i].Value - stockPrices[i - 1].Value) / stockPrices[i - 1].Value;
                returns.Add(new StockReturn(){
                     Date= stockPrices[i].Key,
                     Return =returnValue*100
                });
            }
            
            return returns;
        }

        private StockAlpha CalculateAlpha(List<StockReturn> stockReturns, List<StockReturn> benchmarkReturns)
        { 
            var averageStockReturn = stockReturns.Select(x=>x.Return).Average();
            var averageBenchmarkReturn = benchmarkReturns.Select(x => x.Return).Average();
            
            return new StockAlpha()
            {
                Alpha = averageStockReturn - averageBenchmarkReturn
            };
        }
    }
}