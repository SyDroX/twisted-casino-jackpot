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

        string[] symbols   = GenerateSymbols();
        bool     isWinning = IsWinningRoll(symbols);
        int      reward    = CalculateReward(symbols, isWinning);

        // Cheating logic
        if (isWinning && credits >= 40)
        {
            double cheatChance = credits >= 60 ? 0.6 : 0.3;
            
            if (_random.NextDouble() < cheatChance && reward > 0)
            {
                symbols   = GenerateSymbols(); 
                isWinning = IsWinningRoll(symbols); 
                reward    = isWinning ? CalculateReward(symbols, isWinning) : 0;
            }
        }
        
        int updatedCredits = isWinning ? credits + reward : credits - 1;

        return new RollResult(symbols, updatedCredits, isWinning, reward);
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