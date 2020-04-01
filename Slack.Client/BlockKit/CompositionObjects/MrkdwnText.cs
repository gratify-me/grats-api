using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.CompositionObjects
{
    /// <summary>
    /// An object containing some text formatted as mrkdwn,
    /// our proprietary textual markup that's just different enough from Markdown to frustrate you.
    /// </summary>
    public class MrkdwnText : TextObject
    {
        public const string TypeName = "mrkdwn";

        public MrkdwnText()
        {
            Type = TypeName;
        }

        /// <summary>
        /// When set to false (as is default) URLs will be auto-converted into links,
        /// conversation names will be link-ified, and certain mentions will be automatically parsed.
        /// Using a value of true will skip any preprocessing of this nature,
        /// although you can still include manual parsing strings.
        /// </summary>
        [JsonPropertyName("verbatim")]
        public bool? Verbatim { get; set; }
    }
}
