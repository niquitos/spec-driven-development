using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using TaskTracker.Domain;

namespace TaskTracker.IntegrationTests.Tasks;

public class CreateTaskWithAssigneeTests : IntegrationTestBase
{
    public CreateTaskWithAssigneeTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateTask_WithAssignee_ReturnsTaskWithAssignee()
    {
        // Arrange
        var request = new
        {
            title = "Test Task",
            description = "Test Description",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = "Иван"
        };

        // Act
        var response = await PostAsync("/api/tasks", request);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.Equal("Иван", task.GetProperty("assignee").GetString());
    }

    [Fact]
    public async Task CreateTask_WithoutAssignee_ReturnsTaskWithNullAssignee()
    {
        // Arrange
        var request = new
        {
            title = "Test Task",
            description = "Test Description",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = (string?)null
        };

        // Act
        var response = await PostAsync("/api/tasks", request);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.True(task.TryGetProperty("assignee", out var assigneeProp));
        Assert.Equal(JsonValueKind.Null, assigneeProp.ValueKind);
    }

    [Fact]
    public async Task CreateTask_WithEmptyAssignee_ReturnsTaskWithNullAssignee()
    {
        // Arrange
        var request = new
        {
            title = "Test Task",
            description = "Test Description",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = ""
        };

        // Act
        var response = await PostAsync("/api/tasks", request);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.True(task.TryGetProperty("assignee", out var assigneeProp));
        Assert.True(assigneeProp.ValueKind == JsonValueKind.Null || assigneeProp.GetString() == null);
    }

    [Fact]
    public async Task CreateTask_WithAssignee_ResponseContainsAssigneeInList()
    {
        // Arrange
        var createRequest = new
        {
            title = "Test Task",
            description = "Test Description",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = "Петр"
        };

        var createResponse = await PostAsync("/api/tasks", createRequest);
        createResponse.EnsureSuccessStatusCode();

        // Act
        var getResponse = await Client.GetAsync("/api/tasks?date=2026-05-17");
        getResponse.EnsureSuccessStatusCode();
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>();

        // Assert
        Assert.NotNull(tasks);
        Assert.Contains(tasks, t => t.GetProperty("assignee").GetString() == "Петр");
    }
}
