using AsyncFilterEnumerationPipeline.Registration;
using AsyncFilterEnumerationPipeline.Tests.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace AsyncFilterEnumerationPipeline.Tests;

public class PipelineTest
{
    [SetUp]
    public void Setup()
    {
        _strList.AddRange(
        [
            "ABC",
            "IITO",
            "CLS",
            "AUTO",
            "UNIQLO",
            "RE",
            "AAA",
            "QWERTY",
            "KOTLIN"
        ]);
        
        _firstRightAnswer.AddRange(
            [
                "CLS",
                "UNIQLO",
                "KOTLIN"
        ]);
        
        _secondRightAnswer.AddRange(
        [
            "AUTO",
            "RE",
            "QWERTY"
        ]);

        _services = new ServiceCollection();
        _services.AddPipeline<string>("first")
            .AddFilter<string, EvenFilter>()
            .AddFilter<string, FirstLetterAFilter>()
            .Build();
        
        _services.AddPipeline<string>("second")
            .AddFilter<string, NotEvenFilter>()
            .AddFilter<string, FirstLetterIFilter>()
            .Build();
    }

    private readonly List<string> _strList = [];
    
    private readonly List<string> _firstRightAnswer = [];
    
    private readonly List<string> _secondRightAnswer = [];

    private IServiceCollection _services;

    [Test]
    public async Task Test1()
    {
        await using var provider = _services.BuildServiceProvider();
        
        var firstPipeline = provider.GetKeyedService<Pipeline<string>>("first")!;
        var secondPipeline = provider.GetKeyedService<Pipeline<string>>("second")!;
        
        var firstResult = await firstPipeline.Apply(_strList);

        Assert.That(firstResult, Is.EqualTo(_firstRightAnswer));
        
        var secondResult = await secondPipeline.Apply(_strList);

        Assert.That(secondResult, Is.EqualTo(_secondRightAnswer));
    }
}