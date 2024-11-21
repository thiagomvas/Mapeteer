using Mapeteer;
using System.Linq.Expressions;

var mapper = new Mapper();
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
    public Address Address { get; set; }
}

public record Address(string Street, string City, string State, string Zip);
