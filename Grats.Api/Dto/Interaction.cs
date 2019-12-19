using System.Linq;

namespace Gratify.Grats.Api.Dto
{
    public class Interaction
    {
        public static Interaction SendGrats => new Interaction("send_grats");

        public static Interaction CancelSendGrats => new Interaction("cancel_send_grats");

        public static Interaction ApproveGrats => new Interaction("approve_grats");

        public static Interaction DenyGrats => new Interaction("deny_grats");

        public string Id { get; }

        public Interaction(string id)
        {
            Id = id;
        }

        public bool Is(InteractionPayload interaction) => interaction.Actions.Any(action => action.Value == Id);
    }
}
