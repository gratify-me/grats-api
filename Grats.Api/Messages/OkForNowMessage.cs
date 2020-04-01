using Slack.Client.Chat;
using Slack.Client.Interactions;

namespace Gratify.Grats.Api.Messages
{
    public class OkForNowMessage
    {
        public MessagePayload Draw() =>
            new ResponseMessage("That's OK for now (but in the future you might have to do more to deny grats 😉)");
    }
}
