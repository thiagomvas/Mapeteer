# Mapeteer - Yet another object mapping library.
This library provides a flexible and efficient way to map objects between different layers of your application. Whether you're transforming data between DTOs and domain models or simply want to decouple your object models, this library offers a clean, maintainable solution for object mapping.

Mapeteer aims to make object mapping simple, flexible, and efficient. By offering both automatic and custom mapping strategies, it gives developers full control over how objects are mapped, while simplifying the code needed to achieve common mapping tasks.

> [!IMPORTANT]
> Mapping libraries like Mapeteer are powerful tools but come with their own trade-offs. While they can simplify complex mapping tasks, they may introduce performance overhead or unwanted complexity in some cases. Always consider whether automatic mapping is the best choice for your specific scenario.


## Features
- **Automatic Property Mapping**: Automatically maps properties from source to destination objects based on matching names and compatible types. This eliminates the need for manual property mapping in simple cases.
- **Custom Transformations**: Apply custom logic to the mapped objects after the automatic mapping is performed. This allows you to manipulate the data, e.g., combining properties or performing complex transformations.
- **Assembly-Wide Mapping**: Automatically map all types between entire assemblies with a configurable type comparer. This is useful when working with large applications where manual mapping for every type would be tedious.
- **Straightforward API**: A clean and intuitive API to handle both simple one-to-one property mappings and complex scenarios with ease.
- **Helpful Error Handling**: Detailed and actionable error messages to guide you in fixing mapping issues quickly. The library provides insight into why the mapping failed and how to resolve it.


## Example Usage
Here’s a quick example of how to use Mapeteer for mapping:
```cs
using Mapeteer;

// Initialize the mapper
IMapper mapper = new Mapper();

// Configure automatic mapping between two types
mapper.AutoMap<SourceType, DestinationType>();

// Automatically map DTOs and ViewModels based on their naming convention (Entity -> EntityDTO). 
// See the documentation on more details of how it works.
// Alternatively, pass your own comparer to generate the mappings.
mapper.AutoMapAssemblies(Assembly.Load("Domain"), Assembly.Load("API"));

// Map a single object
DestinationType result = mapper.Map<SourceType, DestinationType>(sourceObject);

// Map a collection of objects
IEnumerable<DestinationType> results = mapper.Map<SourceType, DestinationType>(sourceCollection);

// Add a custom transformation after automatic mapping
mapper.WithTransform<SourceType, DestinationType>((source, destination) => {
    destination.Property = source.Property + " transformed";
    destination.FirstName = source.FullName.Split(" ").First();
});
```

## Contributing
Contributions are welcome! Feel free to create a new issue or fork the repository and submit pull requests for improvements, bug fixes, or feature requests.

## License
This project is licensed under the MIT License.
