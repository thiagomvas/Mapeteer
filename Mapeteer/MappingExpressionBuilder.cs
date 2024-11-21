namespace Mapeteer;

/// <summary>
/// Represents an expression builder for mapping between two types.
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <typeparam name="TDestination"></typeparam>
public class MappingExpressionBuilder<TSource, TDestination> : IMappingExpressionBuilder<TSource, TDestination>
{
    private readonly IMapper _mapper;

    public MappingExpressionBuilder(IMapper mapper)
    {
        _mapper = mapper;
        _mapper.AutoMap<TSource, TDestination>();
    }

    /// <inheritdoc/>
    public IMappingExpressionBuilder<TSource, TDestination> WithTransform(Action<TSource, TDestination> transform)
    {
        _mapper.WithTransform(transform);
        return this;
    }
}
