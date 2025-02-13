﻿using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TwistedCasinoJackpotServer.Entities;
using Xunit;
using Assert = Xunit.Assert;

namespace ServerTests;

public class GameControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient                _client;
    private readonly TestWebApplicationFactory _factory;

    public GameControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client  = _factory.CreateClient();
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
    public async Task Roll_ShouldRollResult_WhenRollSucceeds()
    {
        _factory.SetSessionValue("Credits", 10);
        
        HttpResponseMessage response = await _client.PostAsync("/Game/roll", null);
        var result   = await response.Content.ReadFromJsonAsync<RollResult>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(3, result.Symbols.Length);
        Assert.True(result.Credits > 0);
    }

    [Fact]
    public async Task Roll_ShouldReturnErrorMessage_WhenNoCredits()
    {
        _factory.SetSessionValue("Credits", 0);
        HttpResponseMessage response = await _client.PostAsync("/Game/roll", null);
        var result   = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Not enough credits to play", result.GetProperty("message").GetString());
    }

    [Fact]
    public async Task CashOut_ShouldReturnCashedOutCredits()
    {
        const int credits = 10;
        _factory.SetSessionValue("Credits", credits);
        HttpResponseMessage response = await _client.PostAsync("/Game/cashout", null);
        var                 result   = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK,                                  response.StatusCode);
        Assert.Equal(credits,                                            result.GetProperty("credits").GetInt32());
        Assert.Equal($"Cashed out successfully with {credits} credits!", result.GetProperty("message").GetString());
    }

    [Fact]
    public async Task CashOut_ShouldHandleException()
    {
        HttpResponseMessage response = await _client.PostAsync("/Game/cashout", null);
        var                 result   = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("An error occurred while cashing out.", result.GetProperty("message").GetString());
    }
}