using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;


List<int> numbers = [1, 2, 3, 4, 5];

FrozenSet<int> frozenNumbers = numbers.ToFrozenSet();
ImmutableList<int> immutableNumbers = numbers.ToImmutableList();
ReadOnlyCollection<int> readOnlyNumbers = numbers.AsReadOnly();

numbers.Add(6);

Console.WriteLine($"Frozen: {string.Join(", ", frozenNumbers)}");
Console.WriteLine($"Immutable: {string.Join(", ", immutableNumbers)}");
Console.WriteLine($"ReadOnly: {string.Join(", ", readOnlyNumbers)}");
// Frozen: 1, 2, 3, 4, 5
// Immutable: 1, 2, 3, 4, 5
// ReadOnly: 1, 2, 3, 4, 5, 6


// Uncomment the chosen benchmarks and run it in the Release mode
//BenchmarkRunner.Run<LookupBenchmarks>();
//BenchmarkRunner.Run<CreationBenchmarks>();

public class LookupBenchmarks
{
    [Params(100, 1000, 10_000)]
    public int CollectionSize { get; set; }

    private List<int> _list;
    private ImmutableList<int> _immutableList;
    private HashSet<int> _hashSet;
    private FrozenSet<int> _frozenSet;

    [GlobalSetup]
    public void SetUp()
    {
        _list = Enumerable.Range(0, CollectionSize).ToList();
        _immutableList = Enumerable.Range(0, CollectionSize).ToImmutableList();
        _hashSet = Enumerable.Range(0, CollectionSize).ToHashSet();
        _frozenSet = Enumerable.Range(0, CollectionSize).ToFrozenSet();
    }

    [Benchmark(Baseline = true)]
    public void LookupList()
    {
        for (var i = 0; i < CollectionSize; i++)
            _ = _list.Contains(i);
    }

    [Benchmark]
    public void LookupImmutableList()
    {
        for (var i = 0; i < CollectionSize; i++)
            _ = _immutableList.Contains(i);
    }

    [Benchmark]
    public void LookupHashSet()
    {
        for (var i = 0; i < CollectionSize; i++)
            _ = _hashSet.Contains(i);
    }

    [Benchmark]
    public void LookupFrozenSet()
    {
        for (var i = 0; i < CollectionSize; i++)
            _ = _frozenSet.Contains(i);
    }
}

public class CreationBenchmarks
{
    private readonly int[] Numbers
        = Enumerable.Range(0, 1000).ToArray();

    [Benchmark(Baseline = true)]
    public List<int> ToList() => Numbers.ToList();

    [Benchmark]
    public FrozenSet<int> ToFrozenSet() => Numbers.ToFrozenSet();

    [Benchmark]
    public HashSet<int> ToHashSet() => Numbers.ToHashSet();
}


