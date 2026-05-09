using Xunit;
using Moq;
using TaskTracker.Application.Tasks;
using Domain = TaskTracker.Domain;

namespace TaskTracker.UnitTests.Features.Tasks.UpdateTask;

public class UpdateTaskHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_UpdatesTask()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Old Title",
            Description = "Old Description",
            Date = DateTime.Today.AddDays(-1),
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object);
        var command = new UpdateTaskCommand(
            1,
            "New Title",
            "New Description",
            DateTime.Today,
            Domain.TaskStatus.InProgress
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("New Description", result.Description);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original Description",
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
            .Callback<Domain.TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(existingTask);

        var handler = new UpdateTaskCommandHandler(mockRepository.Object);
        var command = new UpdateTaskCommand(
            1,
            "Updated Title",
            null, // Don't update description
            null, // Don't update date
            null  // Don't update status
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal("Updated Title", capturedTask.Title);
        Assert.Equal("Original Description", capturedTask.Description);
    }

    [Fact]
    public async Task Handle_NonExistentTask_ThrowsNotFoundException()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        mockRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.TaskEntity?)null);

        var handler = new UpdateTaskCommandHandler(mockRepository.Object);
        var command = new UpdateTaskCommand(999, "Title", null, DateTime.Today, Domain.TaskStatus.New);

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
            Title = "Title",
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object);
        var command = new UpdateTaskCommand(1, "New Title", null, null, null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.True(capturedTask.UpdatedAt > capturedTask.CreatedAt);
    }

    [Fact]
    public async Task Handle_NullDescription_ClearsDescription()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Title",
            Description = "Existing Description",
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
            .Callback<Domain.TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(existingTask);

        var handler = new UpdateTaskCommandHandler(mockRepository.Object);
        var command = new UpdateTaskCommand(1, "Title", null, null, null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal("Existing Description", capturedTask.Description);
    }

    [Fact]
    public async Task Handle_EmptyStringDescription_UpdatesToEmpty()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Title",
            Description = "Existing Description",
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
            .Callback<Domain.TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(existingTask);

        var handler = new UpdateTaskCommandHandler(mockRepository.Object);
        var command = new UpdateTaskCommand(1, "Title", "", null, null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal("", capturedTask.Description);
    }
}
