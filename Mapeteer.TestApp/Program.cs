using Mapeteer;

var mapper = new Mapper();
mapper.AddMapper<Address, string>((a) => a.ToString());
mapper.BuildAutoMap<Source, Destination>()
    .WithTransform((src, dest) =>
    {
        dest.FirstName = src.FullName.Split(' ')[0];
    });

var src = new Source { Id = 1, Username = "Test", FullName = "John Doe Smith", Address = new("Foo", "Bar", "Fizz", "58008-420") };

var dest = mapper.EnsureMap<Source, Destination>(src);

Console.WriteLine(dest);

public record Source
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public Address Address { get; set; }
}

public record Destination
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string Address { get; set; }
}
public record Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }

    public Address()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Zip = string.Empty;
    }

    public Address(string street, string city, string state, string zip)
    {
        Street = street;
        City = city;
        State = state;
        Zip = zip;
    }
}

public record Address2
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }

    public Address2()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Zip = string.Empty;
    }

    public Address2(string street, string city, string state, string zip)
    {
        Street = street;
        City = city;
        State = state;
        Zip = zip;
    }
}