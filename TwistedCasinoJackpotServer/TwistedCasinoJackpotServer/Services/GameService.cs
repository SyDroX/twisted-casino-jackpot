using TwistedCasinoJackpotServer.Entities;

namespace TwistedCasinoJackpotServer.Services;

public class GameService
{
    private readonly Random                _random = new();
    private readonly Dictionary<char, int> _rewardTable;
    private readonly int                   _startingCredits;
    private readonly List<char>            _symbols;
    
    private readonly List<(int MinCredits, double CheatChance)> _cheatingRules;

    public GameService(IConfiguration configuration)
    {
        _startingCredits = ConfigureStartingCredits(configuration);
        _cheatingRules   = ConfigureCheatingRules(configuration);
        _rewardTable     = ConfigureRewards(configuration);
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

        // Apply cheating logic dynamically based on config
        double cheatChance = GetCheatChance(credits);
        
        if (isWinningRoll && cheatChance > 0 && _random.NextDouble() < cheatChance)
        {
            symbols       = GenerateSymbols();
            isWinningRoll = IsWinningRoll(symbols);
            reward        = isWinningRoll ? CalculateReward(symbols[0], isWinningRoll) : 0;
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

    private static int ConfigureStartingCredits(IConfiguration configuration)
    {
        return configuration.GetValue<int?>("GameSettings:StartingCredits") ??
               throw new InvalidOperationException("GameSettings:StartingCredits not set in appsettings.json");
    }

    private static Dictionary<char, int> ConfigureRewards(IConfiguration configuration)
    {
        Dictionary<string, int> rewardConfig = configuration.GetSection("GameSettings:Rewards").Get<Dictionary<string, int>>() ??
                                               throw new
                                                   InvalidOperationException("Error loading GameSettings:Rewards from appsettings.json");

        return rewardConfig.ToDictionary(pair => pair.Key[0], pair => pair.Value);
    }

    private static List<(int, double)> ConfigureCheatingRules(IConfiguration configuration)
    {
        // Load cheating rules as a List of Tuples 
        return configuration.GetSection("GameSettings:CheatingRules")
                            .Get<List<Dictionary<string, double>>>()?
                            .Select(rule => ((int)rule["MinCredits"], rule["CheatChance"]))
                            .OrderBy(rule => rule.Item1) // Ensure ordered ascending
                            .ToList() ??
               throw new InvalidOperationException("Error loading GameSettings:CheatingRules from appsettings.json");
    }
    
    private double GetCheatChance(int credits)
    {
        // Find the highest matching rule
        return _cheatingRules.LastOrDefault(rule => credits >= rule.MinCredits).CheatChance;
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