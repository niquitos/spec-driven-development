using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Tasks;
using Domain = TaskTracker.Domain;

namespace TaskTracker.UnitTests.Features.Tasks.UpdateTask;

public class UpdateTaskSwimlaneTests
{
    private static (Mock<ITaskRepository> repository, Domain.TaskEntity task) SetupMockTask(
        int id = 1,
        string? swimlane = null)
    {
        var mockRepository = new Mock<ITaskRepository>();

        var existingTask = new Domain.TaskEntity
        {
            Id = id,
            Title = "Title",
            Description = "Description",
            Date = Domain.TaskStatus.New != default ? DateTime.Today : DateTime.Today,
            Status = Domain.TaskStatus.New,
            Order = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Swimlane = swimlane
        };

        mockRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        mockRepository
            .Setup(r => r.GetByDateAsync(It.IsAny<DateTime>(), It.IsAny<string[]?>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Domain.TaskEntity>());

        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        return (mockRepository, existingTask);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesSwimlane()
    {
        // Arrange
        var (mockRepository, existingTask) = SetupMockTask();

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(
            1,
            "Title",
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0,
            null,
            "Бэкенд"
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Бэкенд", existingTask.Swimlane);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ValidCommand_NormalizesWhitespaceSwimlaneToNull()
    {
        // Arrange
        var (mockRepository, existingTask) = SetupMockTask();

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(
            1,
            "Title",
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0,
            null,
            "  "
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(existingTask.Swimlane);
    }

    [Fact]
    public async Task Handle_ValidCommand_ClearsSwimlane_WhenNull()
    {
        // Arrange
        var (mockRepository, existingTask) = SetupMockTask(swimlane: "Фронтенд");

        var handler = new UpdateTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<UpdateTaskCommandHandler>>());
        var command = new UpdateTaskCommand(
            1,
            "Title",
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0,
            null,
            null
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(existingTask.Swimlane);
    }
}