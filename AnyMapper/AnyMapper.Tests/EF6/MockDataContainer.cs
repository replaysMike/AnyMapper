#if FEATURE_EF
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AnyMapper.Tests.EF6
{
    /// <summary>
    /// Define a library of data to be wired for mocking unit tests
    /// </summary>
    public class MockDataContainer : IDisposable
    {
        private ICollection<DataPair> _data = new List<DataPair>();

        /// <summary>
        /// Add a type to the container and populate it with data
        /// </summary>
        /// <typeparam name="T">The type of data to store in the collection</typeparam>
        /// <param name="data">Data to populate the collection with</param>
        /// <returns></returns>
        public DataPair<T> AddType<T>(IEnumerable<T> data) where T : class
        {
            var dataPair = new DataPair<T>();
            if (_data.Count(x => x.Type == typeof(T)) == 0)
            {
                if (data != null)
                {
                    foreach (var item in data)
                        dataPair.Collection.Add(item);
                }
                // create the mock DbSet using a list as a data source
                dataPair.Set = MockDbSet.ConfigureAsyncMockSet(dataPair.Collection.AsQueryable());
                //  mock the EF Create() method
                dataPair.Set.Setup(m => m.Create()).Returns(Activator.CreateInstance<T>());
                // mock the EF Include() method
                dataPair.Set.Setup(m => m.Include(It.IsAny<string>())).Returns(dataPair.Set.Object);
                // configure the mock DbSet to move new data to our list
                dataPair.Set.EnableRedirectOnAdd<T>(dataPair.Collection);
                _data.Add(dataPair);
            }
            else
            {
                // key already exists, just add the data
                if (data != null)
                {
                    foreach (var item in data)
                        GetCollection<T>().Add(item);
                }
            }
            return dataPair;
        }

        /// <summary>
        /// Add a type to the container and populate it with data
        /// </summary>
        /// <typeparam name="T">The type of data to store in the collection</typeparam>
        /// <param name="data">An array of data to populate the collection with</param>
        /// <returns></returns>
        public DataPair<T> AddType<T>(params T[] data) where T : class
        {
            return AddType(data.AsEnumerable<T>());
        }

        /// <summary>
        /// Add a type to the container
        /// </summary>
        /// <typeparam name="T">The type of data to store in the collection</typeparam>
        /// <returns></returns>
        public DataPair<T> AddType<T>() where T : class
        {
            return AddType<T>(null);
        }

        /// <summary>
        /// Add data for a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void AddData<T>(T item) where T : class
        {
            GetCollection<T>().Add(item);
        }

        /// <summary>
        /// True if the type key is defined in the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Exists<T>() where T : class
        {
            DataPair<T> dataPair = _data.FirstOrDefault(x => x.Type == typeof(T)) as DataPair<T>;
            return dataPair != null;
        }

        /// <summary>
        /// Clear all data but leave types registered
        /// </summary>
        public void Clear()
        {
            foreach (var t in _data)
            {
                t.Clear();
            }
        }

        /// <summary>
        /// Clear all test data in memory and unregister the types
        /// </summary>
        public void ClearAndUnregister()
        {
            _data.Clear();
        }

        /// <summary>
        /// Clear the collection for a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="KeyNotFoundException"></exception>
        public void Clear<T>() where T : class
        {
            var dataPair = _data.FirstOrDefault(x => x.Type == typeof(T)) as DataPair<T>;
            if (dataPair == null)
                throw new KeyNotFoundException($"Key for type {typeof(T)} does not exist.");
            dataPair.Collection.Clear();
        }

        /// <summary>
        /// Get the collection for a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <returns></returns>
        public ICollection<T> GetCollection<T>() where T : class
        {
            var dataPair = _data.FirstOrDefault(x => x.Type == typeof(T)) as DataPair<T>;
            if (dataPair == null)
                throw new KeyNotFoundException($"Key for type {typeof(T)} does not exist.");
            return dataPair.Collection;
        }

        /// <summary>
        /// Get the Mock DbSet for a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <returns></returns>
        public Mock<DbSet<T>> GetSet<T>() where T : class
        {
            var dataPair = _data.FirstOrDefault(x => x.Type == typeof(T)) as DataPair<T>;
            if (dataPair == null)
                throw new KeyNotFoundException($"Key for type {typeof(T)} does not exist.");
            return dataPair.Set;
        }

        public void Dispose()
        {
            _data.Clear();
        }
    }

    public class DataPair<T> : DataPair where T : class
    {
        public ICollection<T> Collection { get; }
        public Mock<DbSet<T>> Set { get; internal set; }
        public override Type Type => typeof(T);
        public override void Clear()
        {
            if (Collection != null)
                Collection.Clear();
        }

        public DataPair()
        {
            Collection = new List<T>();
            Set = new Mock<DbSet<T>>();
        }

        /// <summary>
        /// Configure the context to use the mock DbSet
        /// </summary>
        /// <typeparam name="TResult">The DbContext to define</typeparam>
        /// <param name="context">An instance of the DbContext</param>
        /// <param name="expression">An expression indicating which table we want to use for this DataPair collection</param>
        /// <param name="asNoTracking">True if you wish to enable AsNoTracking() for the DbSet</param>
        public void ForContext<TResult>(Mock<TResult> context, Expression<Func<TResult, DbSet<T>>> expression, bool asNoTracking = false) where TResult : class
        {
            context
                .Setup(expression)
                .Returns(Set.Object);

            if (asNoTracking)
                Set.Setup(x => x.AsNoTracking());
        }

    }

    public abstract class DataPair
    {
        public abstract Type Type { get; }
        public abstract void Clear();
    }

    public static class MockDbContextExtensions
    {
        /// <summary>
        /// Enable a DbSet to use a list as it's data source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="dataBackedList"></param>
        public static void EnableRedirectOnAdd<T>(this Mock<DbSet<T>> dbSet, ICollection<T> dataBackedList) where T : class
        {
            dbSet
                .Setup(m => m.Add(It.IsAny<T>()))
                .Callback<T>(x => dataBackedList.Add(x))
                .Returns<T>(x => x);
        }
    }
}
#endif