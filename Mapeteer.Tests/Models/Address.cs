using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapeteer.Tests.Models;
internal class Address
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
