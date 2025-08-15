using Newtonsoft.Json;
using Soenneker.Documents.Document.Abstract;
using Soenneker.Extensions.String;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Soenneker.Documents.Document;

///<inheritdoc cref="IDocument"/>
public abstract class Document : IDocument
{
    private const char _colon = ':';
    private string? _idCache;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public virtual string Id
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            // Return cache if available
            if (_idCache is not null)
                return _idCache;

            string? pk = PartitionKey;
            string? id = DocumentId;

            // null-safe equal check
            if (string.Equals(id, pk, StringComparison.Ordinal))
                return _idCache = id;

            // Handle potential nulls early (in case a derived type delays initialization)
            if (pk is null) 
                return _idCache = id ?? string.Empty;

            if (id is null) 
                return _idCache = pk;

            int pkLen = pk.Length;
            int idLen = id.Length;

            // Fast paths for empty strings
            if (pkLen == 0)
                return _idCache = id;

            if (idLen == 0) 
                return _idCache = pk;

            // Allocate exactly once
            return _idCache = string.Create(pkLen + 1 + idLen, (pk, id, pkLen), static (span, state) =>
            {
                state.pk.AsSpan().CopyTo(span);
                span[state.pkLen] = _colon;
                state.id.AsSpan().CopyTo(span.Slice(state.pkLen + 1));
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            if (value.IsNullOrWhiteSpace())
                return;

            ReadOnlySpan<char> span = value.AsSpan();

            // If value matches the current composite, skip work (no alloc, no writes)
            // Compare against either "pk:id" or the degenerate case where pk == id == value (no colon)
            if (MatchesCurrentComposite(span))
                return;

            int lastColon = span.LastIndexOf(_colon);

            if (lastColon < 0)
            {
                // No colon: both become 'value' (no new allocation beyond the property reference itself)
                PartitionKey = value;
                DocumentId = value;
                _idCache = value; // cache matches rule "if equal, Id == DocumentId"
                return;
            }

            // Slice and allocate the two parts
            PartitionKey = new string(span.Slice(0, lastColon));
            DocumentId = new string(span.Slice(lastColon + 1));

            // Cache exact input since Id getter would format to the same "pk:id"
            _idCache = value;
        }
    }

    [Required, JsonPropertyName("id")]
    [JsonProperty("id")]
    public virtual string DocumentId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _documentId!;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (!string.Equals(_documentId, value, StringComparison.Ordinal))
            {
                _documentId = value;
                _idCache = null; // invalidate
            }
        }
    }

    private string? _documentId;

    [Required, JsonPropertyName("partitionKey")]
    [JsonProperty("partitionKey")]
    public virtual string PartitionKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _partitionKey!;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (!string.Equals(_partitionKey, value, StringComparison.Ordinal))
            {
                _partitionKey = value;
                _idCache = null; // invalidate
            }
        }
    }

    private string? _partitionKey;

    [Required, JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    public virtual DateTime CreatedAt { get; set; }

    [JsonPropertyName("modifiedAt")]
    [JsonProperty("modifiedAt")]
    public virtual DateTime? ModifiedAt { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MatchesCurrentComposite(ReadOnlySpan<char> value)
    {
        string? pk = PartitionKey;
        string? id = DocumentId;

        // If either is null, can't match composite safely
        if (pk is null || id is null)
            return false;

        // Case 1: pk == id == value (no colon)
        if (string.Equals(pk, id, StringComparison.Ordinal) && value.SequenceEqual(id.AsSpan()))
            return true;

        // Case 2: value == "pk:id"
        int pkLen = pk.Length;
        int idLen = id.Length;

        if (value.Length != pkLen + 1 + idLen)
            return false;

        ReadOnlySpan<char> vSpan = value;
        if (!vSpan.Slice(0, pkLen).SequenceEqual(pk.AsSpan()))
            return false;

        if (vSpan[pkLen] != _colon)
            return false;

        return vSpan.Slice(pkLen + 1, idLen).SequenceEqual(id.AsSpan());
    }
}