using Xunit;
using TaskTracker.Application.Tasks;
using TaskTracker.Domain;

namespace TaskTracker.UnitTests.Tasks;

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator;

    public CreateTaskCommandValidatorTests()
    {
        _validator = new CreateTaskCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenTitleIsEmpty_ReturnsError()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "",
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Title is required", errors);
    }

    [Fact]
    public async Task Validate_WhenTitleExceeds200Characters_ReturnsError()
    {
        // Arrange
        var longTitle = new string('A', 201);
        var command = new CreateTaskCommand(
            longTitle,
            "Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Title must not exceed 200 characters", errors);
    }

    [Fact]
    public async Task Validate_WhenDescriptionExceeds2000Characters_ReturnsError()
    {
        // Arrange
        var longDescription = new string('D', 2001);
        var command = new CreateTaskCommand(
            "Valid Title",
            longDescription,
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Description must not exceed 2000 characters", errors);
    }

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_ReturnsNoErrors()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Valid Title",
            "Valid Description",
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_WhenAssigneeIsWhitespace_ReturnsNoErrors()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Valid Title",
            null,
            DateTime.Today,
            Domain.TaskStatus.New,
            0,
            "   "
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_WhenAssigneeExceeds100Characters_ReturnsError()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Valid Title",
            null,
            DateTime.Today,
            Domain.TaskStatus.New,
            0,
            new string('A', 101)
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Contains("Assignee must not exceed 100 characters", errors);
    }

    [Fact]
    public async Task Validate_WhenAssigneeIsValid_ReturnsNoErrors()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Valid Title",
            null,
            DateTime.Today,
            Domain.TaskStatus.New,
            0,
            "Иван"
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_WhenAssigneeIsNull_ReturnsNoErrors()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Valid Title",
            null,
            DateTime.Today,
            Domain.TaskStatus.New,
            0
        );

        // Act
        var errors = await _validator.Validate(command, CancellationToken.None);

        // Assert
        Assert.Empty(errors);
    }
}
