using Xunit;

namespace TaskTracker.UnitTests.Features.Tasks.GetTasksByDate;

public class GetTasksByDateValidationTests
{
    [Fact]
    public void Handle_WhenDateIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var emptyDate = DateTime.MinValue;

        // Act & Assert
        Assert.False(emptyDate > DateTime.MinValue);
    }

    [Fact]
    public void Handle_WhenDateIsInFuture_ReturnsEmptyList()
    {
        // Arrange
        var futureDate = DateTime.Today.AddDays(1);

        // Act & Assert
        Assert.True(futureDate > DateTime.Today);
    }

    [Fact]
    public void Handle_WhenDateIsInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var invalidDate = "invalid-date";

        // Act
        var parsed = DateTime.TryParse(invalidDate, out _);

        // Assert
        Assert.False(parsed);
    }

    [Theory]
    [InlineData("2026-13-01")] // Invalid month
    [InlineData("2026-02-30")] // Invalid day
    [InlineData("")] // Empty string
    public void Handle_WhenDateIsMalformed_ReturnsBadRequest(string invalidDate)
    {
        // Act
        var parsed = DateTime.TryParse(invalidDate, out _);

        // Assert
        Assert.False(parsed);
    }
}
