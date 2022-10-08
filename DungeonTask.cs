using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map map)
        {
            var startToChests = BfsTask.FindPaths(map, map.InitialPosition, map.Chests);
            var exitToChests = BfsTask.FindPaths(map, map.Exit, map.Chests).ToDictionary(a => a.Value);
            var startToExit = BfsTask.FindPaths(map, map.InitialPosition, new Point[] { map.Exit });

            if (!startToExit.Any())
                return new MoveDirection[0];
            else if (startToChests.Any() && exitToChests.Any())
            {
                var paths = startToChests
                    .OrderBy(path => path.Length + exitToChests[path.Value].Length)
                    .Select(path => new { First = path, Second = exitToChests[path.Value] })
                    .First();
                return GetMoveDirection(paths.First, paths.Second);
            }
            return ConvertToDirection(startToExit.First());
        }

        private static MoveDirection[] GetMoveDirection(
            SinglyLinkedList<Point> first,
            SinglyLinkedList<Point> second)
        {
            var next = second.Previous;
            while (true)
            {
                first = new SinglyLinkedList<Point>(next.Value, first);
                if (next.Previous == null)
                    break;
                next = next.Previous;
            }
            return ConvertToDirection(first);
        }

        private static MoveDirection[] ConvertToDirection(SinglyLinkedList<Point> path)
        {
            return path.Zip(path.Previous, (current, previos) =>
                Walker.ConvertOffsetToDirection(new Size(current.X - previos.X, current.Y - previos.Y)))
                .Reverse()
                .ToArray();
        }
    }
}

