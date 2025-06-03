using Newtonsoft.Json;
using Soenneker.Documents.Document.Abstract;
using Soenneker.Extensions.String;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Soenneker.Documents.Document;

/// <inheritdoc cref="IDocument"/>
public abstract class Document : IDocument
{
    // ── Id combines PartitionKey and DocumentId with a colon, or returns DocumentId if they match.
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public string Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            // If they're identical, no need to allocate a new string.
            if (ReferenceEquals(DocumentId, PartitionKey) || DocumentId.Equals(PartitionKey, StringComparison.Ordinal))
                return DocumentId;

            // Allocate exactly the right length (PartitionKey + ':' + DocumentId).
            int pkLen = PartitionKey.Length;
            int idLen = DocumentId.Length;
            return string.Create(pkLen + 1 + idLen, (PartitionKey, DocumentId), static (span, state) =>
            {
                // Copy PartitionKey
                state.PartitionKey.AsSpan().CopyTo(span);
                // Insert colon
                span[state.PartitionKey.Length] = ':';
                // Copy DocumentId
                state.DocumentId.AsSpan().CopyTo(span.Slice(state.PartitionKey.Length + 1));
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (value.IsNullOrWhiteSpace())
                return;

            // Locate the last colon. If none found, both PK and ID = value.
            ReadOnlySpan<char> span = value.AsSpan();
            int lastColon = span.LastIndexOf(':');

            if (lastColon < 0)
            {
                PartitionKey = value;
                DocumentId = value;
                return;
            }

            // PartitionKey = everything before the last colon
            PartitionKey = new string(span.Slice(0, lastColon));
            // DocumentId   = everything after the last colon
            DocumentId = new string(span.Slice(lastColon + 1));
        }
    }

    [Required, JsonPropertyName("id")]
    [JsonProperty("id")]
    public virtual string DocumentId { get; set; } = null!;

    [Required, JsonPropertyName("partitionKey")]
    [JsonProperty("partitionKey")]
    public virtual string PartitionKey { get; set; } = null!;

    [Required, JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("modifiedAt")]
    [JsonProperty("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }
}