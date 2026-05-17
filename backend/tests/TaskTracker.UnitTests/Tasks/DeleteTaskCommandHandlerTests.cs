using Xunit;
using Moq;
using TaskTracker.Application.Tasks;
using TaskTracker.Domain;

namespace TaskTracker.UnitTests.Tasks;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new DeleteTaskCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenTaskExists_CallsDeleteAsync()
    {
        // Arrange
        var taskId = 1;
        var existingTask = new TaskEntity { Id = taskId, Title = "Task", Date = DateTime.Today };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        // Act
        await _handler.Handle(new DeleteTaskCommand(taskId), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTaskDoesNotExist_DoesNotCallDeleteAsync()
    {
        // Arrange
        var taskId = 1;
        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity?)null);

        // Act
        await _handler.Handle(new DeleteTaskCommand(taskId), CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
