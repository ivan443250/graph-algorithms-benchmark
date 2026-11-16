using GraphAlgorithms;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Shortest Path Algorithms: Floyd-Warshall vs Bellman-Ford");
Console.WriteLine("C# 13 / .NET 10.0");
Console.WriteLine();

Console.WriteLine("Running verification tests...");
RunVerificationTests();
Console.WriteLine("Verification tests passed.");
Console.WriteLine();

Console.WriteLine("Running benchmarks...");
var records = BenchmarkRunner.RunBenchmarks();

string resultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "benchmark_results.json");
resultsPath = Path.GetFullPath(resultsPath);
BenchmarkRunner.SaveToJson(records, resultsPath);

Console.WriteLine();
Console.WriteLine("Done.");

static void RunVerificationTests()
{
    int n = 4;
    var edges = new List<Edge>
    {
        new(0, 1, 5.0),
        new(0, 3, 10.0),
        new(1, 2, 3.0),
        new(2, 3, 1.0)
    };

    var fw = FloydWarshall.Run(n, edges);
    var bfSingle = BellmanFord.Run(n, edges, 0);
    var bfAll = BellmanFord.RunAllPairs(n, edges);

    if (fw.HasNegativeCycle || bfSingle.HasNegativeCycle)
        throw new InvalidOperationException("False negative cycle detection on acyclic graph!");

    if (Math.Abs(fw.Distances[0, 3] - 9.0) > 1e-6 || Math.Abs(bfSingle.Distances[3] - 9.0) > 1e-6)
        throw new InvalidOperationException("Incorrect shortest path distance from 0 to 3!");

    var fwPath = fw.ReconstructPath(0, 3);
    var bfPath = bfSingle.ReconstructPath(0, 3);

    if (fwPath is null || !fwPath.SequenceEqual([0, 1, 2, 3]) ||
        bfPath is null || !bfPath.SequenceEqual([0, 1, 2, 3]))
    {
        throw new InvalidOperationException("Incorrect reconstructed path!");
    }

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            if (Math.Abs(fw.Distances[i, j] - bfAll.Distances[i, j]) > 1e-6)
                throw new InvalidOperationException($"APSP distance mismatch at [{i}, {j}]!");
        }
    }

    int n2 = 5;
    var edges2 = new List<Edge>
    {
        new(0, 1, 6.0),
        new(0, 2, 7.0),
        new(1, 2, 8.0),
        new(1, 3, -4.0),
        new(1, 4, 5.0),
        new(2, 3, 9.0),
        new(2, 4, -3.0),
        new(3, 4, 7.0),
        new(3, 0, 2.0),
        new(4, 1, -2.0)
    };

    var fw2 = FloydWarshall.Run(n2, edges2);
    var bf2 = BellmanFord.Run(n2, edges2, 0);
    if (fw2.HasNegativeCycle || bf2.HasNegativeCycle)
        throw new InvalidOperationException("False negative cycle detection on graph with negative weights!");

    for (int j = 0; j < n2; j++)
    {
        if (Math.Abs(fw2.Distances[0, j] - bf2.Distances[j]) > 1e-6)
            throw new InvalidOperationException($"Distance mismatch for negative weights graph at j={j}!");
    }

    int n3 = 4;
    var edges3 = new List<Edge>
    {
        new(0, 1, 1.0),
        new(1, 2, 2.0),
        new(2, 3, -6.0),
        new(3, 1, 1.0)
    };

    var fw3 = FloydWarshall.Run(n3, edges3);
    var bf3 = BellmanFord.Run(n3, edges3, 0);

    if (!fw3.HasNegativeCycle || !bf3.HasNegativeCycle)
        throw new InvalidOperationException("Failed to detect negative cycle!");
}
