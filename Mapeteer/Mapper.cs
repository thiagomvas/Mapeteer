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
    public IMapper AutoMap<TSource, TDestination>()
    {
        return AutoMap(typeof(TSource), typeof(TDestination));
    }
    public IMapper AutoMap(Type source, Type destination)
    {
        // Check if a mapper already exists
        if (_mappers.ContainsKey((source, destination)))
        {
            return this;
        }

        var sourceProperties = source.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name);
        var destinationProperties = destination.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var sourceParam = Expression.Parameter(source, "source");
        var destinationExpression = Expression.New(destination);

        var bindings = destinationProperties.Select(destProp =>
        {
            var sourceProp = sourceProperties.GetValueOrDefault(destProp.Name);
            if (sourceProp == null)
            {
                return null;
            }

            if (sourceProp.PropertyType != destProp.PropertyType)
            {
                if (_mappers.TryGetValue((sourceProp.PropertyType, destProp.PropertyType), out var mapper))
                {
                    var sourceValue = Expression.Property(sourceParam, sourceProp);
                    var mappedValue = Expression.Invoke(Expression.Constant(mapper), sourceValue);
                    return Expression.Bind(destProp, mappedValue);
                }

                try
                {
                    var autoMapMethod = GetType().GetMethod(nameof(AutoMap), new[] { typeof(Type), typeof(Type) });
                    autoMapMethod?.Invoke(this, new object[] { sourceProp.PropertyType, destProp.PropertyType });

                    if (_mappers.TryGetValue((sourceProp.PropertyType, destProp.PropertyType), out var autoMapper))
                    {
                        var sourceValue = Expression.Property(sourceParam, sourceProp);
                        var mappedValue = Expression.Invoke(Expression.Constant(autoMapper), sourceValue);
                        return Expression.Bind(destProp, mappedValue);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to auto-map {sourceProp.PropertyType} to {destProp.PropertyType}: {ex.Message}");
                    return null; // Skip this property if mapping fails
                }

                return null;
            }

            var sourceValueSimple = Expression.Property(sourceParam, sourceProp);
            var destValue = Expression.Convert(sourceValueSimple, destProp.PropertyType);
            return Expression.Bind(destProp, destValue);
        }).Where(b => b != null);

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
                    ((Action<TSource, TDestination>)transformer)(source, result);
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

}