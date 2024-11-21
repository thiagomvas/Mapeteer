using Mapeteer.Exceptions;
namespace Mapeteer;

/// <summary>
/// An interface for an object mapper.
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Builds an automatic mapping expression between the source and destination types.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>An instance of <see cref="IMappingExpressionBuilder{TSource, TDestination}"/>.</returns>
    IMappingExpressionBuilder<TSource, TDestination> BuildAutoMap<TSource, TDestination>();

    /// <summary>
    /// Configures automatic mapping between the source and destination types.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>An instance of <see cref="IMapper"/>.</returns>
    IMapper AutoMap<TSource, TDestination>();

    /// <summary>
    /// Ensures that the source object is mapped to the destination type.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object to map.</param>
    /// <returns>An instance of the destination type.</returns>
    /// <remarks>Contrary to <see cref="Map{TSource, TDestination}(TSource)"/>, 
    /// this method generates a mapping using <see cref="AutoMap{TSource, TDestination}"/> 
    /// if it doesn't have a direct map, rather than throwing a <see cref="MappingNotFoundException"/>.</remarks>
    TDestination EnsureMap<TSource, TDestination>(TSource source);

    /// <summary>
    /// Maps the source object to the destination type.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object to map.</param>
    /// <returns>An instance of the destination type.</returns>
    /// <exception cref="MappingNotFoundException">
    /// Thrown when a mapping between the source and destination types is not found.
    /// </exception>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Configures two-way automatic mapping between the source and destination types.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>An instance of <see cref="IMapper"/>.</returns>
    IMapper TwoWayAutoMap<TSource, TDestination>();

    /// <summary>
    /// Adds a custom transformation to the mapping configuration.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="transform">The transformation action to apply.</param>
    /// <returns>An instance of <see cref="IMapper"/>.</returns>
    /// <remarks>Transformations are applied after the automatic mapping. This can be used to configure AutoMaps to have different mapping behaviours, or to configure Inner Mapping.</remarks>
    IMapper WithTransform<TSource, TDestination>(Action<TSource, TDestination> transform);

    /// <summary>
    /// Adds a custom mapper to the mapping configuration.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="mapper">The mapping function</param>
    /// <returns>An instance of <see cref="IMapper"/>.</returns>
    IMapper AddMapper<TSource, TDestination>(Func<TSource, TDestination> mapper);
}
