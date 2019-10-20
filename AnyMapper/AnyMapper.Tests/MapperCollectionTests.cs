using AnyMapper.Tests.TestObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnyMapper.Tests
{
    [TestFixture]
    public class MapperCollectionTests
    {
        [Test]
        public void Should_Map_Array()
        {
            Mapper.Initialize();
            var sourceObject = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var destObject = Mapper.Map<int[], int[]>(sourceObject);

            CollectionAssert.AreEqual(sourceObject, destObject);
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

        [Test]
        public void Should_ImplicitMap_SourceArray_To_DestArray()
        {
            var sourceObject = new SourceObject[]
            {
                new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) },
                new SourceObject() { Id = 2, Name = "Source object 2", DateCreated = new DateTime(2018, 2, 1) },
                new SourceObject() { Id = 3, Name = "Source object 3", DateCreated = new DateTime(2018, 3, 1) },
            };
            var destObject = Mapper.Map<SourceObject[], DestObject[]>(sourceObject);

            Assert.AreEqual(3, destObject.Length);
            Assert.AreEqual(sourceObject.First().Id, destObject.First().Id);
            Assert.AreEqual(sourceObject.First().Name, destObject.First().Name);
            Assert.AreEqual(sourceObject.First().DateCreated, destObject.First().DateCreated);
        }

        [Test]
        public void Should_ImplicitMap_SourceCollection_To_DestList()
        {
            var sourceObject = new List<SourceObject>
            {
                new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) },
                new SourceObject() { Id = 2, Name = "Source object 2", DateCreated = new DateTime(2018, 2, 1) },
                new SourceObject() { Id = 3, Name = "Source object 3", DateCreated = new DateTime(2018, 3, 1) },
            };
            var destObject = Mapper.Map<List<SourceObject>, List<DestObject>>(sourceObject);

            Assert.AreEqual(3, destObject.Count);
            Assert.AreEqual(sourceObject.First().Id, destObject.First().Id);
            Assert.AreEqual(sourceObject.First().Name, destObject.First().Name);
            Assert.AreEqual(sourceObject.First().DateCreated, destObject.First().DateCreated);
        }

        [Test]
        public void Should_ImplicitMap_SourceCollection_To_DestCollection()
        {
            ICollection<SourceObject> sourceObject = new List<SourceObject>
            {
                new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) },
                new SourceObject() { Id = 2, Name = "Source object 2", DateCreated = new DateTime(2018, 2, 1) },
                new SourceObject() { Id = 3, Name = "Source object 3", DateCreated = new DateTime(2018, 3, 1) },
            };
            var destObject = Mapper.Map<ICollection<SourceObject>, ICollection<DestObject>>(sourceObject);

            Assert.AreEqual(3, destObject.Count);
            Assert.AreEqual(sourceObject.First().Id, destObject.First().Id);
            Assert.AreEqual(sourceObject.First().Name, destObject.First().Name);
            Assert.AreEqual(sourceObject.First().DateCreated, destObject.First().DateCreated);
        }
    }
}
