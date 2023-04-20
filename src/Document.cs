using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Soenneker.Documents.Document.Abstract;
using Soenneker.Extensions.String;

namespace Soenneker.Documents.Document;

/// <inheritdoc cref="IDocument"/>
public abstract class Document : IDocument
{
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public string Id
    {
        get
        {
            // If you are getting a null reference exception here, it probably means the document was inserted into Cosmos without an id.
            // It's most likely a bad fake generation. Debug and inspect the item, and then build a new AutoFaker override.

            if (DocumentId.Equals(PartitionKey))
                return DocumentId;

            return string.Join(':', PartitionKey, DocumentId);
        }
        set
        {
            if (value.IsNullOrWhiteSpace())
                return;

            if (value.Contains(':'))
            {
                string[] idSplit = value.Split(':');

                if (idSplit.Length == 2)
                {
                    PartitionKey = idSplit[0];
                    DocumentId = idSplit[1];
                }
                else
                {
                    // These are for combined ids.. essentially the document id is the last in the string and everything before that is the partition key
                    PartitionKey = string.Join(':', idSplit, 0, idSplit.Length - 1);
                    DocumentId = idSplit[^1];
                }
                return;
            }

            DocumentId = value;
            PartitionKey = value;
        }
    }

    [Required, JsonPropertyName("id")]
    [JsonProperty("id")]
    public virtual string DocumentId { get; set; } = default!;

    [Required, JsonPropertyName("partitionKey")]
    [JsonProperty("partitionKey")]
    public virtual string PartitionKey { get; set; } = default!;

    [Required, JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("modifiedAt")]
    [JsonProperty("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }
}