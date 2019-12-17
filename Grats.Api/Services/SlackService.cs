using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gratify.Grats.Api.Services
{
    public class SlackService : ISlackService
    {
        private readonly HttpClient _httpClient;

        public SlackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ReplyToInteraction(string responseUrl, string reply)
        {
            var content = new StringContent(reply);
            using (var response = await _httpClient.PostAsync("/grats", content))
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(contentStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }
    }
}