using System.Threading.Tasks;
using Slack.Client.Chat;
using Slack.Client.Primitives;
using Slack.Client.Views;

namespace Gratify.Api.Services
{
    public interface ISlackService
    {
        Task<string> ReplyToInteraction(string responseUrl, MessagePayload reply);

        Task<Channel> GetAppChannel(string userId);

        Task<string> SendMessage(PostMessage message);

        Task<string> OpenModal(string triggerId, ViewPayload modal);

        Task<string> PublishModal(string triggerId, ViewPayload modal);
    }
}