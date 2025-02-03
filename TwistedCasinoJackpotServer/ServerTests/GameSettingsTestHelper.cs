using TwistedCasinoJackpotServer.Services.Configuration;

namespace ServerTests;

public class GameSettingsTestHelper
{
    public static GameSettings GameSettingsMock =  new GameSettings
    {
        StartingCredits = 10,
        Rewards = new Dictionary<string, int>
        {
            { "C", 10 },
            { "L", 20 },
            { "O", 30 },
            { "W", 40 }
        },
        CheatingRules =
        [
            new CheatingRule
            {
                MinCredits  = 40,
                CheatChance = 0.3
            },
            new CheatingRule
            {
                MinCredits  = 60,
                CheatChance = 0.6
            }
        ]
    };
}