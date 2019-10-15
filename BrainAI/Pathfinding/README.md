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
var path = BreadthFirstPathfinder.Search( graph, "c", "e" );
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
var path = graph.Search( new Point( 3, 4 ), new Point( 7, 7 ) );
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
var path = graph.Search( new Point( 3, 4 ), new Point( 7, 7 ) );
```



## Influence map
Influence map based on vector implementation.
Usage of influence map in ai implementation provide easy way to avoid obstacles on the way or prevent colliding with danger objects.

Below is an example of using the `InfluenceMap`. 


```csharp

// Initialize influence map
var influenceMap = new InfluenceMap();

// Add some obstacles that should be avoided but their effect should be on a very close distance
influenceMap.Charges.Add( new InfluenceMap.Charge { Point = new Point(70, 10), Value = -5120000, Fading = InfluenceMap.QuadQuadDistanceFading });
influenceMap.Charges.Add( new InfluenceMap.Charge { Point = new Point(30, 50), Value = -5120000, Fading = InfluenceMap.QuadQuadDistanceFading });

// Add bonus that should attract bot from everywhere on a map
influenceMap.Charges.Add( new InfluenceMap.Charge { Point = new Point(30, 30), Value = 320,      Fading = InfluenceMap.ConstantFading });

// Add map border that is pushing bot away (vectors are just describing perpendicular to the wall and it direction does not metter. Use Value to attract or push away.)
influenceMap.Charges.Add( new InfluenceMap.Charge { Point = new Point(0,     0), Value = -640000,  Fading = new WallDistanceFading( 0,  1) });
influenceMap.Charges.Add( new InfluenceMap.Charge { Point = new Point(0,     0), Value = -640000,  Fading = new WallDistanceFading( 1,  0) });
influenceMap.Charges.Add( new InfluenceMap.Charge { Point = new Point(100, 100), Value = -640000,  Fading = new WallDistanceFading( 1,  0) });
influenceMap.Charges.Add( new InfluenceMap.Charge { Point = new Point(100, 100), Value = -640000,  Fading = new WallDistanceFading( 0,  1) });

// calculate the vector
var direction = influenceMap.FindForceDirection( new Point( 3, 4 ) );
```