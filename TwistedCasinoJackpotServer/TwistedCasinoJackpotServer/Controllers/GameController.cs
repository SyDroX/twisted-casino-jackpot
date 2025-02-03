using Microsoft.AspNetCore.Mvc;
using TwistedCasinoJackpotServer.Entities;
using TwistedCasinoJackpotServer.Services;

namespace TwistedCasinoJackpotServer.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService             _gameService;
    private readonly ILogger<GameController> _logger;

    public GameController(GameService gameService, ILogger<GameController> logger)
    {
        _gameService = gameService;
        _logger      = logger;
    }

    [HttpGet("start")]
    public IActionResult StartGame()
    {
        try
        {
            int credits = HttpContext.Session.GetInt32("Credits") ?? 0;

            if (credits == 0)
            {
                credits = _gameService.GetStartingCredits();
                HttpContext.Session.SetInt32("Credits", credits);
            }

            return Ok(new { Credits = credits });
        }
        catch (Exception exception)
        {
            return HandleExceptionResult(exception, "starting the game");
        }
    }

    [HttpPost("roll")]
    public IActionResult Roll()
    {
        const string errorName = "rolling";
        int? credits = HttpContext.Session.GetInt32("Credits");

        if (credits == null)
        {
            return HandleExceptionResult(new InvalidOperationException("Call to roll without a valid session with credits."), 
                                         errorName);
        }

        try
        {
            RollResult result = _gameService.Roll((int)credits);
            HttpContext.Session.SetInt32("Credits", result.Credits);

            return Ok(result);
        }
        catch (Exception exception)
        {
            return HandleExceptionResult(exception, errorName);
        }
    }

    [HttpPost("cashOut")]
    public IActionResult CashOut()
    {
        const string errorName = "cashing out";
        int?         credits    = HttpContext.Session.GetInt32("Credits");

        if (credits == null)
        {
            return HandleExceptionResult(new InvalidOperationException("Call to cash out without a valid session with credits."), 
                                         errorName);
        }

        try
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                Message = $"Cashed out successfully with {credits} credits!",
                Credits = credits
            });
        }
        catch (Exception ex)
        {
            return HandleExceptionResult(ex, errorName);
        }
    }

    private ObjectResult HandleExceptionResult(Exception ex, string errorName)
    {
        const string errorForUserFormat = "An error occurred while {0}. Please try again later.";
        const string errorForLogFormat  = "Error {0}.";

        _logger.LogError(ex, string.Format(errorForLogFormat, errorName));

        return StatusCode(StatusCodes.Status500InternalServerError,
                          new { Message = string.Format(errorForUserFormat, errorName) });
    }
}