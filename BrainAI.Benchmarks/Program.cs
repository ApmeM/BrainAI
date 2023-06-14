using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BrainAI.Pathfinding;

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
        AStar
    }

    [Params(10, 50, 100)]
    public int ArrayLength { get; set; }

    [Params(PathfinderTypes.BFS, PathfinderTypes.Dijkstra, PathfinderTypes.AStar)]
    public PathfinderTypes PathfinderType { get; set; }

    private IPathfinder<Point>? pathfinder;

    [GlobalSetup]
    public void Setup()
    {
        var graph = new GridGraph(ArrayLength, ArrayLength, true);

        switch (this.PathfinderType)
        {
            case PathfinderTypes.BFS:
                this.pathfinder = new BreadthFirstPathfinder<Point>(graph);
                break;
            case PathfinderTypes.Dijkstra:
                this.pathfinder = new WeightedPathfinder<Point>(graph);
                break;
            case PathfinderTypes.AStar:
                this.pathfinder = new AStarPathfinder<Point>(graph);
                break;
        }
    }

    [Benchmark]
    public void Pathfinding()
    {
        this.pathfinder.Search(new Point(0, 0), new Point(ArrayLength - 1, ArrayLength - 1));
    }
}