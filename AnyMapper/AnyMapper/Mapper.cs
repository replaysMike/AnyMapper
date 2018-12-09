using System;
using TypeSupport;

namespace AnyMapper
{
    /// <summary>
    /// An instance of a mapper
    /// </summary>
    public class Mapper
    {
        private static readonly Mapper _instance = new Mapper();

        public TypeRegistry TypeRegistry { get; }

        // don't mark type as BeforeFieldInit
        static Mapper()
        {
        }

        private Mapper()
        {
            TypeRegistry = TypeRegistry.Configure((c) => { });
        }

        public Mapper(TypeRegistry typeRegistry)
        {
            TypeRegistry = typeRegistry;
        }

        /// <summary>
        /// A mapping instance
        /// </summary>
        public static Mapper Instance
        {
            get
            {
                return _instance;
            }
        }

        #region Public methods

        

        #endregion

        #region Static methods

        /// <summary>
        /// Maps <typeparamref name="TSource"/> to <typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDest Map<TSource, TDest>(TSource source)
        {
            return Activator.CreateInstance<TDest>();
        }

        #endregion
    }
}
