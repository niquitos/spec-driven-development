using Xunit;
using Moq;
using TaskTracker.Application.Tasks;
using TaskTracker.Domain;

namespace TaskTracker.UnitTests.Tasks;

public class GetTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly GetTasksQueryHandler _handler;

    public GetTasksQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new GetTasksQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCalled_InvokesRepositoryGetByDateAsync()
    {
        // Arrange
        var date = DateTime.Today;
        var expectedTasks = new List<TaskEntity>
        {
            new TaskEntity { Id = 1, Title = "Task 1", Date = date },
            new TaskEntity { Id = 2, Title = "Task 2", Date = date }
        };

        _repositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTasks);

        // Act
        var result = await _handler.Handle(new GetTasksQuery(date), CancellationToken.None);

        // Assert
        Assert.Equal(expectedTasks, result);
        _repositoryMock.Verify(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoTasksForDate_ReturnsEmptyList()
    {
        // Arrange
        var date = DateTime.Today;
        _repositoryMock
            .Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TaskEntity>());

        // Act
        var result = await _handler.Handle(new GetTasksQuery(date), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
