using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using NetCoreConfDemoWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NetCoreConfDemoWeb.Services
{
    public static class WeatherForecastServiceExtensions
    {
        public static void AddWeatherForecastService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IProtectedService, ProtectedService>();
        }
    }

    public class ProtectedService : IProtectedService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _httpClient;
        private readonly string _scope = string.Empty;
        private readonly string _baseAddress = string.Empty;
        private readonly ITokenAcquisition _tokenAcquisition;

        public ProtectedService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _scope = configuration["CustomAPI:Scopes"];
            _baseAddress = configuration["CustomAPI:BaseAddress"];
        }


        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync($"{ _baseAddress}/api/weatherforecast");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<WeatherForecast> forecast = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(content);

                return forecast;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<string[]> GetTeamsAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await _httpClient.GetAsync($"{ _baseAddress}/api/teams");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var teams = JsonConvert.DeserializeObject<string[]>(content);

                return teams;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _scope });
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


    }
}
