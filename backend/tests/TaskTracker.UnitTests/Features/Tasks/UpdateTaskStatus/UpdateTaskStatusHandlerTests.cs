using Xunit;
using Moq;
using TaskTracker.Application.Tasks;
using Domain = TaskTracker.Domain;

namespace TaskTracker.UnitTests.Features.Tasks.UpdateTaskStatus;

public class UpdateTaskStatusHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_UpdatesStatusAndOrder()
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

        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var handler = new MoveTaskCommandHandler(mockRepository.Object);
        var command = new MoveTaskCommand(1, Domain.TaskStatus.InProgress, 5);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Domain.TaskStatus.InProgress, result.Status);
        Assert.Equal(5, result.Order);
    }

    [Fact]
    public async Task Handle_NonExistentTask_ThrowsNotFoundException()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        mockRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.TaskEntity?)null);

        var handler = new MoveTaskCommandHandler(mockRepository.Object);
        var command = new MoveTaskCommand(999, Domain.TaskStatus.New, 0);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesTimestamp()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Task",
            Description = null,
            Date = DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        mockRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(existingTask);

        var handler = new MoveTaskCommandHandler(mockRepository.Object);
        var command = new MoveTaskCommand(1, Domain.TaskStatus.InProgress, 0);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.True(capturedTask.UpdatedAt > capturedTask.CreatedAt);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsGetByIdAsync()
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

        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var handler = new MoveTaskCommandHandler(mockRepository.Object);
        var command = new MoveTaskCommand(1, Domain.TaskStatus.InProgress, 0);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsUpdateAsync()
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

        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var handler = new MoveTaskCommandHandler(mockRepository.Object);
        var command = new MoveTaskCommand(1, Domain.TaskStatus.InProgress, 0);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
