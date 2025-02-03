using System.Reflection;
using Microsoft.Extensions.Options;
using Moq;
using TwistedCasinoJackpotServer.Entities;
using TwistedCasinoJackpotServer.Services;
using TwistedCasinoJackpotServer.Services.Configuration;
using Xunit;
using Assert = Xunit.Assert;

namespace ServerTests;

public class GameServiceTests
{
    private readonly Mock<Random>                 _mockRandom;
    private readonly GameService                  _gameService;
    private readonly Mock<IOptions<GameSettings>> _mockGameSettings;
    private readonly GameSettings                 _testSettings;

    public GameServiceTests()
    {
        _testSettings = new GameSettings
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

        _mockGameSettings = new Mock<IOptions<GameSettings>>();
        _mockGameSettings.Setup(gs => gs.Value).Returns(_testSettings);

        _mockRandom  = new Mock<Random>();
        _gameService = new GameService(_mockGameSettings.Object, _mockRandom.Object);
    }

    [Fact]
    public void GetStartingCredits_ShouldReturnConfiguredValue()
    {
        int startingCredits = _gameService.GetStartingCredits();

        Assert.Equal(10, startingCredits);
    }
    
    [Fact]
    public void Roll_WithLosingSymbols_ShouldDeductCredits()
    {
        // Mock random to pick different indices for slots (not a win)
        _mockRandom
            .SetupSequence(r => r.Next(It.IsAny<int>()))
            .Returns(0)  // First symbol (C)
            .Returns(1)  // Second symbol (L)
            .Returns(2); // Third symbol (O) - Different symbols mean no win

        const int initialCredits = 10;
        RollResult       result         = _gameService.Roll(initialCredits);

        Assert.False(result.IsWinning);
        Assert.Equal(initialCredits - 1, result.Credits); // Loses 1 credit
    }

    [Fact]
    public void Roll_WithCreditsOverThreshold_ForceLoosingReRoll()
    {
        _mockRandom
            .SetupSequence(r => r.Next(It.IsAny<int>()))
            .Returns(0)  // First roll: Winning 
            .Returns(0)  
            .Returns(0)
            .Returns(1)  // Second roll: loosing
            .Returns(2)  
            .Returns(3);

        _mockRandom.Setup(r => r.NextDouble()).Returns(0.2);

        const int  initialCredits = 50; // Enough credits to trigger cheating
        RollResult result         = _gameService.Roll(initialCredits);

        Assert.False(result.IsWinning); // Cheat should make it a loss
        Assert.Equal(initialCredits - 1, result.Credits);
    }
    
    
    [Fact]
    public void Roll_WithZeroCredits_ShouldThrowException()
    {
        Assert.Throws<InvalidOperationException>(() => _gameService.Roll(0));
    }
    
    [Xunit.Theory]
    [InlineData('C', true,  10)] // Cherry wins 10 credits
    [InlineData('L', true,  20)] // Lemon wins 20 credits
    [InlineData('O', true,  30)] // Orange wins 30 credits
    [InlineData('W', true,  40)] // Watermelon wins 40 credits
    [InlineData('X', false, 0)] // Invalid symbol should lose
    public void Roll_ShouldReturnExpectedRewards(char symbol, bool isWinning, int expectedReward)
    {
        // Recreate _symbols list from GameSettings
        var symbolsList = new List<char>(_testSettings.Rewards.Keys.Select(k => k[0]));

        if (isWinning)
        {
            // Find the valid index for the winning symbol
            int symbolIndex = symbolsList.IndexOf(symbol);
            
            // Mock Random to always roll a winning combination
            _mockRandom
                .SetupSequence(r => r.Next(It.IsAny<int>()))
                .Returns(symbolIndex) // First slot
                .Returns(symbolIndex)
                .Returns(symbolIndex); // All match = winning roll
        }
        else
        {

            _mockRandom
                .SetupSequence(r => r.Next(It.IsAny<int>()))
                .Returns(0) // 'C'
                .Returns(1) // 'L'
                .Returns(2); // 'O'
        }

        const int initialCredits = 10;
        RollResult       result         = _gameService.Roll(initialCredits);

        if (isWinning)
        {
            Assert.True(result.IsWinning);
            Assert.Equal(initialCredits + expectedReward, result.Credits);
        }
        else
        {
            Assert.False(result.IsWinning);
            Assert.Equal(initialCredits - 1, result.Credits);
        }
    }
    
    [Xunit.Theory]
    [InlineData(39, 0)]
    [InlineData(40, 0.3)]
    [InlineData(59, 0.3)]
    [InlineData(60, 0.6)]
    public void GetCheatChance_ShouldReturnExpectedValue(int credits, double expectedCheatChance)
    {
        // Use reflection to access the private method
        MethodInfo? methodInfo = typeof(GameService).GetMethod("GetCheatChance", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (methodInfo == null)
            throw new Exception("GetCheatChance() method not found.");
        
        // Call GetCheatChance() using reflection
        var cheatChance = (double)(methodInfo.Invoke(_gameService, [credits]) ?? throw new Exception("GetCheatChance() returned null."));

        Assert.Equal(expectedCheatChance, cheatChance);
    }
}