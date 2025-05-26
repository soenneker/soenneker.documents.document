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
}