namespace GraphAlgorithms;

public sealed class BellmanFordResult
{
    public required double[] Distances { get; init; }
    public required int?[] Parents { get; init; }
    public required bool HasNegativeCycle { get; init; }
    public required long OperationsCount { get; init; }

    public List<int>? ReconstructPath(int source, int target)
    {
        if (source == target)
            return [source];

        if (Parents[target] is null)
            return null;

        var path = new List<int>();
        int? curr = target;
        while (curr is not null)
        {
            path.Add(curr.Value);
            if (curr.Value == source)
                break;
            curr = Parents[curr.Value];
        }

        if (path[^1] != source)
            return null;

        path.Reverse();
        return path;
    }
}

public sealed class AllPairsBellmanFordResult
{
    public required double[,] Distances { get; init; }
    public required bool HasNegativeCycle { get; init; }
    public required long TotalOperationsCount { get; init; }
}

public static class BellmanFord
{
    public static BellmanFordResult Run(int n, IReadOnlyList<Edge> edges, int source)
    {
        var dist = new double[n];
        var parent = new int?[n];
        long opCount = 0;

        for (int i = 0; i < n; i++)
        {
            dist[i] = double.PositiveInfinity;
            parent[i] = null;
        }
        dist[source] = 0.0;

        for (int i = 0; i < n - 1; i++)
        {
            bool updated = false;

            for (int e = 0; e < edges.Count; e++)
            {
                opCount++;
                var edge = edges[e];
                if (!double.IsPositiveInfinity(dist[edge.From]))
                {
                    double newDist = dist[edge.From] + edge.Weight;
                    if (newDist < dist[edge.To])
                    {
                        dist[edge.To] = newDist;
                        parent[edge.To] = edge.From;
                        updated = true;
                    }
                }
            }

            if (!updated)
                break;
        }

        bool hasNegativeCycle = false;
        for (int e = 0; e < edges.Count; e++)
        {
            var edge = edges[e];
            if (!double.IsPositiveInfinity(dist[edge.From]) &&
                dist[edge.From] + edge.Weight < dist[edge.To])
            {
                hasNegativeCycle = true;
                break;
            }
        }

        return new BellmanFordResult
        {
            Distances = dist,
            Parents = parent,
            HasNegativeCycle = hasNegativeCycle,
            OperationsCount = opCount
        };
    }

    public static AllPairsBellmanFordResult RunAllPairs(int n, IReadOnlyList<Edge> edges)
    {
        var distMatrix = new double[n, n];
        bool hasNegCycle = false;
        long totalOps = 0;

        for (int src = 0; src < n; src++)
        {
            var result = Run(n, edges, src);
            totalOps += result.OperationsCount;
            if (result.HasNegativeCycle)
                hasNegCycle = true;

            for (int dest = 0; dest < n; dest++)
            {
                distMatrix[src, dest] = result.Distances[dest];
            }
        }

        return new AllPairsBellmanFordResult
        {
            Distances = distMatrix,
            HasNegativeCycle = hasNegCycle,
            TotalOperationsCount = totalOps
        };
    }
}
