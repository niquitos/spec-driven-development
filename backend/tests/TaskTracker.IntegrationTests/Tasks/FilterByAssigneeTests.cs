using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace TaskTracker.IntegrationTests.Tasks;

public class FilterByAssigneeTests : IntegrationTestBase
{
    public FilterByAssigneeTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task GetTasks_WithAssigneesFilter_ReturnsOnlyMatchingTasks()
    {
        // Arrange
        var response1 = await PostAsync("/api/tasks", new
        {
            title = "Task 1",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = "Иван"
        });
        response1.EnsureSuccessStatusCode();

        var response2 = await PostAsync("/api/tasks", new
        {
            title = "Task 2",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 1,
            assignee = "Петр"
        });
        response2.EnsureSuccessStatusCode();

        var response3 = await PostAsync("/api/tasks", new
        {
            title = "Task 3",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 2,
            assignee = "Мария"
        });
        response3.EnsureSuccessStatusCode();

        // Act
        var getResponse = await Client.GetAsync("/api/tasks?date=2026-05-17&assignees=Иван,Петр");
        getResponse.EnsureSuccessStatusCode();
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>();

        // Assert
        Assert.NotNull(tasks);
        Assert.Equal(2, tasks.Count);
        Assert.All(tasks, t =>
        {
            var assignee = t.GetProperty("assignee").GetString();
            Assert.True(assignee == "Иван" || assignee == "Петр",
                $"Expected assignee Иван or Петр, got {assignee}");
        });
    }

    [Fact]
    public async Task GetTasks_WithSingleAssigneeFilter_ReturnsOnlyThatAssignee()
    {
        // Arrange
        await PostAsync("/api/tasks", new
        {
            title = "Task 1",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = "Иван"
        });

        await PostAsync("/api/tasks", new
        {
            title = "Task 2",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 1,
            assignee = "Петр"
        });

        // Act
        var getResponse = await Client.GetAsync("/api/tasks?date=2026-05-17&assignees=Иван");
        getResponse.EnsureSuccessStatusCode();
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>();

        // Assert
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("Иван", tasks[0].GetProperty("assignee").GetString());
    }

    [Fact]
    public async Task GetTasks_WithEmptyAssigneesFilter_ReturnsAllTasks()
    {
        // Arrange
        await PostAsync("/api/tasks", new
        {
            title = "Task 1",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = "Иван"
        });

        await PostAsync("/api/tasks", new
        {
            title = "Task 2",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 1,
            assignee = "Петр"
        });

        // Act — no assignees filter
        var getResponse = await Client.GetAsync("/api/tasks?date=2026-05-17");
        getResponse.EnsureSuccessStatusCode();
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>();

        // Assert
        Assert.NotNull(tasks);
        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task GetTasks_WithAssigneesFilter_ReturnsEmptyListWhenNoMatch()
    {
        // Arrange
        await PostAsync("/api/tasks", new
        {
            title = "Task 1",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = "Иван"
        });

        // Act
        var getResponse = await Client.GetAsync("/api/tasks?date=2026-05-17&assignees=Несуществующий");
        getResponse.EnsureSuccessStatusCode();
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<JsonElement>>();

        // Assert
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }
}
