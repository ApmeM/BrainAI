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
            
            foreach (var wall in graph.Walls)
            {
                if (visited.Contains(wall))
                {
                    continue;
                }

                visited.Add(wall);

                list.Clear();

                var p0 = list.AddLast(new Point(wall.X, wall.Y));
                var p1 = list.AddLast(new Point(wall.X + 1, wall.Y));
                var p2 = list.AddLast(new Point(wall.X + 1, wall.Y + 1));
                var p3 = list.AddLast(new Point(wall.X, wall.Y + 1));

                DFS(graph, visited, list, wall, new Point(1, 0), p1);
                DFS(graph, visited, list, wall, new Point(-1, 0), p3);
                DFS(graph, visited, list, wall, new Point(0, +1), p2);
                DFS(graph, visited, list, wall, new Point(0, -1), p0);

                Cleanup(list);
                var points = list.ToList();
                Scale(points, scale);

                result.AddObstacle(points);
            }
        }

        private static void Scale(List<Point> list, int scale)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = new Point(list[i].X * scale, list[i].Y * scale);
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

                var dir1 = new Point(cur.X - prev.X, cur.Y - prev.Y);
                var dir2 = new Point(next.X - cur.X, next.Y - cur.Y);
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
            var wall = new Point(p.X + direction.X, p.Y + direction.Y);

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
                p2 = list.AddAfter(afterNode, new Point(wall.X + 1, wall.Y + 1));
                p1 = list.AddAfter(afterNode, new Point(wall.X + 1, wall.Y));
            }
            else if (direction.X < 0)
            {
                p2 = afterNode;
                p1 = afterNode.Next;
                p0 = list.AddAfter(afterNode, new Point(wall.X, wall.Y));
                p3 = list.AddAfter(afterNode, new Point(wall.X, wall.Y + 1));
            }
            else if (direction.Y > 0)
            {

                p0 = afterNode.Next;
                p1 = afterNode;
                p3 = list.AddAfter(afterNode, new Point(wall.X, wall.Y + 1));
                p2 = list.AddAfter(afterNode, new Point(wall.X + 1, wall.Y + 1));

            }
            else
            {
                p3 = afterNode;
                p2 = afterNode.Next;
                p1 = list.AddAfter(afterNode, new Point(wall.X + 1, wall.Y));
                p0 = list.AddAfter(afterNode, new Point(wall.X, wall.Y));
            }

            DFS(graph, visited, list, wall, new Point(1, 0), p1);
            DFS(graph, visited, list, wall, new Point(-1, 0), p3);
            DFS(graph, visited, list, wall, new Point(0, +1), p2);
            DFS(graph, visited, list, wall, new Point(0, -1), p0);
        }
    }
}