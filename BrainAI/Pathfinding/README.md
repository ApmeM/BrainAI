Pathfinding
==========
BrainAI provides three pathfinding algorithms out of the box: Breadth First Search, Dijkstra (weighted) and Astar (weighted with heuristic). 
All three are well suited for not only grid based graphs but also graphs of any type. 
Which algorithm should you use? 
That depends on your specific graph and needs. 
The next sections will go into a bit more detail on each of the algorithms so that you can intelligently decide which to use.

It should be noted that none of the search algorithms require the nodes searched to be locations. 
Pathfinding doesn't care about what data it is finding a path for. 
All it needs to know is the node and that node's neighbors (edges). 
The nodes can be anything at all: paths in a dialog tree, actions for AI (Astar is used by the Goal Oriented Action Planner) or even strings (see below for an example). 
Edges can be traversable one way or both ways.

The Graph interfaces for Breadth First Search, Dijkstra and Astar are all fully generic so you get to decide what data your nodes need. 
In the simplest case the nodes can be a `Point` (see `UnweightedGridGraph`, `WeightedGridGraph` and `AstarGridGraph` for examples). 
If your nodes need a bunch of precomputed data you can use any class that you want for them. 
This allows you to precompute (offline or at map load time) any data that you might need for the actual path search.

# Features

- Multiple pathfinding algorythms: BFS, Dijkstra and A*
- Ability to split search process across multiple calls
- Minimum memory allocations during search.
- Predefined graphs implementations

## Benchmark result

The benchmarks located under BrainAI.Benchmarks folder are executed with the command

```
dotnet run --project BrainAI.Benchmark -c Release
```

With the following result:

|      Method |   MapSize |   GraphType |  RunsCount | PathfinderType |          Mean |         Error |        StdDev |        Median | Allocated |
|------------ |---------- |------------ |----------- |--------------- |--------------:|--------------:|--------------:|--------------:|----------:|
| Pathfinding | Small_Map |        Grid | Single_Run |            BFS |     24.442 us |     0.4845 us |     1.1230 us |     24.892 us |         - |
| Pathfinding | Small_Map |        Grid | Single_Run |       Dijkstra |     29.843 us |     0.5968 us |     0.6634 us |     29.878 us |         - |
| Pathfinding | Small_Map |        Grid | Single_Run |          AStar |      9.008 us |     0.1802 us |     0.3878 us |      9.164 us |         - |
| Pathfinding | Small_Map |        Grid |  Multi_Run |            BFS |  1,280.551 us |    25.5894 us |    64.1988 us |  1,303.727 us |       2 B |
| Pathfinding | Small_Map |        Grid |  Multi_Run |       Dijkstra |  1,501.176 us |    29.5683 us |    54.0673 us |  1,512.843 us |       2 B |
| Pathfinding | Small_Map |        Grid |  Multi_Run |          AStar |    452.570 us |     9.0116 us |    19.7808 us |    461.432 us |         - |
| Pathfinding | Small_Map | StrightEdge | Single_Run |            BFS |      3.234 us |     0.0276 us |     0.0245 us |      3.233 us |         - |
| Pathfinding | Small_Map | StrightEdge | Single_Run |       Dijkstra |      2.269 us |     0.0450 us |     0.1034 us |      2.290 us |         - |
| Pathfinding | Small_Map | StrightEdge | Single_Run |          AStar |      1.793 us |     0.0159 us |     0.0149 us |      1.789 us |         - |
| Pathfinding | Small_Map | StrightEdge |  Multi_Run |            BFS |    160.997 us |     3.1973 us |     4.5855 us |    162.271 us |         - |
| Pathfinding | Small_Map | StrightEdge |  Multi_Run |       Dijkstra |    118.141 us |     1.1586 us |     1.0271 us |    118.236 us |         - |
| Pathfinding | Small_Map | StrightEdge |  Multi_Run |          AStar |     88.516 us |     1.7495 us |     4.4532 us |     90.549 us |         - |
| Pathfinding | Large_Map |        Grid | Single_Run |            BFS |  1,374.749 us |    12.4010 us |    10.9932 us |  1,375.848 us |       2 B |
| Pathfinding | Large_Map |        Grid | Single_Run |       Dijkstra |  1,714.988 us |    16.7163 us |    14.8186 us |  1,717.076 us |       2 B |
| Pathfinding | Large_Map |        Grid | Single_Run |          AStar |    112.336 us |     2.2112 us |     4.8535 us |    110.592 us |         - |
| Pathfinding | Large_Map |        Grid |  Multi_Run |            BFS | 72,733.293 us | 1,454.2995 us | 3,130.5332 us | 71,047.618 us |     117 B |
| Pathfinding | Large_Map |        Grid |  Multi_Run |       Dijkstra | 78,872.639 us |   494.4383 us |   462.4979 us | 78,849.505 us |     624 B |
| Pathfinding | Large_Map |        Grid |  Multi_Run |          AStar |  5,539.623 us |    43.8150 us |    36.5875 us |  5,526.731 us |       7 B |
| Pathfinding | Large_Map | StrightEdge | Single_Run |            BFS |    109.160 us |     2.0753 us |     1.9412 us |    109.801 us |         - |
| Pathfinding | Large_Map | StrightEdge | Single_Run |       Dijkstra |    173.351 us |     1.0684 us |     0.9994 us |    173.621 us |       1 B |
| Pathfinding | Large_Map | StrightEdge | Single_Run |          AStar |     59.632 us |     1.1753 us |     2.0584 us |     60.111 us |         - |
| Pathfinding | Large_Map | StrightEdge |  Multi_Run |            BFS |  5,818.599 us |   114.3515 us |   225.7188 us |  5,891.887 us |       6 B |
| Pathfinding | Large_Map | StrightEdge |  Multi_Run |       Dijkstra |  9,080.278 us |   103.1797 us |    91.4661 us |  9,077.030 us |      13 B |
| Pathfinding | Large_Map | StrightEdge |  Multi_Run |          AStar |  2,756.823 us |    15.9156 us |    14.8875 us |  2,756.441 us |       3 B |

