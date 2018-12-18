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
        private EFDbContext _context;
        private MockDataContainer _dataContainer = new MockDataContainer();

        [OneTimeSetUp]
        public void Setup()
        {
            _dataContainer.ClearAndUnregister();
            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.SaveChanges())
                .Returns(1);
            _context = new EFDbContext();
            _dataContainer
               .AddType<DbObject>(
                    new DbObject { Id = 1, Name = "Name1", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow },
                    new DbObject { Id = 2, Name = "Name2", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow },
                    new DbObject { Id = 3, Name = "Name3", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow }
                )
               .ForContext<EFDbContext>(mockContext, x => x.DbObjects);
            _dataContainer
               .AddType<ChildDbObject>(
                    new ChildDbObject { Id = 1, Name = "Child1-1", ParentDbObjectId = 1 },
                    new ChildDbObject { Id = 2, Name = "Child1-1", ParentDbObjectId = 1 },
                    new ChildDbObject { Id = 3, Name = "Child2-1", ParentDbObjectId = 2 },
                    new ChildDbObject { Id = 4, Name = "Child2-1", ParentDbObjectId = 2 }
                )
               .ForContext<EFDbContext>(mockContext, x => x.ChildDbObjects);
            var test = _context.ChildDbObjects.Count();
            // populate the references
            foreach (var row in _context.ChildDbObjects)
                row.ParentDbObject = _context.DbObjects.Where(x => x.Id == row.ParentDbObjectId).FirstOrDefault();
            foreach (var row in _context.DbObjects)
                row.ChildDbObjects = _context.ChildDbObjects.Where(x => x.ParentDbObjectId == row.Id).ToList();

            Mapper.Initialize();
        }

        [Test]
        public void DoThing()
        {
            var row1 = _context.DbObjects
                .Include(x => x.ChildDbObjects)
                .Where(x => x.Id == 1).FirstOrDefault();
            var newRow = Mapper.Map<DbObject, DbObject>(row1, MapOptions.IgnoreEntityKeys);
            _context.DbObjects.Add(newRow);
            var rowsModified = _context.SaveChanges();
            Assert.AreEqual(3, rowsModified);
        }
    }
}
#endif