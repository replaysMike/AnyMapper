using System;
using System.Linq.Expressions;

namespace AnyMapper
{
    public interface IMappingExpression<TSource, TDest>
    {
        Expression<Func<TDest, object>> Destination { get; }
        Expression<Func<TSource, object>> Source { get; }

        /// <summary>
        /// Map one member to another
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IMappingExpression<TSource, TDest> ForMember(Expression<Func<TDest, Object>> destination, Expression<Func<TSource, Object>> source);

        /// <summary>
        /// Map one member to another
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="resolutionMethod"></param>
        /// <returns></returns>
        IMappingExpression<TSource, TDest> ForMember(Expression<Func<TDest, object>> destination, Func<TSource, MappingContext, Object> resolutionMethod);
    }
}
