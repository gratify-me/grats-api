using System.Threading.Tasks;

namespace Gratify.Grats.Api.Services
{
    public interface ISlackService
    {
        Task<string> ReplyToInteraction(string responseUrl, string reply);
    }
}