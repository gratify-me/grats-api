namespace Gratify.Grats.Api.Dto
{
    public class Submission
    {
        public static Submission SendGrats => new Submission("send-grats-modal");

        public static Submission AddTeamMember => new Submission("add_team_member_modal");

        public static Submission ForwardGrats => new Submission("forward-grats-modal");

        public string Id { get; }

        public Submission(string id)
        {
            Id = id;
        }

        public bool Is(GratsViewSubmission submission) => Id == submission.View.CallbackId;

        public bool Is(GratsViewSubmission submission, out int draftId)
        {
            var ids = submission.View.CallbackId.Split('|');
            var modalId = ids[0];
            draftId = int.Parse(ids[1]);

            return modalId == Id;
        }
    }
}