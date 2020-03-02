using System.Threading.Tasks;
using Gratify.Grats.Api.Dto;
using Slack.Client.Chat;
using Slack.Client.Views;

namespace Gratify.Grats.Api.Services
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