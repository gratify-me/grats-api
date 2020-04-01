using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// A content divider, like an <hr>, to split up different blocks inside of a message.
    /// The divider block is nice and neat, requiring only a type.
    /// Available in surfaces: Modals, Messages, Home tabs
    /// </summary>
    public class Divider : LayoutBlock
    {
        public Divider()
        {
            Type = TypeName;
        }

        public const string TypeName = "divider";
    }
}
