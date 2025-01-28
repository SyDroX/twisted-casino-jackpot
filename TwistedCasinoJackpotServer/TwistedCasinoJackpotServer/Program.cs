using TwistedCasinoJackpotServer.Middleware;
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
            options.IdleTimeout        = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly    = true;
            options.Cookie.IsEssential = true;
        });
        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Localhost",
                              policyBuilder =>
                              {
                                  // Accept any port on localhost
                                  policyBuilder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                                               .AllowAnyHeader()
                                               .AllowAnyMethod();
                              });
            options.AddPolicy("Production",
                              policyBuilder =>
                              {
                                  policyBuilder
                                      .WithOrigins("https://myproductiondomain.com") // Add allowed origins
                                      .AllowAnyHeader() // Allow any HTTP header
                                      .AllowAnyMethod(); // Allow any HTTP method (GET, POST, etc.)
                              });
        });
        builder.Services.AddSingleton<GameService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Casino API v1");
                c.RoutePrefix = string.Empty; // Access Swagger UI at the root (http://localhost:XXXX/)
            });
            app.UseSwagger();
            app.UseCors("Localhost");
        }
        else
        {
            app.UseCors("Production");
        }

        app.UseSession();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}