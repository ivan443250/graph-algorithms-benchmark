using GraphAlgorithms;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Shortest Path Algorithms: Floyd-Warshall vs Bellman-Ford");
Console.WriteLine("C# 13 / .NET 10.0");
Console.WriteLine();

Console.WriteLine("Running benchmarks...");
var records = BenchmarkRunner.RunBenchmarks();

string resultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "benchmark_results.json");
resultsPath = Path.GetFullPath(resultsPath);
BenchmarkRunner.SaveToJson(records, resultsPath);

Console.WriteLine();
Console.WriteLine("Done.");
