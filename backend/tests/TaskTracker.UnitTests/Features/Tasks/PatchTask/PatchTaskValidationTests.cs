using Xunit;
using TaskTracker.Application.Tasks;
using Domain = TaskTracker.Domain;

namespace TaskTracker.UnitTests.Features.Tasks.PatchTask;

public class PatchTaskValidationTests
{
    private readonly PatchTaskCommandValidator _validator = new();

    [Fact]
    public async Task Validate_EmptyPatch_NoErrors()
    {
        var command = new PatchTaskCommand(1);
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_TitleProvidedAndEmpty_ReturnsError()
    {
        var command = new PatchTaskCommand(1, Title: "");
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Contains(errors, e => e.Contains("Title is required"));
    }

    [Fact]
    public async Task Validate_TitleProvidedAndWhitespace_ReturnsError()
    {
        var command = new PatchTaskCommand(1, Title: "   ");
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Contains(errors, e => e.Contains("Title is required"));
    }

    [Fact]
    public async Task Validate_TitleExceedsMaxLength_ReturnsError()
    {
        var command = new PatchTaskCommand(1, Title: new string('A', 201));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Contains(errors, e => e.Contains("Title must not exceed 200 characters"));
    }

    [Fact]
    public async Task Validate_TitleAtMaxLength_NoErrors()
    {
        var command = new PatchTaskCommand(1, Title: new string('A', 200));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_DescriptionExceedsMaxLength_ReturnsError()
    {
        var command = new PatchTaskCommand(1, Description: new string('B', 2001));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Contains(errors, e => e.Contains("Description must not exceed 2000 characters"));
    }

    [Fact]
    public async Task Validate_DescriptionAtMaxLength_NoErrors()
    {
        var command = new PatchTaskCommand(1, Description: new string('B', 2000));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_AssigneeExceedsMaxLength_ReturnsError()
    {
        var command = new PatchTaskCommand(1, Assignee: new string('C', 101));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Contains(errors, e => e.Contains("Assignee must not exceed 100 characters"));
    }

    [Fact]
    public async Task Validate_AssigneeAtMaxLength_NoErrors()
    {
        var command = new PatchTaskCommand(1, Assignee: new string('C', 100));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_SwimlaneExceedsMaxLength_ReturnsError()
    {
        var command = new PatchTaskCommand(1, Swimlane: new string('D', 101));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Contains(errors, e => e.Contains("Swimlane must not exceed 100 characters"));
    }

    [Fact]
    public async Task Validate_SwimlaneAtMaxLength_NoErrors()
    {
        var command = new PatchTaskCommand(1, Swimlane: new string('D', 100));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_InvalidStatus_ReturnsError()
    {
        var command = new PatchTaskCommand(1, Status: (Domain.TaskStatus)99);
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Contains(errors, e => e.Contains("Invalid status value"));
    }

    [Fact]
    public async Task Validate_ValidStatus_NoErrors()
    {
        var command = new PatchTaskCommand(1, Status: Domain.TaskStatus.InProgress);
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task Validate_MultipleErrors_ReturnsAll()
    {
        var command = new PatchTaskCommand(1, Title: "", Assignee: new string('X', 101));
        var errors = await _validator.Validate(command, CancellationToken.None);
        Assert.True(errors.Count() >= 2);
    }
}