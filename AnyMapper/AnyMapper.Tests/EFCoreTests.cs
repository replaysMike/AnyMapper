#if FEATURE_EFCORE
using AnyMapper.Tests.EFCore;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnyMapper.Tests
{
    [TestFixture]
    public class EFCoreTests
    {
        private DbContextOptions<EFCoreDbContext> _options;

        [OneTimeSetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<EFCoreDbContext>()
                .UseInMemoryDatabase(databaseName: "EFCore")
                .Options;

            using (var context = new EFCoreDbContext(_options))
            {
                context.DbObjects.Add(new DbObject { Name = "Name1", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow });
                context.DbObjects.Add(new DbObject { Name = "Name2", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow });
                context.DbObjects.Add(new DbObject { Name = "Name3", Description = "Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow });

                context.SaveChanges();

                context.ChildDbObjects.Add(new ChildDbObject { Name = "Child1-1", ParentDbObjectId = 1 });
                context.ChildDbObjects.Add(new ChildDbObject { Name = "Child1-2", ParentDbObjectId = 1 });
                context.ChildDbObjects.Add(new ChildDbObject { Name = "Child2-1", ParentDbObjectId = 2 });
                context.ChildDbObjects.Add(new ChildDbObject { Name = "Child2-2", ParentDbObjectId = 2 });
                context.SaveChanges();
            }

            Mapper.Initialize();
        }

        [Test]
        public void Should_Duplicate_Row()
        {
            using (var context = new EFCoreDbContext(_options))
            {
                var row1 = context.DbObjects
                    .Include(x => x.ChildDbObjects)
                    .Where(x => x.Id == 1).FirstOrDefault();
                var newRow = Mapper.Map<DbObject, DbObject>(row1, MapOptions.IgnoreEntityKeys);
                context.DbObjects.Add(newRow);
                var rowsModified = context.SaveChanges();
                Assert.AreEqual(3, rowsModified);
            }
        }

        [Test]
        public void ShouldNot_Duplicate_Row()
        {
            using (var context = new EFCoreDbContext(_options))
            {
                var row1 = context.DbObjects.Where(x => x.Id == 1).FirstOrDefault();
                var newRow = Mapper.Map<DbObject, DbObject>(row1, MapOptions.None);
                Assert.Throws<InvalidOperationException>(() => context.DbObjects.Add(newRow));
            }
        }

        [Test]
        public void Should_Create_Row_FromDto()
        {
            using (var context = new EFCoreDbContext(_options))
            {
                var row1 = new DbObjectDto { Name = "Name4", Description = "Test Description", DateCreatedUtc = DateTime.UtcNow, DateModifiedUtc = DateTime.UtcNow };
                var newRow = Mapper.Map<DbObjectDto, DbObject>(row1);
                context.DbObjects.Add(newRow);
                var rowsModified = context.SaveChanges();
                Assert.AreEqual(1, rowsModified);
            }
        }
    }
}
#endif
