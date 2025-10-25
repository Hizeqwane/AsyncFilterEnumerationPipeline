using System.Runtime.CompilerServices;

namespace AsyncFilterEnumerationPipeline.Tests.Filters;

/// <inheritdoc />
public class FirstLetterIFilter : IFilter<string>
{
    /// <inheritdoc />
    public async IAsyncEnumerable<string> Apply(
        IAsyncEnumerable<string> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var s in source.WithCancellation(cancellationToken))
        {
            if (!s.StartsWith('i') && !s.StartsWith('I'))
                yield return s;
        }
    }
}