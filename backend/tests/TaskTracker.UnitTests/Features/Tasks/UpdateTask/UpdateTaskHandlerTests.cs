using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
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
        Domain.TaskEntity? capturedTask = null;

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
            .Callback<Domain.TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(existingTask);

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(
            1,
            "New Title",
            "New Description",
            DateTime.Today,
            Domain.TaskStatus.InProgress,
            5
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal("New Title", capturedTask.Title);
        Assert.Equal("New Description", capturedTask.Description);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(
            1,
            "Updated Title",
            "Original Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(999, "Title", null, DateTime.Today, Domain.TaskStatus.New, 0);

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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(1, "New Title", null, DateTime.Today, Domain.TaskStatus.New, 0);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.True(capturedTask.UpdatedAt > capturedTask.CreatedAt);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesOrder()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Title",
            Description = "Description",
            Date = DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var existingTask2 = new Domain.TaskEntity
        {
            Id = 2,
            Title = "Task 2",
            Description = null,
            Date = DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        mockRepository
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        mockRepository
            .Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask2);

        mockRepository
            .Setup(r => r.GetByDateAsync(It.IsAny<DateTime>(), It.IsAny<string[]?>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { existingTask, existingTask2 });

        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(existingTask);

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(1, "Title", "Description", DateTime.Today, Domain.TaskStatus.New, 1);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal(1, capturedTask.Order);
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(1, "Title", "", DateTime.Today, Domain.TaskStatus.New, 0);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal("", capturedTask.Description);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesAssignee()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Title",
            Description = "Description",
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(1, "Title", "Description", DateTime.Today, Domain.TaskStatus.New, 0, "Петр");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal("Петр", capturedTask.Assignee);
    }

    [Fact]
    public async Task Handle_ValidCommand_ClearsAssigneeWhenNull()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Title",
            Description = "Description",
            Date = DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 0,
            Assignee = "Иван",
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(1, "Title", "Description", DateTime.Today, Domain.TaskStatus.New, 0, null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Null(capturedTask.Assignee);
    }

    [Fact]
    public async Task Handle_ValidCommand_ClearsAssigneeWhenEmptyString()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        Domain.TaskEntity? capturedTask = null;

        var existingTask = new Domain.TaskEntity
        {
            Id = 1,
            Title = "Title",
            Description = "Description",
            Date = DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 0,
            Assignee = "Иван",
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

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(1, "Title", "Description", DateTime.Today, Domain.TaskStatus.New, 0, "");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Null(capturedTask.Assignee);
    }
}
