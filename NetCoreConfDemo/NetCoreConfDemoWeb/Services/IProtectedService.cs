using Microsoft.Graph;
using NetCoreConfDemoWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreConfDemoWeb.Services
{
    public interface IProtectedService
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();

        Task<string[]> GetTeamsAsync();
    }
}
