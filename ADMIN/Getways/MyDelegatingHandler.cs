using ADMIN.Services.Exceptions;
using ADMIN.Services.IdentityService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ADMIN.Getways
{
    public class MyDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MyDelegatingHandler> _logger;
        private readonly IIdentityService _identityService;
        public MyDelegatingHandler(IHttpContextAccessor httpContextAccessor, ILogger<MyDelegatingHandler> logger, IIdentityService identityService)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _identityService = identityService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage httpResponseMessage;
            //try
            //{
            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            string refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //if (string.IsNullOrEmpty(accessToken))
            //{
            //    throw new Exception($"Access token is missing for the request {request.RequestUri}");
            //}
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var headers = _httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("X-Correlation-ID") && !string.IsNullOrEmpty(headers["X-Correlation-ID"]))
            {
                request.Headers.Add("X-Correlation-ID", headers["X-Correlation-ID"].ToString());
            }

            httpResponseMessage = await base.SendAsync(request, cancellationToken);

            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var tokenResponse = await _identityService.GetTokensByRefresh();
                if (!string.IsNullOrEmpty(tokenResponse.Token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                    headers = _httpContextAccessor.HttpContext.Request.Headers;
                    if (headers.ContainsKey("X-Correlation-ID") && !string.IsNullOrEmpty(headers["X-Correlation-ID"]))
                    {
                        request.Headers.Add("X-Correlation-ID", headers["X-Correlation-ID"].ToString());
                    }
                    httpResponseMessage = await base.SendAsync(request, cancellationToken);
                }
            }
            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //_logger.LogError("Failed to run http query {RequestUri}", request.RequestUri);
                throw new UnauthorizedException();
            }
            httpResponseMessage.EnsureSuccessStatusCode();
            //}
            //catch (Exception ex)
            //{
            //    //_logger.LogError(ex, "Failed to run http query {RequestUri}", request.RequestUri);
            //    throw new UnauthorizedException();
            //}
            return httpResponseMessage;
        }
    }
}
