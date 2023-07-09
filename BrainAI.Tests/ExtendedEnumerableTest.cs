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
            var list = new List<int> { 1, 2, 3, 4 };
            var ext = new ExtendedEnumerable<int>(list, 6);

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 1, 2 }, ext);
        }

        [Test]
        public void MuchMoreThanExists()
        {
            var list = new List<int> { 1, 2, 3, 4 };
            var ext = new ExtendedEnumerable<int>(list, 11);

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3 }, ext);
        }

        [Test]
        public void LessThanExists()
        {
            var list = new List<int> { 1, 2, 3, 4 };
            var ext = new ExtendedEnumerable<int>(list, 3);

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, ext);
        }

        [Test]
        public void OriginalEmpty()
        {
            var list = new List<int>();
            var ext = new ExtendedEnumerable<int>(list, 3);

            CollectionAssert.AreEqual(new List<int>(), ext);
        }
    }
}
