using ADMIN.Getways.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ADMIN.Getways
{
    public class ResponseGetaway : IResponseGetaway
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly IConfiguration _configuration;

        public ResponseGetaway(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<string> GetTAsync(string url)
        {
            var client = _clientFactory.CreateClient("recipeService");
            //client.BaseAddress = new Uri(_configuration["Service:BaseAddress"]);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(client.BaseAddress + url),
            };
            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SendStringAsync<T>(List<KeyValuePair<string, string>> queryParams, string url, HttpMethod method)
        {
            var client = _clientFactory.CreateClient("recipeService");

            Dictionary<string, string> values = new();
            if (queryParams is not null)
            {
                foreach (var item in queryParams)
                {
                    values.Add(item.Key, item.Value);
                }
            }

            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(client.BaseAddress + url),
                Content = new FormUrlEncodedContent(values)
            };
            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> SendTAsync<T>(T t, string url, HttpMethod method)
        {
            var client = _clientFactory.CreateClient("recipeService");
            //client.BaseAddress = new Uri(_configuration["Service:BaseAddress"]);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(client.BaseAddress + url),
                Content = new StringContent(JsonConvert.SerializeObject(t, Formatting.Indented), Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
