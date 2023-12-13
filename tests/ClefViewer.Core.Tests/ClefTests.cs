using ClefViewer.Core.Models;
using FluentAssertions;

namespace ClefViewer.Core.Tests;

public class ClefTests : IClassFixture<JsonConverterFixture>
{
    public ClefTests(JsonConverterFixture fixture)
    {
        fixture.Init();
    }

    [Fact]
    public void Parse_FromValidVerboseClef_ReturnsParsed()
    {
        // Arrange
        var clefLine =
            "{\"@t\":\"2023-11-16T10:26:32.541Z\",\"@mt\":\"Simple Log with object {@TestObject}\",\"@l\":\"Verbose\", \"TestObject\": {\"Name\": \"Test\", \"Id\": 1} ,\"SourceContext\":\"Some.Application.Context\",\"ThreadId\":16}";

       var expected = new Clef(
            new DateTimeOffset(2023, 11, 16, 10, 26, 32, 541, TimeSpan.Zero),
            null,
            "Simple Log with object {@TestObject}",
            "Verbose",
            string.Empty,
            null,
            new Dictionary<string, object>
            {
                {"TestObject", new Dictionary<string, object> {{"Name", "Test"}, {"Id", 1}}},
                {"SourceContext", "Some.Application.Context"},
                {"ThreadId", 16}
            });

        // Act
        var actual = Clef.Parse(clefLine);

        // Assert
        actual.Timestamp.Should().Be(expected.Timestamp);
        actual.MessageTemplate.Should().Be(expected.MessageTemplate);
        actual.Level.Should().Be(expected.Level);
        actual.Exception.Should().Be(expected.Exception);
    }
}