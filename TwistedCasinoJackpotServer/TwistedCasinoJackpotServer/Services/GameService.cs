using TwistedCasinoJackpotServer.Entities;

namespace TwistedCasinoJackpotServer.Services;

public class GameService
{
    private readonly Random                _random = new();
    private readonly Dictionary<char, int> _rewardTable;
    private readonly int                   _startingCredits;
    private readonly List<char>            _symbols;

    public GameService(IConfiguration configuration)
    {
        Dictionary<string, int> rewardConfig = configuration.GetSection("GameSettings:Rewards").Get<Dictionary<string, int>>() ??
                                               throw new
                                                   InvalidOperationException("Error loading GameSettings:Rewards from appsettings.json");
        _rewardTable     = rewardConfig.ToDictionary(kvp => kvp.Key[0], kvp => kvp.Value);
        _startingCredits = configuration.GetValue<int?>("GameSettings:StartingCredits") 
                        ?? throw new InvalidOperationException("GameSettings:StartingCredits not set in appsettings.json");
        _symbols         = new List<char>(_rewardTable.Keys.Count);
        _symbols.AddRange(_rewardTable.Keys);
    }

    public int GetStartingCredits()
    {
        return _startingCredits;
    }

    public RollResult Roll(int credits)
    {
        if (credits <= 0)
        {
            throw new InvalidOperationException("Not enough credits to play.");
        }

        char[] symbols       = GenerateSymbols();
        bool   isWinningRoll = IsWinningRoll(symbols);
        int    reward        = CalculateReward(symbols[0], isWinningRoll);

        // Cheating logic
        if (isWinningRoll && credits >= 40)
        {
            double cheatChance = credits >= 60 ? 0.6 : 0.3;

            if (_random.NextDouble() < cheatChance && reward > 0)
            {
                symbols       = GenerateSymbols();
                isWinningRoll = IsWinningRoll(symbols);
                reward        = isWinningRoll ? CalculateReward(symbols[0], isWinningRoll) : 0;
            }
        }

        int updatedCredits = isWinningRoll ? credits + reward : credits - 1;

        return new RollResult(symbols, updatedCredits, isWinningRoll, GetRollResultMessage(isWinningRoll, reward));
    }

    private static bool IsWinningRoll(char[] symbols)
    {
        return symbols.Distinct().Count() == 1;
    }

    private int CalculateReward(char symbol, bool isWinning)
    {
        return !isWinning ? 0 : _rewardTable.GetValueOrDefault(symbol, 0);
    }

    private static string GetRollResultMessage(bool won, int reward)
    {
        return won ? $"You won {reward}! credits" : "You lost!";
    }

    private char[] GenerateSymbols()
    {
        return
        [
            _symbols[_random.Next(_symbols.Count)],
            _symbols[_random.Next(_symbols.Count)],
            _symbols[_random.Next(_symbols.Count)]
        ];
    }
}