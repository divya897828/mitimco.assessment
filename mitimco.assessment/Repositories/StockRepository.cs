using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using mitimco.assessment.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace mitimco.assessment.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);
        public StockRepository(HttpClient httpClient, IOptions<ApiSettings> apiSettings, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _cache = cache;
        }

        public async Task<Dictionary<DateTime, decimal>> GetStockDataAsync(string ticker, DateTime startDate, DateTime endDate)
        {
            Dictionary<string, TimeSeriesDaily> apiResponse = new Dictionary<string, TimeSeriesDaily>();
            var cacheKey = $"stock_data_{ticker}_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}";
            if (_cache.TryGetValue(cacheKey, out Dictionary<string, TimeSeriesDaily> cachedData))
            {
                Console.WriteLine("Cache hit for key: " + cacheKey);
                apiResponse = cachedData;
            }
            else {
                string url = $"{_apiSettings.BaseUrl}?function=TIME_SERIES_DAILY&symbol={ticker}&apikey={_apiSettings.ApiKey}&datatype=json";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // Throw InvalidOperationException if the response status is not successful
                    throw new InvalidOperationException($"Error fetching data: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiData = JsonConvert.DeserializeObject<AlphaVantageResponse>(responseContent);

                apiResponse = apiData?.TimeSeriesDaily;
                _cache.Set(cacheKey, apiResponse, _cacheDuration);
            }

            if (apiResponse == null)
                return null;

            // Filter data by date range
            var filteredData = apiResponse
                .Where(d => DateTime.TryParse(d.Key, out DateTime date) && date >= startDate && date <= endDate)
                .ToDictionary(d => DateTime.Parse(d.Key), d => decimal.Parse(d.Value.Close));

            
            return filteredData;
        }

    }
}