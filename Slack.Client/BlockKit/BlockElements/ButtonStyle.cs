namespace Slack.Client.BlockKit.BlockElements
{
    /// <summary>
    /// Decorates buttons with alternative visual color schemes. Use this option with restraint.
    /// If you don't include this field, the default button style will be used.
    /// </summary>
    public enum ButtonStyle
    {
        /// <summary>
        /// Primary gives buttons a green outline and text, ideal for affirmation or confirmation actions.
        /// Primary should only be used for one button within a set.
        /// </summary>
        Primary = 0,

        /// <summary>
        /// Danger gives buttons a red outline and text, and should be used when the action is destructive.
        /// Use danger even more sparingly than primary.
        /// </summary>
        Danger = 1,
    }
}
