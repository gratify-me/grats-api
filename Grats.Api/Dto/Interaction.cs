using System.Linq;

namespace Gratify.Grats.Api.Dto
{
    public class Interaction
    {
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
