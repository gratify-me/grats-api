﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.BlockKit.LayoutBlocks
{
    /// <summary>
    /// A block that is used to hold interactive elements.
    /// </summary>
    public class Actions : LayoutBlock
    {
        public Actions()
        {
            Type = TypeName;
        }

        public Actions(string id, object[] elements)
        {
            Type = TypeName;
            BlockId = id;
            Elements = elements;
        }

        public const string TypeName = "actions";

        /// <summary>
        /// An array of interactive element objects - buttons, select menus, overflow menus, or date pickers.
        /// There is a maximum of 5 elements in each action block.
        /// </summary>
        [Required]
        [JsonPropertyName("elements")]
        public object[] Elements { get; set; }
    }
}
