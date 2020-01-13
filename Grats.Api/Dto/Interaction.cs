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

        public bool Is(InteractionPayload interaction, out int gratsId)
        {
            var anAction = interaction.Actions.FirstOrDefault(action => action.Value.Contains(Id));
            if (anAction != null)
            {
                gratsId = int.Parse(anAction.Value.Split('|')[1]);
                return true;
            }
            else
            {
                gratsId = -1;
                return false;
            }
        }
    }
}
