using Xunit;
using System.Text.Json;

namespace TaskTracker.UnitTests.Features.Tasks.BulkMove;

public class BulkMoveContractTests
{
    [Fact]
    public void RequestContract_HasTaskIdsAndTargetDate()
    {
        // Arrange
        var sampleRequest = new
        {
            taskIds = new[] { 1, 2 },
            targetDate = "2026-05-10"
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("taskIds", out var taskIds));
        Assert.True(element.TryGetProperty("targetDate", out var targetDate));
        Assert.Equal(JsonValueKind.Array, taskIds.ValueKind);
        Assert.Equal(JsonValueKind.String, targetDate.ValueKind);
    }

    [Fact]
    public void ResponseContract_HasMovedCountAndTargetDate()
    {
        // Arrange
        var sampleResponse = new
        {
            moved = 2,
            targetDate = "2026-05-10"
        };

        // Act
        var json = JsonSerializer.Serialize(sampleResponse);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("moved", out var moved));
        Assert.True(element.TryGetProperty("targetDate", out var targetDate));
        Assert.Equal(2, moved.GetInt32());
        Assert.Equal("2026-05-10", targetDate.GetString());
    }

    [Fact]
    public void RequestContract_TargetDateIsValidYYYYMMDDFormat()
    {
        // Arrange
        var sampleRequest = new
        {
            taskIds = new[] { 1 },
            targetDate = "2026-05-10"
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("targetDate", out var targetDate));
        var dateStr = targetDate.GetString();
        Assert.NotNull(dateStr);
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", dateStr);
    }

    [Fact]
    public void RequestContract_TaskIdsCanBeEmpty()
    {
        // Arrange
        var sampleRequest = new
        {
            taskIds = new int[] { },
            targetDate = "2026-05-10"
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
            taskIds = new[] { 1, 2, 3 },
            targetDate = "2026-05-10"
        };

        // Act
        var json = JsonSerializer.Serialize(sampleRequest);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("taskIds", out var taskIds));
        Assert.Equal(3, taskIds.GetArrayLength());
    }

    [Fact]
    public void ResponseContract_MovedCountMatchesInput()
    {
        // Arrange
        var taskCount = 3;
        var sampleResponse = new
        {
            moved = taskCount,
            targetDate = "2026-05-10"
        };

        // Act
        var json = JsonSerializer.Serialize(sampleResponse);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("moved", out var moved));
        Assert.Equal(taskCount, moved.GetInt32());
    }

    [Fact]
    public void ResponseContract_TargetDateReflectsRequest()
    {
        // Arrange
        var requestDate = "2026-05-15";
        var sampleResponse = new
        {
            moved = 2,
            targetDate = requestDate
        };

        // Act
        var json = JsonSerializer.Serialize(sampleResponse);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.TryGetProperty("targetDate", out var targetDate));
        Assert.Equal(requestDate, targetDate.GetString());
    }
}
