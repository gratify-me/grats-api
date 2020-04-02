using System.Threading.Tasks;
using Gratify.Api.Database.Entities;
using Gratify.Api.Services;
using Slack.Client.BlockKit.BlockElements;
using Slack.Client.BlockKit.LayoutBlocks;
using Slack.Client.Chat;
using Slack.Client.Interactions;

namespace Gratify.Api.Messages
{
    public class RequestGratsReview
    {
        private readonly string _approve = $"{typeof(RequestGratsReview)}.Approve";
        private readonly string _deny = $"{typeof(RequestGratsReview)}.Deny";
        private readonly string _forward = $"{typeof(RequestGratsReview)}.Forward";
        private readonly InteractionService _interactions;

        public RequestGratsReview(InteractionService interactions)
        {
            _interactions = interactions;
        }

        public PostMessage Message(Review review) =>
            new PostMessage(
                text: $"<@{review.Grats.Draft.Author}> wants to send grats to <@{review.Grats.Recipient}>!",
                blocks: new LayoutBlock[]
                {
                    new Section(
                        id: "AuthorAndRecipient",
                        text: $"<@{review.Grats.Draft.Author}> wants to send grats to <@{review.Grats.Recipient}>!"),

                    new Section(
                        id: "Challenge",
                        text: $"*Challenge:*\n_{review.Grats.Challenge}_"),

                    new Section(
                        id: "Action",
                        text: $"*Action:*\n_{review.Grats.Action}_"),

                    new Section(
                        id: "Result",
                        text: $"*Result:*\n_{review.Grats.Result}_"),

                    new Actions(
                        id: "ApproveDenyOrForward",
                        elements: new Button[]
                        {
                            new Button(
                                id: _approve,
                                correlationId: review.CorrelationId,
                                text: "Approve",
                                style: ButtonStyle.Primary),

                            new Button(
                                id: _deny,
                                correlationId: review.CorrelationId,
                                text: "Deny",
                                style: ButtonStyle.Danger),

                            new Button(
                                id: _forward,
                                correlationId: review.CorrelationId,
                                text: "Forward"),
                        })
                });

        public async Task OnSubmit(Action action, string responseUrl, string triggerId)
        {
            if (action.ActionId == _approve)
            {
                var approval = new Approval(
                    correlationId: action.CorrelationId,
                    approvedAt: System.DateTime.UtcNow);

                await _interactions.ApproveGrats(
                    approval: approval,
                    respondWith: new ResponseMessage("Grats approved ✔"),
                    responseUrl: responseUrl);
            }
            else if (action.ActionId == _deny)
            {
                var denial = new Denial(
                    correlationId: action.CorrelationId,
                    reason: "To be implemented",
                    deniedAt: System.DateTime.UtcNow);

                await _interactions.DenyGrats(
                    denial: denial,
                    respondWith: new ResponseMessage("That's OK for now (but in the future you might have to do more to deny grats 😉)"),
                    responseUrl: responseUrl);
            }
            else if (action.ActionId == _forward)
            {
                await _interactions.OpenForwardReview(action.CorrelationId, triggerId);
            }
        }
    }
}
