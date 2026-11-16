using System.Diagnostics;
using System.Text.Json;

namespace GraphAlgorithms;

public sealed class BenchmarkRecord
{
    public required string GraphType { get; init; } // "Dense" or "Sparse"
    public required int VertexCount { get; init; }
    public required int EdgeCount { get; init; }
    public required double FloydWarshallTimeMs { get; init; }
    public required long FloydWarshallOps { get; init; }
    public required double BellmanFordSingleTimeMs { get; init; }
    public required long BellmanFordSingleOps { get; init; }
    public required double BellmanFordAllPairsTimeMs { get; init; }
    public required long BellmanFordAllPairsOps { get; init; }
}

public static class BenchmarkRunner
{
    public static List<BenchmarkRecord> RunBenchmarks()
    {
        Console.WriteLine("=========================================================");
        Console.WriteLine("   ЗАПУСК ЭКСПЕРИМЕНТАЛЬНОГО СТЕНДА СРАВНЕНИЯ АЛГОРИТМОВ");
        Console.WriteLine("   Язык: C# 13 / .NET 10.0, Высокоточный таймер Stopwatch");
        Console.WriteLine("=========================================================");

        // Фаза прогрева JIT (JIT Warm-up)
        Console.WriteLine("Фаза прогрева JIT...");
        for (int i = 0; i < 5; i++)
        {
            var warmupEdges = GraphGenerator.GenerateRandomGraph(20, 0.5, seed: 42 + i);
            _ = FloydWarshall.Run(20, warmupEdges);
            _ = BellmanFord.Run(20, warmupEdges, 0);
            _ = BellmanFord.RunAllPairs(20, warmupEdges);
        }
        Console.WriteLine("Прогрев завершен.\n");

        int[] vertexSizes = [10, 20, 30, 50, 75, 100, 150, 200, 250];
        var records = new List<BenchmarkRecord>();

        // 1. Тестирование на плотных графах (p = 0.7)
        Console.WriteLine("--- Серия 1: Плотные графы (плотность p = 0.70) ---");
        foreach (int v in vertexSizes)
        {
            int iterations = v switch
            {
                <= 30 => 50,
                <= 75 => 20,
                <= 150 => 10,
                _ => 5
            };

            var record = RunBenchmarkForSize(v, isDense: true, iterations);
            records.Add(record);
            PrintRecord(record);
        }

        // 2. Тестирование на разреженных графах (E ~ 4 * V)
        Console.WriteLine("\n--- Серия 2: Разреженные графы (E ~ 4 * V) ---");
        foreach (int v in vertexSizes)
        {
            int iterations = v switch
            {
                <= 30 => 50,
                <= 75 => 20,
                <= 150 => 10,
                _ => 5
            };

            var record = RunBenchmarkForSize(v, isDense: false, iterations);
            records.Add(record);
            PrintRecord(record);
        }

        return records;
    }

    private static BenchmarkRecord RunBenchmarkForSize(int v, bool isDense, int iterations)
    {
        double totalFloydMs = 0;
        long totalFloydOps = 0;

        double totalBellmanSingleMs = 0;
        long totalBellmanSingleOps = 0;

        double totalBellmanAllPairsMs = 0;
        long totalBellmanAllPairsOps = 0;

        int totalEdges = 0;

        var sw = new Stopwatch();

        for (int it = 0; it < iterations; it++)
        {
            int seed = 1000 + v * 100 + it;
            var edges = isDense
                ? GraphGenerator.GenerateDenseGraph(v, density: 0.70, seed: seed)
                : GraphGenerator.GenerateSparseGraph(v, edgesPerVertex: 4.0, seed: seed);

            totalEdges += edges.Count;

            // 1. Измерение Floyd-Warshall
            GC.Collect();
            GC.WaitForPendingFinalizers();
            sw.Restart();
            var fwRes = FloydWarshall.Run(v, edges);
            sw.Stop();
            totalFloydMs += sw.Elapsed.TotalMilliseconds;
            totalFloydOps += fwRes.OperationsCount;

            // 2. Измерение Bellman-Ford (Single-Source от вершины 0)
            GC.Collect();
            GC.WaitForPendingFinalizers();
            sw.Restart();
            var bfRes = BellmanFord.Run(v, edges, 0);
            sw.Stop();
            totalBellmanSingleMs += sw.Elapsed.TotalMilliseconds;
            totalBellmanSingleOps += bfRes.OperationsCount;

            // 3. Измерение Bellman-Ford (All-Pairs)
            GC.Collect();
            GC.WaitForPendingFinalizers();
            sw.Restart();
            var bfAllRes = BellmanFord.RunAllPairs(v, edges);
            sw.Stop();
            totalBellmanAllPairsMs += sw.Elapsed.TotalMilliseconds;
            totalBellmanAllPairsOps += bfAllRes.TotalOperationsCount;
        }

        return new BenchmarkRecord
        {
            GraphType = isDense ? "Dense" : "Sparse",
            VertexCount = v,
            EdgeCount = totalEdges / iterations,
            FloydWarshallTimeMs = totalFloydMs / iterations,
            FloydWarshallOps = totalFloydOps / iterations,
            BellmanFordSingleTimeMs = totalBellmanSingleMs / iterations,
            BellmanFordSingleOps = totalBellmanSingleOps / iterations,
            BellmanFordAllPairsTimeMs = totalBellmanAllPairsMs / iterations,
            BellmanFordAllPairsOps = totalBellmanAllPairsOps / iterations
        };
    }

    private static void PrintRecord(BenchmarkRecord r)
    {
        Console.WriteLine($"V={r.VertexCount,3} | E={r.EdgeCount,5} | " +
                          $"Флойд-Уоршелл: {r.FloydWarshallTimeMs,8:F3} мс | " +
                          $"Беллман-Форд (1 исток): {r.BellmanFordSingleTimeMs,6:F3} мс | " +
                          $"Беллман-Форд (все пары): {r.BellmanFordAllPairsTimeMs,8:F3} мс");
    }

    public static void SaveToJson(List<BenchmarkRecord> records, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(records, options);
        File.WriteAllText(filePath, json);
        Console.WriteLine($"\nРезультаты сохранены в: {filePath}");
    }
}
