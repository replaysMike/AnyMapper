using AnyMapper.Tests.TestObjects;
using NUnit.Framework;
using System;

namespace AnyMapper.Tests
{
    [TestFixture]
    public class MapperTests
    {
        [Test]
        public void Should_Create_MappingProfile()
        {
            var testProfile = new TestProfile();
            
            Assert.IsTrue(testProfile.IsValid);
        }

        [Test]
        public void Should_ImplicitMap_SourceObject_To_DestObject()
        {
            var sourceObject = new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
            var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
        }

        [Test]
        public void Should_ProfileMap_SourceObject_To_DestObject()
        {
            var testProfile = new TestProfile();
            var sourceObject = new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
            var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
        }
    }
}
