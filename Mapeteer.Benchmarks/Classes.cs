namespace Mapeteer.Benchmarks
{
    // Represents a person with a profile
    public class Source
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public List<Order> Orders { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Status Status { get; set; }  // Enum to string mapping
        public Profile Profile { get; set; }  // Complex nested object
    }

    public class Target
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public List<OrderDTO> OrderDetails { get; set; }
        public string DateOfBirthFormatted { get; set; }  // Date transformation
        public string Status { get; set; }  // Enum to string transformation
        public ProfileDTO ProfileDetails { get; set; }  // Complex nested object mapping
    }

    // A nested object that represents an address
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

    // Represents an order with more complex details
    public class Order
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }  // Date transformation required
        public OrderStatus Status { get; set; }  // Enum to string mapping
        public string CustomerRemarks { get; set; }
    }

    // DTO for order, simplified version
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderDateFormatted { get; set; }  // Date formatting
        public string Status { get; set; }  // Enum to string
    }

    // Enum for status to test more complex mappings
    public enum Status
    {
        Active,
        Inactive,
        Pending,
        Suspended
    }

    // Enum for order status to test complex mappings
    public enum OrderStatus
    {
        Pending,
        Completed,
        Shipped,
        Cancelled
    }

    // A profile with nested objects
    public class Profile
    {
        public string Bio { get; set; }
        public DateTime JoinDate { get; set; }
        public List<string> Hobbies { get; set; }
        public ContactInfo Contact { get; set; }  // Nested complex object
    }

    // DTO for profile details
    public class ProfileDTO
    {
        public string Bio { get; set; }
        public string JoinDateFormatted { get; set; }  // Date formatting
        public List<string> Hobbies { get; set; }
        public ContactInfoDTO ContactDetails { get; set; }  // Nested object mapping
    }

    // Represents a contact information with more complex data
    public class ContactInfo
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SocialMedia { get; set; }
    }

    // DTO for contact info
    public class ContactInfoDTO
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SocialMedia { get; set; }
    }
}
