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

|      Method | ArrayLength | PathfinderType |          Mean |       Error |      StdDev |        Median | Allocated |
|------------ |------------ |--------------- |--------------:|------------:|------------:|--------------:|----------:|
| Pathfinding |          10 |            BFS |     32.292 us |   0.0706 us |   0.0589 us |     32.284 us |         - |
| Pathfinding |          10 |       Dijkstra |     81.640 us |   1.5845 us |   3.2008 us |     81.590 us |         - |
| Pathfinding |          10 |          AStar |      9.367 us |   0.0264 us |   0.0221 us |      9.366 us |         - |
| Pathfinding |          50 |            BFS |  1,184.628 us |   2.3382 us |   2.0727 us |  1,184.386 us |       2 B |
| Pathfinding |          50 |       Dijkstra |  2,760.720 us |  37.8106 us |  35.3681 us |  2,751.496 us |       3 B |
| Pathfinding |          50 |          AStar |     91.371 us |   1.6556 us |   1.5487 us |     92.229 us |         - |
| Pathfinding |         100 |            BFS |  6,339.231 us |  36.5804 us |  32.4276 us |  6,332.480 us |       6 B |
| Pathfinding |         100 |       Dijkstra | 15,072.149 us | 299.8539 us | 466.8364 us | 14,818.340 us |      16 B |
| Pathfinding |         100 |          AStar |    302.692 us |   6.0176 us |  17.1685 us |    302.455 us |         - |

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
The `UnweightedGraph<T>` is a concrete implementation that you can use as well (an example is below). 
The `UnweightedGridGraph` is also a concrete implementation that can be used directly or as a starting point for grid based graphs. 

Lets take a look at a functional example of a node based graph of plain old strings. 
This illustrates how pathfinding can be used to solve non-spatial based problems.


```csharp
// create an UnweightedGraph with strings as nodes
var graph = new UnweightedGraph<string>();
	
// add a set of 5 nodes and edges for each
graph.Edges["a"] = new string[] { "b" }; // a→b
graph.Edges["b"] = new string[] { "a", "c", "d" }; // b→a, b→c, b→d
graph.Edges["c"] = new string[] { "a" }; // c→a
graph.Edges["d"] = new string[] { "e", "a" }; // d→e, d→a
graph.Edges["e"] = new string[] { "b" }; // e→b

// calculate a path from "c" to "e". The result is c→a→b→d→e, which we can confirm by looking at the edge comments above.
var path = new BreadthFirstPathfinder(graph).Search("c", "e" );
```


## Dijkstra (aka Weighted)
Dijkstra expands on Breadth First Search by introducing a cost for each edge that the algorithm takes into account when pathfinding. 
This lends you more control over the pathfinding process. 
You can add edges with higher costs (moving along a road vs moving through a mud bog for example) to provide a hint that the algorithm should choose the lowest cost path. 
The algorithm will ask you for a cost to get from one node to another and you can choose to return any value that makes sense. 
It will then use the information to find not a only a path to the goal but the lowest cost path.

To implement Dijkstra you have to provide a graph that implements the interface `IWeightedGraph<T>`. 
The `WeightedGridGraph` is a concrete implementation that can be used directly or as a starting point for grid based graphs. 

Below is an example of using the `WeightedGridGraph`. 

```csharp

var graph = new WeightedGridGraph();

// add some weighted nodes
graph.WeightedNodes.Add( new Point( 3, 3 ) );
graph.WeightedNodes.Add( new Point( 3, 4 ) );
graph.WeightedNodes.Add( new Point( 4, 3 ) );
graph.WeightedNodes.Add( new Point( 4, 4 ) );
graph.Walls.Add(new Point(4, 5))
graph.Walls.Add(new Point(5, 5))

// calculate the path
var path = new WeightedPathfinder(graph).Search( new Point( 3, 4 ), new Point( 7, 7 ) );
```


## Astar
Astar is probably the most well known of all pathfinding algorithms. 
It differs from Dijkstra in that it introduces a heuristic for each edge that the algorithm takes into account when pathfinding. 
This lets you add some interesting elements such as edges with higher costs (moving along a road vs moving through a mud bog for example.) 
The algorithm will ask you for a cost to get from one node to another and you can choose to return any value that makes sense. 
It will then use the information to find not a only a path to the goal but the lowest cost path.

To implement Astar you have to provide a graph that implements the interface `IAstarGraph<T>`.
The `AstarGridGraph` is a concrete implementation that can be used directly or as a starting point for grid based graphs. 

Below is an example of using the `AstarGridGraph`. 

# Graphs

3 types of graphs available out of the box:

- GridGraph - Graph represents a 2d array (not really implemente this way). Each point is connected to its neighbours to the left/right/up/down and diagonals (if enabled). Have a natural borders of 0 x 0 to Width x Height. Walls are specified in a HashSet.
- EdgesGraph - Basic graph with vertices and edges. Each verticle can be connected with any number of other points by the directed edges. This graph can not be used in AStar pathfinder as there is no way to specify heuristic calculation for base type.
- EdgesPointGraph - Extension for EdgesGraph that as a vertex use Point class. In this case heuristic can easily be calculated with a manhatten distance and can be used in AStar pathfinder.
- StrightEdgeGraph - graph that can be built using polygon obstacles on a map. Connections between polygon points are built automatically and the final path is built to avoid obstacles intersection.

```csharp

var graph = new AstarGridGraph();

// add some weighted nodes
graph.WeightedNodes.Add( new Point( 3, 3 ) );
graph.WeightedNodes.Add( new Point( 3, 4 ) );
graph.WeightedNodes.Add( new Point( 4, 3 ) );
graph.WeightedNodes.Add( new Point( 4, 4 ) );
graph.Walls.Add(new Point(4, 5))
graph.Walls.Add(new Point(5, 5))

// calculate the path
var path = new AstarPathfinder(graph).Search( new Point( 3, 4 ), new Point( 7, 7 ) );
```

