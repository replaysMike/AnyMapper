using AnyMapper.Tests.TestObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnyMapper.Tests
{
    [TestFixture]
    public class MapperTypeTests
    {
        [Test]
        public void Should_Map_Collection()
        {
            Mapper.Initialize();
            var sourceObject = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            var destObject = Mapper.Map<List<int>, List<int>>(sourceObject);

            CollectionAssert.AreEqual(sourceObject, destObject);
        }

        [Test]
        public void Should_Map_Dictionary()
        {
            Mapper.Initialize();
            var sourceObject = new Dictionary<int, string>() { { 1, "Name1" }, { 2, "Name2" }, { 3, "Name3" }, };
            var destObject = Mapper.Map<Dictionary<int, string>, Dictionary<int, string>>(sourceObject);

            CollectionAssert.AreEqual(sourceObject, destObject);
        }

        [Test]
        public void Should_Map_Array()
        {
            Mapper.Initialize();
            var sourceObject = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var destObject = Mapper.Map<int[], int[]>(sourceObject);

            CollectionAssert.AreEqual(sourceObject, destObject);
        }

        [Test]
        public void Should_Map_String()
        {
            Mapper.Initialize();
            var sourceObject = "Test data";
            var destObject = Mapper.Map<string, string>(sourceObject);

            Assert.AreEqual(sourceObject, destObject);
        }

        [Test]
        public void Should_Map_ExistingCollection()
        {
            Mapper.Initialize();
            var sourceObject = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            var destObject = new List<int>() { 100, 200 };
            destObject = Mapper.Map<List<int>, List<int>>(sourceObject, destObject);

            // there should be 2 extra elements in destObject
            Assert.AreEqual(sourceObject.Count + 2, destObject.Count);
        }
    }
}
