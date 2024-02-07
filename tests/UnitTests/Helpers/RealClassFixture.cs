using AutoFixture;
using AutoFixture.AutoMoq;

namespace UnitTests.Helpers;

public static class RealClassFixture
{
    public static IFixture Create(bool configureMembers = false)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = configureMembers });

        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        fixture.Customizations.Insert(0, new RealClassGenerator());
        return fixture;
    }
}
