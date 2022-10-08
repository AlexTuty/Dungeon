using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeon
{
    public class BfsTask
    {
        public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Point[] chests)
        {
            var visited = new HashSet<Point>();
            var queue = new Queue<SinglyLinkedList<Point>>();
            queue.Enqueue(new SinglyLinkedList<Point>(start));

            while (queue.Count != 0)
            {
                var path = queue.Dequeue();

                if (!visited.Contains(path.Value))
                {
                    visited.Add(path.Value);

                    if (chests.Contains(path.Value))
                        yield return path;

                    Step(map, queue, path);
                }
            }
        }

        private static void Step(Map map, Queue<SinglyLinkedList<Point>> queue, SinglyLinkedList<Point> path)
        {
            for (var dy = -1; dy <= 1; dy++)
            {
                for (var dx = -1; dx <= 1; dx++)
                {
                    var point = new Point(path.Value.X + dx, path.Value.Y + dy);
                    if (dx != 0 && dy != 0 || dx == 0 && dy == 0)
                        continue;
                    else if (map.InBounds(point) && map.Dungeon[point.X, point.Y] == MapCell.Empty)
                        queue.Enqueue(new SinglyLinkedList<Point>(point, path));
                }
            }
        }
    }
}