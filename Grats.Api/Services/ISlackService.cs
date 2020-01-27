using System.Threading.Tasks;
using Gratify.Grats.Api.Dto;

namespace Gratify.Grats.Api.Services
{
    public interface ISlackService
    {
        Task<string> ReplyToInteraction(string responseUrl, object reply);

        Task<Channel> GetAppChannel(User user);

        Task<string> SendMessage(object message);

        Task<string> OpenModal(string triggerId, object modal);

        Task<string> PublishModal(string triggerId, object modal);
    }
}