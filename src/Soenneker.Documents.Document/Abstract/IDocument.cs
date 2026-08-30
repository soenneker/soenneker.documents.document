using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Soenneker.Entities.Entity.Abstract;

namespace Soenneker.Documents.Document.Abstract;

/// <summary>
/// Defines storage identity and timestamp metadata for a document.
/// </summary>
public interface IDocument
{
    /// <summary>
    /// Gets or sets the internal composite identifier built from the partition key and document identifier.
    /// </summary>
    /// <remarks>
    /// The getter joins differing non-empty values as <c>PartitionKey:DocumentId</c>. The setter splits at the last colon; a value without a colon assigns both keys. Null or whitespace input is ignored.
    /// </remarks>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    string? Id { get; set; }

    /// <summary>
    /// Gets or sets the storage document identifier serialized as <c>id</c>.
    /// </summary>
    [Required, JsonPropertyName("id")]
    [JsonProperty("id")]
    string? DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the storage partition key serialized as <c>partitionKey</c>.
    /// </summary>
    [Required, JsonPropertyName("partitionKey")]
    [JsonProperty("partitionKey")]
    string? PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    [Required, JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the most recent modification timestamp, if known.
    /// </summary>
    [JsonPropertyName("modifiedAt")]
    [JsonProperty("modifiedAt")]
    DateTimeOffset? ModifiedAt { get; set; }
}
