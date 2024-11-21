namespace Mapeteer.Benchmarks;
public class Source
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public List<Order> Orders { get; set; }
}

public class Target
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public List<OrderDTO> OrderDetails { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

public class Order
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderDTO
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
}