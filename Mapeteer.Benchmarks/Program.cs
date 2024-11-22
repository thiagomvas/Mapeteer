
using BenchmarkDotNet.Running;
using Mapeteer.Benchmarks;

var summary = BenchmarkRunner.Run<MappingBenchmarks>();
return;

var mapeteer = new Mapeteer.Mapper();
mapeteer.AutoMap<ContactInfo, ContactInfoDTO>()
    .AutoMap<Order, OrderDTO>( new() {
        { nameof(Order.OrderDate), nameof(OrderDTO.OrderDateFormatted) }

})
    .AddTypeConverter<DateTime, string>(d => d.ToString("yyyy-MM-dd"))
    .AddTypeConverter<Status, string>(s =>  s.ToString())
    .AddTypeConverter<OrderStatus, string>(o => o.ToString());
mapeteer.AutoMap<Profile, ProfileDTO>(new()
{
    { nameof(Profile.Contact), nameof(ProfileDTO.ContactDetails) },
    { nameof(Profile.JoinDate), nameof(ProfileDTO.JoinDateFormatted) }
});

mapeteer.BuildAutoMap<Order, OrderDTO>();

mapeteer.AutoMap<Source, Target>(new Dictionary<string, string>() {
            { nameof(Source.DateOfBirth), nameof(Target.DateOfBirthFormatted) },
            { nameof(Source.Profile), nameof(Target.ProfileDetails) }

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


var autoMapper = config.CreateMapper();

var source = new Source
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
};

// Map Source to Target using Mapeteer
var target = mapeteer.Map<Source, Target>(source);

// Map Source to Target using AutoMapper
var autoMapperTarget = autoMapper.Map<Target>(source);

// Compare the results
if (target.Id == autoMapperTarget.Id &&
    target.Name == autoMapperTarget.Name &&
    target.DateOfBirthFormatted == autoMapperTarget.DateOfBirthFormatted &&
    target.Status == autoMapperTarget.Status &&
    target.Address.Street == autoMapperTarget.Address.Street &&
    target.Address.City == autoMapperTarget.Address.City &&
    target.Address.State == autoMapperTarget.Address.State &&
    target.Address.PostalCode == autoMapperTarget.Address.PostalCode &&
    target.OrderDetails.Count == autoMapperTarget.OrderDetails.Count &&
    target.OrderDetails[0].OrderId == autoMapperTarget.OrderDetails[0].OrderId &&
    target.OrderDetails[0].Amount == autoMapperTarget.OrderDetails[0].Amount &&
    target.OrderDetails[0].OrderDateFormatted == autoMapperTarget.OrderDetails[0].OrderDateFormatted &&
    target.OrderDetails[0].Status == autoMapperTarget.OrderDetails[0].Status &&
    target.OrderDetails[1].OrderId == autoMapperTarget.OrderDetails[1].OrderId &&
    target.OrderDetails[1].Amount == autoMapperTarget.OrderDetails[1].Amount &&
    target.OrderDetails[1].OrderDateFormatted == autoMapperTarget.OrderDetails[1].OrderDateFormatted &&
    target.OrderDetails[1].Status == autoMapperTarget.OrderDetails[1].Status &&
    target.ProfileDetails.Bio == autoMapperTarget.ProfileDetails.Bio &&
    target.ProfileDetails.JoinDateFormatted == autoMapperTarget.ProfileDetails.JoinDateFormatted &&
    target.ProfileDetails.ContactDetails.PhoneNumber == autoMapperTarget.ProfileDetails.ContactDetails.PhoneNumber &&
    target.ProfileDetails.ContactDetails.Email == autoMapperTarget.ProfileDetails.ContactDetails.Email &&
    target.ProfileDetails.ContactDetails.SocialMedia == autoMapperTarget.ProfileDetails.ContactDetails.SocialMedia &&
    target.ProfileDetails.Hobbies.Count == autoMapperTarget.ProfileDetails.Hobbies.Count &&
    target.ProfileDetails.Hobbies[0] == autoMapperTarget.ProfileDetails.Hobbies[0] &&
    target.ProfileDetails.Hobbies[1] == autoMapperTarget.ProfileDetails.Hobbies[1])
{
    Console.WriteLine("Mapping successful.");
}
else
{
    Console.WriteLine("Mapping failed.");
}