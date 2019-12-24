using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gratify.Grats.Api.Services
{
    public class SlackService : ISlackService
    {
        private const string SlackApiUrl = "https://slack.com/api";
        private readonly HttpClient _httpClient;

        public SlackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ReplyToInteraction(string responseUrl, object reply)
        {
            var json = JsonConvert.SerializeObject(reply);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var response = await _httpClient.PostAsync(responseUrl, content))
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(contentStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        public async Task<string> SendMessage(object message)
        {
            var url = $"{SlackApiUrl}/chat.postMessage";
            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var response = await _httpClient.PostAsync(url, content))
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                using (var streamReader = new StreamReader(contentStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        // https://api.slack.com/surfaces/modals/using
        public async Task<string> OpenModal(string triggerId, object modal)
        {
            var url = $"{SlackApiUrl}/views.open";
            var payload = new
            {
                trigger_id = triggerId,
                view = modal,
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var response = await _httpClient.PostAsync(url, content))
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