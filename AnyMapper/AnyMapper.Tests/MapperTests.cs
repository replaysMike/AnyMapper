using AnyMapper.Tests.TestObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
            Assert.AreEqual(destObject.IsEnabled, false);
            Assert.IsNull(destObject.Description);
        }

        [Test]
        public void Should_ImplicitMap_DestObject_To_SourceObject()
        {
            var destObject = new DestObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1), Items = new List<SimpleObject> { { new SimpleObject("Test1", "Name1") }, { new SimpleObject("Test2", "Name2") } } };
            var sourceObject = Mapper.Map<DestObject, SourceObject>(destObject);

            Assert.AreEqual(destObject.Id, sourceObject.Id);
            Assert.AreEqual(destObject.Name, sourceObject.Name);
            Assert.AreEqual(destObject.DateCreated, sourceObject.DateCreated);
            CollectionAssert.AreEquivalent(destObject.Items, sourceObject.Items);
        }

        [Test]
        public void Should_ProfileMap_SourceObject_To_DestObject()
        {
            var profile = new TestProfile();
            var sourceObject = new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
            Mapper.Configure(config =>
            {
                config.AddProfile(profile);
            });
            var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
        }

        [Test]
        public void Should_ProfileMap_SourceObject_To_UniqueObject()
        {
            var profile = new UniqueProfile();
            var sourceObject = new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
            Mapper.Configure(config =>
            {
                config.AddProfile(profile);
            });
            var destObject = Mapper.Map<SourceObject, UniqueObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.UserId);
            Assert.AreEqual(sourceObject.Name, destObject.FullName);
            Assert.AreEqual(DateTime.MinValue, destObject.LogTime);
        }

        [Test]
        public void Should_DiscoverAssemblyProfiles()
        {
            Mapper.Initialize();
            var registry = Mapper.Instance.Registry;

            Assert.IsNotNull(registry);
            Assert.AreEqual(2, registry.ObjectMappings.Count);
        }
    }
}
