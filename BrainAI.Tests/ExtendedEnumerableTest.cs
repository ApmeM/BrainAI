namespace BrainAI.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using BrainAI.Pathfinding;

    using NUnit.Framework;

    [TestFixture]
    public class ExtendedEnumerableTest
    {
        [Test]
        public void MoreThanExists()
        {
            var list = new Pathfinding.Lookup<int, int> { { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 } };
            var ext = new ExtendedEnumerable<int>(list[1], 6);

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 1, 2 }, ext);
        }

        [Test]
        public void MuchMoreThanExists()
        {
            var list = new Pathfinding.Lookup<int, int> { { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 } };
            var ext = new ExtendedEnumerable<int>(list[1], 11);

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3 }, ext);
        }

        [Test]
        public void LessThanExists()
        {
            var list = new Pathfinding.Lookup<int, int> { { 1, 1 }, { 1, 2 }, { 1, 3 }, { 1, 4 } };
            var ext = new ExtendedEnumerable<int>(list[1], 3);

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, ext);
        }

        [Test]
        public void OriginalEmpty()
        {
            var list = new Pathfinding.Lookup<int, int>();
            var ext = new ExtendedEnumerable<int>(list[1], 3);

            CollectionAssert.AreEqual(new List<int>(), ext);
        }
    }
}
