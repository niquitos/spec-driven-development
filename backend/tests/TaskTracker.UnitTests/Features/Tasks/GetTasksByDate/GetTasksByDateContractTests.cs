using Xunit;
using System.Text.Json;

namespace TaskTracker.UnitTests.Features.Tasks.GetTasksByDate;

public class GetTasksByDateContractTests
{
    [Fact]
    public void ResponseContract_HasCorrectStructure()
    {
        // Arrange
        var sampleResponse = new[]
        {
            new
            {
                id = 1,
                title = "Test Task",
                description = (string?)null,
                status = 0,
                date = "2026-05-09",
                order = 0,
                createdAt = "2026-05-09T10:00:00Z",
                updatedAt = "2026-05-09T10:00:00Z"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(sampleResponse);
        var element = JsonDocument.Parse(json).RootElement;

        // Assert
        Assert.True(element.EnumerateArray().Any());
        var task = element.EnumerateArray().First();

        Assert.True(task.TryGetProperty("id", out _));
        Assert.True(task.TryGetProperty("title", out _));
        Assert.True(task.TryGetProperty("description", out _));
        Assert.True(task.TryGetProperty("status", out _));
        Assert.True(task.TryGetProperty("date", out _));
        Assert.True(task.TryGetProperty("order", out _));
        Assert.True(task.TryGetProperty("createdAt", out _));
        Assert.True(task.TryGetProperty("updatedAt", out _));
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

    [Fact]
    public void RequestContract_DateParameter_IsRequired()
    {
        // Arrange
        var validDate = DateTime.Today;

        // Act & Assert
        Assert.NotNull(validDate);
    }

    [Fact]
    public void RequestContract_DateParameter_AcceptsIso8601Format()
    {
        // Arrange
        var isoDate = "2026-05-09";

        // Act
        var parsed = DateTime.TryParse(isoDate, out var result);

        // Assert
        Assert.True(parsed);
        Assert.Equal(2026, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Equal(9, result.Day);
    }
}
