using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace Dungeon
{
    [TestFixture]
    public class Dungeon_Should
    {
        [Test]
        public void ReturnPathToExit_IfChestIsUnreachable() // Обратный путь к выходу, если ChestIs недоступны
        {
            var textMap = new[]
            {
                "PE#",
                "###",
                "C  "
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new[] { MoveDirection.Right }, path);
        }

        [Test]
        public void ReturnPathToExit_IfNoChests() // Возврат пути к выходу, если нет сундуков
        {
            var textMap = new[]
            {
                "PE",
                "  "
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new[] { MoveDirection.Right }, path);
        }

        [Test]
        public void ReturnEmptyPath_WhenNoPathsToChestAndExit() // Возврат пустого пути при отсутствии путей к сундуку и выходу
        {
            var textMap = new[]
            {
                "P ",
                "##",
                "CE"
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new MoveDirection[0], path);
        }

        [Test]
        public void ReturnEmptyPath_WhenHasPathToChestButNoPathToExit() // Возврат пустого пути, если есть путь к сундуку, но нет пути к выходу
        {
            var textMap = new[]
            {
                "PC",
                "##",
                "CE"
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new MoveDirection[0], path);
        }

        [Test]
        public void ReturnEmptyPath_WhenNoPathToExit() // Возврат пустого пути при отсутствии пути к выходу
        {
            var textMap = new[]
            {
                "P#",
                "#E"
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new MoveDirection[0], path);
        }

        [Test]
        public void ReturnCorrectPath_OnEmptyDungeon()
        {
            var textMap = new[]
            {
                "P ",
                "CE"
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new[] { MoveDirection.Down, MoveDirection.Right }, path);
        }

        [Test]
        public void ReturnCorrectPath_OnSimpleDungeon()
        {
            var textMap = new[]
            {
                "P #",
                "#C#",
                "E  "
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new[] { MoveDirection.Right, MoveDirection.Down, MoveDirection.Down, MoveDirection.Left }, path);
        }

        [Test]
        public void Return_ShortestPath1()
        {
            var textMap = new[]
            {
                "   ",
                " P ",
                " CE"
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new[] { MoveDirection.Down, MoveDirection.Right }, path);
        }

        [Test]
        public void Return_ShortestPath2()
        {
            var textMap = new[]
            {
                "ECC",
                " P ",
                "CCC"
            };
            var map = Map.FromLines(textMap);

            var path = DungeonTask.FindShortestPath(map);

            Assert.AreEqual(new[] { MoveDirection.Up, MoveDirection.Left }, path);
        }

        [Test]
        public void ReturnShortestPath3()
        {
            var map = Map.FromText(Properties.Resources.BigTestDungeon);
            var expectedLength = 211;

            var path = DungeonTask.FindShortestPath(map);

            IsValidPath(map, path, expectedLength);
        }

        [Test, Order(8)]
        [TestCase(500, 40, 50)]
        //[TestCase(500, 81, 103)]
        public void Large_Map_Filled_With_Chests(int timeout, int width, int height)
        {
            var lines = new string[height];
            lines[0] = $"P{new string('C', width - 1)}";
            foreach (var i in Enumerable.Range(1, height - 1))
            {
                lines[i] = new string('C', width);
            }
            var miniMap = Map.FromLines(lines);
            var sw = Stopwatch.StartNew();
            DungeonTask.FindShortestPath(miniMap);
            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds <= timeout, $"Actual time: {sw.ElapsedMilliseconds}, expected {timeout}");
        }

        private void IsValidPath(Map map, MoveDirection[] path, int expectedPathLength)
        {
            var chestTaken = false;
            var chestSet = new HashSet<Point>(map.Chests);
            var walker = new Walker(map.InitialPosition);
            foreach (var step in path)
            {
                walker = walker.WalkInDirection(map, step);
                if (walker.PointOfCollision.HasValue)
                    Assert.Fail($"Collided with wall at {walker.PointOfCollision.Value}");
                if (chestSet.Contains(walker.Position))
                    chestTaken = true;
            }
            Assert.True(chestTaken, "Player did not take any chest.");
            Assert.AreEqual(map.Exit, walker.Position, "Player did not reach the exit.");
            Assert.GreaterOrEqual(path.Length, expectedPathLength, "Hmm.... Seems to be an error in the checker. Please notify us in the comments below.");
            Assert.AreEqual(expectedPathLength, path.Length, "Path must be shortest.");
        }
    }
}

