using System.Runtime.CompilerServices;

namespace AsyncFilterEnumerationPipeline.Tests.Filters;

/// <inheritdoc />
public class EvenFilter(bool includeFirst = true) : IFilter<string>
{
    /// <inheritdoc />
    public async IAsyncEnumerable<string> Apply(
        IAsyncEnumerable<string> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var flag = !includeFirst;
        
        await foreach (var s in source.WithCancellation(cancellationToken))
        {
            flag = !flag;
            if (flag)
                yield return s;
        }
    }
}