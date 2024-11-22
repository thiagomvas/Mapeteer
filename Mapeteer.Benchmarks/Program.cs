
using BenchmarkDotNet.Running;
using Mapeteer.Benchmarks;

//var summary = BenchmarkRunner.Run<MappingBenchmarks>();
//return;

var mapeteer = new Mapeteer.Mapper()
    //.AddTypeConverter<DateTime, string>(d => d.ToString("yyyy-MM-dd"))
    .AddTypeConverter<Status, string>(s => s.ToString())
    .AddTypeConverter<OrderStatus, string>(o => o.ToString());
mapeteer.AutoMap<ContactInfo, ContactInfoDTO>()
    .AutoMap<Order, OrderDTO>(new() {
        { nameof(Order.OrderDate), nameof(OrderDTO.OrderDateFormatted) }
    });
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
    Profile = new Profile
    {
        Bio = "Software developer from Springfield.",
        Contact = new ContactInfo
        {
            PhoneNumber = "555-1234",
            Email = "Foobar@gmail.com",
            SocialMedia = "@johndoe"
        },
        JoinDate = new DateTime(2020, 3, 10)
    }
};

// Map Source to Target using Mapeteer
var target = mapeteer.Map<Source, Target>(source);

Console.WriteLine();