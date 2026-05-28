using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Domain = TaskTracker.Domain;

namespace TaskTracker.IntegrationTests.Tasks;

public class PatchTaskTests : IntegrationTestBase
{
    public PatchTaskTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    private async Task<HttpResponseMessage> CreateTaskAsync(object taskData)
    {
        return await PostAsync("/api/tasks", taskData);
    }

    private async Task<int> CreateTaskAndGetIdAsync(string title = "Test Task", string? swimlane = null, string? assignee = null)
    {
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var taskData = new
        {
            Title = title,
            Date = date,
            Status = 0,
            Swimlane = swimlane,
            Assignee = assignee
        };
        var response = await CreateTaskAsync(taskData);
        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<JsonElement>();
        return task.GetProperty("id").GetInt32();
    }

    private async Task<JsonElement> GetTaskByIdAsync(int taskId)
    {
        var response = await Client.GetAsync($"/api/tasks/{taskId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    [Fact]
    public async Task PatchTask_UpdatesOnlyStatus()
    {
        var taskId = await CreateTaskAndGetIdAsync("Original", swimlane: "Фронтенд", assignee: "Анна");

        var patchData = new { status = "InProgress" };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.Equal((int)Domain.TaskStatus.InProgress, task.GetProperty("status").GetInt32());
        Assert.Equal("Фронтенд", task.GetProperty("swimlane").GetString());
        Assert.Equal("Анна", task.GetProperty("assignee").GetString());
        Assert.Equal("Original", task.GetProperty("title").GetString());
    }

    [Fact]
    public async Task PatchTask_EmptyBody_NoChanges()
    {
        var taskId = await CreateTaskAndGetIdAsync("Test", swimlane: "Бэкенд");

        var patchData = new { };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var task = await GetTaskByIdAsync(taskId);
        Assert.Equal("Бэкенд", task.GetProperty("swimlane").GetString());
        Assert.Equal("Test", task.GetProperty("title").GetString());
    }

    [Fact]
    public async Task PatchTask_NonExistentTask_ReturnsNotFound()
    {
        var patchData = new { title = "Updated" };
        var response = await Client.PatchAsync("/api/tasks/99999", JsonContent.Create(patchData));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchTask_ClearsSwimlane()
    {
        var taskId = await CreateTaskAndGetIdAsync("Task", swimlane: "Дизайн");

        var patchData = new { swimlane = "" };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.True(task.GetProperty("swimlane").ValueKind == JsonValueKind.Null
            || string.IsNullOrEmpty(task.GetProperty("swimlane").GetString()));
    }

    [Fact]
    public async Task PatchTask_ClearsAssignee()
    {
        var taskId = await CreateTaskAndGetIdAsync("Task", assignee: "Мария");

        var patchData = new { assignee = "" };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.True(task.GetProperty("assignee").ValueKind == JsonValueKind.Null
            || string.IsNullOrEmpty(task.GetProperty("assignee").GetString()));
    }

    [Fact]
    public async Task PatchTask_UpdatesSwimlane()
    {
        var taskId = await CreateTaskAndGetIdAsync("Task", swimlane: "Старый");

        var patchData = new { swimlane = "Новый" };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.Equal("Новый", task.GetProperty("swimlane").GetString());
    }

    [Fact]
    public async Task PatchTask_MultipleFields_UpdatesAll()
    {
        var taskId = await CreateTaskAndGetIdAsync("Original", swimlane: "Старый", assignee: "Анна");

        var patchData = new { title = "Updated Title", status = "Done", swimlane = "Новый" };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.Equal("Updated Title", task.GetProperty("title").GetString());
        Assert.Equal((int)Domain.TaskStatus.Done, task.GetProperty("status").GetInt32());
        Assert.Equal("Новый", task.GetProperty("swimlane").GetString());
        Assert.Equal("Анна", task.GetProperty("assignee").GetString());
    }

    [Fact]
    public async Task PatchTask_NormalizesWhitespaceSwimlane()
    {
        var taskId = await CreateTaskAndGetIdAsync("Task", swimlane: "Старый");

        var patchData = new { swimlane = "   " };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.True(task.GetProperty("swimlane").ValueKind == JsonValueKind.Null
            || string.IsNullOrEmpty(task.GetProperty("swimlane").GetString()));
    }

    [Fact]
    public async Task PatchTask_NormalizesWhitespaceAssignee()
    {
        var taskId = await CreateTaskAndGetIdAsync("Task", assignee: "Анна");

        var patchData = new { assignee = "   " };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.True(task.GetProperty("assignee").ValueKind == JsonValueKind.Null
            || string.IsNullOrEmpty(task.GetProperty("assignee").GetString()));
    }

    [Fact]
    public async Task PatchTask_UpdatesTitle()
    {
        var taskId = await CreateTaskAndGetIdAsync("Old Title");

        var patchData = new { title = "New Title" };
        var response = await Client.PatchAsync($"/api/tasks/{taskId}", JsonContent.Create(patchData));

        response.EnsureSuccessStatusCode();

        var task = await GetTaskByIdAsync(taskId);
        Assert.Equal("New Title", task.GetProperty("title").GetString());
    }
}