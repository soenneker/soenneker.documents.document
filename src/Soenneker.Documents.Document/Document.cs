using Newtonsoft.Json;
using Soenneker.Documents.Document.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Soenneker.Documents.Document;

/// <inheritdoc cref="IDocument"/>
public abstract class Document : IDocument
{
    private const char _colon = ':';

    private string? _idCache;
    private string? _documentId;
    private string? _partitionKey;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public string? Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            string? cached = _idCache;
            if (cached is not null)
                return cached;

            string? pk = _partitionKey;
            string? id = _documentId;

            if (string.IsNullOrEmpty(pk))
                return _idCache = id;

            if (string.IsNullOrEmpty(id))
                return _idCache = pk;

            if (ReferenceEquals(pk, id) || string.Equals(pk, id, StringComparison.Ordinal))
                return _idCache = id;

            return _idCache = string.Create(pk.Length + 1 + id.Length, (pk, id), static (dst, state) =>
            {
                state.pk.AsSpan().CopyTo(dst);
                dst[state.pk.Length] = _colon;
                state.id.AsSpan().CopyTo(dst[(state.pk.Length + 1)..]);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            string? cached = _idCache;
            if (cached == value)
                return;

            ReadOnlySpan<char> span = value.AsSpan();
            int lastColon = span.LastIndexOf(_colon);

            if (lastColon < 0)
            {
                if (_partitionKey != value)
                    _partitionKey = value;

                if (_documentId != value)
                    _documentId = value;

                _idCache = value;
                return;
            }

            ReadOnlySpan<char> pkSpan = span[..lastColon];
            ReadOnlySpan<char> idSpan = span[(lastColon + 1)..];

            bool pkMatches = pkSpan.SequenceEqual((_partitionKey ?? string.Empty).AsSpan());
            bool idMatches = idSpan.SequenceEqual((_documentId ?? string.Empty).AsSpan());

            if (!pkMatches)
                _partitionKey = pkSpan.Length == 0 ? string.Empty : new string(pkSpan);

            if (!idMatches)
                _documentId = idSpan.Length == 0 ? string.Empty : new string(idSpan);

            _idCache = value;
        }
    }

    [Required]
    [JsonPropertyName("id")]
    [JsonProperty("id")]
    public string? DocumentId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _documentId;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (_documentId == value)
                return;

            _documentId = value;
            _idCache = null;
        }
    }

    [Required]
    [JsonPropertyName("partitionKey")]
    [JsonProperty("partitionKey")]
    public string? PartitionKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _partitionKey;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (_partitionKey == value)
                return;

            _partitionKey = value;
            _idCache = null;
        }
    }

    [Required]
    [JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("modifiedAt")]
    [JsonProperty("modifiedAt")]
    public DateTimeOffset? ModifiedAt { get; set; }
}