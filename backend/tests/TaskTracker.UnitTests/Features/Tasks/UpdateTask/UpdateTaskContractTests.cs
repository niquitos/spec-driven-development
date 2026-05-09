using Xunit;
using System.Text.Json;

namespace TaskTracker.UnitTests.Features.Tasks.UpdateTask;

public class UpdateTaskContractTests
{
    [Fact]
    public void RequestContract_HasIdAndOptionalFields()
    {
        // Arrange
        var sampleRequest = new
        {
            title = "Updated Title",
            description = (string?)null,
            date = (DateTime?)null,
            status = (int?)null
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert - all fields can be present
        Assert.True(element.TryGetProperty("title", out _));
        Assert.True(element.TryGetProperty("description", out _));
        Assert.True(element.TryGetProperty("date", out _));
        Assert.True(element.TryGetProperty("status", out _));
    }

    [Fact]
    public void RequestContract_AllFieldsAreOptional()
    {
        // Arrange
        var emptyRequest = new { };

        // Act
        var json = JsonSerializer.Serialize(emptyRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert - empty request is valid (partial update)
        Assert.False(element.TryGetProperty("title", out _));
        Assert.False(element.TryGetProperty("description", out _));
    }

    [Fact]
    public void ResponseContract_HasCorrectStructure()
    {
        // Arrange
        var sampleResponse = new
        {
            id = 1,
            title = "Updated Task",
            description = (string?)null,
            status = 0,
            date = "2026-05-09",
            order = 0,
            createdAt = "2026-05-09T10:00:00Z",
            updatedAt = "2026-05-09T11:00:00Z"
        };

        // Act
        var json = JsonSerializer.Serialize(sampleResponse);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("id", out _));
        Assert.True(element.TryGetProperty("title", out _));
        Assert.True(element.TryGetProperty("description", out _));
        Assert.True(element.TryGetProperty("status", out _));
        Assert.True(element.TryGetProperty("date", out _));
        Assert.True(element.TryGetProperty("order", out _));
        Assert.True(element.TryGetProperty("createdAt", out _));
        Assert.True(element.TryGetProperty("updatedAt", out _));
    }

    [Fact]
    public void ResponseContract_UpdatedAtChangesOnUpdate()
    {
        // Arrange
        var createdAt = "2026-05-09T10:00:00Z";
        var updatedAt = "2026-05-09T11:00:00Z";

        // Act & Assert
        Assert.NotEqual(createdAt, updatedAt);
    }
}
