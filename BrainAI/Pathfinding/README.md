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
|      Method | ArrayLength | UseStrightEdge | PathFindingRuns | PathfinderType |           Mean |         Error |        StdDev |         Median |            P95 | Allocated |
|------------ |------------ |--------------- |---------------- |--------------- |---------------:|--------------:|--------------:|---------------:|---------------:|----------:|
| Pathfinding |          10 |          False |               1 |            BFS |      26.676 us |     0.3713 us |     0.3473 us |      26.772 us |      26.969 us |         - |
| Pathfinding |          10 |          False |               1 |       Dijkstra |      51.175 us |     1.0211 us |     2.4466 us |      49.841 us |      55.113 us |         - |
| Pathfinding |          10 |          False |               1 |          AStar |       9.719 us |     0.0726 us |     0.0643 us |       9.696 us |       9.809 us |         - |
| Pathfinding |          10 |          False |              10 |            BFS |     269.064 us |     0.6449 us |     0.5717 us |     269.081 us |     269.744 us |         - |
| Pathfinding |          10 |          False |              10 |       Dijkstra |     509.135 us |     1.5010 us |     1.3306 us |     509.050 us |     510.973 us |       1 B |
| Pathfinding |          10 |          False |              10 |          AStar |     100.940 us |     0.2035 us |     0.1903 us |     100.876 us |     101.206 us |         - |
| Pathfinding |          10 |          False |              50 |            BFS |   1,325.226 us |     6.4410 us |     5.0287 us |   1,323.869 us |   1,333.690 us |       2 B |
| Pathfinding |          10 |          False |              50 |       Dijkstra |   2,502.287 us |    12.9523 us |    12.1156 us |   2,499.021 us |   2,516.859 us |       3 B |
| Pathfinding |          10 |          False |              50 |          AStar |     522.226 us |     2.8526 us |     2.6683 us |     522.161 us |     525.830 us |       1 B |
| Pathfinding |          10 |           True |               1 |            BFS |      34.049 us |     0.2154 us |     0.4150 us |      33.946 us |      34.330 us |         - |
| Pathfinding |          10 |           True |               1 |       Dijkstra |      61.935 us |     0.2705 us |     0.2530 us |      61.908 us |      62.335 us |         - |
| Pathfinding |          10 |           True |               1 |          AStar |      17.696 us |     0.1113 us |     0.1041 us |      17.725 us |      17.811 us |         - |
| Pathfinding |          10 |           True |              10 |            BFS |     267.850 us |     1.1917 us |     1.1147 us |     267.722 us |     269.662 us |         - |
| Pathfinding |          10 |           True |              10 |       Dijkstra |     521.001 us |     1.1029 us |     0.9210 us |     520.771 us |     522.277 us |       1 B |
| Pathfinding |          10 |           True |              10 |          AStar |     112.877 us |     2.1738 us |     5.9874 us |     109.325 us |     122.950 us |         - |
| Pathfinding |          10 |           True |              50 |            BFS |   1,321.059 us |    17.9173 us |    34.0896 us |   1,313.688 us |   1,427.462 us |       2 B |
| Pathfinding |          10 |           True |              50 |       Dijkstra |   2,684.310 us |    10.7980 us |     9.5722 us |   2,684.841 us |   2,698.006 us |       3 B |
| Pathfinding |          10 |           True |              50 |          AStar |     511.128 us |    10.1200 us |    21.3466 us |     499.801 us |     554.360 us |       1 B |
| Pathfinding |          50 |          False |               1 |            BFS |   1,023.317 us |     0.9996 us |     0.9350 us |   1,023.150 us |   1,024.634 us |       2 B |
| Pathfinding |          50 |          False |               1 |       Dijkstra |   1,502.961 us |     4.2612 us |     3.7775 us |   1,502.539 us |   1,507.690 us |       2 B |
| Pathfinding |          50 |          False |               1 |          AStar |     134.658 us |     0.4475 us |     0.4186 us |     134.616 us |     135.291 us |         - |
| Pathfinding |          50 |          False |              10 |            BFS |  10,700.253 us |    50.3373 us |    44.6227 us |  10,709.098 us |  10,757.115 us |      13 B |
| Pathfinding |          50 |          False |              10 |       Dijkstra |  15,001.758 us |    30.8403 us |    28.8480 us |  15,005.544 us |  15,042.619 us |      26 B |
| Pathfinding |          50 |          False |              10 |          AStar |   1,335.091 us |     3.5452 us |     3.1427 us |   1,335.023 us |   1,339.298 us |       2 B |
| Pathfinding |          50 |          False |              50 |            BFS |  51,132.841 us |    46.5494 us |    43.5424 us |  51,120.757 us |  51,197.873 us |      82 B |
| Pathfinding |          50 |          False |              50 |       Dijkstra |  75,856.325 us |   288.6234 us |   269.9785 us |  75,809.130 us |  76,301.801 us |     758 B |
| Pathfinding |          50 |          False |              50 |          AStar |   7,355.819 us |   132.2865 us |   241.8934 us |   7,455.821 us |   7,541.490 us |      13 B |
| Pathfinding |          50 |           True |               1 |            BFS |   1,613.221 us |    31.7675 us |    43.4837 us |   1,632.070 us |   1,650.149 us |       2 B |
| Pathfinding |          50 |           True |               1 |       Dijkstra |   2,158.922 us |     8.4397 us |     7.8945 us |   2,158.617 us |   2,170.139 us |       3 B |
| Pathfinding |          50 |           True |               1 |          AStar |     660.985 us |     3.0724 us |     2.8740 us |     661.980 us |     664.371 us |       1 B |
| Pathfinding |          50 |           True |              10 |            BFS |  10,853.880 us |    26.2956 us |    24.5970 us |  10,851.483 us |  10,889.583 us |      13 B |
| Pathfinding |          50 |           True |              10 |       Dijkstra |  16,474.230 us |    66.0608 us |    58.5612 us |  16,486.654 us |  16,546.103 us |      26 B |
| Pathfinding |          50 |           True |              10 |          AStar |   2,074.850 us |     9.7943 us |     8.1786 us |   2,075.783 us |   2,086.448 us |       3 B |
| Pathfinding |          50 |           True |              50 |            BFS |  52,062.248 us |    60.9905 us |    57.0505 us |  52,048.164 us |  52,153.807 us |   1,912 B |
| Pathfinding |          50 |           True |              50 |       Dijkstra |  79,046.819 us |   295.5048 us |   276.4154 us |  79,075.404 us |  79,433.181 us |     136 B |
| Pathfinding |          50 |           True |              50 |          AStar |   7,074.905 us |    44.5669 us |    39.5074 us |   7,077.113 us |   7,140.155 us |       6 B |
| Pathfinding |         100 |          False |               1 |            BFS |   6,512.820 us |    19.7202 us |    18.4463 us |   6,513.478 us |   6,540.504 us |       6 B |
| Pathfinding |         100 |          False |               1 |       Dijkstra |   9,087.343 us |    38.1070 us |    35.6453 us |   9,095.392 us |   9,131.073 us |      13 B |
| Pathfinding |         100 |          False |               1 |          AStar |     448.573 us |     1.6642 us |     1.4753 us |     448.609 us |     450.696 us |         - |
| Pathfinding |         100 |          False |              10 |            BFS |  70,208.434 us |   307.0935 us |   272.2304 us |  70,268.823 us |  70,690.866 us |     102 B |
| Pathfinding |         100 |          False |              10 |       Dijkstra |  97,179.548 us |   435.7577 us |   407.6080 us |  97,222.472 us |  97,706.084 us |     136 B |
| Pathfinding |         100 |          False |              10 |          AStar |   4,628.331 us |    14.2808 us |    12.6596 us |   4,627.571 us |   4,647.579 us |       6 B |
| Pathfinding |         100 |          False |              50 |            BFS | 350,755.400 us | 1,197.4155 us |   999.8962 us | 350,994.046 us | 352,150.444 us |   5,584 B |
| Pathfinding |         100 |          False |              50 |       Dijkstra | 455,693.376 us | 1,788.6878 us | 1,585.6251 us | 455,615.633 us | 458,165.705 us |   2,168 B |
| Pathfinding |         100 |          False |              50 |          AStar |  21,386.565 us |   110.4555 us |    97.9159 us |  21,361.167 us |  21,562.847 us |      26 B |
| Pathfinding |         100 |           True |               1 |            BFS |  10,019.283 us |    38.4330 us |    34.0699 us |  10,010.480 us |  10,073.312 us |      13 B |
| Pathfinding |         100 |           True |               1 |       Dijkstra |  12,460.881 us |    71.0278 us |    66.4394 us |  12,474.942 us |  12,540.345 us |      13 B |
| Pathfinding |         100 |           True |               1 |          AStar |   3,713.460 us |    14.3264 us |    12.6999 us |   3,711.689 us |   3,733.891 us |       3 B |
| Pathfinding |         100 |           True |              10 |            BFS |  74,627.974 us |   230.3585 us |   192.3597 us |  74,581.330 us |  74,952.514 us |     265 B |
| Pathfinding |         100 |           True |              10 |       Dijkstra |  95,502.810 us |   148.2677 us |   123.8103 us |  95,540.990 us |  95,635.319 us |     880 B |
| Pathfinding |         100 |           True |              10 |          AStar |   7,606.013 us |    10.0293 us |     9.3814 us |   7,609.409 us |   7,618.126 us |       6 B |
| Pathfinding |         100 |           True |              50 |            BFS | 325,789.926 us | 2,083.5505 us | 1,847.0132 us | 326,265.258 us | 327,782.162 us |     816 B |
| Pathfinding |         100 |           True |              50 |       Dijkstra | 462,956.540 us | 1,313.5988 us | 1,164.4711 us | 463,222.066 us | 464,023.905 us |   2,168 B |
| Pathfinding |         100 |           True |              50 |          AStar |  24,324.418 us |   195.9561 us |   192.4551 us |  24,303.119 us |  24,658.969 us |      26 B |

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