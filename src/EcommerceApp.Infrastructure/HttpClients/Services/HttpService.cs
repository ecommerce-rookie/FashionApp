using Infrastructure.HttpClients.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.HttpClients.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> UpdateStatusUser(Guid userId, string status)
        {
            var url = $"{Constants.ApiVersion}/users/{userId}/status";
            var json = JsonSerializer.Serialize(status);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);

            return !response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : null;

        }
    }
}
