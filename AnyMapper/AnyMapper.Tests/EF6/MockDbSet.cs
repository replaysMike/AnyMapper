#if FEATURE_EF
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyMapper.Tests.EF6
{
    /// <summary>
    /// A static method class for configuring mocked <see cref="DbSet"/> instances.
    /// </summary>
    public class MockDbSet
    {
        public static Mock<DbSet<T>> ConfigureAsyncMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator);

            mockSet.Setup(x => x.Include(It.IsAny<string>()))
                .Returns(mockSet.Object);

            // Mock FindAsync()
            var primaryKeyProperties = EFSaveChangesBehaviors.GetPrimaryKeyNamesUsingReflection(typeof(T), null).ToList();
            if (primaryKeyProperties.Any())
            {
                mockSet
                    .Setup(m => m.FindAsync(It.IsAny<object[]>()))
                    .Returns((object[] keyValueToFind) =>
                    {
                        if (primaryKeyProperties.Count > 1)
                            throw new NotSupportedException("ConfigureAsyncMockSet FindAsync() does not currently support composite keys");

                        return Task.FromResult(data.SingleOrDefault(x =>
                            primaryKeyProperties.First().GetValue(x).Equals(keyValueToFind.First())));
                    });
            }

            return mockSet;
        }

        public static Mock<DbSet<T>> ConfigureMockSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator);

            mockSet.Setup(x => x.Include(It.IsAny<string>()))
                .Returns(mockSet.Object);

            return mockSet;
        }
    }
}
#endif