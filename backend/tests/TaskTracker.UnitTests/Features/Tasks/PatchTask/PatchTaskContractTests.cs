using Xunit;
using System.Text.Json;

namespace TaskTracker.UnitTests.Features.Tasks.PatchTask;

public class PatchTaskContractTests
{
    [Fact]
    public void RequestContract_AllFieldsAreOptional()
    {
        var emptyRequest = new { };
        var json = JsonSerializer.Serialize(emptyRequest);
        var element = JsonDocument.Parse(json).RootElement;
        Assert.False(element.TryGetProperty("title", out _));
        Assert.False(element.TryGetProperty("description", out _));
        Assert.False(element.TryGetProperty("status", out _));
        Assert.False(element.TryGetProperty("date", out _));
        Assert.False(element.TryGetProperty("order", out _));
        Assert.False(element.TryGetProperty("assignee", out _));
        Assert.False(element.TryGetProperty("swimlane", out _));
    }

    [Fact]
    public void RequestContract_NullMeansClear_OptionalFields()
    {
        var request = new
        {
            assignee = (string?)null,
            swimlane = (string?)null,
            description = (string?)null
        };
        var json = JsonSerializer.Serialize(request);
        var element = JsonDocument.Parse(json).RootElement;
        Assert.True(element.TryGetProperty("assignee", out var assigneeProp));
        Assert.True(assigneeProp.ValueKind == JsonValueKind.Null);
        Assert.True(element.TryGetProperty("swimlane", out var swimlaneProp));
        Assert.True(swimlaneProp.ValueKind == JsonValueKind.Null);
        Assert.True(element.TryGetProperty("description", out var descProp));
        Assert.True(descProp.ValueKind == JsonValueKind.Null);
    }

    [Fact]
    public void RequestContract_EmptyStringMeansClear()
    {
        var request = new { assignee = "", swimlane = "" };
        var json = JsonSerializer.Serialize(request);
        var element = JsonDocument.Parse(json).RootElement;
        Assert.True(element.TryGetProperty("assignee", out var assigneeProp));
        Assert.Equal("", assigneeProp.GetString());
        Assert.True(element.TryGetProperty("swimlane", out var swimlaneProp));
        Assert.Equal("", swimlaneProp.GetString());
    }

    [Fact]
    public void RequestContract_SingleFieldPatch()
    {
        var request = new { status = "InProgress" };
        var json = JsonSerializer.Serialize(request);
        var element = JsonDocument.Parse(json).RootElement;
        Assert.True(element.TryGetProperty("status", out _));
        Assert.False(element.TryGetProperty("title", out _));
        Assert.False(element.TryGetProperty("swimlane", out _));
    }

    [Fact]
    public void RequestContract_MultipleFieldsPatch()
    {
        var request = new { status = "InProgress", order = 2, swimlane = "Фронтенд" };
        var json = JsonSerializer.Serialize(request);
        var element = JsonDocument.Parse(json).RootElement;
        Assert.True(element.TryGetProperty("status", out _));
        Assert.True(element.TryGetProperty("order", out _));
        Assert.True(element.TryGetProperty("swimlane", out _));
        Assert.False(element.TryGetProperty("title", out _));
    }
}