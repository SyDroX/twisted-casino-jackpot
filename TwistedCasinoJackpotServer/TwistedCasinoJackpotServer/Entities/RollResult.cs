namespace TwistedCasinoJackpotServer.Entities;

public class RollResult
{
    public char[] Symbols   { get; set; }
    public int    Credits   { get; set; }
    public bool   IsWinning { get; set; }
    public string Message   { get; set; }

    public RollResult(char[] symbols, int credits, bool isWinning, string message = "")
    {
        Symbols   = symbols;
        Credits   = credits;
        IsWinning = isWinning;
        Message   = message;
    }
}