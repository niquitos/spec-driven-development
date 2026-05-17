using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace TaskTracker.IntegrationTests.Tasks;

public class DiagnosticTest : IntegrationTestBase
{
    public DiagnosticTest(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Debug_Get_Response()
    {
        // First create a task
        var postResponse = await PostAsync("/api/tasks", new
        {
            title = "Debug Task",
            description = "",
            date = "2026-05-17T00:00:00",
            status = 0,
            order = 0,
            assignee = "Иван"
        });
        
        var postContent = await postResponse.Content.ReadAsStringAsync();
        Assert.True(postResponse.IsSuccessStatusCode, $"POST failed: {postResponse.StatusCode} - {postContent}");
        
        var created = JsonSerializer.Deserialize<JsonElement>(postContent);
        var createdId = created.GetProperty("id").GetInt32();
        
        // Now get by ID
        var getByIdResponse = await Client.GetAsync($"/api/tasks/{createdId}");
        var getByIdContent = await getByIdResponse.Content.ReadAsStringAsync();
        Assert.True(getByIdResponse.IsSuccessStatusCode, $"GET by ID failed: {getByIdResponse.StatusCode} - {getByIdContent}");
        
        // Now get by date
        var getByDateResponse = await Client.GetAsync("/api/tasks?date=2026-05-17");
        var getByDateContent = await getByDateResponse.Content.ReadAsStringAsync();
        Assert.True(getByDateResponse.IsSuccessStatusCode, $"GET by date failed: {getByDateResponse.StatusCode} - {getByDateContent}");
        
        var tasks = JsonSerializer.Deserialize<List<JsonElement>>(getByDateContent);
        Assert.NotNull(tasks);
        Assert.NotEmpty(tasks);
        Assert.Single(tasks);
        Assert.Equal("Иван", tasks[0].GetProperty("assignee").GetString());
    }
}
