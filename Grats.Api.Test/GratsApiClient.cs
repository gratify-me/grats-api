using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gratify.Grats.Api.Test
{
    public class GratsApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public GratsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> Grats(Dictionary<string, string> payload)
        {
            var content = new FormUrlEncodedContent(payload);
            using (var response = await _httpClient.PostAsync("/grats", content))
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(contentStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        public async Task<string> Interactive(Dictionary<string, string> payload)
        {
            var content = new FormUrlEncodedContent(payload);
            using (var response = await _httpClient.PostAsync("/interactive", content))
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(contentStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        public async Task<string> Ping()
        {
            using (var response = await _httpClient.GetAsync("ping"))
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(contentStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        public void Dispose() => _httpClient.Dispose();
    }
}