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
        {
            throw new InvalidOperationException("Not enough credits to play.");
        }

        string[] symbols       = GenerateSymbols();
        bool     isWinningRoll = IsWinningRoll(symbols);
        int      reward        = CalculateReward(symbols, isWinningRoll);

        // Cheating logic
        if (isWinningRoll && credits >= 40)
        {
            double cheatChance = credits >= 60 ? 0.6 : 0.3;

            if (_random.NextDouble() < cheatChance && reward > 0)
            {
                symbols       = GenerateSymbols();
                isWinningRoll = IsWinningRoll(symbols);
                reward        = isWinningRoll ? CalculateReward(symbols, isWinningRoll) : 0;
            }
        }

        int updatedCredits = isWinningRoll ? credits + reward : credits - 1;

        return new RollResult(symbols, updatedCredits, isWinningRoll, reward, GetRollResultMessage(isWinningRoll));
    }

    private static bool IsWinningRoll(string[] symbols)
    {
        return symbols.Distinct().Count() == 1;
    }

    private static int CalculateReward(string[] symbols, bool isWinning)
    {
        if (!isWinning) return 0;

        return symbols[0] switch
        {
            "C" => 10, // Cherry
            "L" => 20, // Lemon
            "O" => 30, // Orange
            "W" => 40, // Watermelon
            _ => 0
        };
    }

    private static string GetRollResultMessage(bool won)
    {
        return won ? "You won!" : "You lost!";
    }

    private string[] GenerateSymbols()
    {
        return
        [
            Symbols[_random.Next(Symbols.Length)],
            Symbols[_random.Next(Symbols.Length)],
            Symbols[_random.Next(Symbols.Length)]
        ];
    }
}