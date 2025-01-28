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
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        WebApplication app = builder.Build();
        
        // Enable middleware to serve Swagger and Swagger UI
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Casino API v1");
                c.RoutePrefix = string.Empty; // Access Swagger UI at the root (http://localhost:5000/)
            });
        }
        
        app.UseSession();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}