using System.Runtime.CompilerServices;

namespace AsyncFilterEnumerablePipeline.Filters;

/// <inheritdoc />
public class EvenFilter : IFilter<string>
{
    /// <inheritdoc />
    public async IAsyncEnumerable<string> Apply(
        IAsyncEnumerable<string> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var flag = false;
        
        await foreach (var s in source.WithCancellation(cancellationToken))
        {
            flag = !flag;
            if (flag)
                yield return s;
        }
    }
}