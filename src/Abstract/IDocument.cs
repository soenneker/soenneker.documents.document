﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Soenneker.Entities.Entity.Abstract;

namespace Soenneker.Documents.Document.Abstract;

/// <summary>
/// The base document type providing a building block for storage objects <para/>
/// Documents may or may not have their own separate containers. They are not tied to only one repository. <para/>
/// A parent document may have children documents exist on them.
/// </summary>
public interface IDocument
{
    /// <summary>
    /// This is unused by CosmosDb, is for internal identification <para/>
    /// PartitionKey:DocumentId construction... unless DocumentId = PartitionId (then it's only one id)
    /// </summary>
    /// <remarks>
    /// During GET it builds the return value from joining PartitionKey and DocumentId (PartitionKey:DocumentId) <para/>
    /// During SET it sets the DocumentId and PartitionKey of the document.
    /// </remarks>
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    string Id { get; set; }

    /// <summary>
    /// Maps/serializes to the "id" json property within the document <para/>
    /// Overridable.
    /// </summary>
    [Required, JsonPropertyName("id")]
    [JsonProperty("id")]
    string DocumentId { get; set; }

    /// <summary>
    /// Usage of the PartitionKey may be different depending on the document/entity/container. <para/>
    /// Maps to the "partitionKey" json property within the document. <para/>
    /// Overridable.
    /// </summary>
    [Required, JsonPropertyName("partitionKey")]
    [JsonProperty("partitionKey")]
    string PartitionKey { get; set; }

    /// <inheritdoc cref="IEntity.CreatedAt"/>
    [Required, JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IEntity.ModifiedAt"/>
    [JsonPropertyName("modifiedAt")]
    [JsonProperty("modifiedAt")]
    DateTime? ModifiedAt { get; set; }
}