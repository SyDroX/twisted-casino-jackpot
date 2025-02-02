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
        int credits = HttpContext.Session.GetInt32("Credits") ?? 0;

        if (credits <= 0)
        {
            return Ok(new
            {
                Message = "You have no more credits to play. Please restart the game.",
                Credits = 0
            });
        }

        try
        {
            RollResult result = _gameService.Roll(credits);
            HttpContext.Session.SetInt32("Credits", result.Credits);

            return Ok(result);
        }
        catch (Exception exception)
        {
            return HandleExceptionResult(exception, "rolling");
        }
    }

    [HttpPost("cashout")]
    public IActionResult CashOut()
    {
        int credits = HttpContext.Session.GetInt32("Credits") ?? 0;

        try
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                Message = "Cashed out successfully!",
                Credits = credits
            });
        }
        catch (Exception ex)
        {
            return HandleExceptionResult(ex, "cashing out");
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