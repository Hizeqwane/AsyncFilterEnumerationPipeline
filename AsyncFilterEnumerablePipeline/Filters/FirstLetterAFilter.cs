using System.Runtime.CompilerServices;

namespace AsyncFilterEnumerablePipeline.Filters;

/// <inheritdoc />
public class FirstLetterAFilter : IFilter<string>
{
    /// <inheritdoc />
    public async IAsyncEnumerable<string> Apply(
        IAsyncEnumerable<string> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var s in source.WithCancellation(cancellationToken))
        {
            if (!s.StartsWith('a') && !s.StartsWith('A'))
                yield return s;
        }
    }
}