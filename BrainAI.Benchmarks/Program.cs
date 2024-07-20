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

    public enum GraphTypes
    {
        Grid,
        StrightEdge,
    }

    public enum RunsCountType
    {
        Single_Run = 1,
        Multi_Run = 50,
    }

    public enum MapSizeType
    {
        Small_Map = 8,
        Large_Map = 60,
    }

    [Params(MapSizeType.Small_Map, MapSizeType.Large_Map)]
    public MapSizeType MapSize { get; set; } = MapSizeType.Small_Map;

    [Params(GraphTypes.Grid, GraphTypes.StrightEdge)]
    public GraphTypes GraphType { get; set; } = GraphTypes.StrightEdge;

    [Params(RunsCountType.Single_Run, RunsCountType.Multi_Run)]
    public RunsCountType RunsCount { get; set; } = RunsCountType.Multi_Run;

    [Params(PathfinderTypes.BFS, PathfinderTypes.Dijkstra, PathfinderTypes.AStar)]
    public PathfinderTypes PathfinderType { get; set; } = PathfinderTypes.AStar;

    private IPathfinder<Point>? pathfinder;
    private GridGraph? gridGraph;
    private StrightEdgeGraph strightEdgeGraph = new StrightEdgeGraph();

    [GlobalSetup]
    public void Setup()
    {
        this.gridGraph = new GridGraph((int)this.MapSize, (int)this.MapSize, true);
        int x;
        int y;
        for (var step = 0; step < (int)this.MapSize / 4 - 1; step++)
        {
            x = step * 4;
            for (y = x + 1; y < (int)this.MapSize - 1; y++)
            {
                gridGraph.Walls.Add(new Point(x, y));
                gridGraph.Walls.Add(new Point(x + 1, y));
            }

            y = step * 4 + 2;
            for (x = y + 1; x < (int)this.MapSize - 1; x++)
            {
                gridGraph.Walls.Add(new Point(x, y));
                gridGraph.Walls.Add(new Point(x, y + 1));
            }
        }
        GridToStrightEdgeConverter.Default.BuildGraph(gridGraph!, strightEdgeGraph);

        var graph = GraphType == GraphTypes.Grid ? (IAstarGraph<Point>)gridGraph : (IAstarGraph<Point>)strightEdgeGraph;

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
        var start = this.GraphType == GraphTypes.Grid ? new Point(0, 0) : new Point(0, 1);
        var end = this.GraphType == GraphTypes.Grid ? new Point((int)MapSize - 1, (int)MapSize - 1) : new Point((int)MapSize - 5, (int)MapSize - 4);
        for (var i = 0; i < (int)this.RunsCount; i++)
        {
            this.pathfinder!.Search(start, end);
            var pathData = this.pathfinder!.ResultPath;
            if (pathData.Count == 0)
            {
                throw new System.Exception("Path not found.");
            }
        }
    }
}