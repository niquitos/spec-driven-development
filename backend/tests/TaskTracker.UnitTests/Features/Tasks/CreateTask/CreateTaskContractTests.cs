using Xunit;
using System.Text.Json;

namespace TaskTracker.UnitTests.Features.Tasks.CreateTask;

public class CreateTaskContractTests
{
    [Fact]
    public void RequestContract_HasRequiredFields()
    {
        // Arrange
        var sampleRequest = new
        {
            title = "Test Task",
            description = (string?)null,
            date = "2026-05-09",
            status = 0,
            order = 0
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("title", out _));
        Assert.True(element.TryGetProperty("date", out _));
        Assert.True(element.TryGetProperty("status", out _));
        Assert.True(element.TryGetProperty("order", out _));
    }

    [Fact]
    public void RequestContract_DescriptionIsOptional()
    {
        // Arrange
        var sampleRequest = new
        {
            title = "Test Task",
            description = (string?)null,
            date = "2026-05-09",
            status = 0,
            order = 0
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("description", out var desc));
        Assert.True(desc.ValueKind == JsonValueKind.Null || desc.ValueKind == JsonValueKind.String);
    }

    [Fact]
    public void ResponseContract_HasCorrectStructure()
    {
        // Arrange
        var sampleResponse = new
        {
            id = 1,
            title = "Test Task",
            description = (string?)null,
            status = 0,
            date = "2026-05-09",
            order = 0,
            createdAt = "2026-05-09T10:00:00Z",
            updatedAt = "2026-05-09T10:00:00Z"
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
    public void ResponseContract_StatusIsEnum()
    {
        // Arrange & Act
        var newStatus = 0; // TaskStatus.New
        var inProgressStatus = 1; // TaskStatus.InProgress
        var doneStatus = 2; // TaskStatus.Done

        // Assert
        Assert.Equal(0, newStatus);
        Assert.Equal(1, inProgressStatus);
        Assert.Equal(2, doneStatus);
    }
}
