using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Tasks;
using Domain = TaskTracker.Domain;

namespace TaskTracker.UnitTests.Features.Tasks.PatchTask;

public class PatchTaskHandlerTests
{
    private static (Mock<ITaskRepository> repository, Domain.TaskEntity task) SetupMockTask(
        int id = 1,
        string title = "Test Task",
        string? description = "Description",
        string? assignee = null,
        string? swimlane = null,
        Domain.TaskStatus status = Domain.TaskStatus.New,
        int order = 0)
    {
        var mockRepository = new Mock<ITaskRepository>();

        var existingTask = new Domain.TaskEntity
        {
            Id = id,
            Title = title,
            Description = description,
            Date = DateTime.Today,
            Status = status,
            Order = order,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Assignee = assignee,
            Swimlane = swimlane
        };

        mockRepository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        mockRepository
            .Setup(r => r.GetByDateAsync(It.IsAny<DateTime>(), It.IsAny<string[]>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Domain.TaskEntity>());

        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Domain.TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        return (mockRepository, existingTask);
    }

    [Fact]
    public async Task Handle_UpdatesOnlyStatus()
    {
        var (mockRepository, existingTask) = SetupMockTask(swimlane: "Фронтенд", assignee: "Анна");
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Status: Domain.TaskStatus.InProgress);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Domain.TaskStatus.InProgress, existingTask.Status);
        Assert.Equal("Фронтенд", existingTask.Swimlane);
        Assert.Equal("Анна", existingTask.Assignee);
        Assert.Equal("Test Task", existingTask.Title);
    }

    [Fact]
    public async Task Handle_UpdatesOnlyOrder_PreservesSwimlane()
    {
        var (mockRepository, existingTask) = SetupMockTask(swimlane: "Бэкенд", order: 0);
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Order: 3);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(3, existingTask.Order);
        Assert.Equal("Бэкенд", existingTask.Swimlane);
    }

    [Fact]
    public async Task Handle_ClearsAssigneeWithEmptyString()
    {
        var (mockRepository, existingTask) = SetupMockTask(assignee: "Анна");
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Assignee: "");

        await handler.Handle(command, CancellationToken.None);

        Assert.Null(existingTask.Assignee);
    }

    [Fact]
    public async Task Handle_ClearsSwimlaneWithEmptyString()
    {
        var (mockRepository, existingTask) = SetupMockTask(swimlane: "Дизайн");
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Swimlane: "");

        await handler.Handle(command, CancellationToken.None);

        Assert.Null(existingTask.Swimlane);
    }

    [Fact]
    public async Task Handle_NormalizesWhitespaceSwimlaneToNull()
    {
        var (mockRepository, existingTask) = SetupMockTask();
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Swimlane: "   ");

        await handler.Handle(command, CancellationToken.None);

        Assert.Null(existingTask.Swimlane);
    }

    [Fact]
    public async Task Handle_NormalizesWhitespaceAssigneeToNull()
    {
        var (mockRepository, existingTask) = SetupMockTask();
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Assignee: "   ");

        await handler.Handle(command, CancellationToken.None);

        Assert.Null(existingTask.Assignee);
    }

    [Fact]
    public async Task Handle_NormalizesWhitespaceDescriptionToNull()
    {
        var (mockRepository, existingTask) = SetupMockTask(description: "Old description");
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Description: "   ");

        await handler.Handle(command, CancellationToken.None);

        Assert.Null(existingTask.Description);
    }

    [Fact]
    public async Task Handle_UpdatesTitle()
    {
        var (mockRepository, existingTask) = SetupMockTask();
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Title: "New Title");

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("New Title", existingTask.Title);
    }

    [Fact]
    public async Task Handle_UpdatesUpdatedAt()
    {
        var (mockRepository, existingTask) = SetupMockTask();
        var beforeUpdate = existingTask.UpdatedAt;
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Title: "Updated");

        await handler.Handle(command, CancellationToken.None);

        Assert.True(existingTask.UpdatedAt >= beforeUpdate);
    }

    [Fact]
    public async Task Handle_NonExistentTask_ThrowsNotFoundException()
    {
        var mockRepository = new Mock<ITaskRepository>();
        mockRepository
            .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.TaskEntity?)null);

        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(999, Title: "Test");

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyPatch_NoChanges()
    {
        var (mockRepository, existingTask) = SetupMockTask(
            title: "Original", description: "Desc", assignee: "Анна", swimlane: "Фронтенд");
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("Original", existingTask.Title);
        Assert.Equal("Desc", existingTask.Description);
        Assert.Equal("Анна", existingTask.Assignee);
        Assert.Equal("Фронтенд", existingTask.Swimlane);
        Assert.Equal(Domain.TaskStatus.New, existingTask.Status);
    }

    [Fact]
    public async Task Handle_MultipleFields_UpdatesAll()
    {
        var (mockRepository, existingTask) = SetupMockTask(swimlane: "Старый");
        var handler = new PatchTaskCommandHandler(mockRepository.Object, Mock.Of<ILogger<PatchTaskCommandHandler>>());
        var command = new PatchTaskCommand(1, Title: "New Title", Status: Domain.TaskStatus.Done, Swimlane: "Новый");

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal("New Title", existingTask.Title);
        Assert.Equal(Domain.TaskStatus.Done, existingTask.Status);
        Assert.Equal("Новый", existingTask.Swimlane);
    }
}