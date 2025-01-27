using TwistedCasinoJackpotServer.Services;

namespace TwistedCasinoJackpotServer;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout        = TimeSpan.FromMinutes(30); // Session timeout
            options.Cookie.HttpOnly    = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddSingleton<GameService>();
        
        WebApplication app = builder.Build();

        app.UseSession();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}