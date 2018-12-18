#if FEATURE_EF
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(data.Provider);
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