using System;
using System.Collections.Generic;
namespace Mapeteer.Tests.Models;
internal class Destination
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public int Age { get; set; } // Unmapped property
    public Destination()
    {
        Id = 0;
        Username = string.Empty;
        FullName = string.Empty;
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Zip = string.Empty;
    }
    public Destination(int id, string username, string fullName, string street, string city, string state, string zip)
    {
        Id = id;
        Username = username;
        FullName = fullName;
        Street = street;
        City = city;
        State = state;
        Zip = zip;
    }
}
