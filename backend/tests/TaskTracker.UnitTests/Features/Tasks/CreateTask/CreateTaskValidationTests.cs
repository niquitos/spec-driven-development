using Xunit;
using Moq;
using TaskTracker.Application.Tasks;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.UnitTests.Features.Tasks.CreateTask;

public class CreateTaskValidationTests
{
    [Fact]
    public async Task Validate_TitleIsRequired_ReturnsError()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var command = new CreateTaskCommand(
            string.Empty,
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Title is required", errors);
    }

    [Fact]
    public async Task Validate_TitleWithWhitespace_ReturnsError()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var command = new CreateTaskCommand(
            "   ",
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Title is required", errors);
    }

    [Fact]
    public async Task Validate_TitleExceedsMaxLength_ReturnsError()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var longTitle = new string('A', 201);
        var command = new CreateTaskCommand(
            longTitle,
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Title must not exceed 200 characters", errors);
    }

    [Fact]
    public async Task Validate_DescriptionExceedsMaxLength_ReturnsError()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var longDescription = new string('B', 2001);
        var command = new CreateTaskCommand(
            "Valid Title",
            longDescription,
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Description must not exceed 2000 characters", errors);
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsNoErrors()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var command = new CreateTaskCommand(
            "Valid Title",
            "Valid description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_NullDescription_ReturnsNoErrors()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var command = new CreateTaskCommand(
            "Valid Title",
            null,
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_TitleAtMaxLength_ReturnsNoErrors()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var maxTitle = new string('A', 200);
        var command = new CreateTaskCommand(
            maxTitle,
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_DescriptionAtMaxLength_ReturnsNoErrors()
    {
        // Arrange
        var validator = new CreateTaskCommandValidator();
        var maxDescription = new string('B', 2000);
        var command = new CreateTaskCommand(
            "Title",
            maxDescription,
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }
}
