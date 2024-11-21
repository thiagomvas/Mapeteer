namespace Mapeteer.Tests.Models;
internal class Source
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public Address Address { get; set; }

    public bool IsAdmin { get; set; } // Unmapped property
}
