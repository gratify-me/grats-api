using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Slack.Client.Primitives
{
    /// <summary>
    /// For larger collections like channel and user lists, Slack API methods return results using a cursor-based pagination.
    /// A cursor-paginated method returns two things: a portion of the total set of results, and a cursor that points to the next portion of the results.
    ///
    /// https://api.slack.com/docs/pagination
    /// </summary>
    public class Paginated<T>
    {
        /// <summary>
        /// Determines whether or not the request was successful.
        /// </summary>
        [Required]
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        /// <summary>
        /// A page of items in the collection.
        /// </summary>
        [Required]
        [JsonPropertyName("members")]
        public T[] Members { get; set; }

        /// <summary>
        /// Metadata containing a reference to the cursor pointing to the next page.
        /// </summary>
        [Required]
        [JsonPropertyName("response_metadata")]
        public ResponseMetadata Metadata { get; set; }
    }

    public class ResponseMetadata
    {
        /// <summary>
        /// A string pointing at the next page of results. To retrieve the next page of results,
        /// provide this value as the cursor parameter to the paginated method.
        /// When you reach the end of a paginated collection, the next_cursor becomes but an empty string.
        /// </summary>
        [Required]
        [JsonPropertyName("next_cursor")]
        public string NextCursor { get; set; }
    }
}
