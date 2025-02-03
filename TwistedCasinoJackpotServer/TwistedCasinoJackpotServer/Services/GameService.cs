using Microsoft.Extensions.Options;
using TwistedCasinoJackpotServer.Entities;
using TwistedCasinoJackpotServer.Services.Configuration;

namespace TwistedCasinoJackpotServer.Services;

public class OutOfCreditsException(string message) : Exception(message);

public class GameService
{
    private readonly Random       _random;
    private readonly GameSettings _gameSettings;
    private readonly List<char>   _symbols;

    public GameService(IOptions<GameSettings> gameSettings, Random? random = null)
    {
        _gameSettings = gameSettings.Value;
        _symbols      = new List<char>(_gameSettings.Rewards.Keys.Count);
        _symbols.AddRange(_gameSettings.Rewards.Keys.Select(key => key[0]));
        _random       = random ?? new Random();
    }

    public int GetStartingCredits()
    {
        return _gameSettings.StartingCredits;
    }

    public RollResult Roll(int credits)
    {
        if (credits <= 0)
        {
            throw new OutOfCreditsException("Not enough credits to play.");
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
        return !isWinning ? 0 : _gameSettings.Rewards.GetValueOrDefault(symbol.ToString(), 0);
    }

    private static string GetRollResultMessage(bool won, int reward)
    {
        return won ? $"You won {reward}! credits" : "You lost!";
    }
    
    private double GetCheatChance(int credits)
    {
        CheatingRule? cheatRule = _gameSettings.CheatingRules.LastOrDefault(rule => credits >= rule.MinCredits);
        
        return cheatRule?.CheatChance ?? 0 ;
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