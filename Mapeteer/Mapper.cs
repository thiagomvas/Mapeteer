namespace Mapeteer;

using Mapeteer.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// Represents an object mapper.
/// </summary>
public class Mapper : IMapper
{
    private readonly Dictionary<(Type, Type), Delegate> _mappers = new();
    private readonly Dictionary<(Type, Type), ICollection<Delegate>> _transformers = new();
    private readonly Dictionary<(Type, Type), Delegate> _typeConverters = new();


    /// <inheritdoc/>
    public IMapper AutoMapAssemblies(Assembly sourceLib, Assembly destinationLib)
    {
        return AutoMapAssemblies(sourceLib,
            destinationLib,
            (s, d) =>
            {
                // Compare by convention
                return d.Name.StartsWith(s.Name) && (d.Name.EndsWith("Dto", StringComparison.InvariantCultureIgnoreCase)
                                                    || d.Name.EndsWith("ViewModel", StringComparison.InvariantCultureIgnoreCase)
                                                    || d.Name.EndsWith("Vm", StringComparison.InvariantCultureIgnoreCase));
            });
    }

    /// <inheritdoc/>
    public IMapper AutoMapAssemblies(Assembly sourceLib, Assembly destinationLib, Func<Type, Type, bool> comparer)
    {
        var sourceTypes = sourceLib.GetTypes();
        var destinationTypes = destinationLib.GetTypes();

        var pairs = sourceTypes.SelectMany(s => destinationTypes.Select(d => (s, d)))
            .Where(pair => comparer(pair.s, pair.d));

        foreach (var (source, destination) in pairs)
        {
            AutoMap(source, destination);
        }

        return this;
    }
    /// <inheritdoc/>
    public IMapper AddTypeConverter<TSource, TDestination>(Func<TSource, TDestination> converter)
    {
        _typeConverters[(typeof(TSource), typeof(TDestination))] = converter;
        return this;
    }

    /// <inheritdoc/>
    public IMapper AutoMap<TSource, TDestination>()
    {
        return AutoMap(typeof(TSource), typeof(TDestination));
    }
    /// <inheritdoc/>
    public IMapper AutoMap<TSource, TDestination>(Dictionary<string, string> propertyMap)
    {
        return AutoMap(typeof(TSource), typeof(TDestination), propertyMap);
    }
    /// <inheritdoc/>
    public IMapper AutoMap(Type source, Type destination)
    {
        return AutoMap(source, destination, new Dictionary<string, string>());
    }
    /// <inheritdoc/>

    public IMapper AutoMap(Type source, Type destination, Dictionary<string, string> propertyMap)
    {
        if(source == typeof(string) && destination == typeof(string))
        {
            return this;
        }
        // Reverse propertyMap key and value
        var reversedPropertyMap = propertyMap.ToDictionary(x => x.Value, x => x.Key);
        if (_mappers.ContainsKey((source, destination)))
            return this;

        var sourceProperties = source.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name);
        var destinationProperties = destination.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var sourceParam = Expression.Parameter(source, "source");
        NewExpression? destinationExpression;

        if(source == typeof(string) && destination == typeof(string))
        {
            destinationExpression = Expression.New(destination.GetConstructor([typeof(char[])]), Expression.Constant(Array.Empty<char>()));
        }
        else
        {
            destinationExpression = Expression.New(destination);
        }

        var bindings = destinationProperties
            .Select(destProp => new { DestProp = destProp, SourceProp = sourceProperties.GetValueOrDefault(reversedPropertyMap.GetValueOrDefault(destProp.Name) ?? destProp.Name) })
            .Where(pair => pair.SourceProp != null)
            .Select(pair =>
            {
                var sourceProp = pair.SourceProp!;
                var destProp = pair.DestProp;

                Expression sourceValue = Expression.Property(sourceParam, sourceProp);

                if (_typeConverters.TryGetValue((sourceProp.PropertyType, destProp.PropertyType), out var converter))
                {
                    var convertedValue = Expression.Invoke(Expression.Constant(converter), sourceValue);
                    return Expression.Bind(destProp, convertedValue);
                }

                if (sourceProp.PropertyType != destProp.PropertyType)
                {
                    Delegate? mapper = null;
                    if (!_mappers.TryGetValue((sourceProp.PropertyType, destProp.PropertyType), out mapper))
                    {
                        try
                        {
                            AutoMap(sourceProp.PropertyType, destProp.PropertyType, propertyMap);
                            _mappers.TryGetValue((sourceProp.PropertyType, destProp.PropertyType), out mapper);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to auto-map {sourceProp.PropertyType} to {destProp.PropertyType}: {ex.Message}");
                            return null;
                        }
                    }
                    if (mapper != null)
                    {
                        return Expression.Bind(destProp, Expression.Invoke(Expression.Constant(mapper), sourceValue));
                    }
                }
                return Expression.Bind(destProp, sourceValue);
            })
            .Where(b => b != null);

        var memberInit = Expression.MemberInit(destinationExpression, bindings);
        var lambda = Expression.Lambda(memberInit, sourceParam).Compile();
        _mappers[(source, destination)] = lambda;
        return this;
    }



    /// <inheritdoc/>
    public IMapper TwoWayAutoMap<TSource, TDestination>()
    {
        AutoMap<TSource, TDestination>();
        AutoMap<TDestination, TSource>();
        return this;
    }
    /// <inheritdoc/>
    public IMapper WithTransform<TSource, TDestination>(Action<TSource, TDestination> transform)
    {
        if (!_transformers.ContainsKey((typeof(TSource), typeof(TDestination))))
        {
            _transformers[(typeof(TSource), typeof(TDestination))] = new List<Delegate>();
        }
        _transformers[(typeof(TSource), typeof(TDestination))].Add(transform);
        return this;
    }
    /// <inheritdoc/>
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        var result = GenerateMappedObject<TSource, TDestination>(source);
        if (result != null)
        {
            return result;
        }

        throw new MappingNotFoundException(typeof(TSource), typeof(TDestination));
    }

    /// <inheritdoc/>
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
            if (_transformers.TryGetValue((typeof(TSource), typeof(TDestination)), out var transformers))
            {
                foreach (var transformer in transformers)
                {
                    transformer.DynamicInvoke(source, result);
                }
            }
            return result;
        }
        return default;
    }

    /// <inheritdoc/>
    public IMapper AddMapper<TSource, TDestination>(Func<TSource, TDestination> mapper)
    {
        _mappers[(typeof(TSource), typeof(TDestination))] = mapper;
        return this;
    }

    /// <inheritdoc/>
    public IMappingExpressionBuilder<TSource, TDestination> BuildAutoMap<TSource, TDestination>()
    {
        return new MappingExpressionBuilder<TSource, TDestination>(this);
    }

    /// <inheritdoc/>
    public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
    {
        return source.Select(Map<TSource, TDestination>);
    }

    /// <inheritdoc/>
    public IEnumerable<TDestination> EnsureMap<TSource, TDestination>(IEnumerable<TSource> source)
    {
        return source.Select(EnsureMap<TSource, TDestination>);
    }


}