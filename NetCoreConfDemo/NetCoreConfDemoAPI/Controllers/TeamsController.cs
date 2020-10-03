using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace NetCoreConfDemoAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ILogger<TeamsController> _logger;

        private readonly ITokenAcquisition _tokenAcquisition;

        private readonly GraphServiceClient _graphServiceClient;

        public TeamsController(ILogger<TeamsController> logger,
                ITokenAcquisition tokenAcquisition,
                GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
            _graphServiceClient = graphServiceClient;
        }

        [AuthorizeForScopes(ScopeKeySection = "GraphAPI:Scopes")]
        [HttpGet]
        public async Task<string[]> Get()
        {
            try
            {
                var teams = await _graphServiceClient.Me.JoinedTeams.Request().GetAsync();

                return teams.Select(t=>t.DisplayName).ToArray();
            }
            catch (MsalException ex)
            {
                HttpContext.Response.ContentType = "text/plain";
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await HttpContext.Response.WriteAsync("An authentication error occurred while acquiring a token for downstream API\n" + ex.ErrorCode + "\n" + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is MicrosoftIdentityWebChallengeUserException challengeException)
                {
                    await _tokenAcquisition.ReplyForbiddenWithWwwAuthenticateHeaderAsync("user.read".Split(" "),
                        challengeException.MsalUiRequiredException);
                }
                else
                {
                    HttpContext.Response.ContentType = "text/plain";
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await HttpContext.Response.WriteAsync("An error occurred while calling the downstream API\n" + ex.Message);
                }
                return null;
            }
        }
    }
}
