namespace TwistedCasinoJackpotServer.Entities;

public class RollResult
{
    public string[] Symbols        { get; set; }
    public int      UpdatedCredits { get; set; }
    public int      Reward         { get; set; }
    public bool     Won            { get; set; }

    public RollResult(string[] symbols, int updatedCredits, bool won, int reward)
    {
        Symbols        = symbols;
        UpdatedCredits = updatedCredits;
        Won            = won;
        Reward         = reward;
    }
}