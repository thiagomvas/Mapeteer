using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mapeteer;

/// <summary>
/// An interface for a expression builder for mapping between two types.
/// </summary>
/// <typeparam name="TSource">The source type</typeparam>
/// <typeparam name="TDestination">The destination type</typeparam>
public interface IMappingExpressionBuilder<TSource, TDestination>
{
    /// <summary>
    /// Creates a transformation for <typeparamref name="TSource"/> to <typeparamref name="TDestination"/>.
    /// </summary>
    /// <param name="transform">The transformation to add</param>
    /// <returns>The same instance of <see cref="IMappingExpressionBuilder{TSource, TDestination}"/>. </returns>
    IMappingExpressionBuilder<TSource, TDestination> WithTransform(Action<TSource, TDestination> transform);
}
