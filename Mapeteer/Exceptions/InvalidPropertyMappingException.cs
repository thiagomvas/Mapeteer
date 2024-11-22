using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapeteer.Exceptions;

[Serializable]
public class InvalidPropertyMappingException : Exception
{
	public InvalidPropertyMappingException() { }
	public InvalidPropertyMappingException(string message) : base(message) { }
	public InvalidPropertyMappingException(string message, Exception inner) : base(message, inner) { }
	public InvalidPropertyMappingException(Type sourceType, Type destinationType) : base($"Could not convert property from '{sourceType.Name}' to '{destinationType.Name}'. This happens when there is not a valid mapper or a type converter function to perform a conversion. See 'Mapper.AddTypeConverter'.") { }
	public InvalidPropertyMappingException(Type sourceType, Type destinationType, Exception inner) : base($"Could not convert property from '{sourceType.Name}' to '{destinationType.Name}'. This happens when there is not a valid mapper or a type converter function to perform a conversion. See 'Mapper.AddTypeConverter'.", inner) { }
    protected InvalidPropertyMappingException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
