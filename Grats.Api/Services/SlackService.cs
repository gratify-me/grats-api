using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gratify.Grats.Api.Services
{
    public class SlackService : ISlackService
    {
        private readonly HttpClient _httpClient;

        public SlackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ReplyToInteraction(string responseUrl, object reply)
        {
            var json = JsonConvert.SerializeObject(reply);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
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