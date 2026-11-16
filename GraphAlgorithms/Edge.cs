namespace GraphAlgorithms;

/// <summary>
/// Представление ориентированного взвешенного ребра графа.
/// </summary>
/// <param name="From">Индекс начальной вершины.</param>
/// <param name="To">Индекс конечной вершины.</param>
/// <param name="Weight">Вес ребра.</param>
public readonly record struct Edge(int From, int To, double Weight);
