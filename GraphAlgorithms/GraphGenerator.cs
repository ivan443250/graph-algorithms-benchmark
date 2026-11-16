namespace GraphAlgorithms;

public static class GraphGenerator
{
    public static List<Edge> GenerateRandomGraph(int n, double density, double minWeight = 1.0, double maxWeight = 100.0, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var edges = new List<Edge>();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i == j) continue;

                if (rng.NextDouble() < density)
                {
                    double weight = minWeight + rng.NextDouble() * (maxWeight - minWeight);
                    edges.Add(new Edge(i, j, Math.Round(weight, 2)));
                }
            }
        }

        for (int i = 0; i < n - 1; i++)
        {
            if (!edges.Any(e => e.From == i && e.To == i + 1))
            {
                double weight = minWeight + rng.NextDouble() * (maxWeight - minWeight);
                edges.Add(new Edge(i, i + 1, Math.Round(weight, 2)));
            }
        }

        return edges;
    }

    public static List<Edge> GenerateSparseGraph(int n, double edgesPerVertex = 4.0, double minWeight = 1.0, double maxWeight = 100.0, int? seed = null)
    {
        double density = Math.Min(1.0, edgesPerVertex / Math.Max(1, n - 1));
        return GenerateRandomGraph(n, density, minWeight, maxWeight, seed);
    }

    public static List<Edge> GenerateDenseGraph(int n, double density = 0.7, double minWeight = 1.0, double maxWeight = 100.0, int? seed = null)
    {
        return GenerateRandomGraph(n, density, minWeight, maxWeight, seed);
    }
}
