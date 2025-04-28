using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartEstate.Routing.Models;
using SmartEstate.Routing.Services.Interfaces;

namespace SmartEstate.Routing.Services
{
    public class RoutingService : IRoutingService
    {
        private readonly HttpClient _httpClient;
        private readonly GraphHopperOptions _options;

        public RoutingService(HttpClient httpClient, IOptions<GraphHopperOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }

        public async Task<RoutingResponse> GetWalkingRouteAsync(RoutingRequest request)
        {
            var url = $"{_options.RouteEndpoint}?" +
                      $"point={request.OriginLatitude.ToString(CultureInfo.InvariantCulture)}," +
                      $"{request.OriginLongitude.ToString(CultureInfo.InvariantCulture)}" +
                      $"&point={request.DestinationLatitude.ToString(CultureInfo.InvariantCulture)}," +
                      $"{request.DestinationLongitude.ToString(CultureInfo.InvariantCulture)}" +
                      $"&vehicle=foot" +
                      $"&key={_options.ApiKey}" +
                      "&points_encoded=false";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
    
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoutingResponse>(content);
        }
    }
}