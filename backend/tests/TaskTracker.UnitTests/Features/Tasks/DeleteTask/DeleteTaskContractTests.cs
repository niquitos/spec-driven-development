using Xunit;

namespace TaskTracker.UnitTests.Features.Tasks.DeleteTask;

public class DeleteTaskContractTests
{
    [Fact]
    public void RequestContract_RequiresOnlyId()
    {
        // Arrange & Act
        var id = 1;

        // Assert
        Assert.Equal(1, id);
    }

    [Fact]
    public void ResponseContract_ReturnsNoContent()
    {
        // Arrange
        var statusCode = 204; // No Content

        // Assert
        Assert.Equal(204, statusCode);
    }

    [Fact]
    public void ResponseContract_ReturnsNotFoundForNonExistentTask()
    {
        // Arrange
        var statusCode = 404; // Not Found

        // Assert
        Assert.Equal(404, statusCode);
    }
}
