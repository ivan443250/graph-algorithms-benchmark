namespace GraphAlgorithms;

public sealed class FloydWarshallResult
{
    public required double[,] Distances { get; init; }
    public required int?[,] NextNodes { get; init; }
    public required bool HasNegativeCycle { get; init; }
    public required long OperationsCount { get; init; }

    public List<int>? ReconstructPath(int u, int v)
    {
        if (NextNodes[u, v] is null)
            return null;

        var path = new List<int> { u };
        int curr = u;
        while (curr != v)
        {
            var next = NextNodes[curr, v];
            if (next is null) return null;
            curr = next.Value;
            path.Add(curr);
        }
        return path;
    }
}

public static class FloydWarshall
{
    public static FloydWarshallResult Run(int n, IReadOnlyList<Edge> edges)
    {
        var dist = new double[n, n];
        var next = new int?[n, n];
        long opCount = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                dist[i, j] = (i == j) ? 0.0 : double.PositiveInfinity;
                next[i, j] = (i == j) ? i : null;
            }
        }

        foreach (var edge in edges)
        {
            if (edge.Weight < dist[edge.From, edge.To])
            {
                dist[edge.From, edge.To] = edge.Weight;
                next[edge.From, edge.To] = edge.To;
            }
        }

        for (int k = 0; k < n; k++)
        {
            for (int i = 0; i < n; i++)
            {
                double dIK = dist[i, k];
                if (double.IsPositiveInfinity(dIK))
                    continue;

                for (int j = 0; j < n; j++)
                {
                    opCount++;
                    double newDist = dIK + dist[k, j];
                    if (newDist < dist[i, j])
                    {
                        dist[i, j] = newDist;
                        next[i, j] = next[i, k];
                    }
                }
            }
        }

        bool hasNegativeCycle = false;
        for (int i = 0; i < n; i++)
        {
            if (dist[i, i] < 0)
            {
                hasNegativeCycle = true;
                break;
            }
        }

        return new FloydWarshallResult
        {
            Distances = dist,
            NextNodes = next,
            HasNegativeCycle = hasNegativeCycle,
            OperationsCount = opCount
        };
    }
}
