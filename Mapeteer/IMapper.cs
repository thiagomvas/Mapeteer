
namespace Mapeteer;

public interface IMapper
{
    IMappingExpressionBuilder<TSource, TDestination> BuildAutoMap<TSource, TDestination>();
    IMapper AutoMap<TSource, TDestination>();
    TDestination EnsureMap<TSource, TDestination>(TSource source);
    TDestination Map<TSource, TDestination>(TSource source);
    IMapper TwoWayAutoMap<TSource, TDestination>();
    IMapper WithTransform<TSource, TDestination>(Action<TSource, TDestination> transform);
}