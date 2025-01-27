using TwistedCasinoJackpotServer.Entities;

namespace TwistedCasinoJackpotServer.Services;

public class GameService
{
    private const int StartingCredits = 10;

    private static readonly string[] Symbols = ["C", "L", "O", "W"];

    private readonly Random _random = new();

    public static int StartGame()
    {
        return StartingCredits;
    }

    public RollResult Roll(int credits)
    {
        if (credits <= 0)
            throw new InvalidOperationException("Not enough credits to play.");

        string[] rollResult = GenerateRoll();

        bool isWinning = rollResult.Distinct().Count() == 1;
        var  reward    = 0;

        if (isWinning)
        {
            reward = rollResult[0] switch
            {
                "C" => 10, // Cherry
                "L" => 20, // Lemon
                "O" => 30, // Orange
                "W" => 40, // Watermelon
                _ => 0
            };
        }

        // Cheating logic
        if (credits >= 40)
        {
            double cheatChance = credits >= 60 ? 0.6 : 0.3;

            if (_random.NextDouble() < cheatChance && reward > 0)
            {
                rollResult = GenerateRoll();
                // Check is winning again?
            }
        }

        int updatedCredits = isWinning ? credits + reward : credits - 1;

        return new RollResult(rollResult, reward, isWinning, updatedCredits);
    }

    private string[] GenerateRoll()
    {
        return
        [
            Symbols[_random.Next(Symbols.Length)],
            Symbols[_random.Next(Symbols.Length)],
            Symbols[_random.Next(Symbols.Length)]
        ];
    }
}