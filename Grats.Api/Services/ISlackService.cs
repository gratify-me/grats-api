using System.Threading.Tasks;

namespace Gratify.Grats.Api.Services
{
    public interface ISlackService
    {
        Task<string> ReplyToInteraction(string responseUrl, object reply);

        Task<string> SendMessage(object message);

        Task<string> OpenModal(string triggerId, object modal);
    }
}