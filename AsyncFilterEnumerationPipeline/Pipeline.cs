namespace AsyncFilterEnumerationPipeline;

/// <summary>
/// Пайплайн фильтров
/// </summary>
public class Pipeline<T>(IEnumerable<IFilter<T>> filters)
{
    /// <summary>
    /// Применить пайплайн фильтров
    /// </summary>
    public async Task<List<T>> Apply(IEnumerable<T> source, CancellationToken cancellationToken = default) =>
        await filters
            .Aggregate(
                source.ToAsyncEnumerable(),
                (current, filter) => filter.Apply(current, cancellationToken))
            .ToListAsync(cancellationToken);
}