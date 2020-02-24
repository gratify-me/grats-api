using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gratify.Grats.Api.Dto;

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
            var json = JsonSerializer.Serialize<object>(reply, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(responseUrl, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            return await streamReader.ReadToEndAsync();
        }

        public async Task<Channel> GetAppChannel(string userId)
        {
            var url = $"{SlackApiUrl}/conversations.open";
            var message = new
            {
                users = userId,
            };

            var json = JsonSerializer.Serialize<object>(message, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var openResponse = JsonSerializer.Deserialize<ConversationsOpenResponse>(responseContent);
            if (!openResponse.Ok)
            {
                throw new Exception(openResponse.Error);
            }

            return openResponse.Channel;
        }

        public async Task<string> SendMessage(object message)
        {
            var url = $"{SlackApiUrl}/chat.postMessage";
            var json = JsonSerializer.Serialize<object>(message, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            return await streamReader.ReadToEndAsync();
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

            var json = JsonSerializer.Serialize<object>(payload, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            return await streamReader.ReadToEndAsync();
        }

        // https://api.slack.com/surfaces/modals/using
        public async Task<string> PublishModal(string userId, object modal)
        {
            var url = $"{SlackApiUrl}/views.publish";
            var payload = new
            {
                user_id = userId,
                view = modal,
            };

            var json = JsonSerializer.Serialize<object>(payload, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            return await streamReader.ReadToEndAsync();
        }
    }
}