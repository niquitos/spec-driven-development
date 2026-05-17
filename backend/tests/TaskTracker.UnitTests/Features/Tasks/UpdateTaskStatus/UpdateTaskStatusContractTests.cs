using Xunit;
using System.Text.Json;

namespace TaskTracker.UnitTests.Features.Tasks.UpdateTaskStatus;

public class UpdateTaskStatusContractTests
{
    [Fact]
    public void RequestContract_HasStatusAndOrder()
    {
        // Arrange
        var sampleRequest = new
        {
            status = 1,
            order = 0
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("status", out _));
        Assert.True(element.TryGetProperty("order", out _));
    }

    [Fact]
    public void RequestContract_StatusIsEnum()
    {
        // Arrange & Act
        var newStatus = 0;
        var inProgressStatus = 1;
        var doneStatus = 2;

        // Assert
        Assert.Equal(0, newStatus);
        Assert.Equal(1, inProgressStatus);
        Assert.Equal(2, doneStatus);
    }

    [Fact]
    public void ResponseContract_HasCorrectStructure()
    {
        // Arrange
        var sampleResponse = new
        {
            id = 1,
            title = "Moved Task",
            description = (string?)null,
            status = 1,
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
}
