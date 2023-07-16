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
|      Method | ArrayLength | UseStrightEdge | PathFindingRuns | PathfinderType |          Mean |        Error |        StdDev |        Median | Allocated |
|------------ |------------ |--------------- |---------------- |--------------- |--------------:|-------------:|--------------:|--------------:|----------:|
| Pathfinding |          10 |          False |               1 |            BFS |      27.48 us |     0.225 us |      0.188 us |      27.47 us |         - |
| Pathfinding |          10 |          False |               1 |       Dijkstra |      53.08 us |     0.321 us |      0.268 us |      53.12 us |         - |
| Pathfinding |          10 |          False |               1 |          AStar |      10.65 us |     0.095 us |      0.084 us |      10.66 us |         - |
| Pathfinding |          10 |          False |              10 |            BFS |     308.56 us |     6.073 us |     12.128 us |     313.61 us |         - |
| Pathfinding |          10 |          False |              10 |       Dijkstra |     534.09 us |     3.847 us |      3.410 us |     534.94 us |       1 B |
| Pathfinding |          10 |          False |              10 |          AStar |     106.57 us |     1.052 us |      0.932 us |     106.30 us |         - |
| Pathfinding |          10 |          False |              50 |            BFS |   1,490.46 us |    20.591 us |     18.254 us |   1,488.34 us |       2 B |
| Pathfinding |          10 |          False |              50 |       Dijkstra |   2,868.28 us |    42.619 us |     35.588 us |   2,858.45 us |       3 B |
| Pathfinding |          10 |          False |              50 |          AStar |     616.05 us |    12.154 us |     29.120 us |     621.59 us |       1 B |
| Pathfinding |          10 |           True |               1 |            BFS |      43.59 us |     0.687 us |      0.941 us |      43.46 us |         - |
| Pathfinding |          10 |           True |               1 |       Dijkstra |      48.36 us |     0.960 us |      1.655 us |      48.80 us |         - |
| Pathfinding |          10 |           True |               1 |          AStar |      40.56 us |     0.337 us |      0.298 us |      40.55 us |         - |
| Pathfinding |          10 |           True |              10 |            BFS |     143.20 us |     2.777 us |      2.598 us |     142.24 us |         - |
| Pathfinding |          10 |           True |              10 |       Dijkstra |     184.58 us |     3.393 us |      3.174 us |     184.63 us |       1 B |
| Pathfinding |          10 |           True |              10 |          AStar |     164.38 us |     3.255 us |      3.045 us |     164.42 us |       5 B |
| Pathfinding |          10 |           True |              50 |            BFS |     556.81 us |     3.876 us |      3.236 us |     558.48 us |       2 B |
| Pathfinding |          10 |           True |              50 |       Dijkstra |     622.70 us |    10.140 us |      9.959 us |     620.25 us |       1 B |
| Pathfinding |          10 |           True |              50 |          AStar |     644.04 us |    16.588 us |     48.909 us |     627.54 us |       1 B |
| Pathfinding |          50 |          False |               1 |            BFS |   1,233.62 us |    21.659 us |     28.163 us |   1,221.47 us |       2 B |
| Pathfinding |          50 |          False |               1 |       Dijkstra |   1,805.66 us |    35.400 us |     48.456 us |   1,826.71 us |       2 B |
| Pathfinding |          50 |          False |               1 |          AStar |     176.28 us |     3.518 us |      6.861 us |     178.20 us |         - |
| Pathfinding |          50 |          False |              10 |            BFS |  13,388.71 us |   164.038 us |    145.416 us |  13,393.93 us |      13 B |
| Pathfinding |          50 |          False |              10 |       Dijkstra |  17,315.52 us |   182.591 us |    161.862 us |  17,374.47 us |      26 B |
| Pathfinding |          50 |          False |              10 |          AStar |   1,605.39 us |    31.486 us |     54.313 us |   1,612.98 us |       2 B |
| Pathfinding |          50 |          False |              50 |            BFS |  65,105.64 us | 1,444.234 us |  4,258.356 us |  65,023.11 us |      91 B |
| Pathfinding |          50 |          False |              50 |       Dijkstra |  91,905.56 us | 1,445.624 us |  2,569.595 us |  92,299.78 us |     136 B |
| Pathfinding |          50 |          False |              50 |          AStar |   7,753.12 us |    67.315 us |     74.820 us |   7,745.57 us |      13 B |
| Pathfinding |          50 |           True |               1 |            BFS |   2,350.71 us |    52.638 us |    154.378 us |   2,366.32 us |       3 B |
| Pathfinding |          50 |           True |               1 |       Dijkstra |   5,120.55 us |   100.988 us |    108.057 us |   5,104.98 us |       9 B |
| Pathfinding |          50 |           True |               1 |          AStar |   1,693.70 us |    19.544 us |     29.253 us |   1,685.44 us |       2 B |
| Pathfinding |          50 |           True |              10 |            BFS |   3,510.59 us |    68.923 us |    103.160 us |   3,528.05 us |       3 B |
| Pathfinding |          50 |           True |              10 |       Dijkstra |   7,614.82 us |   210.356 us |    620.240 us |   7,338.55 us |       6 B |
| Pathfinding |          50 |           True |              10 |          AStar |   2,623.33 us |    47.774 us |     95.411 us |   2,616.92 us |       3 B |
| Pathfinding |          50 |           True |              50 |            BFS |   8,024.46 us |   171.839 us |    506.672 us |   7,943.17 us |      13 B |
| Pathfinding |          50 |           True |              50 |       Dijkstra |  16,944.58 us |   378.435 us |  1,115.823 us |  16,617.79 us |      26 B |
| Pathfinding |          50 |           True |              50 |          AStar |   7,930.88 us |   122.293 us |    125.586 us |   7,910.70 us |      13 B |
| Pathfinding |         100 |          False |               1 |            BFS |   7,980.14 us |    70.254 us |     58.665 us |   7,993.64 us |      13 B |
| Pathfinding |         100 |          False |               1 |       Dijkstra |  12,055.17 us |   305.725 us |    901.437 us |  12,103.56 us |      13 B |
| Pathfinding |         100 |          False |               1 |          AStar |     525.24 us |     6.481 us |     10.279 us |     526.87 us |       1 B |
| Pathfinding |         100 |          False |              10 |            BFS |  78,220.95 us |   831.208 us |    736.844 us |  78,042.53 us |     227 B |
| Pathfinding |         100 |          False |              10 |       Dijkstra | 130,546.07 us | 1,453.004 us |  1,359.141 us | 130,659.55 us |   1,334 B |
| Pathfinding |         100 |          False |              10 |          AStar |   5,347.00 us |    40.068 us |     35.519 us |   5,350.75 us |       6 B |
| Pathfinding |         100 |          False |              50 |            BFS | 401,881.39 us | 6,879.061 us | 12,048.121 us | 405,599.34 us |     816 B |
| Pathfinding |         100 |          False |              50 |       Dijkstra | 649,048.47 us | 5,614.323 us |  4,688.214 us | 648,912.15 us |   5,256 B |
| Pathfinding |         100 |          False |              50 |          AStar |  25,814.14 us |   491.779 us |    410.658 us |  25,716.72 us |      26 B |
| Pathfinding |         100 |           True |               1 |            BFS |  11,398.99 us |   221.779 us |    280.480 us |  11,511.58 us |      13 B |
| Pathfinding |         100 |           True |               1 |       Dijkstra |  33,535.46 us |   556.512 us |    493.334 us |  33,400.74 us |      51 B |
| Pathfinding |         100 |           True |               1 |          AStar |  18,568.33 us |   384.583 us |  1,121.847 us |  18,861.15 us |      26 B |
| Pathfinding |         100 |           True |              10 |            BFS |  14,927.35 us |   370.474 us |  1,092.351 us |  15,456.84 us |      13 B |
| Pathfinding |         100 |           True |              10 |       Dijkstra |  39,958.63 us |   461.197 us |    385.120 us |  39,975.49 us |      51 B |
| Pathfinding |         100 |           True |              10 |          AStar |  21,544.23 us |   489.689 us |  1,443.858 us |  21,022.65 us |      26 B |
| Pathfinding |         100 |           True |              50 |            BFS |  25,987.00 us |   499.623 us |    442.903 us |  25,906.93 us |      26 B |
| Pathfinding |         100 |           True |              50 |       Dijkstra |  69,629.72 us | 1,302.185 us |  1,393.323 us |  69,932.55 us |     102 B |
| Pathfinding |         100 |           True |              50 |          AStar |  43,520.29 us |   668.145 us |    624.983 us |  43,337.43 us |      63 B |

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

Below is the usage example for `StrightEdgeGraph`

```csharp

var graph = new StrightEdgeGraph();

// Add some obstacles
graph.AddObstacle(
    new List<Point>{
            new Point( 200, 300),
            new Point(1000, 300),
            new Point(1000, 500),
            new Point( 200, 500),
        });

// Calculate the path
var path = new AstarPathfinder<Point>(graph).Search( new Point( 100, 100 ), new Point( 900, 900 ) );
// The result will be: 100x100, 200x500, 900x900
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