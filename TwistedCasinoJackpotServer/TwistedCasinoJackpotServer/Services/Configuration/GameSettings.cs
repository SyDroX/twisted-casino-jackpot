namespace TwistedCasinoJackpotServer.Services.Configuration;

public class GameSettings
{
    public int                     StartingCredits { get; init; }
    public Dictionary<string, int> Rewards         { get; init; }  = new();
    public List<CheatingRule>      CheatingRules   { get; init; } = [];
}

public class CheatingRule
{
    public int    MinCredits;
    public double CheatChance;
}


