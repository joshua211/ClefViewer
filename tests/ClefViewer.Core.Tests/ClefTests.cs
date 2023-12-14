using ClefViewer.Core.Models;
using FluentAssertions;

namespace ClefViewer.Core.Tests;

public class ClefTests
{
    [Fact]
    public void Parse_FromValidVerboseClef_ReturnsParsed()
    {
        // Arrange
        var clefLine =
            "{\"@t\":\"2023-11-16T10:26:32.541Z\",\"@mt\":\"Simple Log with object {@TestObject}\",\"@l\":\"Verbose\", \"TestObject\": {\"Name\": \"Test\", \"Id\": 1} ,\"SourceContext\":\"Some.Application.Context\",\"ThreadId\":16}";
        var dateTimeOffset = DateTimeOffset.Parse("2023-11-16T10:26:32.541Z");
        var template = "Simple Log with object {@TestObject}";
        var level = "Verbose";
        var exception = (Exception?)null;
        // Act
        var actual = Clef.Parse(clefLine);

        // Assert
        actual.Timestamp.Should().Be(dateTimeOffset);
        actual.MessageTemplate.Should().Be(template);
        actual.Level.Should().Be(level);
        actual.Exception.Should().Be(exception);
    }
}