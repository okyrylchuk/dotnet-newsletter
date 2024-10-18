using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Text;

BenchmarkRunner.Run<StringBenchmarks>();

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "RatioSD",
            "Gen0", "Gen1", "Gen2", "Median")]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
public class StringBenchmarks
{
    [Params(100, 1_000, 10_000)]
    public int Count { get; set; }

    [Benchmark]
    public string StringConcatenation()
    {
        string result = string.Empty;
        for (int i = 0; i < Count; i++)
            result += "Hello World!";
        return result;
    }

    [Benchmark(Baseline = true)]
    public string StringBuilder()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < Count; i++)
            sb.Append("Hello World!");
        return sb.ToString();
    }
}

