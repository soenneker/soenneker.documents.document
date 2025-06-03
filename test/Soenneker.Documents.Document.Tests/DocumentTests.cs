using AwesomeAssertions;
using Mapster;
using Soenneker.Tests.Unit;
using Soenneker.Utils.String;
using Xunit;

namespace Soenneker.Documents.Document.Tests;

public class DocumentTests : UnitTest
{
    [Fact]
    public void Id_assembly_with_combined_partitionKey_should_give_correct_result()
    {
        var guid1 = Faker.Random.Guid().ToString();
        var guid2 = Faker.Random.Guid().ToString();
        var guid3 = Faker.Random.Guid().ToString();

        var document = new TestDocument
        {
            DocumentId = guid1,
            PartitionKey = StringUtil.ToCombinedId(guid2, guid3)
        };

        document.Id.Should().Be($"{guid2}:{guid3}:{guid1}");
    }

    [Fact]
    public void Id_disassembly_with_combined_partitionKey_should_give_correct_result()
    {
        var guid1 = Faker.Random.Guid().ToString();
        var guid2 = Faker.Random.Guid().ToString();
        var guid3 = Faker.Random.Guid().ToString();

        var id = $"{guid2}:{guid3}:{guid1}";

        var document = new TestDocument
        {
            Id = id
        };

        document.PartitionKey.Should().Be($"{guid2}:{guid3}");
        document.DocumentId.Should().Be(guid1);
    }

    [Fact]
    public void Adaption_from_entity_should_not_throw_with_no_id_set()
    {
        var entity = new TestEntity();
        var document = entity.Adapt<TestDocument>();
        document.Should().NotBeNull();
    }

    [Fact]
    public void GetId_WhenDocumentIdEqualsPartitionKey_ReturnsDocumentIdDirectly()
    {
        var doc = new TestDocument
        {
            PartitionKey = "sameValue",
            DocumentId = "sameValue"
        };

        // Since they match, no allocation beyond returning DocumentId
        string id = doc.Id;
        id.Should().Be("sameValue");
    }

    [Fact]
    public void GetId_WhenPartitionKeyAndDocumentIdDiffer_ReturnsPartitionKeyColonDocumentId()
    {
        var doc = new TestDocument
        {
            PartitionKey = "pkValue",
            DocumentId = "docValue"
        };

        // Expect "pkValue:docValue"
        string id = doc.Id;
        id.Should().Be("pkValue:docValue");
    }

    [Fact]
    public void SetId_NullOrWhitespace_DoesNotChangeExistingKeys()
    {
        var doc = new TestDocument
        {
            PartitionKey = "initialPK",
            DocumentId = "initialID"
        };

        // Attempt to set with null
        doc.Id = null;
        doc.PartitionKey.Should().Be("initialPK");
        doc.DocumentId.Should().Be("initialID");

        // Attempt to set with empty string
        doc.Id = "";
        doc.PartitionKey.Should().Be("initialPK");
        doc.DocumentId.Should().Be("initialID");

        // Attempt to set with whitespace
        doc.Id = "   ";
        doc.PartitionKey.Should().Be("initialPK");
        doc.DocumentId.Should().Be("initialID");
    }

    [Fact]
    public void SetId_NoColon_SetsBothPartitionAndDocumentToFullValue()
    {
        var doc = new TestDocument();

        doc.Id = "onlyValue";
        doc.PartitionKey.Should().Be("onlyValue");
        doc.DocumentId.Should().Be("onlyValue");

        // Getter should now return "onlyValue" (they match)
        doc.Id.Should().Be("onlyValue");
    }

    [Fact]
    public void SetId_SingleColon_SplitsAtThatPoint()
    {
        var doc = new TestDocument();

        doc.Id = "left:right";
        doc.PartitionKey.Should().Be("left");
        doc.DocumentId.Should().Be("right");

        // Getter should reconstruct "left:right"
        doc.Id.Should().Be("left:right");
    }

    [Fact]
    public void SetId_MultipleColons_PartitionIsEverythingBeforeLastColon()
    {
        var doc = new TestDocument();

        doc.Id = "a:b:c";
        doc.PartitionKey.Should().Be("a:b");
        doc.DocumentId.Should().Be("c");

        // Getter should reconstruct exactly "a:b:c"
        doc.Id.Should().Be("a:b:c");
    }

    [Fact]
    public void Setter_ThenGetter_RetainsRoundTripBehavior()
    {
        var inputs = new[]
        {
                "plainValue",
                "one:two",
                "x:y:z:w"
            };

        foreach (var input in inputs)
        {
            var doc = new TestDocument();
            doc.Id = input;

            // After setting, getter must return exactly the same string.
            doc.Id.Should().Be(input);
        }
    }

    [Fact]
    public void InitialNullKeys_SetIdNull_LeavesKeysNull()
    {
        var doc = new TestDocument();

        // Both PartitionKey and DocumentId start as null
        doc.PartitionKey.Should().BeNull();
        doc.DocumentId.Should().BeNull();

        // Setting Id = null should do nothing
        doc.Id = null;
        doc.PartitionKey.Should().BeNull();
        doc.DocumentId.Should().BeNull();

        // Setting Id = whitespace should do nothing
        doc.Id = "   ";
        doc.PartitionKey.Should().BeNull();
        doc.DocumentId.Should().BeNull();
    }
}