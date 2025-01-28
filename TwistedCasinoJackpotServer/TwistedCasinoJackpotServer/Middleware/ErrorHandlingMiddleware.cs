namespace TwistedCasinoJackpotServer.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate                  _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Call the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An unhandled exception occurred");

            // Return a standardized error response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = StatusCodes.Status500InternalServerError;

            var errorResponse = new
            {
                Message = "An unexpected error occurred. Please try again later.",
                Details = ex.Message // For development, you might include more detailed info
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}