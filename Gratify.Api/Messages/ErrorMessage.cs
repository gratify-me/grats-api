using Slack.Client.Chat;

namespace Gratify.Api.Messages
{
    public class ErrorMessage
    {
        public MessagePayload Draw() =>
            new ResponseMessage("Whoops! Looks like something went wrong 💩");
    }
}
