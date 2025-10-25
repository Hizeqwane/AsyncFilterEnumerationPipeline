using AsyncFilterEnumerablePipeline.Filters;

namespace AsyncFilterEnumerablePipeline.Tests;

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
        
        _rightAnswer.AddRange(
            [
                "CLS",
                "UNIQLO",
                "KOTLIN"
        ]);
        
        _filters.AddRange(
            new EvenFilter(),
            new FirstLetterAFilter()
        );

        _pipeline = new Pipeline<string>(_filters);
    }

    private readonly List<string> _strList = [];
    
    private readonly List<string> _rightAnswer = [];

    private readonly List<IFilter<string>> _filters = [];

    private Pipeline<string> _pipeline;

    [Test]
    public async Task Test1()
    {
        var result = await _pipeline.Apply(_strList);

        Assert.That(result, Is.EqualTo(_rightAnswer));
    }
}