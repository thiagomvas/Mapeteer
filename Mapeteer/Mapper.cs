namespace Mapeteer;

using Mapeteer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public class Mapper : IMapper
{
    private readonly Dictionary<(Type, Type), Delegate> _mappers = new();
    private readonly Dictionary<(Type, Type), ICollection<Delegate>> _transformers = new();

    public IMapper AutoMap<TSource, TDestination>()
    {
        var srcType = typeof(TSource);
        var destType = typeof(TDestination);
        // Check if a mapper already exists
        if (_mappers.ContainsKey((srcType, destType)))
        {
            return this;
        }

        var sourcePropDict = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name);
        var destProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var sourceParam = Expression.Parameter(srcType, "source");
        var destination = Expression.New(destType);

        var bindings = destProps.Select(destProp =>
        {
            var sourceProp = sourcePropDict.GetValueOrDefault(destProp.Name);
            if (sourceProp == null)
            {
                return null;
            }
            if(sourceProp.PropertyType != destProp.PropertyType)
            {
                if (_mappers.TryGetValue((sourceProp.PropertyType, destProp.PropertyType), out var mapper))
                {
                    var typedMapper = mapper;
                    var sourceValue2 = Expression.Property(sourceParam, sourceProp);
                    var destValue2 = Expression.Invoke(Expression.Constant(typedMapper), sourceValue2);
                    return Expression.Bind(destProp, destValue2);
                }
                else return null;

            }

            var sourceValue = Expression.Property(sourceParam, sourceProp);
            var destValue = Expression.Convert(sourceValue, destProp.PropertyType);
            return Expression.Bind(destProp, destValue);
        }).Where(b => b != null);

        var memberInit = Expression.MemberInit(destination, bindings);
        var lambda = Expression.Lambda<Func<TSource, TDestination>>(memberInit, sourceParam).Compile();

        _mappers[(srcType, destType)] = lambda;

        return this;
    }
    public IMapper TwoWayAutoMap<TSource, TDestination>()
    {
        AutoMap<TSource, TDestination>();
        AutoMap<TDestination, TSource>();
        return this;
    }
    public IMapper WithTransform<TSource, TDestination>(Action<TSource, TDestination> transform)
    {
        if(!_transformers.ContainsKey((typeof(TSource), typeof(TDestination))))
        {
            _transformers[(typeof(TSource), typeof(TDestination))] = new List<Delegate>();
        }
        _transformers[(typeof(TSource), typeof(TDestination))].Add(transform);
        return this;
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        var result = GenerateMappedObject<TSource, TDestination>(source);
        if (result != null)
        {
            return result;
        }

        throw new MappingNotFoundException(typeof(TSource), typeof(TDestination));
    }

    public TDestination EnsureMap<TSource, TDestination>(TSource source)
    {
        var result = GenerateMappedObject<TSource, TDestination>(source);
        if (result != null)
        {
            return result;
        }
        AutoMap<TSource, TDestination>();
        return Map<TSource, TDestination>(source);
    }

    private TDestination? GenerateMappedObject<TSource, TDestination>(TSource source) 
    {
        if (_mappers.TryGetValue((typeof(TSource), typeof(TDestination)), out var mapper))
        {
            var typedMapper = (Func<TSource, TDestination>)mapper;

            var result = typedMapper(source);   
            if(_transformers.TryGetValue((typeof(TSource), typeof(TDestination)), out var transformers))
            {
                foreach (var transformer in transformers)
                {
                    ((Action<TSource, TDestination>)transformer)(source, result);
                }
            }
            return result;
        }
        return default;
    }

    public IMapper AddMapper<TSource, TDestination>(Func<TSource, TDestination> mapper)
    {
        _mappers[(typeof(TSource), typeof(TDestination))] = mapper;
        return this;
    }

    public IMappingExpressionBuilder<TSource, TDestination> BuildAutoMap<TSource, TDestination>()
    {
        return new MappingExpressionBuilder<TSource, TDestination>(this);
    }
}