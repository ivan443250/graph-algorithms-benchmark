namespace GraphAlgorithms;

/// <summary>
/// Генератор случайных взвешенных ориентированных графов.
/// </summary>
public static class GraphGenerator
{
    /// <summary>
    /// Генерация графа с заданной вероятностью появления ребра (модель Эрдёша-Реньи G(n, p)).
    /// </summary>
    /// <param name="n">Количество вершин.</param>
    /// <param name="density">Плотность (вероятность существования ребра от 0.0 до 1.0).</param>
    /// <param name="minWeight">Минимальный вес ребра (положительный для исключения отрицательных циклов в замерах).</param>
    /// <param name="maxWeight">Максимальный вес ребра.</param>
    /// <param name="seed">Начальное число генератора ПСЧ.</param>
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

        // Гарантируем, что граф содержит хотя бы остовную цепь для связности
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

    /// <summary>
    /// Генерация разреженного графа со средним числом рёбер ~k*V.
    /// </summary>
    public static List<Edge> GenerateSparseGraph(int n, double edgesPerVertex = 4.0, double minWeight = 1.0, double maxWeight = 100.0, int? seed = null)
    {
        double density = Math.Min(1.0, edgesPerVertex / Math.Max(1, n - 1));
        return GenerateRandomGraph(n, density, minWeight, maxWeight, seed);
    }

    /// <summary>
    /// Генерация плотного графа (вероятность рёбер ~0.7).
    /// </summary>
    public static List<Edge> GenerateDenseGraph(int n, double density = 0.7, double minWeight = 1.0, double maxWeight = 100.0, int? seed = null)
    {
        return GenerateRandomGraph(n, density, minWeight, maxWeight, seed);
    }
}
