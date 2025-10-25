namespace AsyncFilterEnumerationPipeline;

/// <summary>
/// Фильтр
/// </summary>
public interface IFilter<T>
{
    /// <summary>
    /// Применить фильтр
    /// </summary>
    IAsyncEnumerable<T> Apply(IAsyncEnumerable<T> source, CancellationToken cancellationToken = default);
}