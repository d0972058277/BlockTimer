using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace BlockTimer.Tests;

public class BlockTimerTests
{
    [Theory]
    [InlineData(100, 0, LogLevel.Information)]
    [InlineData(100, 200, LogLevel.Warning)]
    public void 測量Aciton_應該進行記錄(long warningThreshold, int delay, LogLevel expectLogLevel)
    {
        // Given
        var logger = new Mock<ILogger<BlockTimer>>();

        var services = new ServiceCollection();
        services.AddTransient(sp => logger.Object);
        services.AddBlockTimer(o => o.WarningThreshold = warningThreshold);

        var provider = services.BuildServiceProvider();
        var blockTimer = provider.GetRequiredService<IBlockTimer>();

        var action = () =>
        {
            Task.Delay(delay).Wait();
        };

        // When
        blockTimer.Measure(action);

        // Then
        var regex = new Regex("Action costs .+ ms.");
        logger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == expectLogLevel),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((obj, type) => regex.Match(obj.ToString()!).Success),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Theory]
    [InlineData(100, 0, LogLevel.Information)]
    [InlineData(100, 200, LogLevel.Warning)]
    public void 測量Func_應該進行記錄(long warningThreshold, int delay, LogLevel expectLogLevel)
    {
        // Given
        var logger = new Mock<ILogger<BlockTimer>>();

        var services = new ServiceCollection();
        services.AddTransient(sp => logger.Object);
        services.AddBlockTimer(o => o.WarningThreshold = warningThreshold);

        var provider = services.BuildServiceProvider();
        var blockTimer = provider.GetRequiredService<IBlockTimer>();

        var func = () =>
        {
            Task.Delay(delay).Wait();
            return true;
        };

        // When
        var result = blockTimer.Measure(func);

        // Then
        result.Should().BeTrue();
        var regex = new Regex("Func<.+> costs .+ ms.");
        logger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == expectLogLevel),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((obj, type) => regex.Match(obj.ToString()!).Success),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Theory]
    [InlineData(100, 0, LogLevel.Information)]
    [InlineData(100, 200, LogLevel.Warning)]
    public async Task 測量Func_Task_應該進行記錄(long warningThreshold, int delay, LogLevel expectLogLevel)
    {
        // Given
        var logger = new Mock<ILogger<BlockTimer>>();

        var services = new ServiceCollection();
        services.AddTransient(sp => logger.Object);
        services.AddBlockTimer(o => o.WarningThreshold = warningThreshold);

        var provider = services.BuildServiceProvider();
        var blockTimer = provider.GetRequiredService<IBlockTimer>();

        var func = () =>
        {
            return Task.Delay(delay);
        };

        // When
        await blockTimer.Measure(func);

        // Then
        var regex = new Regex("Func<Task> costs .+ ms.");
        logger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == expectLogLevel),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((obj, type) => regex.Match(obj.ToString()!).Success),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Theory]
    [InlineData(100, 0, LogLevel.Information)]
    [InlineData(100, 200, LogLevel.Warning)]
    public async Task 測量Func_Task_T_應該進行記錄(long warningThreshold, int delay, LogLevel expectLogLevel)
    {
        // Given
        var logger = new Mock<ILogger<BlockTimer>>();

        var services = new ServiceCollection();
        services.AddTransient(sp => logger.Object);
        services.AddBlockTimer(o => o.WarningThreshold = warningThreshold);

        var provider = services.BuildServiceProvider();
        var blockTimer = provider.GetRequiredService<IBlockTimer>();

        var func = async () =>
        {
            await Task.Delay(delay);
            return true;
        };

        // When
        var result = await blockTimer.Measure(func);

        // Then
        result.Should().BeTrue();
        var regex = new Regex("Func<Task<.+>> costs .+ ms.");
        logger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == expectLogLevel),
            It.Is<EventId>(eventId => eventId.Id == 0),
            It.Is<It.IsAnyType>((obj, type) => regex.Match(obj.ToString()!).Success),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }
}
