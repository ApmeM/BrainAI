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
        AStarForStrightEdge
    }

    [Params(/*10, 50, */100)]
    public int ArrayLength { get; set; }

    [Params(/*PathfinderTypes.BFS, PathfinderTypes.Dijkstra, PathfinderTypes.AStar, */PathfinderTypes.AStarForStrightEdge)]
    public PathfinderTypes PathfinderType { get; set; }

    private IPathfinder<Point>? pathfinder;

    [GlobalSetup]
    public void Setup()
    {

        switch (this.PathfinderType)
        {
            case PathfinderTypes.BFS:
                {
                    var graph = new GridGraph(ArrayLength, ArrayLength, true);
                    this.pathfinder = new BreadthFirstPathfinder<Point>(graph);
                }
                break;
            case PathfinderTypes.Dijkstra:
                {
                    var graph = new GridGraph(ArrayLength, ArrayLength, true);
                    this.pathfinder = new WeightedPathfinder<Point>(graph);
                }
                break;
            case PathfinderTypes.AStar:
                {
                    var graph = new GridGraph(ArrayLength, ArrayLength, true);
                    this.pathfinder = new AStarPathfinder<Point>(graph);
                }
                break;
            case PathfinderTypes.AStarForStrightEdge:
                {
                    var graph = new StrightEdgeGraph();

                    for (var i = 0; i < ArrayLength / 2; i++)
                    {
                        var j = i / 2;
                        switch (i % 2)
                        {
                            case 0:
                                graph.AddObstacle(
                                    new List<Point>{
                                        new Point(j*4 + 0 + 0, j*4 + 0 + 1),
                                        new Point(j*4 + 2 + 0, j*4 + 0 + 1),
                                        new Point(j*4 + 2 + 0, j*4 + ArrayLength + 1),
                                        new Point(j*4 + 0 + 0, j*4 + ArrayLength + 1),
                                    });
                                break;
                            case 1:
                                graph.AddObstacle(
                                    new List<Point>{
                                        new Point(j*4 + 0 + 3, j*4 + 0 + 2),
                                        new Point(j*4 + 2 + 3, j*4 + 0 + 2),
                                        new Point(j*4 + 2 + 3, j*4 + 2 + 2),
                                        new Point(j*4 + 0 + 3, j*4 + 2 + 2),
                                    });
                                break;
                        }
                    }

                    this.pathfinder = new AStarPathfinder<Point>(graph);
                }
                break;
        }
    }

    [Benchmark]
    public void Pathfinding()
    {
        this.pathfinder.Search(new Point(0, 0), new Point(ArrayLength - 1, ArrayLength - 1));
    }
}