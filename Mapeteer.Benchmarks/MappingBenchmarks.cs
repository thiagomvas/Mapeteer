﻿using BenchmarkDotNet.Attributes;

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
                Id = 1,
                Name = "John Doe",
                DateOfBirth = new DateTime(1990, 5, 15), // Sample Date
                Status = Status.Active, // Enum value
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "Springfield",
                    State = "IL",
                    PostalCode = "62701"
                },
                Orders = new List<Order>
                {
                    new Order
                    {
                        OrderId = 101,
                        Amount = 150.75m,
                        OrderDate = new DateTime(2024, 11, 21),
                        Status = OrderStatus.Pending,
                        CustomerRemarks = "Urgent"
                    },
                    new Order
                    {
                        OrderId = 102,
                        Amount = 75.50m,
                        OrderDate = new DateTime(2024, 11, 22),
                        Status = OrderStatus.Completed,
                        CustomerRemarks = "Deliver ASAP"
                    }
                },
                Profile = new Profile
                {
                    Bio = "Software developer from Springfield.",
                    Contact = new ContactInfo
                    {
                        PhoneNumber = "555-1234",
                        Email = "Foobar@gmail.com",
                        SocialMedia = "@johndoe"
                    },
                    Hobbies = new List<string> { "https://twitter.com/johndoe", "https://github.com/johndoe" },
                    JoinDate = new DateTime(2020, 3, 10)
                }
            });

            mapeteer = new Mapeteer.Mapper();
            mapeteer.AutoMap<ContactInfo, ContactInfoDTO>()
                .AutoMap<Order, OrderDTO>();
            mapeteer.BuildAutoMap<Profile, ProfileDTO>()
                .WithTransform((src, dest) =>
                {
                    dest.JoinDateFormatted = src.JoinDate.ToString("yyyy-MM-dd");
                    dest.ContactDetails = mapeteer.Map<ContactInfo, ContactInfoDTO>(src.Contact);
                });

            mapeteer.BuildAutoMap<Order, OrderDTO>()
                .WithTransform((src, dest) =>
                {
                    dest.OrderDateFormatted = src.OrderDate.ToString("yyyy-MM-dd");
                    dest.Status = src.Status.ToString();
                });

            mapeteer
                .BuildAutoMap<Source, Target>()
                .WithTransform((src, dest) =>
                {
                    dest.OrderDetails = mapeteer.Map<Order, OrderDTO>(src.Orders).ToList();

                    dest.DateOfBirthFormatted = src.DateOfBirth.ToString("yyyy-MM-dd");

                    dest.Status = src.Status.ToString();
                    dest.ProfileDetails = mapeteer.Map<Profile, ProfileDTO>(src.Profile);
                });



            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                // Map ContactInfo to ContactInfoDTO
                cfg.CreateMap<ContactInfo, ContactInfoDTO>()
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.SocialMedia, opt => opt.MapFrom(src => src.SocialMedia));

                // Map Profile to ProfileDTO
                cfg.CreateMap<Profile, ProfileDTO>()
                    .ForMember(dest => dest.JoinDateFormatted, opt => opt.MapFrom(src => src.JoinDate.ToString("yyyy-MM-dd")))
                    .ForMember(dest => dest.ContactDetails, opt => opt.MapFrom(src => src.Contact)); // ContactInfo to ContactInfoDTO

                // Map Order to OrderDTO
                cfg.CreateMap<Order, OrderDTO>()
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))  // Enum to string mapping
                    .ForMember(dest => dest.OrderDateFormatted, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy-MM-dd")));  // Date transformation

                // Map Source to Target (with nested mappings)
                cfg.CreateMap<Source, Target>()
                    .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.Orders))  // Map Orders to OrderDetails
                    .ForMember(dest => dest.DateOfBirthFormatted, opt => opt.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")))  // Date transformation
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))  // Enum to string mapping
                    .ForMember(dest => dest.ProfileDetails, opt => opt.MapFrom(src => src.Profile));  // Map Profile to ProfileDetails
            });

            // Create the AutoMapper mapper instance
            autoMapper = config.CreateMapper();

        }
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
