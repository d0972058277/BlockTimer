namespace BlockTimer;

public interface IBlockTimer
{
    void Measure(Action action, string? blockName = default);
    T Measure<T>(Func<T> func, string? blockName = default);
    Task Measure(Func<Task> func, string? blockName = default);
    Task<T> Measure<T>(Func<Task<T>> func, string? blockName = default);
}
