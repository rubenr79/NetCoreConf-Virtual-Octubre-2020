using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using NetCoreConfDemoWeb.Services;

namespace NetCoreConfDemoWeb.Controllers
{
    public class ProtectedApiController : Controller
    {
        private IProtectedService _service;

        public ProtectedApiController(IProtectedService service)
        {
            _service = service;
        }

        
        [AuthorizeForScopes(ScopeKeySection = "CustomAPI:Scopes")]
        public async Task<ActionResult> Index()
        {
            return View(await _service.GetWeatherForecastAsync());
        }

        [AuthorizeForScopes(ScopeKeySection = "CustomAPI:Scopes")]
        public async Task<ActionResult> Teams()
        {
            return View(await _service.GetTeamsAsync());
        }


    }
}