Note those allocated bytes probably related to dotnet behavior: https://github.com/dotnet/BenchmarkDotNet/pull/1543

## Common issues

To reduce memory allocations during search do not forget to override GethashCode, Equals and IEquatable<T> for your struct that is used instead of Point.
Without overriding dotnet will use those methods from object and will require boxing that will allocate memory and require garbage collection to execute.
See [question](https://stackoverflow.com/questions/76412110/c-sharp-hashset-allocate-memory-on-each-add-even-within-capacity/76420468) on stack overflow.

# Detailed description

## Breadth First Search
Often called flood fill when used on a grid, Breadth First Search uses an expanding frontier that radiates out from the start position visiting all neighbor nodes on the way. 
When it reaches the goal it stops the search and returns the path. 
If no valid path is found null is returned. 
Breadth First Search is well suited for graphs that have uniform traversal cost between edges.

To implement Breadth First Search all you have to do is satisfy the single-method interface `IUnweightedGraph<T>`. 

Lets take a look at a functional example of a node based graph of plain old strings. 
This illustrates how pathfinding can be used to solve non-spatial based problems.
The `EdgesGraph` used in the example is a concrete implementation that can be used directly or as a starting point for grid based graphs. 


```csharp
// Create an UnweightedGraph with strings as nodes
var graph = new EdgesGraph<string>();
	
// Add a set of 5 nodes and edges for each
graph.Edges["a"] = new List<string> { "b" }; // a→b
graph.Edges["b"] = new List<string> { "a", "c", "d" }; // b→a, b→c, b→d
graph.Edges["c"] = new List<string> { "a" }; // c→a
graph.Edges["d"] = new List<string> { "e", "a" }; // d→e, d→a
graph.Edges["e"] = new List<string> { "b" }; // e→b

// Calculate a path from "c" to "e". The result is c→a→b→d→e, which we can confirm by looking at the edge comments above.
var path = new BreadthFirstPathfinder<string>(graph).Search("c", "e" );
```


## Dijkstra (aka Weighted)
Dijkstra expands on Breadth First Search by introducing a cost for each edge that the algorithm takes into account when pathfinding. 
This lends you more control over the pathfinding process. 
You can add edges with higher costs (moving along a road vs moving through a mud bog for example) to provide a hint that the algorithm should choose the lowest cost path. 
The algorithm will ask you for a cost to get from one node to another and you can choose to return any value that makes sense. 
It will then use the information to find not a only a path to the goal but the lowest cost path.

To implement Dijkstra you have to provide a graph that implements the interface `IWeightedGraph<T>`. 

The `GridGraph` is another concrete implementation that can be used directly. 
Below is an example of using the `WeightedPathfinder` for `GridGraph`. 

```csharp

var graph = new GridGraph();

graph.DefaultWeight = 2;

graph.Weights[new Point( 3, 3 )] = 5;
graph.Weights[new Point( 3, 4 )] = 5;
graph.Weights[new Point( 4, 3 )] = 5;
graph.Weights[new Point( 4, 4 )] = 5;

graph.Walls.Add(new Point(4, 5))
graph.Walls.Add(new Point(5, 5))

// Calculate the path
var path = new WeightedPathfinder<Point>(graph).Search( new Point( 3, 4 ), new Point( 7, 7 ) );
```


## Astar
Astar is probably the most well known of all pathfinding algorithms. 
It differs from Dijkstra in that it introduces a heuristic for each edge that the algorithm takes into account when pathfinding. 
This lets you add some interesting elements such as edges with higher costs (moving along a road vs moving through a mud bog for example.) 
The algorithm will ask you for a cost to get from one node to another and you can choose to return any value that makes sense. 
It will then use the information to find not a only a path to the goal but the lowest cost path.

To implement Astar you have to provide a graph that implements the interface `IAstarGraph<T>`.

Below is an example of using the `AstarPathfinder` for `GridGraph`. 


```csharp

var graph = new GridGraph();

// Add some weighted nodes
graph.WeightedNodes.Add( new Point( 3, 3 ) );
graph.WeightedNodes.Add( new Point( 3, 4 ) );
graph.WeightedNodes.Add( new Point( 4, 3 ) );
graph.WeightedNodes.Add( new Point( 4, 4 ) );
graph.Walls.Add(new Point(4, 5))
graph.Walls.Add(new Point(5, 5))

// Calculate the path
var path = new AstarPathfinder<Point>(graph).Search( new Point( 3, 4 ), new Point( 7, 7 ) );
```


# Graphs

4 types of graphs available out of the box:

- GridGraph - Graph represents a 2d array (not really implemented this way). Each point is connected to its neighbours to the left/right/up/down and diagonals (if enabled). Have a natural borders of 0 x 0 to Width x Height. Walls are specified in a HashSet.
- EdgesGraph - Basic graph with vertices and edges. Each verticle can be connected with any number of other points by the directed edges. This graph can not be used in AStar pathfinder as there is no way to specify heuristic calculation for base type.
- EdgesPointGraph - Extension for EdgesGraph that as a vertex use Point class. In this case heuristic can easily be calculated with a manhatten distance and can be used in AStar pathfinder.
- StrightEdgeGraph - graph that can be built using polygon obstacles on a map. Connections between polygon points are built automatically and the final path is built to avoid obstacles intersection.

Usage examples for `GridGraph`, `EdgesGraph` and `EdgesPointGraph` can be found above.

`StrightEdgeGraph` is more complex and in some cases requires additional handling. Below is the usage example for `StrightEdgeGraph`

```csharp

// Create a graph with 4 points of the single obstacle
var graph = new StrightEdgeGraph();
graph.AddPoint(1, new Point(200, 300));
graph.AddPoint(1, new Point(1000, 300));
graph.AddPoint(1, new Point(1000, 500));
graph.AddPoint(1, new Point(200, 500));

var pathData = DoSearch(graph, new Point(100, 100), new Point(900, 900));

// The result will be: 100x100, 200x500, 900x900
private List<Point> DoSearch_UsedInReadme(StrightEdgeGraph graph, Point start, Point end)
{
    var result = new List<Point>();
    // Check if end is visible from start.
    if (graph.IsVisible(start, end))
    {
        result.Add(start);
        result.Add(end);
        return result;
    }

    // Find closest visible start point to start from.
    HashSet<Point> starts = new HashSet<Point>();
    graph.FindVisiblePoints(start, starts);
    // Find all visible nodes to end point.
    HashSet<Point> ends = new HashSet<Point>();
    graph.FindVisiblePoints(end, ends);
    if (!starts.Any() || !ends.Any())
    {
        // It might happen that there are no visible points for the following reasons:
        // 1. Graph is empty. In this case start and end are directly connected.
        // 2. If the rounding walls looks like well (all visible points are concave).
        return null;
    }

    // Do the search.
    // WARNING: Do not use Astar here as AStar is not really multigoal search as it have a heuristics calculations based on a single target. Instead it took first goal from set and tries to get to it. 
    // If you want to use AStar here - please provide the exact end goal point (e.g. find the closest points from all the visible points and use it).
    var pathfinder = new WeightedPathfinder<Point>(graph);
    pathfinder.Search(starts.OrderBy(a => (a - start).LengthQuad).First(), ends);
    if (pathfinder.ResultPath.Count == 0)
    {
        // Path not found.
        return null;
    }

    // As we start from closest start point it might happen that some further points are also visible and we can remove them from the list.
    var found = false;
    for (var i = pathfinder.ResultPath.Count; i > 0; i--)
    {
        if (found)
        {
            pathfinder.ResultPath.RemoveAt(i - 1);
            continue;
        }
        found = starts.Contains(pathfinder.ResultPath[i - 1]);
    }

    // Add start and end points if they are not on the graph.
    if (pathfinder.ResultPath[pathfinder.ResultPath.Count - 1] != end)
    {
        pathfinder.ResultPath.Add(end);
    }
    if (pathfinder.ResultPath[0] != start)
    {
        pathfinder.ResultPath.Insert(0, start);
    }
    // It would be good to copy the result to another list, so it will not be lost after next Search call.
    return new List<Point>(pathfinder.ResultPath);

}

```

It is also possible to create `StrightEdgeGraph` from `GridGraph` as all the necessary information is available there:

```csharp

// Generating grid graph that looks like this:
// #####
// #   #
// # # #
// #####
var grid = new GridGraph(5, 5);
grid.Walls.Add(new Point(0, 0));
grid.Walls.Add(new Point(0, 1));
grid.Walls.Add(new Point(0, 2));
grid.Walls.Add(new Point(0, 3));
grid.Walls.Add(new Point(1, 3));
grid.Walls.Add(new Point(2, 3));
grid.Walls.Add(new Point(3, 3));
grid.Walls.Add(new Point(4, 3));
grid.Walls.Add(new Point(4, 2));
grid.Walls.Add(new Point(4, 1));
grid.Walls.Add(new Point(4, 0));
grid.Walls.Add(new Point(3, 0));
grid.Walls.Add(new Point(2, 0));
grid.Walls.Add(new Point(1, 0));
grid.Walls.Add(new Point(2, 1));

// Convert grid graph to stright edge graph where each # converted to square 10x10
var graph = new StrightEdgeGraph();
GridToStrightEdgeConverter.Default.BuildGraph(grid, graph, 10);

var pathData = new AStarPathfinder<Point>(graph).Search(new Point(15, 15), new Point(35, 15));
// The result will be: 15x15, 20x20, 30x200, 35x15
```