using System;
using System.Linq.Expressions;

namespace AnyMapper
{
    public interface IMappingExpression<TSource, TDest>
    {
        /// <summary>
        /// Map one member to another
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IMappingExpression<TSource, TDest> ForMember(Expression<Func<TSource, Object>> destination, Expression<Func<TDest, Object>> source);
    }
}
