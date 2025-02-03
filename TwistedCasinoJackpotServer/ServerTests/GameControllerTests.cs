using System.Net;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace ServerTests;

public class GameControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GameControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task StartGame_ShouldReturnStartingCredits()
    {
        HttpResponseMessage response = await _client.GetAsync("/Game/start");
        var result   = await response.Content.ReadFromJsonAsync<dynamic>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(10,                result?.GetProperty("credits").GetInt32());
    }

    [Fact]
    public async Task StartGame_ShouldHandleException()
    {
        HttpResponseMessage response = await _client.GetAsync("/Game/start");
        var result   = await response.Content.ReadFromJsonAsync<dynamic>();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("An error occurred while starting the game.", (string)result!.Message);
    }
}