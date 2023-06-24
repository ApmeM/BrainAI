namespace BrainAI.Tests
{
    using System.Linq;
    using BrainAI.Pathfinding;

    using NUnit.Framework;

    [TestFixture]
    public class LookupTest
    {
        private Pathfinding.Lookup<Point, Point> lookup;

        [SetUp]
        public void Setup()
        {
            this.lookup = new Pathfinding.Lookup<Point, Point>();
            this.lookup.Add(new Point(1, 1), new Point(2, 2));
            this.lookup.Add(new Point(1, 1), new Point(3, 3));
            this.lookup.Add(new Point(2, 1), new Point(3, 3));
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(3, lookup.Count);
        }
        [Test]
        public void Test2()
        {
            Assert.AreEqual(1, lookup[new Point(2,1)].Count());
        }
        [Test]
        public void Test3()
        {
            Assert.AreEqual(2, lookup[new Point(1,1)].Count());
        }
        [Test]
        public void Test4()
        {
            Assert.AreEqual(lookup.count, lookup.Sum(a=>a.Count()));
        }
    }
}
