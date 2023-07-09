namespace BrainAI.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class LookupTest
    {
        [Test]
        public void CountsTest()
        {
            var lookup = new Pathfinding.Lookup<int, int>();
            lookup.Add(1,2);
            lookup.Add(2,3);
            lookup.Add(1,3);

            Assert.AreEqual(3, lookup.Count);
            Assert.AreEqual(1, lookup.Find(2).Count);
            Assert.AreEqual(2, lookup.Find(1).Count);
        }

        [Test]
        public void RemoveLastElement()
        {
            var lookup = new Pathfinding.Lookup<int, int>();
            lookup.Add(1, 2);
            lookup.Add(2, 3);

            lookup.Remove(2, 3);
            Assert.AreEqual(0, lookup.Find(2).Count);
        }

        [Test]
        public void IgnoreDuplicate()
        {
            var lookup = new Pathfinding.Lookup<int, int>(true);
            lookup.Add(1, 2);
            lookup.Add(1, 3);
            lookup.Add(2, 3);
            lookup.Add(1, 2);
            lookup.Add(1, 3);

            Assert.AreEqual(3, lookup.Count);
            CollectionAssert.AreEqual(new List<int> { 2, 3 }, lookup.Find(1));
            CollectionAssert.AreEqual(new List<int> { 3 }, lookup.Find(2));
        }

        [Test]
        public void CheckLookupOrder()
        {
            var lookup = new Pathfinding.Lookup<int, int>(false);
            lookup.Add(1, 1);
            lookup.Add(1, 3);
            lookup.Add(1, 1);
            lookup.Add(1, 5);

            CollectionAssert.AreEqual(new List<int> { 1, 3, 1, 5 }, lookup.Find(1));
        }
    }
}
