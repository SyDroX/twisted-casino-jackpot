using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TwistedCasinoJackpotServer;
using TwistedCasinoJackpotServer.Controllers;
using TwistedCasinoJackpotServer.Services;
using TwistedCasinoJackpotServer.Services.Configuration;

namespace ServerTests;

public class TestWebApplicationFactory : WebApplicationFactory<ProgramTestEntry>
{
    private Dictionary<string, int> TestSession { get; } = new();
    
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

            var          mockGameSettings = new Mock<IOptions<GameSettings>>();
            GameSettings gameSettings     = GameSettingsTestHelper.GameSettingsMock;

            mockGameSettings.Setup(gs => gs.Value).Returns(gameSettings);
            services.AddSingleton(new GameService(mockGameSettings.Object, new Random()));

            var mockLogger = new Mock<ILogger<GameController>>();
            services.AddSingleton(mockLogger.Object);

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout        = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly    = true;
                options.Cookie.IsEssential = true;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers().AddApplicationPart(typeof(GameController).Assembly);
        });
        
        builder.Configure(app =>
        {
            app.UseRouting();
            app.UseSession();
            app.Use(async (context, next) =>
            {
                SetSessionValues(context);
                await next();
            });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        });
    }

    private void SetSessionValues(HttpContext context)
    {
        foreach ((string key, int value) in TestSession)
        {
            context.Session.SetInt32(key, value);
        }
    }
    public void SetSessionValue(string key, int value)
    {
        TestSession[key] = value;
    }
}