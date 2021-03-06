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
        public void Should_ImplicitMap_To_DestObject()
        {
            var sourceObject = new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
            var destObject = Mapper.Map<DestObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
            Assert.AreEqual(destObject.IsEnabled, false);
            Assert.IsNull(destObject.Description);
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
            var sourceObject = new DestObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1), Items = new List<SimpleObject> { { new SimpleObject("Test1", "Name1") }, { new SimpleObject("Test2", "Name2") } } };
            var destObject = Mapper.Map<DestObject, SourceObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
            CollectionAssert.AreEquivalent(sourceObject.Items, destObject.Items);
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
        public void Should_ImplicitMap_ReadOnlyProperties()
        {
            var sourceObject = new DestObject(333) { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1), Items = new List<SimpleObject> { { new SimpleObject("Test1", "Name1") }, { new SimpleObject("Test2", "Name2") } } };
            var destObject = Mapper.Map<DestObject, SourceObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.AreEqual(sourceObject.ReadOnlyId, destObject.ReadOnlyId);
            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
            CollectionAssert.AreEquivalent(sourceObject.Items, destObject.Items);
        }

        [Test]
        public void Should_ImplicitMap_ReadOnlyFields()
        {
            var sourceObject = new DestObject("readonlyfieldval") { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1), Items = new List<SimpleObject> { { new SimpleObject("Test1", "Name1") }, { new SimpleObject("Test2", "Name2") } } };
            var destObject = Mapper.Map<DestObject, SourceObject>(sourceObject);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.IsTrue(sourceObject.ValidateReadOnlyField("readonlyfieldval"));
            Assert.IsTrue(destObject.ValidateReadOnlyField("readonlyfieldval"));
            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
            CollectionAssert.AreEquivalent(sourceObject.Items, destObject.Items);
        }

        [Test]
        public void Should_ImplicitMap_ReadOnlyComputedFields()
        {
            var sourceObject = new Person { Name = "Test User", DOB = new DateTime(1985, 11, 14) };
            var destObject = Mapper.Map<Person, Customer>(sourceObject);

            Assert.AreEqual(sourceObject.Name, destObject.Name);
            Assert.AreEqual(sourceObject.DOB, destObject.DOB);
            Assert.AreEqual(sourceObject.Age, destObject.Age);
        }

        [Test]
        public void Should_ImplicitMap_NonNullableToNullable()
        {
            var sourceObject = new SourceObject { NullableInt = 123 };
            var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject);

            Assert.NotNull(destObject.NullableInt);
            Assert.AreEqual(sourceObject.NullableInt, destObject.NullableInt.Value);
        }

        [Test]
        public void Should_ImplicitMap_NullableToNonNullable()
        {
            var sourceObject = new DestObject { NullableInt = 123 };
            var destObject = Mapper.Map<DestObject, SourceObject>(sourceObject);

            Assert.NotNull(sourceObject.NullableInt);
            Assert.AreEqual(sourceObject.NullableInt.Value, destObject.NullableInt);
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

        [Test]
        public void Should_UseConfiguredProfiles()
        {
            var profiles = new List<Profile>();
            profiles.Add(new TestProfile());
            Mapper.Initialize(profiles);
            var registry = Mapper.Instance.Registry;

            Assert.IsNotNull(registry);
            Assert.AreEqual(1, registry.ObjectMappings.Count);
        }

        [Test]
        public void Should_ImplicitMap_SourceObject_To_DestObject_WithIgnore()
        {
            var sourceObject = new SourceObject() { Id = 1, Name = "Source object", DateCreated = new DateTime(2018, 1, 1) };
            var destObject = Mapper.Map<SourceObject, DestObject>(sourceObject, x => x.Name);

            Assert.AreEqual(sourceObject.Id, destObject.Id);
            Assert.AreEqual(null, destObject.Name);
            Assert.AreEqual(sourceObject.DateCreated, destObject.DateCreated);
            Assert.AreEqual(destObject.IsEnabled, false);
            Assert.IsNull(destObject.Description);
        }
    }
}
