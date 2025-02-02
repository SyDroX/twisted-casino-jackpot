using Microsoft.Extensions.Options;
using TwistedCasinoJackpotServer.Middleware;
using TwistedCasinoJackpotServer.Services;
using TwistedCasinoJackpotServer.Services.Configuration;

namespace TwistedCasinoJackpotServer;

public static class Program
{
    private const string CorsPolicyName = "DynamicCorsPolicy";

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDistributedMemoryCache(); // Ensure session storage is available
        builder.Services.AddControllers();
        builder.ConfigureAllowedHosts();
        builder.AddSessionService();
        builder.AddCorsService();
        
        builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings"));
        builder.Services.AddSingleton<GameService>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        WebApplication app = builder.Build();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        
        app.ValidateGameSettings();
        
        app.UseSession();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        
        
        if (app.Environment.IsDevelopment())
        {
           app.UseDevelopmentSettings();
        }
        else
        {
            app.UseProductionSettings();
        }
        
        app.UseCors(CorsPolicyName);
        app.Run();
    }

    private static void ConfigureAllowedHosts(this WebApplicationBuilder builder)
    {
        string? allowedHosts = builder.Configuration["AllowedHosts"];

        if (allowedHosts == null)
        {
            throw new Exception("AllowedHosts is not set in appsettings.json");
        }

        builder.WebHost.UseKestrel()
               .UseSetting("AllowedHosts", allowedHosts);
    }

    private static void AddSessionService(this WebApplicationBuilder builder)
    {
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout        = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly    = true;
            options.Cookie.IsEssential = true;
        });
    }

    private static void AddCorsService(this WebApplicationBuilder builder)
    {
        IConfigurationSection corsSettings = builder.Configuration.GetSection("Cors");

        string[] allowedOrigins   = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? [];
        string[] allowedMethods   = corsSettings.GetSection("AllowedMethods").Get<string[]>() ?? ["GET", "POST"];
        string[] allowedHeaders   = corsSettings.GetSection("AllowedHeaders").Get<string[]>() ?? ["Content-Type"];
        var      allowCredentials = corsSettings.GetValue<bool>("AllowCredentials");

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName,
                              policyBuilder =>
                              {
                                  if (builder.Environment.IsDevelopment())
                                  {
                                      policyBuilder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                                  }
                                  else
                                  {
                                      policyBuilder.WithOrigins(allowedOrigins);
                                  }

                                  policyBuilder.WithMethods(allowedMethods)
                                               .WithHeaders(allowedHeaders);

                                  if (allowCredentials)
                                  {
                                      policyBuilder.AllowCredentials();
                                  }
                                  else
                                  {
                                      policyBuilder.DisallowCredentials();
                                  }
                              });
        });
    }

    private static void ValidateGameSettings(this WebApplication app)
    {
        // Validate configuration after binding
        GameSettings? gameSettings = app.Services.GetRequiredService<IOptions<GameSettings>>().Value;

        if (gameSettings == null)
        {
            throw new InvalidOperationException("GameSettings configuration is missing in appsettings.json.");
        }

        if (gameSettings.StartingCredits <= 0)
        {
            throw new InvalidOperationException("GameSettings:StartingCredits must be greater than zero.");
        }

        if (gameSettings.Rewards == null || gameSettings.Rewards.Count == 0)
        {
            throw new InvalidOperationException("GameSettings:Rewards is missing or empty.");
        }

        if (gameSettings.CheatingRules == null || gameSettings.CheatingRules.Count == 0)
        {
            throw new InvalidOperationException("GameSettings:CheatingRules is missing or empty.");
        }
    }

    private static void UseDevelopmentSettings(this WebApplication app)
    {
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Twisted Casino Jackpot API v1");
            c.RoutePrefix = string.Empty; // Access Swagger UI at the root (http://localhost:XXXX/)
        });
        app.UseSwagger();
    }

    private static void UseProductionSettings(this WebApplication app)
    {
        app.UseHttpsRedirection();
    }
}