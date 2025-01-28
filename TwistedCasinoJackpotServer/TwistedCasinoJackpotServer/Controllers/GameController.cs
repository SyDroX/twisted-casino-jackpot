using Microsoft.AspNetCore.Mvc;
using TwistedCasinoJackpotServer.Entities;
using TwistedCasinoJackpotServer.Services;

namespace TwistedCasinoJackpotServer.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("start")]
    public IActionResult StartGame()
    {
        int credits = GameService.StartGame();
        HttpContext.Session.SetInt32("Credits", credits);

        return Ok(new { Credits = credits });
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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Message = $"An unexpected error occurred: {ex.Message}"
            });
        }
    }

    [HttpPost("cashout")]
    public IActionResult CashOut()
    {
        int credits = HttpContext.Session.GetInt32("Credits") ?? 0;
        HttpContext.Session.Clear();

        return Ok(new
        {
            Message = "Cashed out successfully!",
            Credits = credits
        });
    }
}