using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Slack.Client.Chat;
using Slack.Client.Chat.Converters;
using Slack.Client.Conversations;
using Slack.Client.Primitives;
using Slack.Client.Views;
using Slack.Client.Views.Converters;

namespace Slack.Client
{
    public class SlackService
    {
        private const string SlackApiUrl = "https://slack.com/api";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public SlackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            _options.Converters.Add(new ViewPayloadConverter());
            _options.Converters.Add(new MessagePayloadConverter());
        }

        public async Task<string> ReplyToInteraction(string responseUrl, MessagePayload reply)
        {
            var json = JsonSerializer.Serialize(reply, _options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(responseUrl, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            return await streamReader.ReadToEndAsync();
        }

        public async Task<Channel> GetAppChannel(string userId)
        {
            var url = $"{SlackApiUrl}/conversations.open";
            var message = new Slack.Client.Conversations.Open
            {
                Users = userId,
            };

            var json = JsonSerializer.Serialize(message, _options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var openResponse = JsonSerializer.Deserialize<OpenResponse>(responseContent);
            if (!openResponse.Ok)
            {
                throw new Exception(openResponse.Error);
            }

            return openResponse.Channel;
        }

        public async Task<ApiResponse> SendMessage(PostMessage message)
        {
            var url = $"{SlackApiUrl}/chat.postMessage";
            var json = JsonSerializer.Serialize(message, _options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<ApiResponse>(contentStream);
        }

        public async Task<ApiResponse> UpdateMessage(UpdateMessage message)
        {
            var url = $"{SlackApiUrl}/chat.update";
            var json = JsonSerializer.Serialize(message, _options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<ApiResponse>(contentStream);
        }

        // https://api.slack.com/surfaces/modals/using
        public async Task<string> OpenModal(string triggerId, ViewPayload view)
        {
            var url = $"{SlackApiUrl}/views.open";
            var payload = new Slack.Client.Views.Open
            {
                TriggerId = triggerId,
                View = view,
            };

            var json = JsonSerializer.Serialize(payload, _options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            return await streamReader.ReadToEndAsync();
        }

        // https://api.slack.com/surfaces/modals/using
        public async Task<string> PublishModal(string userId, ViewPayload view)
        {
            var url = $"{SlackApiUrl}/views.publish";
            var payload = new Publish
            {
                UserId = userId,
                View = view,
            };

            var json = JsonSerializer.Serialize(payload, _options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(url, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            return await streamReader.ReadToEndAsync();
        }

        // TODO: Does this actually get users from all different teams?
        // https://api.slack.com/methods/users.list
        public async IAsyncEnumerable<User> GetUsers()
        {
            var limit = 200;
            var includeLocale = false;
            var cursor = string.Empty;
            do
            {
                var page = await GetUsersPage(limit, includeLocale,  cursor);
                foreach (var user in page.Members)
                {
                    yield return user;
                }

                cursor = page.Metadata.NextCursor;
            }
            while (cursor != string.Empty);
        }

        private async Task<Paginated<User>> GetUsersPage(int limit, bool includeLocale, string cursor)
        {
            var url = $"{SlackApiUrl}/users.list?include_locale={includeLocale}&limit={limit}&cursor={WebUtility.UrlEncode(cursor)}";
            using var response = await _httpClient.GetAsync(url);
            var contentStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<Paginated<User>>(contentStream);
        }
    }
}