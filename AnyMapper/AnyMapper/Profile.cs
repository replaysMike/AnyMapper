using System;
using System.Collections.Generic;
using System.Text;

namespace AnyMapper
{
    /// <summary>
    /// A mapping profile
    /// </summary>
    public class Profile
    {
        public IMappingExpression<TSource, TDest> CreateMap<TSource, TDest>()
        {
            return new MappingExpression<TSource, TDest>();
        }

        /// <summary>
        /// Asserts a valid configuration
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get
            {
                return true;
            }
        }
    }
}
