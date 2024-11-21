using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapeteer.Exceptions;


[Serializable]
public class MappingNotFoundException : Exception
{
	public MappingNotFoundException() { }
	public MappingNotFoundException(string message) : base(message) { }
	public MappingNotFoundException(string message, Exception inner) : base(message, inner) { }
    public MappingNotFoundException(Type sourceType, Type destinationType)
        : base(GenerateMessage(sourceType, destinationType)) { }

    private static string GenerateMessage(Type sourceType, Type destinationType)
    {
        return $"Mapping not found: The mapping from '{sourceType.FullName}' to '{destinationType.FullName}' could not be found. " +
               "Ensure that a valid mapping is registered for these types. " +
               "You may need to create or configure a custom mapping between them.";
    }
    protected MappingNotFoundException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
