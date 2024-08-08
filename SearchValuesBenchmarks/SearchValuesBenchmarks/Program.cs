using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Buffers;


BenchmarkRunner.Run<SearchValuesBenchmarks>();

[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public class SearchValuesBenchmarks
{
    private static string bookContent = new HttpClient().GetStringAsync("https://www.gutenberg.org/cache/epub/729/pg729.txt").Result;
    private static readonly SearchValues<char> s_searchValues 
        = SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");


    [Benchmark]
    public int StringIndexOfAny()
    {
        char[] searchValues = 
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
            .ToArray();

        int count = 0, index = 0;
        while ((index = bookContent.IndexOfAny(searchValues, index)) >= 0)
        {
            count++;
            index++;
        }

        return count;
    }

    [Benchmark]
    public int SpanIndexOfAny()
    {
        string searchValues 
            = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        ReadOnlySpan<char> remaining = bookContent;
        int count = 0, pos;
        while ((pos = remaining.IndexOfAny(searchValues)) >= 0)
        {
            count++;
            remaining = remaining.Slice(pos + 1);
        }

        return count;
    }

    [Benchmark(Baseline = true)]
    public int Search_Values()
    {
        ReadOnlySpan<char> remaining = bookContent;
        int count = 0, pos;
        while ((pos = remaining.IndexOfAny(s_searchValues)) >= 0)
        {
            count++;
            remaining = remaining.Slice(pos + 1);
        }

        return count;
    }
}
