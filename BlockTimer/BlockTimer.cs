using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlockTimer;

internal class BlockTimer : IBlockTimer
{
    private readonly ILogger<BlockTimer> _logger;
    private readonly BlockTimerOptions _options;

    internal BlockTimer(ILogger<BlockTimer> logger, IOptions<BlockTimerOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public void Measure(Action action, string? blockName = null)
    {
        if (string.IsNullOrWhiteSpace(blockName))
            blockName = action.GetTypeName();

        var sw = new Stopwatch();

        sw.Start();
        action();
        sw.Stop();

        Log(blockName, sw);
    }

    public T Measure<T>(Func<T> func, string? blockName = null)
    {
        if (string.IsNullOrWhiteSpace(blockName))
            blockName = func.GetTypeName();

        var sw = new Stopwatch();

        sw.Start();
        var result = func();
        sw.Stop();

        Log(blockName, sw);

        return result;
    }

    public async Task Measure(Func<Task> func, string? blockName = null)
    {
        if (string.IsNullOrWhiteSpace(blockName))
            blockName = func.GetTypeName();

        var sw = new Stopwatch();

        sw.Start();
        await func();
        sw.Stop();

        Log(blockName, sw);
    }

    public async Task<T> Measure<T>(Func<Task<T>> func, string? blockName = null)
    {
        if (string.IsNullOrWhiteSpace(blockName))
            blockName = func.GetTypeName();

        var sw = new Stopwatch();

        sw.Start();
        var result = await func();
        sw.Stop();

        Log(blockName, sw);

        return result;
    }

    private void Log(string blockName, Stopwatch stopwatch)
    {
        if (stopwatch.ElapsedMilliseconds >= _options.WarningThreshold)
        {
            _logger.LogWarning("{BlockName} costs {ElapsedMilliseconds} ms.", blockName, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation("{BlockName} costs {ElapsedMilliseconds} ms.", blockName, stopwatch.ElapsedMilliseconds);
        }
    }
}
