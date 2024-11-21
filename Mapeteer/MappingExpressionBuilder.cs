namespace Mapeteer;
public class MappingExpressionBuilder<TSource, TDestination> : IMappingExpressionBuilder<TSource, TDestination>
{
    private readonly IMapper _mapper;

    public MappingExpressionBuilder(IMapper mapper)
    {
        _mapper = mapper;
        _mapper.AutoMap<TSource, TDestination>();
    }

    public IMappingExpressionBuilder<TSource, TDestination> WithTransform(Action<TSource, TDestination> transform)
    {
        _mapper.WithTransform(transform);
        return this;
    }
}
