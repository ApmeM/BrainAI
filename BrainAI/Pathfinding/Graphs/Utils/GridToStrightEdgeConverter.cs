using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainAI.Pathfinding
{
    public class GridToStrightEdgeConverter
    {
        public static GridToStrightEdgeConverter Default = new GridToStrightEdgeConverter();

        private LinkedList<Point> list = new LinkedList<Point>();
        private HashSet<Point> visited = new HashSet<Point>();

        public void BuildGraph(GridGraph graph, StrightEdgeGraph result, int scale = 1)
        {
            visited.Clear();
            result.Clear();
            var obstacle = 0;
            foreach (var wall in graph.Walls)
            {
                if (visited.Contains(wall))
                {
                    continue;
                }

                visited.Add(wall);

                list.Clear();

                var p0 = list.AddLast(wall);
                var p1 = list.AddLast(wall + Point.Right);
                var p2 = list.AddLast(wall + Point.Right + Point.Down);
                var p3 = list.AddLast(wall + Point.Down);

                DFS(graph, visited, list, wall, Point.Right, p1);
                DFS(graph, visited, list, wall, Point.Left, p3);
                DFS(graph, visited, list, wall, Point.Down, p2);
                DFS(graph, visited, list, wall, Point.Up, p0);

                Cleanup(list);

                foreach (var point in list)
                {
                    var newPoint = point * scale;
                    result.AddPoint(obstacle, newPoint);
                }

                obstacle++;
            }
        }

        private static void Cleanup(LinkedList<Point> list)
        {
            var node = list.First;
            while (node != null)
            {
                if ((node.Previous ?? list.Last).Value == (node.Next ?? list.First).Value)
                {
                    var toRemove = node;
                    node = node.Previous ?? list.Last;
                    list.Remove(toRemove.Next ?? list.First);
                    list.Remove(toRemove);
                }
                else
                {
                    node = node.Next;
                }
            }

            node = list.First;
            while (node != null)
            {
                var prev = (node.Previous ?? list.Last).Value;
                var next = (node.Next ?? list.First).Value;
                var cur = node.Value;

                var dir1 = cur - prev;
                var dir2 = next - cur;
                if (
                    Math.Sign(dir1.X) == Math.Sign(dir2.X) &&
                    Math.Sign(dir1.Y) == Math.Sign(dir2.Y)
                )
                {
                    var toRemove = node;
                    node = node.Next;
                    list.Remove(toRemove);
                }
                else
                {
                    node = node.Next;

                }
            }
        }

        private static void DFS(GridGraph graph, HashSet<Point> visited, LinkedList<Point> list, Point p, Point direction, LinkedListNode<Point> afterNode)
        {
            var wall = p + direction;

            if (!graph.Walls.Contains(wall) ||
                visited.Contains(wall))
            {
                return;
            }

            visited.Add(wall);

            LinkedListNode<Point> p1;
            LinkedListNode<Point> p2;
            LinkedListNode<Point> p3;
            LinkedListNode<Point> p0;

            if (direction.X > 0)
            {
                p0 = afterNode;
                p3 = afterNode.Next;
                p2 = list.AddAfter(afterNode, wall + Point.Right + Point.Down);
                p1 = list.AddAfter(afterNode, wall + Point.Right);
            }
            else if (direction.X < 0)
            {
                p2 = afterNode;
                p1 = afterNode.Next;
                p0 = list.AddAfter(afterNode, wall);
                p3 = list.AddAfter(afterNode, wall + Point.Down);
            }
            else if (direction.Y > 0)
            {

                p0 = afterNode.Next;
                p1 = afterNode;
                p3 = list.AddAfter(afterNode, wall + Point.Down);
                p2 = list.AddAfter(afterNode, wall + Point.Right + Point.Down);

            }
            else
            {
                p3 = afterNode;
                p2 = afterNode.Next;
                p1 = list.AddAfter(afterNode, wall + Point.Right);
                p0 = list.AddAfter(afterNode, wall);
            }

            DFS(graph, visited, list, wall, Point.Right, p1);
            DFS(graph, visited, list, wall, Point.Left, p3);
            DFS(graph, visited, list, wall, Point.Down, p2);
            DFS(graph, visited, list, wall, Point.Up, p0);
        }
    }
}