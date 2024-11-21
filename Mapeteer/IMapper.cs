using Mapeteer.Exceptions;
using System.Reflection;
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
    /// Configures automatic mapping between the source and destination types.
    /// </summary>
    /// <param name="source">The source type.</param>
    /// <param name="destination">The destination type.</param>
    /// <returns>An instance of <see cref="IMapper"/>.</returns>
    IMapper AutoMap(Type source, Type destination);

    /// <summary>
    /// Configures automatic mapping between the source and destination types in the specified assemblies.
    /// </summary>
    /// <param name="sourceLib">The source assembly containing all source types.</param>
    /// <param name="destinationLib">The source assembly containing all destination types.</param>
    /// <returns>An instance of <see cref="IMapper"/>.</returns>
    /// <remarks>
    /// By default, this method uses a comparer that matches types by name convention. Assuming an <c>Entity</c> class, 
    /// it'll try to create mappings for <c>EntityDto</c>, <c>EntityViewModel</c>, and <c>EntityVm</c> classes, if they exist.
    /// <br/>
    /// Realistically can only be used in specific cases as it compares purely on conventions.
    /// <br/>
    /// For more complex scenarios, or when the types don't follow the convention, use <see cref="AutoMapAssemblies(Assembly, Assembly, Func{Type, Type, bool})"/>
    /// or <b>preferrably</b>  generate the mappings using <see cref="AutoMap{TSource, TDestination}()"/>.
    /// </remarks>
    IMapper AutoMapAssemblies(Assembly sourceLib, Assembly destinationLib);

    /// <summary>
    /// Configures automatic mapping between the source and destination types in the specified assemblies.
    /// </summary>
    /// <param name="sourceLib">The source assembly containing all source types.</param>
    /// <param name="destinationLib">The source assembly containing all destination types.</param>
    /// <param name="comparer">A comparer function that compares two types to see if they can be mapped.</param>
    /// <returns>An instance of <see cref="IMapper"/>.</returns>
    /// <remarks>
    /// It is recommended to manually generate the mappings using <see cref="AutoMap{TSource, TDestination}()"/>, 
    /// however, if the types you wish to map follow a predictable convention, this can be used to generate mappings
    /// for them, following the provided comparer.
    /// </remarks>
    IMapper AutoMapAssemblies(Assembly sourceLib, Assembly destinationLib, Func<Type, Type, bool> comparer);


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
