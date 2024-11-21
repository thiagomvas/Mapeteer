using BenchmarkDotNet.Attributes;

namespace Mapeteer.Benchmarks;
public class MappingBenchmarks
{
    private List<Source> sources;
    private Mapeteer.IMapper mapeteer;
    private AutoMapper.IMapper autoMapper;

    [Params(10, 100, 1000)]
    public int ItemCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        sources = new List<Source>();
        for (int i = 0; i < ItemCount; i++)
        {
            sources.Add(new Source
            {
                Id = i,
                Name = $"Name {i}",
                Address = new Address
                {
                    Street = $"Street {i}",
                    City = $"City {i}"
                },
                Orders = new List<Order>
                {
                    new Order
                    {
                        OrderId = i,
                        Amount = i * 100
                    }
                }
            });
        }

        mapeteer = new Mapeteer.Mapper();
        mapeteer
            .AutoMap<Order, OrderDTO>()
            .BuildAutoMap<Source, Target>()
            .WithTransform((src, dest) =>
            {
                dest.OrderDetails = mapeteer.Map<Order, OrderDTO>(src.Orders).ToList();
            });

        var config = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Order, OrderDTO>();
            cfg.CreateMap<Source, Target>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.Orders));
        });

        autoMapper = config.CreateMapper();
    }

    [Benchmark]
    public List<Target> Mapeteer()
    {
        return mapeteer.Map<Source, Target>(sources).ToList();
    }
    [Benchmark]
    public List<Target> AutoMapper()
    {
        return autoMapper.Map<IEnumerable<Target>>(sources).ToList();
    }
    [Benchmark]
    public List<Target> Manual()
    {
        return sources.Select(s => new Target
        {
            Id = s.Id,
            Name = s.Name,
            Address = s.Address,
            OrderDetails = s.Orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                Amount = o.Amount
            }).ToList()
        }).ToList();
    }
}
