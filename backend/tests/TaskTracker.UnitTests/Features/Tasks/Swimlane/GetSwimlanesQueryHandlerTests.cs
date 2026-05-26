using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Tasks;

namespace TaskTracker.UnitTests.Features.Tasks.Swimlane;

public class GetSwimlanesQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<ILogger<GetSwimlanesQueryHandler>> _loggerMock;
    private readonly GetSwimlanesQueryHandler _handler;

    public GetSwimlanesQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _loggerMock = new Mock<ILogger<GetSwimlanesQueryHandler>>();
        _handler = new GetSwimlanesQueryHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUniqueSwimlanes()
    {
        // Arrange
        var date = new DateTime(2025, 6, 1);
        var swimlanes = new[] { "Фронтенд", "Бэкенд" };

        _repositoryMock
            .Setup(r => r.GetSwimlanesAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(swimlanes);

        var query = new GetSwimlanesQuery(date);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(swimlanes, result);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyArray_WhenNoSwimlanes()
    {
        // Arrange
        var date = new DateTime(2025, 6, 1);

        _repositoryMock
            .Setup(r => r.GetSwimlanesAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());

        var query = new GetSwimlanesQuery(date);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_InvokesRepositoryWithCorrectDate()
    {
        // Arrange
        var date = new DateTime(2025, 6, 15);

        _repositoryMock
            .Setup(r => r.GetSwimlanesAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Фронтенд" });

        var query = new GetSwimlanesQuery(date);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetSwimlanesAsync(date, It.IsAny<CancellationToken>()), Times.Once);
    }
}