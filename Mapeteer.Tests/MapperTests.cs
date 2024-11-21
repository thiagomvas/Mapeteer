using Mapeteer.Exceptions;
using Mapeteer.Tests.Models;
using NuGet.Frameworks;

namespace Mapeteer.Tests;

public class MapperTests
{
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new Mapper();
    }

    [Test]
    public void Map_WithoutMapping_ShouldThrowException()
    {
        var source = new Source();
        Assert.Throws<MappingNotFoundException>(() => _mapper.Map<Source, Destination>(source));
    }

    [Test]
    public void EnsureMap_WithValidSourceAndDestination_NoTransform_ShouldMap()
    {
        // Arrange
        var source = new Source
        {
            Id = 1,
            Username = "johndoe",
            FullName = "John Doe",
            Address = new Address("123 Main St", "Springfield", "IL", "62701")
        };
        // Act
        var destination = _mapper.EnsureMap<Source, Destination>(source);
        // Assert
        Assert.That(destination.Id, Is.EqualTo(source.Id));
        Assert.That(destination.Username, Is.EqualTo(source.Username));
        Assert.That(destination.FullName, Is.EqualTo(source.FullName));
    }

    [Test]
    public void EnsureMap_WithValidSourceAndDestination_WithTransform_ShouldMap()
    {
        // Arrange
        _mapper.BuildAutoMap<Source, Destination>()
            .WithTransform((src, dest) =>
            {
                dest.Street = src.Address.Street;
                dest.City = src.Address.City;
                dest.State = src.Address.State;
                dest.Zip = src.Address.Zip;
            });
        var source = new Source
        {
            Id = 1,
            Username = "johndoe",
            FullName = "John Doe",
            Address = new Address("123 Main St", "Springfield", "IL", "62701")
        };
        // Act
        var destination = _mapper.EnsureMap<Source, Destination>(source);
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(destination.Id, Is.EqualTo(source.Id));
            Assert.That(destination.Username, Is.EqualTo(source.Username));
            Assert.That(destination.FullName, Is.EqualTo(source.FullName));
            Assert.That(destination.Street, Is.EqualTo(source.Address.Street));
            Assert.That(destination.City, Is.EqualTo(source.Address.City));
            Assert.That(destination.State, Is.EqualTo(source.Address.State));
            Assert.That(destination.Zip, Is.EqualTo(source.Address.Zip));
        });
    }

    [Test]
    public void Map_WithInvalidSourceAndDestination_ShouldNotMap()
    {
        var invalidSrc = new IncompatibleSource();
        _mapper.AutoMap<IncompatibleSource, IncompatibleDestination>();

        Assert.That(invalidSrc.Foo.ToString(), Is.Not.EqualTo(_mapper.Map<IncompatibleSource, IncompatibleDestination>(invalidSrc).Foo));
    }

    [Test]
    public void Map_WithValidInnerMappingsManuallyGenerated_ShouldMap()
    {
        var entity = new Entity()
        {
            Id = 10,
            Data = new Source()
            {
                Id = 1,
                Username = "johndoe",
                FullName = "John Doe",
                Address = new Address("123 Main St", "Springfield", "IL", "62701")
            }
        };
        _mapper.BuildAutoMap<Entity, EntityDTO>()
            .WithTransform((src, dest) =>
            {
                dest.Data = _mapper.Map<Source, Destination>(src.Data);
            });
        _mapper.BuildAutoMap<Source, Destination>()
            .WithTransform((src, dest) =>
            {
                dest.Street = src.Address.Street;
                dest.City = src.Address.City;
                dest.State = src.Address.State;
                dest.Zip = src.Address.Zip;
            });

        var dto = _mapper.Map<Entity, EntityDTO>(entity);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(entity.Id));
            Assert.That(dto.Data.Id, Is.EqualTo(entity.Data.Id));
            Assert.That(dto.Data.Username, Is.EqualTo(entity.Data.Username));
            Assert.That(dto.Data.FullName, Is.EqualTo(entity.Data.FullName));
            Assert.That(dto.Data.Street, Is.EqualTo(entity.Data.Address.Street));
            Assert.That(dto.Data.City, Is.EqualTo(entity.Data.Address.City));
            Assert.That(dto.Data.State, Is.EqualTo(entity.Data.Address.State));
            Assert.That(dto.Data.Zip, Is.EqualTo(entity.Data.Address.Zip));
        });
    }

    [Test]
    public void Map_WithValidInnerMappingsAutoGenerated_ShouldMap()
    {
        var entity = new Entity()
        {
            Id = 10,
            Data = new Source()
            {
                Id = 1,
                Username = "johndoe",
                FullName = "John Doe",
                Address = new Address("123 Main St", "Springfield", "IL", "62701")
            }
        };
        _mapper.BuildAutoMap<Entity, EntityDTO>();

        var dto = _mapper.Map<Entity, EntityDTO>(entity);

        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(entity.Id));
            Assert.That(dto.Data.Username, Is.EqualTo(entity.Data.Username));
            Assert.That(dto.Data.FullName, Is.EqualTo(entity.Data.FullName));

            // The address properties should be ignored because they are not mapped via transform.
        });
    }

    [Test]
    public void TwoWayMapping_WithValidSourceAndDestination_IgnoringInnerMappings_ShouldMapBackAndForth()
    {
        _mapper.TwoWayAutoMap<Source, Destination>();
        var source = new Source
        {
            Id = 1,
            Username = "johndoe",
            FullName = "John Doe",
            Address = new Address("123 Main St", "Springfield", "IL", "62701")
        };
        var destination = _mapper.Map<Source, Destination>(source);
        var source2 = _mapper.Map<Destination, Source>(destination);
        Assert.Multiple(() =>
        {
            Assert.That(source.Id, Is.EqualTo(source2.Id));
            Assert.That(source.Username, Is.EqualTo(source2.Username));
            Assert.That(source.FullName, Is.EqualTo(source2.FullName));
        });
    }
}