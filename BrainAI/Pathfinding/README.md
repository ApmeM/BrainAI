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
|      Method | ArrayLength | UseStrightEdge | PathFindingRuns | PathfinderType |           Mean |         Error |        StdDev |         Median | Allocated |
|------------ |------------ |--------------- |---------------- |--------------- |---------------:|--------------:|--------------:|---------------:|----------:|
| Pathfinding |          10 |          False |               1 |            BFS |      19.155 us |     0.0554 us |     0.0518 us |      19.159 us |         - |
| Pathfinding |          10 |          False |               1 |       Dijkstra |      38.052 us |     0.2686 us |     0.2381 us |      38.095 us |         - |
| Pathfinding |          10 |          False |               1 |          AStar |       8.231 us |     0.0202 us |     0.0189 us |       8.229 us |         - |
| Pathfinding |          10 |          False |              10 |            BFS |     189.587 us |     1.1289 us |     1.0007 us |     189.546 us |         - |
| Pathfinding |          10 |          False |              10 |       Dijkstra |     390.890 us |     1.0295 us |     0.9126 us |     390.659 us |         - |
| Pathfinding |          10 |          False |              10 |          AStar |      81.006 us |     0.6442 us |     0.5029 us |      81.013 us |         - |
| Pathfinding |          10 |          False |              50 |            BFS |     954.884 us |     1.7725 us |     1.5713 us |     955.019 us |       1 B |
| Pathfinding |          10 |          False |              50 |       Dijkstra |   1,912.335 us |     6.1702 us |     5.1524 us |   1,910.986 us |       3 B |
| Pathfinding |          10 |          False |              50 |          AStar |     413.984 us |     1.6436 us |     1.6143 us |     414.152 us |         - |
| Pathfinding |          10 |           True |               1 |            BFS |      33.620 us |     0.1962 us |     0.1835 us |      33.641 us |         - |
| Pathfinding |          10 |           True |               1 |       Dijkstra |      36.157 us |     0.6131 us |     0.9724 us |      35.912 us |         - |
| Pathfinding |          10 |           True |               1 |          AStar |      31.199 us |     0.1012 us |     0.0845 us |      31.164 us |       1 B |
| Pathfinding |          10 |           True |              10 |            BFS |     104.313 us |     0.5688 us |     0.4441 us |     104.222 us |         - |
| Pathfinding |          10 |           True |              10 |       Dijkstra |     121.497 us |     1.0003 us |     0.7809 us |     121.270 us |       1 B |
| Pathfinding |          10 |           True |              10 |          AStar |     116.071 us |     2.2480 us |     3.4999 us |     117.298 us |         - |
| Pathfinding |          10 |           True |              50 |            BFS |     407.197 us |     5.5142 us |     5.1580 us |     405.984 us |         - |
| Pathfinding |          10 |           True |              50 |       Dijkstra |     480.515 us |     1.9971 us |     1.5592 us |     480.244 us |      18 B |
| Pathfinding |          10 |           True |              50 |          AStar |     456.502 us |     2.0076 us |     1.7797 us |     455.938 us |       2 B |
| Pathfinding |          50 |          False |               1 |            BFS |     390.706 us |     1.1021 us |     0.8605 us |     390.659 us |         - |
| Pathfinding |          50 |          False |               1 |       Dijkstra |     804.307 us |    15.1645 us |    19.1782 us |     813.230 us |       1 B |
| Pathfinding |          50 |          False |               1 |          AStar |      64.997 us |     1.2637 us |     1.7716 us |      65.747 us |         - |
| Pathfinding |          50 |          False |              10 |            BFS |   3,877.194 us |    76.4613 us |   152.7015 us |   3,800.986 us |       3 B |
| Pathfinding |          50 |          False |              10 |       Dijkstra |   8,094.760 us |   155.4436 us |   185.0445 us |   8,215.428 us |      13 B |
| Pathfinding |          50 |          False |              10 |          AStar |     629.147 us |    10.8188 us |    10.1199 us |     631.574 us |       1 B |
| Pathfinding |          50 |          False |              50 |            BFS |  20,857.442 us |    57.3558 us |    47.8947 us |  20,855.471 us |      26 B |
| Pathfinding |          50 |          False |              50 |       Dijkstra |  37,938.707 us |   396.1959 us |   486.5641 us |  38,010.646 us |      68 B |
| Pathfinding |          50 |          False |              50 |          AStar |   3,144.514 us |    61.8260 us |    68.7194 us |   3,191.002 us |       3 B |
| Pathfinding |          50 |           True |               1 |            BFS |   1,222.268 us |     6.3542 us |     5.6328 us |   1,222.455 us |       2 B |
| Pathfinding |          50 |           True |               1 |       Dijkstra |   2,941.100 us |     9.7403 us |     8.6345 us |   2,941.703 us |       3 B |
| Pathfinding |          50 |           True |               1 |          AStar |     965.920 us |     7.7331 us |     7.2335 us |     965.637 us |      18 B |
| Pathfinding |          50 |           True |              10 |            BFS |   1,790.751 us |    13.1503 us |    10.9811 us |   1,784.942 us |       3 B |
| Pathfinding |          50 |           True |              10 |       Dijkstra |   4,155.120 us |    13.7839 us |    11.5102 us |   4,153.934 us |       6 B |
| Pathfinding |          50 |           True |              10 |          AStar |   1,460.836 us |     5.1378 us |     4.5545 us |   1,461.115 us |       2 B |
| Pathfinding |          50 |           True |              50 |            BFS |   4,558.160 us |    90.3630 us |   184.5877 us |   4,440.283 us |       6 B |
| Pathfinding |          50 |           True |              50 |       Dijkstra |   9,871.654 us |   107.2216 us |   100.2952 us |   9,865.504 us |      13 B |
| Pathfinding |          50 |           True |              50 |          AStar |   4,163.580 us |    27.3683 us |    25.6003 us |   4,163.484 us |       6 B |
| Pathfinding |         100 |          False |               1 |            BFS |   2,483.591 us |    19.8876 us |    16.6070 us |   2,481.188 us |       3 B |
| Pathfinding |         100 |          False |               1 |       Dijkstra |   3,894.520 us |     9.6774 us |     8.5788 us |   3,896.026 us |       3 B |
| Pathfinding |         100 |          False |               1 |          AStar |     179.502 us |     0.9007 us |     0.8425 us |     179.322 us |         - |
| Pathfinding |         100 |          False |              10 |            BFS |  22,589.469 us |   334.2733 us |   260.9786 us |  22,526.070 us |      26 B |
| Pathfinding |         100 |          False |              10 |       Dijkstra |  41,341.992 us |   205.9528 us |   182.5718 us |  41,272.038 us |      68 B |
| Pathfinding |         100 |          False |              10 |          AStar |   1,683.755 us |    24.5760 us |    21.7860 us |   1,691.098 us |       2 B |
| Pathfinding |         100 |          False |              50 |            BFS | 125,735.818 us | 1,081.4026 us | 1,011.5447 us | 125,551.980 us |     540 B |
| Pathfinding |         100 |          False |              50 |       Dijkstra | 188,074.048 us |   826.0569 us |   732.2779 us | 188,217.476 us |     475 B |
| Pathfinding |         100 |          False |              50 |          AStar |   8,286.484 us |    81.2525 us |    86.9392 us |   8,289.346 us |      13 B |
| Pathfinding |         100 |           True |               1 |            BFS |   5,259.559 us |    31.0538 us |    25.9313 us |   5,247.139 us |       6 B |
| Pathfinding |         100 |           True |               1 |       Dijkstra |  14,986.735 us |   102.4844 us |    85.5791 us |  14,954.563 us |      26 B |
| Pathfinding |         100 |           True |               1 |          AStar |   8,352.387 us |    43.8859 us |    41.0509 us |   8,354.614 us |      13 B |
| Pathfinding |         100 |           True |              10 |            BFS |   6,765.001 us |    26.5032 us |    23.4944 us |   6,758.410 us |       6 B |
| Pathfinding |         100 |           True |              10 |       Dijkstra |  18,232.532 us |    39.6492 us |    37.0879 us |  18,235.182 us |      28 B |
| Pathfinding |         100 |           True |              10 |          AStar |  10,775.457 us |    68.2771 us |    63.8664 us |  10,760.647 us |      13 B |
| Pathfinding |         100 |           True |              50 |            BFS |  13,345.381 us |    68.2701 us |    57.0086 us |  13,355.093 us |      13 B |
| Pathfinding |         100 |           True |              50 |       Dijkstra |  34,759.267 us |   164.7298 us |   154.0884 us |  34,762.349 us |      54 B |
| Pathfinding |         100 |           True |              50 |          AStar |  22,395.569 us |   447.8772 us |   697.2908 us |  21,999.400 us |      26 B |

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

// Add 4 pointsof the same obstacle some obstacles
graph.AddPoint(1, new Point( 200, 300));
graph.AddPoint(1, new Point(1000, 300));
graph.AddPoint(1, new Point(1000, 500));
graph.AddPoint(1, new Point( 200, 500));

// Calculate the path
var start = graph.FindClosestVisiblePoint(new Point(100, 100));
var end = graph.FindClosestVisiblePoint(new Point(900, 900));

var path = new AstarPathfinder<Point>(graph).Search( start, end );
// The result will be: 200x300, 200x500
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