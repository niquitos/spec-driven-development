using Xunit;
using Moq;
using TaskTracker.Application.Tasks;
using Domain = TaskTracker.Domain;

namespace TaskTracker.UnitTests.Features.Tasks.DeleteTask;

public class DeleteTaskHandlerTests
{
    [Fact]
    public async Task Handle_ExistingTask_DeletesTask()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Task to Delete",
            Description = null,
            Date = DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        mockRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var handler = new DeleteTaskCommandHandler(mockRepository.Object);
        var command = new DeleteTaskCommand(1);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentTask_DoesNotCallDelete()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        mockRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.TaskEntity?)null);

        var handler = new DeleteTaskCommandHandler(mockRepository.Object);
        var command = new DeleteTaskCommand(999);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExistingTask_CallsGetByIdAsync()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Task",
            Description = null,
            Date = DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        mockRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var handler = new DeleteTaskCommandHandler(mockRepository.Object);
        var command = new DeleteTaskCommand(1);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
