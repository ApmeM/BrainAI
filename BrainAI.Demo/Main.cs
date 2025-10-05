using Godot;
using System;
using MazeGenerators;
using MazeGenerators.Utils;
using BrainAI.Pathfinding;
using System.Collections.Generic;

public class Main : Node2D
{
    private GeneratorSettings settings = new GeneratorSettings
    {
        Width = 31,
        Height = 21,
        Random = new Random(0),
    };
    IPathfinder<Point> pathfinder;
    List<Point> paths;

    public override void _Ready()
    {
        this.GetNode<Button>("./Container/Base").Connect("pressed", this, nameof(BFSPressed));
        this.GetNode<Button>("./Container/Tree").Connect("pressed", this, nameof(DijkstraPressed));
        this.GetNode<Button>("./Container/Life").Connect("pressed", this, nameof(AStarPressed));
        this.GetNode<Timer>("./Timer").Connect("timeout", this, nameof(Timeout));
    }

    private void Timeout()
    {
        if (pathfinder == null)
        {
            return;
        }

        var tileMap = this.GetNode<TileMap>("./TileMap");

        foreach (var visited in this.pathfinder.VisitedNodes)
        {
            var x = visited.Key.X;
            var y = visited.Key.Y;
            tileMap.SetCellv(new Godot.Vector2(x, y), 0, autotileCoord: new Godot.Vector2(0, 0));
        }

        pathfinder.ContinueSearch(1, this.paths);
        if (this.paths.Count > 0)
        {
            pathfinder = null;
        }
    }

    private void BFSPressed()
    {
        var (graph, start, end) = BuildGraph();
        this.pathfinder = new BreadthFirstPathfinder<Point>(graph);
        this.paths = new List<Point>();
        pathfinder.Search(start, end, 1, this.paths);
    }

    private void DijkstraPressed()
    {
        var (graph, start, end) = BuildGraph();
        this.pathfinder = new WeightedPathfinder<Point>(graph);
        this.paths = new List<Point>();
        pathfinder.Search(start, end, 1, this.paths);
    }

    private void AStarPressed()
    {
        var (graph, start, end) = BuildGraph();
        this.pathfinder = new AStarPathfinder<Point>(graph);
        this.paths = new List<Point>();
        pathfinder.Search(start, end, 1, this.paths);
    }

    private (GridGraph, Point, Point) BuildGraph()
    {
        var fluent = Fluent
            .Build(settings)
            .GenerateField()
            .GenerateRooms()
            .GrowMaze()
            .GenerateConnectors()
            .RemoveDeadEnds()
            .BuildWalls();

        var graph = new GridGraph(settings.Width, settings.Height);

        var tileMap = this.GetNode<TileMap>("./TileMap");
        var start = default(Point);
        var end = default(Point);
        for (var x = 0; x < fluent.settings.Width; x++)
        {
            for (var y = 0; y < fluent.settings.Height; y++)
            {
                var cell = fluent.result.Paths[x, y];
                if (cell == fluent.settings.EmptyTileId)
                {
                    tileMap.SetCellv(new Godot.Vector2(x, y), 0, autotileCoord: new Godot.Vector2(0, 0));
                }
                else if (cell == fluent.settings.MazeTileId)
                {
                    tileMap.SetCellv(new Godot.Vector2(x, y), 0, autotileCoord: new Godot.Vector2(1, 0));
                    if (start.X == 0 && start.Y == 0)
                    {
                        start = new Point(x, y);
                    }
                    end = new Point(x, y);
                }
                else if (cell == fluent.settings.WallTileId)
                {
                    tileMap.SetCellv(new Godot.Vector2(x, y), 0, autotileCoord: new Godot.Vector2(0, 1));
                    graph.Walls.Add(new Point(x, y));
                }
                else if (cell == fluent.settings.JunctionTileId)
                {
                    tileMap.SetCellv(new Godot.Vector2(x, y), 0, autotileCoord: new Godot.Vector2(5, 0));
                    graph.Weights[new Point(x, y)] = graph.DefaultWeight * 2;
                }
                else if (cell == fluent.settings.RoomTileId)
                {
                    tileMap.SetCellv(new Godot.Vector2(x, y), 0, autotileCoord: new Godot.Vector2(14, 0));
                }
            }
        }
        return (graph, start, end);
    }
}
