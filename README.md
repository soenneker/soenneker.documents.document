[![](https://img.shields.io/nuget/v/Soenneker.Documents.Document.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Documents.Document/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.documents.document/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.documents.document/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Documents.Document.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Documents.Document/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.documents.document/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.documents.document/actions/workflows/codeql.yml)

# Soenneker.Documents.Document

Provides a base document model with storage identity, partition, and timestamp fields.

## Installation

```bash
dotnet add package Soenneker.Documents.Document
```

## Define a document

```csharp
using Soenneker.Documents.Document;

public sealed class CustomerDocument : Document
{
    public string Name { get; set; } = null!;
}

var customer = new CustomerDocument
{
    DocumentId = "customer-42",
    PartitionKey = "tenant-7",
    CreatedAt = DateTimeOffset.UtcNow,
    Name = "Ada Lovelace"
};

string? internalId = customer.Id; // tenant-7:customer-42
```

`DocumentId` serializes as `id`; `PartitionKey` serializes as `partitionKey`. `CreatedAt` and `ModifiedAt` serialize as `createdAt` and `modifiedAt`. These names are declared for both System.Text.Json and Newtonsoft.Json.

`Id` is an internal convenience value and is ignored by both serializers. Its behavior is:

- If one key is missing, it returns the other.
- If both keys are equal, it returns that value once.
- If the keys differ, it returns `PartitionKey:DocumentId`.
- Assigning a value containing colons splits at the last colon.
- Assigning a value without a colon sets both `PartitionKey` and `DocumentId` to that value.
- Assigning null, empty, or whitespace does nothing.

Because the format has no escaping, `DocumentId` should not contain a colon when `Id` must round-trip through its setter. A partition key may contain colons because parsing reserves the final segment for `DocumentId`.

```csharp
customer.Id = "tenant:region:customer-42";

// PartitionKey == "tenant:region"
// DocumentId == "customer-42"
```

New instances do not automatically receive identifiers or timestamps. The `[Required]` attributes provide validation metadata but do not initialize values or make mutable instances thread-safe; populate and validate documents before persistence.
