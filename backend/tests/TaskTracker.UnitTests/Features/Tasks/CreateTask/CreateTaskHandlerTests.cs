using Xunit;
using Moq;
using TaskTracker.Application.Tasks;
using TaskTracker.Domain;

namespace TaskTracker.UnitTests.Features.Tasks.CreateTask;

public class CreateTaskHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesTask()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        var createdTask = new TaskEntity
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            Date = DateTime.Today,
            Status = TaskStatus.New,
            Order = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        var handler = new CreateTaskCommandHandler(mockRepository.Object);
        var command = new CreateTaskCommand(
            "Test Task",
            "Test Description",
            DateTime.Today,
            TaskStatus.New,
            0
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("Test Description", result.Description);
        mockRepository.Verify(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsCreatedAt()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        TaskEntity? capturedTask = null;

        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(new TaskEntity { Id = 1 });

        var handler = new CreateTaskCommandHandler(mockRepository.Object);
        var command = new CreateTaskCommand(
            "Test Task",
            null,
            DateTime.Today,
            TaskStatus.New,
            0
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.True(capturedTask.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsUpdatedAt()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        TaskEntity? capturedTask = null;

        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(new TaskEntity { Id = 1 });

        var handler = new CreateTaskCommandHandler(mockRepository.Object);
        var command = new CreateTaskCommand(
            "Test Task",
            null,
            DateTime.Today,
            TaskStatus.New,
            0
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.True(capturedTask.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsStatus()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        TaskEntity? capturedTask = null;

        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(new TaskEntity { Id = 1 });

        var handler = new CreateTaskCommandHandler(mockRepository.Object);
        var command = new CreateTaskCommand(
            "Test Task",
            null,
            DateTime.Today,
            TaskStatus.InProgress,
            0
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal(TaskStatus.InProgress, capturedTask.Status);
    }

    [Fact]
    public async Task Handle_ValidCommand_SetsOrder()
    {
        // Arrange
        var mockRepository = new Mock<ITaskRepository>();
        TaskEntity? capturedTask = null;

        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TaskEntity, CancellationToken>((t, _) => capturedTask = t)
            .ReturnsAsync(new TaskEntity { Id = 1 });

        var handler = new CreateTaskCommandHandler(mockRepository.Object);
        var command = new CreateTaskCommand(
            "Test Task",
            null,
            DateTime.Today,
            TaskStatus.New,
            5
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal(5, capturedTask.Order);
    }
}
