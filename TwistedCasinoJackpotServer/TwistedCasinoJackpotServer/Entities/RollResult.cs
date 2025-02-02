namespace TwistedCasinoJackpotServer.Entities;

public class RollResult
{
    public string[] Symbols   { get; set; }
    public int      Credits   { get; set; }
    public bool     IsWinning { get; set; }
    public string   Message   { get; set; }

    public RollResult(string[] symbols, int credits, bool isWinning, string message = "")
    {
        Symbols   = symbols;
        Credits   = credits;
        IsWinning = isWinning;
        Message   = message;
    }
}