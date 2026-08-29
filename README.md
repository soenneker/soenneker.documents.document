[![](https://img.shields.io/nuget/v/Soenneker.Documents.Document.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Documents.Document/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.documents.document/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.documents.document/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Documents.Document.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Documents.Document/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.documents.document/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.documents.document/actions/workflows/codeql.yml)

# Soenneker.Documents.Document

The base document type providing a building block for storage objects Documents may or may not have their own separate containers. They are not tied to only one repository. A parent document may have children documents exist on them.

## Install

```bash
dotnet add package Soenneker.Documents.Document
```

## What you get

- `IDocument` — The base document type providing a building block for storage objects Documents may or may not have their own separate containers. They are not tied to only one repository. A parent document may have children documents exist on them.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IDocument.Id` | This is unused by CosmosDb, it is for internal identification PartitionKey:DocumentId construction... unless DocumentId = PartitionId (then it's only one id). This also supports 'combined ids'. For example, a partition key could be guid1:guid2, and the document id is guid3. It would return guid1:guid2:guid3. | During GET it builds the return value from joining PartitionKey and DocumentId (PartitionKey:DocumentId) During SET it sets the DocumentId and PartitionKey of the document. |
| `IDocument.DocumentId` | Maps/serializes to the "id" json property within the document Overridable. | Maps/serializes to the "id" json property within the document Overridable. |
| `IDocument.PartitionKey` | Usage of the PartitionKey may be different depending on the document/entity/container. Maps to the "partitionKey" json property within the document. Supports 'combined ids' with colon between the parts. Overridable. | Usage of the PartitionKey may be different depending on the document/entity/container. Maps to the "partitionKey" json property within the document. Supports 'combined ids' with colon between the parts. Overridable. |
| `IDocument.CreatedAt` | Gets or sets created at. | Gets or sets created at. |
| `IDocument.ModifiedAt` | Gets or sets modified at. | Gets or sets modified at. |

## Important behavior

- `IDocument.Id`: During GET it builds the return value from joining PartitionKey and DocumentId (PartitionKey:DocumentId) During SET it sets the DocumentId and PartitionKey of the document.
