using System.Linq;

namespace Gratify.Grats.Api.Dto
{
    public class Interaction
    {
        public static Interaction ApproveGrats => new Interaction("approve_grats");

        public static Interaction DenyGrats => new Interaction("deny_grats");

        public static Interaction ForwardGrats => new Interaction("forward_grats");

        public static Interaction AddTeamMember => new Interaction("add_new_team_member");

        public static Interaction RemoveTeamMember => new Interaction("remove_team_member");

        public string Id { get; }

        public Interaction(string id)
        {
            Id = id;
        }

        public bool Is(InteractionPayload interaction) => interaction.Actions.Any(action => action.Value == Id);

        public bool Is(InteractionPayload interaction, out int gratsId)
        {
            var result = Is(interaction, out string id);
            if (id == null)
            {
                gratsId = -1;
            }
            else
            {
                gratsId = int.Parse(id);
            }

            return result;
        }

        public bool Is(InteractionPayload interaction, out string id)
        {
            var anAction = interaction.Actions.FirstOrDefault(action => action.Value.Contains(Id));
            if (anAction != null)
            {
                id = anAction.Value.Split('|')[1];
                return true;
            }
            else
            {
                id = null;
                return false;
            }
        }
    }
}
