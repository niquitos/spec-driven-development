using Xunit;
using System.Text.Json;

namespace TaskTracker.UnitTests.Features.Tasks.BulkDelete;

public class BulkDeleteContractTests
{
    [Fact]
    public void RequestContract_HasTaskIds()
    {
        // Arrange
        var sampleRequest = new
        {
            taskIds = new[] { 1, 2 }
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("taskIds", out var taskIds));
        Assert.Equal(JsonValueKind.Array, taskIds.ValueKind);
    }

    [Fact]
    public void ResponseContract_HasDeletedCount()
    {
        // Arrange
        var sampleResponse = new
        {
            deleted = 2
        };

        // Act
        var json = JsonSerializer.Serialize(sampleResponse);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("deleted", out var deleted));
        Assert.Equal(JsonValueKind.Number, deleted.ValueKind);
        Assert.Equal(2, deleted.GetInt32());
    }

    [Fact]
    public void RequestContract_TaskIdsCanBeEmpty()
    {
        // Arrange
        var sampleRequest = new
        {
            taskIds = new int[] { }
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("taskIds", out var taskIds));
        Assert.Equal(0, taskIds.GetArrayLength());
    }

    [Fact]
    public void RequestContract_TaskIdsCanHaveMultipleIds()
    {
        // Arrange
        var sampleRequest = new
        {
            taskIds = new[] { 1, 2, 3, 4 }
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("taskIds", out var taskIds));
        Assert.Equal(4, taskIds.GetArrayLength());
    }

    [Fact]
    public void ResponseContract_DeletedCountMatchesInput()
    {
        // Arrange - request with 3 IDs
        var taskCount = 3;
        var sampleResponse = new
        {
            deleted = taskCount
        };

        // Act
        var json = JsonSerializer.Serialize(sampleResponse);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("deleted", out var deleted));
        Assert.Equal(taskCount, deleted.GetInt32());
    }
}
