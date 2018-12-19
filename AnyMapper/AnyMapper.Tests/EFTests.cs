#if FEATURE_EF
using AnyMapper.Tests.EF6;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace AnyMapper.Tests
{
    [TestFixture]
    public class EFTests
    {
        private Mock<EFDbContext> _context;
        private MockDataContainer _dataContainer = new MockDataContainer();

        [OneTimeSetUp]
        public void Setup()
        {
            _dataContainer.ClearAndUnregister();
            _context = new Mock<EFDbContext>();
            // enable auto-increment in our in-memory database
            _context.EnableAutoIncrementOnSave();
            _dataContainer
               .AddType<DbObject>(
                    new DbObject { Id = 1, Name = "Name1", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow },
                    new DbObject { Id = 2, Name = "Name2", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow },
                    new DbObject { Id = 3, Name = "Name3", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow }
                )
               .ForContext<EFDbContext>(_context, x => x.DbObjects);
            _dataContainer
               .AddType<ChildDbObject>(
                    new ChildDbObject { Id = 1, Name = "Child1-1", ParentDbObjectId = 1 },
                    new ChildDbObject { Id = 2, Name = "Child1-1", ParentDbObjectId = 1 },
                    new ChildDbObject { Id = 3, Name = "Child2-1", ParentDbObjectId = 2 },
                    new ChildDbObject { Id = 4, Name = "Child2-1", ParentDbObjectId = 2 }
                )
               .ForContext<EFDbContext>(_context, x => x.ChildDbObjects);

            // populate the references
            foreach (var row in _context.Object.ChildDbObjects)
                row.ParentDbObject = _context.Object.DbObjects.Where(x => x.Id == row.ParentDbObjectId).FirstOrDefault();
            foreach (var row in _context.Object.DbObjects)
                row.ChildDbObjects = _context.Object.ChildDbObjects.Where(x => x.ParentDbObjectId == row.Id).ToList();

            Mapper.Initialize();
        }

        [Test]
        public void Should_Duplicate_Row_IgnoreKeys()
        {
            var row1 = _context.Object.DbObjects
                .Include(x => x.ChildDbObjects)
                .Where(x => x.Id == 1).FirstOrDefault();
            var newRow = Mapper.Map<DbObject, DbObject>(row1, MapOptions.IgnoreEntityKeys);
            _context.Object.DbObjects.Add(newRow);
            var rowsModified = _context.Object.SaveChanges();

            // note: children entries are not inserted using mock context with EF6
            Assert.AreEqual(1, rowsModified);
            Assert.AreNotEqual(0, newRow.Id);
        }

        [Test]
        public void Should_Duplicate_Row_IgnoreAutoIncrement()
        {
            var row1 = _context.Object.DbObjects
                .Include(x => x.ChildDbObjects)
                .Where(x => x.Id == 1).FirstOrDefault();
            var newRow = Mapper.Map<DbObject, DbObject>(row1, MapOptions.IgnoreEntityAutoIncrementProperties);
            _context.Object.DbObjects.Add(newRow);
            var rowsModified = _context.Object.SaveChanges();

            // note: children entries are not inserted using mock context with EF6
            Assert.AreEqual(1, rowsModified);
            Assert.AreNotEqual(0, newRow.Id);
        }

        [Test]
        public void ShouldNot_Duplicate_Row()
        {
            var row1 = _context.Object.DbObjects
                .Include(x => x.ChildDbObjects)
                .Where(x => x.Id == 1).FirstOrDefault();
            var newRow = Mapper.Map<DbObject, DbObject>(row1, MapOptions.None);
            _context.Object.DbObjects.Add(newRow);
            var rowsModified = _context.Object.SaveChanges();

            // note: children entries are not inserted using mock context with EF6
            Assert.AreEqual(1, rowsModified);
            // normally this would be a key violation, but our mock context doesn't throw
            Assert.AreEqual(2, _context.Object.DbObjects.Count(x => x.Id == 1));
        }

        [Test]
        public void Should_Create_Row_FromDto()
        {
            var row1 = new DbObjectDto { Id = 1, Name = "Name1", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow };
            var newRow = Mapper.Map<DbObjectDto, DbObject>(row1, MapOptions.IgnoreEntityKeys);
            _context.Object.DbObjects.Add(newRow);
            Assert.AreEqual(0, newRow.Id);
            var rowsModified = _context.Object.SaveChanges();

            // note: children entries are not inserted using mock context with EF6
            Assert.AreEqual(1, rowsModified);
            Assert.AreNotEqual(0, newRow.Id);
        }
    }
}
#endif