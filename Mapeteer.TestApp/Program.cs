using Mapeteer;

var mapper = new Mapper();
mapper.AutoMap<Source, Destination>()
    .WithTransform<Source, Destination>((src, dest) =>
    {
        dest.FirstName = src.FullName.Split(' ')[0];
    });

var src = new Source { Id = 1, Username = "Test", FullName = "John Doe Smith" };

var dest = mapper.EnsureMap<Source, Destination>(src);

Console.WriteLine(dest);


public record Source
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
}

public record Destination
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
}
