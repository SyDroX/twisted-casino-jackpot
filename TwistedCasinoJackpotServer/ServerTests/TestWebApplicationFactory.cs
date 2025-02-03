using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.Options;
using TwistedCasinoJackpotServer;
using TwistedCasinoJackpotServer.Controllers;
using TwistedCasinoJackpotServer.Services;
using TwistedCasinoJackpotServer.Services.Configuration;

public class TestWebApplicationFactory : WebApplicationFactory<ProgramTestEntry>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing GameService if registered
            ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(GameService));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var mockGameSettings = new Mock<IOptions<GameSettings>>();
            var gameSettings = new GameSettings
            {
                StartingCredits = 10,
                Rewards         = new Dictionary<string, int> { { "C", 10 }, { "L", 20 }, { "O", 30 }, { "W", 40 } },
                CheatingRules =
                [
                    new CheatingRule
                    {
                        MinCredits  = 40,
                        CheatChance = 0.3
                    },
                    new CheatingRule
                    {
                        MinCredits  = 60,
                        CheatChance = 0.6
                    }
                ]
            };
            
            mockGameSettings.Setup(gs => gs.Value).Returns(gameSettings);
            services.AddSingleton(new GameService(mockGameSettings.Object, new Random()));
            
            var mockLogger = new Mock<ILogger<GameController>>();
            services.AddSingleton(mockLogger.Object);
        });
    }
}