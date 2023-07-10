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
| Pathfinding |          10 |          False |               1 |            BFS |      29.275 us |     0.2817 us |     0.2635 us |      29.269 us |         - |
| Pathfinding |          10 |          False |               1 |       Dijkstra |      52.882 us |     1.0276 us |     1.2996 us |      52.166 us |         - |
| Pathfinding |          10 |          False |               1 |          AStar |       9.265 us |     0.1820 us |     0.2430 us |       9.086 us |         - |
| Pathfinding |          10 |          False |              10 |            BFS |     312.508 us |     6.2481 us |    11.4251 us |     318.306 us |         - |
| Pathfinding |          10 |          False |              10 |       Dijkstra |     527.082 us |     2.3006 us |     4.2644 us |     528.296 us |       1 B |
| Pathfinding |          10 |          False |              10 |          AStar |      97.189 us |     1.8281 us |     1.7955 us |      97.799 us |         - |
| Pathfinding |          10 |          False |              50 |            BFS |   1,450.839 us |     7.1819 us |     5.9972 us |   1,449.064 us |       2 B |
| Pathfinding |          10 |          False |              50 |       Dijkstra |   2,721.756 us |    12.2617 us |    11.4696 us |   2,721.046 us |       3 B |
| Pathfinding |          10 |          False |              50 |          AStar |     509.553 us |     1.5896 us |     1.4869 us |     509.188 us |       1 B |
| Pathfinding |          10 |           True |               1 |            BFS |      35.792 us |     0.1592 us |     0.1490 us |      35.788 us |         - |
| Pathfinding |          10 |           True |               1 |       Dijkstra |      67.865 us |     1.0353 us |     0.9684 us |      68.096 us |         - |
| Pathfinding |          10 |           True |               1 |          AStar |      15.639 us |     0.0163 us |     0.0145 us |      15.643 us |         - |
| Pathfinding |          10 |           True |              10 |            BFS |     295.678 us |     2.3814 us |     2.1110 us |     295.404 us |         - |
| Pathfinding |          10 |           True |              10 |       Dijkstra |     581.938 us |     9.6368 us |     8.5428 us |     582.981 us |       1 B |
| Pathfinding |          10 |           True |              10 |          AStar |      98.815 us |     0.2845 us |     0.2376 us |      98.857 us |         - |
| Pathfinding |          10 |           True |              50 |            BFS |   1,628.141 us |    32.2444 us |    31.6683 us |   1,646.313 us |       2 B |
| Pathfinding |          10 |           True |              50 |       Dijkstra |   2,670.954 us |     3.6353 us |     3.4004 us |   2,669.872 us |       3 B |
| Pathfinding |          10 |           True |              50 |          AStar |     459.660 us |     1.8995 us |     1.7768 us |     459.036 us |       1 B |
| Pathfinding |          50 |          False |               1 |            BFS |   1,204.774 us |     4.2915 us |     4.0143 us |   1,205.569 us |       2 B |
| Pathfinding |          50 |          False |               1 |       Dijkstra |   2,095.259 us |    39.8336 us |    42.6215 us |   2,101.160 us |       3 B |
| Pathfinding |          50 |          False |               1 |          AStar |     112.521 us |     0.4429 us |     0.3458 us |     112.499 us |         - |
| Pathfinding |          50 |          False |              10 |            BFS |  11,636.020 us |    35.9500 us |    31.8687 us |  11,642.315 us |      13 B |
| Pathfinding |          50 |          False |              10 |       Dijkstra |  18,577.975 us |   106.4632 us |    88.9016 us |  18,616.995 us |      26 B |
| Pathfinding |          50 |          False |              10 |          AStar |   1,113.935 us |     1.5883 us |     1.4080 us |   1,113.936 us |       2 B |
| Pathfinding |          50 |          False |              50 |            BFS |  60,382.630 us |   174.3251 us |   145.5693 us |  60,342.383 us |     177 B |
| Pathfinding |          50 |          False |              50 |       Dijkstra |  96,677.092 us |   711.2724 us |   665.3246 us |  96,780.090 us |   3,826 B |
| Pathfinding |          50 |          False |              50 |          AStar |   5,890.465 us |    30.8789 us |    28.8841 us |   5,895.873 us |       6 B |
| Pathfinding |          50 |           True |               1 |            BFS |   1,520.631 us |     4.8579 us |     3.7927 us |   1,519.966 us |       2 B |
| Pathfinding |          50 |           True |               1 |       Dijkstra |   2,225.684 us |     4.0935 us |     3.6287 us |   2,225.805 us |       3 B |
| Pathfinding |          50 |           True |               1 |          AStar |     511.817 us |     1.7997 us |     1.6834 us |     511.910 us |       1 B |
| Pathfinding |          50 |           True |              10 |            BFS |  13,515.406 us |    34.6839 us |    30.7464 us |  13,522.898 us |      13 B |
| Pathfinding |          50 |           True |              10 |       Dijkstra |  21,189.563 us |    62.1623 us |    51.9083 us |  21,194.235 us |      26 B |
| Pathfinding |          50 |           True |              10 |          AStar |   1,566.256 us |     5.2937 us |     4.9517 us |   1,565.572 us |       2 B |
| Pathfinding |          50 |           True |              50 |            BFS |  60,312.540 us |   255.0182 us |   238.5441 us |  60,302.035 us |      91 B |
| Pathfinding |          50 |           True |              50 |       Dijkstra |  96,272.761 us |   255.1801 us |   213.0869 us |  96,244.788 us |     267 B |
| Pathfinding |          50 |           True |              50 |          AStar |   6,234.362 us |    33.5339 us |    31.3676 us |   6,225.565 us |       6 B |
| Pathfinding |         100 |          False |               1 |            BFS |   8,146.157 us |    38.0460 us |    33.7268 us |   8,144.293 us |       6 B |
| Pathfinding |         100 |          False |               1 |       Dijkstra |  11,164.120 us |    28.0948 us |    26.2799 us |  11,163.568 us |      13 B |
| Pathfinding |         100 |          False |               1 |          AStar |     324.250 us |     0.5522 us |     0.5165 us |     324.161 us |         - |
| Pathfinding |         100 |          False |              10 |            BFS |  74,994.145 us |   381.2899 us |   338.0035 us |  74,919.351 us |     202 B |
| Pathfinding |         100 |          False |              10 |       Dijkstra | 113,648.145 us |   677.5953 us |   633.8230 us | 113,611.486 us |     320 B |
| Pathfinding |         100 |          False |              10 |          AStar |   3,498.362 us |    66.6145 us |    74.0419 us |   3,529.807 us |       3 B |
| Pathfinding |         100 |          False |              50 |            BFS | 375,443.427 us | 2,692.7943 us | 2,387.0920 us | 374,489.102 us |   2,160 B |
| Pathfinding |         100 |          False |              50 |       Dijkstra | 583,842.371 us | 1,359.0121 us | 1,061.0273 us | 583,882.623 us |   1,696 B |
| Pathfinding |         100 |          False |              50 |          AStar |  17,311.426 us |   339.1948 us |   464.2937 us |  17,014.168 us |      26 B |
| Pathfinding |         100 |           True |               1 |            BFS |  10,206.489 us |    37.1959 us |    31.0603 us |  10,199.456 us |      13 B |
| Pathfinding |         100 |           True |               1 |       Dijkstra |  13,717.419 us |    39.3605 us |    34.8921 us |  13,714.252 us |      13 B |
| Pathfinding |         100 |           True |               1 |          AStar |   2,445.645 us |     8.4955 us |     7.9467 us |   2,448.350 us |       3 B |
| Pathfinding |         100 |           True |              10 |            BFS |  84,855.785 us |   339.3697 us |   317.4466 us |  84,817.748 us |     136 B |
| Pathfinding |         100 |           True |              10 |       Dijkstra | 114,611.955 us |   522.2878 us |   436.1340 us | 114,541.519 us |   1,027 B |
| Pathfinding |         100 |           True |              10 |          AStar |   6,121.504 us |    46.0513 us |    40.8233 us |   6,114.454 us |       6 B |
| Pathfinding |         100 |           True |              50 |            BFS | 385,253.758 us | 7,686.8322 us | 9,440.1202 us | 385,437.944 us |     816 B |
| Pathfinding |         100 |           True |              50 |       Dijkstra | 580,502.924 us | 2,170.0646 us | 2,029.8797 us | 579,891.359 us |   4,992 B |
| Pathfinding |         100 |           True |              50 |          AStar |  18,424.003 us |   137.0396 us |   106.9915 us |  18,378.532 us |      26 B |

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