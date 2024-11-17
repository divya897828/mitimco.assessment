using System;
namespace mitimco.assessment.Repositories
{
    public interface IStockRepository
    {
        Task<Dictionary<DateTime, decimal>> GetStockDataAsync(string ticker, DateTime startDate, DateTime endDate);
    }
}

