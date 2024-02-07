using System.Reflection;
using AutoFixture.Kernel;

namespace UnitTests.Helpers;

public class RealClassGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var type = request as Type;
        if (type is not { IsInterface: true })
        {
            return new NoSpecimen();
        }

        var concreteType = FindConcreteType(type);
        return concreteType == null ? new NoSpecimen() : context.Resolve(concreteType);
    }

    private static Type? FindConcreteType(Type interfaceType)
    {
        var interfaceNameWithoutI = interfaceType.Name[1..];
        if (string.IsNullOrWhiteSpace(interfaceNameWithoutI))
        {
            return null;
        }

        var allTypes = Assembly.Load("Application").GetTypes();

        var concreteTypes = allTypes
            .Where(x => x is { IsClass: true, IsAbstract: false } && interfaceType.IsAssignableFrom(x));

        return concreteTypes.FirstOrDefault();
    }
}
