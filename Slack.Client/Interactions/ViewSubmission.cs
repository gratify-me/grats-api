using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Slack.Client.Primitives;
using Slack.Client.Views;

namespace Slack.Client.Interactions
{
    /// <summary>
    /// Type: view_submission
    /// Received when a user submits a view in a modal.
    /// https://api.slack.com/reference/interaction-payloads/views#view_submission
    /// </summary>
    public class ViewSubmission : InteractionPayload
    {
        public const string TypeName = "view_submission";

        public ViewSubmission()
        {
            Type = TypeName;
        }

        /// <summary>
        /// The source view of the modal that the user submitted.
        /// </summary>
        [JsonPropertyName("view")]
        public Modal View { get; set; }

        public T GetStateValue<T>(string key) => GetStateValue<T>(View.State.Values, key.Split("."));

        private T GetStateValue<T>(Dictionary<string, JsonElement> values, IEnumerable<string> keys)
        {
            var first = keys.First();
            var rest = keys.Skip(1);
            var value = values[first].GetRawText();
            if (!rest.Any())
            {
                // TODO: Slack sender en variant av input-elementet som ble fylt ut. Ikke bare en verdi som kan hentes direkte.
                // Har lagt dette til på et par input-typer, men ikke alle.
                return JsonSerializer.Deserialize<T>(value);
            }

            var innerValues = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(value);

            return GetStateValue<T>(innerValues, rest);
        }

        public Guid CorrelationId => Guid.Parse(View.PrivateMetadata);
    }
}
