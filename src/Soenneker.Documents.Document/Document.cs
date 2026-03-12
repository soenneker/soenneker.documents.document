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
            string? cached = _idCache;
            if (cached is not null)
                return cached;

            string? pk = PartitionKey;
            string? id = DocumentId;

            if (pk is null)
                return _idCache = id ?? string.Empty;

            if (id is null)
                return _idCache = pk;

            if (ReferenceEquals(pk, id) || string.Equals(pk, id, StringComparison.Ordinal))
                return _idCache = id;

            int pkLen = pk.Length;
            int idLen = id.Length;

            if (pkLen == 0)
                return _idCache = id;

            if (idLen == 0)
                return _idCache = pk;

            int totalLen = pkLen + 1 + idLen;

            return _idCache = string.Create(totalLen, (pk, id, pkLen), static (dst, state) =>
            {
                state.pk.AsSpan()
                     .CopyTo(dst);
                dst[state.pkLen] = _colon;
                state.id.AsSpan()
                     .CopyTo(dst.Slice(state.pkLen + 1));
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            if (value.IsNullOrWhiteSpace())
                return;

            if (ReferenceEquals(_idCache, value) || string.Equals(_idCache, value, StringComparison.Ordinal))
                return;

            ReadOnlySpan<char> s = value.AsSpan();
            int lastColon = s.LastIndexOf(_colon);

            // Read virtuals once (avoid re-reading via helper)
            string? pk = PartitionKey;
            string? id = DocumentId;

            if (pk is not null && id is not null)
            {
                if (lastColon < 0)
                {
                    // no-colon form: matches only if pk==id==value
                    if ((ReferenceEquals(pk, id) || string.Equals(pk, id, StringComparison.Ordinal)) && s.Length == id.Length && s.SequenceEqual(id.AsSpan()))
                    {
                        _idCache = value;
                        return;
                    }
                }
                else
                {
                    // composite form: "pk:id"
                    int pkLen = pk.Length;
                    int idLen = id.Length;

                    if (s.Length == pkLen + 1 + idLen && s[pkLen] == _colon && s.Slice(0, pkLen)
                                                                                .SequenceEqual(pk.AsSpan()) && s.Slice(pkLen + 1, idLen)
                            .SequenceEqual(id.AsSpan()))
                    {
                        _idCache = value;
                        return;
                    }
                }
            }

            if (lastColon < 0)
            {
                PartitionKey = value;
                DocumentId = value;
                _idCache = value;
                return;
            }

            PartitionKey = lastColon == 0 ? string.Empty : new string(s.Slice(0, lastColon));
            DocumentId = lastColon == s.Length - 1 ? string.Empty : new string(s.Slice(lastColon + 1));
            _idCache = value;
        }
    }

    [Required, JsonPropertyName("id")]
    [JsonProperty("id")]
    public virtual string DocumentId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (string.Equals(field, value, StringComparison.Ordinal))
                return;

            field = value;
            _idCache = null;
        }
    }

    [Required, JsonPropertyName("partitionKey")]
    [JsonProperty("partitionKey")]
    public virtual string PartitionKey
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (string.Equals(field, value, StringComparison.Ordinal))
                return;

            field = value;
            _idCache = null;
        }
    }

    [Required, JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    public virtual DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("modifiedAt")]
    [JsonProperty("modifiedAt")]
    public virtual DateTimeOffset? ModifiedAt { get; set; }
}