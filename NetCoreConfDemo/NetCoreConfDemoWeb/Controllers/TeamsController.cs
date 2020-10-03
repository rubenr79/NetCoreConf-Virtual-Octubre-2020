using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Newtonsoft.Json;

namespace NetCoreConfDemoWeb.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ILogger<TeamsController> _logger;

        private readonly GraphServiceClient _graphServiceClient;

        public TeamsController(ILogger<TeamsController> logger,
                          GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        [AuthorizeForScopes(ScopeKeySection = "GraphAPI:Scopes")]
        public async Task<IActionResult> Index()
        {
            var teams = await _graphServiceClient.Me.JoinedTeams.Request().GetAsync();

            ViewData["JoinedTeams"] = teams;            

            return View();
        }

    }
}
