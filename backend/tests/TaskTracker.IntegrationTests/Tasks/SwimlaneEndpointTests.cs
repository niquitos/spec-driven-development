using System.Net;
using System.Net.Http.Json;
using Xunit;
using TaskTracker.Domain;

namespace TaskTracker.IntegrationTests.Tasks;

public class SwimlaneEndpointTests : IntegrationTestBase
{
    public SwimlaneEndpointTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    private async Task<HttpResponseMessage> CreateTaskAsync(object taskData)
    {
        return await PostAsync("/api/tasks", taskData);
    }

    [Fact]
    public async Task GetSwimlanes_ReturnsUniqueSwimlanes()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        await CreateTaskAsync(new { Title = "Task 1", Date = date, Status = 0, Swimlane = "Фронтенд" });
        await CreateTaskAsync(new { Title = "Task 2", Date = date, Status = 0, Swimlane = "Бэкенд" });
        await CreateTaskAsync(new { Title = "Task 3", Date = date, Status = 0, Swimlane = "Фронтенд" });

        // Act
        var result = await GetAsync<string[]>($"/api/tasks/swimlanes?date={date}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
    }

    [Fact]
    public async Task GetSwimlanes_ReturnsEmptyArray_WhenNoSwimlanes()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        await CreateTaskAsync(new { Title = "Task 1", Date = date, Status = 0 });

        // Act
        var result = await GetAsync<string[]>($"/api/tasks/swimlanes?date={date}");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSwimlanes_ReturnsBadRequest_WhenDateMissing()
    {
        // Act
        var response = await Client.GetAsync("/api/tasks/swimlanes");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithSwimlane_ReturnsCreated()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var taskData = new { Title = "Swimlane Task", Date = date, Status = 0, Swimlane = "Фронтенд" };

        // Act
        var response = await CreateTaskAsync(taskData);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskEntity>();
        Assert.NotNull(task);
        Assert.Equal("Фронтенд", task.Swimlane);
    }

    [Fact]
    public async Task CreateTask_WithSwimlaneExceedingMaxLength_ReturnsBadRequest()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var longSwimlane = new string('A', 101);
        var taskData = new { Title = "Long Swimlane Task", Date = date, Status = 0, Swimlane = longSwimlane };

        // Act
        var response = await CreateTaskAsync(taskData);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithWhitespaceSwimlane_NormalizesToNull()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var taskData = new { Title = "Whitespace Swimlane Task", Date = date, Status = 0, Swimlane = "   " };

        // Act
        var response = await CreateTaskAsync(taskData);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskEntity>();
        Assert.NotNull(task);
        Assert.Null(task.Swimlane);
    }

    [Fact]
    public async Task UpdateTask_WithSwimlane_ReturnsNoContent()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var createResponse = await CreateTaskAsync(new { Title = "Task", Date = date, Status = 0 });
        var created = await createResponse.Content.ReadFromJsonAsync<TaskEntity>();

        // Act
        var updateData = new { Title = "Updated Task", Description = (string?)null, Date = date, Status = 0, Order = 0, Assignee = (string?)null, Swimlane = "Бэкенд" };
        var response = await PutAsync($"/api/tasks/{created!.Id}", updateData);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify swimlane was updated
        var updated = await GetAsync<TaskEntity>($"/api/tasks/{created.Id}");
        Assert.Equal("Бэкенд", updated!.Swimlane);
    }

    [Fact]
    public async Task UpdateTask_ClearSwimlane_ReturnsNoContent()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var createResponse = await CreateTaskAsync(new { Title = "Task", Date = date, Status = 0, Swimlane = "Фронтенд" });
        var created = await createResponse.Content.ReadFromJsonAsync<TaskEntity>();

        // Act
        var updateData = new { Title = "Updated Task", Description = (string?)null, Date = date, Status = 0, Order = 0, Assignee = (string?)null, Swimlane = (string?)null };
        var response = await PutAsync($"/api/tasks/{created!.Id}", updateData);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updated = await GetAsync<TaskEntity>($"/api/tasks/{created.Id}");
        Assert.Null(updated!.Swimlane);
    }

    [Fact]
    public async Task GetTasks_FilterBySwimlanes_ReturnsMatchingTasks()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        await CreateTaskAsync(new { Title = "Task 1", Date = date, Status = 0, Swimlane = "Фронтенд" });
        await CreateTaskAsync(new { Title = "Task 2", Date = date, Status = 0, Swimlane = "Бэкенд" });
        await CreateTaskAsync(new { Title = "Task 3", Date = date, Status = 0 });

        // Act
        var result = await GetAsync<TaskEntity[]>($"/api/tasks?date={date}&swimlanes=Фронтенд");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Фронтенд", result[0].Swimlane);
    }

    [Fact]
    public async Task GetTasks_FilterByAssigneesAndSwimlanes_ReturnsIntersection()
    {
        // Arrange
        var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        await CreateTaskAsync(new { Title = "Task 1", Date = date, Status = 0, Assignee = "Иван", Swimlane = "Фронтенд" });
        await CreateTaskAsync(new { Title = "Task 2", Date = date, Status = 0, Assignee = "Петр", Swimlane = "Фронтенд" });
        await CreateTaskAsync(new { Title = "Task 3", Date = date, Status = 0, Assignee = "Иван", Swimlane = "Бэкенд" });

        // Act
        var result = await GetAsync<TaskEntity[]>($"/api/tasks?date={date}&assignees=Иван&swimlanes=Фронтенд");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Иван", result[0].Assignee);
        Assert.Equal("Фронтенд", result[0].Swimlane);
    }
}