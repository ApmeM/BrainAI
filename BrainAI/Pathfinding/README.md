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
| Pathfinding | Small_Map |        Grid | Single_Run |            BFS |     10.536 us |     0.0241 us |     0.0201 us |     10.534 us |         - |
| Pathfinding | Small_Map |        Grid | Single_Run |       Dijkstra |     20.356 us |     0.0627 us |     0.0489 us |     20.354 us |         - |
| Pathfinding | Small_Map |        Grid | Single_Run |          AStar |      6.304 us |     0.0296 us |     0.0262 us |      6.307 us |         - |
| Pathfinding | Small_Map |        Grid |  Multi_Run |            BFS |    528.040 us |     1.4537 us |     1.2887 us |    528.158 us |       1 B |
| Pathfinding | Small_Map |        Grid |  Multi_Run |       Dijkstra |  1,022.049 us |     1.2233 us |     1.0215 us |  1,021.768 us |       2 B |
| Pathfinding | Small_Map |        Grid |  Multi_Run |          AStar |    315.511 us |     0.8892 us |     0.8317 us |    315.340 us |         - |
| Pathfinding | Small_Map | StrightEdge | Single_Run |            BFS |      1.074 us |     0.0027 us |     0.0025 us |      1.074 us |         - |
| Pathfinding | Small_Map | StrightEdge | Single_Run |       Dijkstra |      1.572 us |     0.0070 us |     0.0058 us |      1.573 us |         - |
| Pathfinding | Small_Map | StrightEdge | Single_Run |          AStar |      1.116 us |     0.0099 us |     0.0083 us |      1.117 us |         - |
| Pathfinding | Small_Map | StrightEdge |  Multi_Run |            BFS |     54.328 us |     1.0842 us |     1.8410 us |     53.257 us |         - |
| Pathfinding | Small_Map | StrightEdge |  Multi_Run |       Dijkstra |     73.962 us |     0.3187 us |     0.2661 us |     73.982 us |         - |
| Pathfinding | Small_Map | StrightEdge |  Multi_Run |          AStar |     56.830 us |     0.7733 us |     0.6037 us |     56.817 us |         - |
| Pathfinding | Large_Map |        Grid | Single_Run |            BFS |    586.147 us |     2.0184 us |     1.7893 us |    586.383 us |       1 B |
| Pathfinding | Large_Map |        Grid | Single_Run |       Dijkstra |  1,094.835 us |     7.8513 us |     7.3441 us |  1,092.305 us |       2 B |
| Pathfinding | Large_Map |        Grid | Single_Run |          AStar |     82.236 us |     0.3322 us |     0.3107 us |     82.195 us |         - |
| Pathfinding | Large_Map |        Grid |  Multi_Run |            BFS | 31,506.632 us |   153.7770 us |   136.3193 us | 31,463.148 us |      51 B |
| Pathfinding | Large_Map |        Grid |  Multi_Run |       Dijkstra | 56,491.047 us | 1,119.2601 us | 1,960.2937 us | 55,324.941 us |      91 B |
| Pathfinding | Large_Map |        Grid |  Multi_Run |          AStar |  4,025.165 us |    74.8224 us |    73.4856 us |  4,066.758 us |       7 B |
| Pathfinding | Large_Map | StrightEdge | Single_Run |            BFS |     35.132 us |     0.4308 us |     0.4030 us |     35.294 us |         - |
| Pathfinding | Large_Map | StrightEdge | Single_Run |       Dijkstra |    102.068 us |     0.4420 us |     0.3918 us |    102.098 us |         - |
| Pathfinding | Large_Map | StrightEdge | Single_Run |          AStar |     35.200 us |     0.3344 us |     0.2792 us |     35.308 us |         - |
| Pathfinding | Large_Map | StrightEdge |  Multi_Run |            BFS |  1,829.183 us |    35.8304 us |    59.8645 us |  1,796.778 us |       3 B |
| Pathfinding | Large_Map | StrightEdge |  Multi_Run |       Dijkstra |  5,191.182 us |    15.9122 us |    14.1058 us |  5,189.956 us |       6 B |
| Pathfinding | Large_Map | StrightEdge |  Multi_Run |          AStar |  1,838.879 us |    36.7060 us |    64.2876 us |  1,874.565 us |       2 B |

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
    // Check if end is visible from start.
    if (graph.IsVisible(start, end))
    {
        return new List<Point> { start, end };
    }

    // Find closest visible start point to start from.
    HashSet<Point> starts = null;
    graph.FindVisiblePoints(start, ref starts);
    // Find all visible nodes to end point.
    HashSet<Point> ends = null;
    graph.FindVisiblePoints(end, ref ends);
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
    var pathData = new WeightedPathfinder<Point>(graph).Search(starts.OrderBy(a => (a - start).LengthQuad).First(), ends);
    if (pathData == null)
    {
        // Path not found.
        return null;
    }

    // As we start from closest start point it might happen that some further points are also visible and we can remove them from the list.
    var found = false;
    for (var i = pathData.Count; i > 0; i--)
    {
        if (found)
        {
            pathData.RemoveAt(i - 1);
            continue;
        }
        found = starts.Contains(pathData[i - 1]);
    }

    // Add start and end points if they are not on the graph.
    if (pathData[pathData.Count - 1] != end)
    {
        pathData.Add(end);
    }
    if (pathData[0] != start)
    {
        pathData.Insert(0, start);
    }
    return pathData;
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