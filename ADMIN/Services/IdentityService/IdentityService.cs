using ADMIN.Getways.Interface;
using ADMIN.Models.CustomModels.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ADMIN.Services.IdentityService
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IResponseGetaway _responseGetaway;
        private readonly IHttpClientFactory _httpClientFactory;

        public IdentityService(IHttpContextAccessor httpContextAccessor, IResponseGetaway responseGetaway, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _responseGetaway = responseGetaway;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JWTModel> GetTokensByRefresh()
        {
            string refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            JwtRefresh jwtRefresh = new()
            {
                refreshToken = refreshToken
            };
            JWTModel responseModel = new();

            if (!string.IsNullOrEmpty(refreshToken))
            {

                var client = _httpClientFactory.CreateClient("refreshService");
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(client.BaseAddress + "api/Login/refreshToken"),
                    Content = new StringContent(JsonConvert.SerializeObject(jwtRefresh, Formatting.Indented), Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);

                if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                {
                    responseModel = JsonConvert.DeserializeObject<JWTModel>(await response.Content.ReadAsStringAsync());
                }
                AuthenticationProperties authenticationProperties = new();

                authenticationProperties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, responseModel.Token);
                authenticationProperties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, responseModel.RefreshToken);
            }
            return responseModel;
        }
    }
}
