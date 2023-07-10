using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BrainAI.Pathfinding;
using System.Collections.Generic;

[MemoryDiagnoser]
public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<Program>();
    }

    public enum PathfinderTypes
    {
        BFS,
        Dijkstra,
        AStar,
    }

    [Params(10, 50, 100)]
    public int ArrayLength { get; set; } = 50;

    [Params(false, true)]
    public bool UseStrightEdge { get; set; } = true;

    [Params(1, 10, 50)]
    public int PathFindingRuns { get; set; } = 10;

    [Params(PathfinderTypes.BFS, PathfinderTypes.Dijkstra, PathfinderTypes.AStar)]
    public PathfinderTypes PathfinderType { get; set; } = PathfinderTypes.AStar;

    private IPathfinder<Point>? pathfinder;
    private GridGraph? graph;
    private StrightEdgeGraph strightEdge = new StrightEdgeGraph();

    [GlobalSetup]
    public void Setup()
    {
        graph = new GridGraph(ArrayLength, ArrayLength, true);
        int x;
        int y;
        for (var step = 0; step < ArrayLength / 4 - 1; step++)
        {
            x = step * 4;
            for (y = x + 1; y < ArrayLength - 1; y++)
            {
                graph.Walls.Add(new Point(x, y));
                graph.Walls.Add(new Point(x + 1, y));
            }

            y = step * 4 + 2;
            for (x = y + 1; x < ArrayLength - 1; x++)
            {
                graph.Walls.Add(new Point(x, y));
                graph.Walls.Add(new Point(x, y + 1));
            }
        }

        GridToStrightEdgeConverter.Default.BuildGraph(graph!, strightEdge);

        System.Console.WriteLine(graph);

        switch (this.PathfinderType)
        {
            case PathfinderTypes.BFS:
                {
                    this.pathfinder = new BreadthFirstPathfinder<Point>(graph);
                }
                break;
            case PathfinderTypes.Dijkstra:
                {
                    this.pathfinder = new WeightedPathfinder<Point>(graph);
                }
                break;
            case PathfinderTypes.AStar:
                {
                    this.pathfinder = new AStarPathfinder<Point>(graph);
                }
                break;
            default:
                throw new System.Exception($"Unkonwn pathfinder type {this.PathfinderType}");
        }
    }

    [Benchmark]
    public void Pathfinding()
    {
        IAstarGraph<Point> newGraph = this.UseStrightEdge ? strightEdge : graph!;

        for (var i = 0; i < this.PathFindingRuns; i++)
        {
            var pathData = this.pathfinder!.Search(new Point(0, 0), new Point(ArrayLength - 1, ArrayLength - 1));
            if (pathData == null)
            {
                throw new System.Exception("Path not found.");
            }
        }
    }
}