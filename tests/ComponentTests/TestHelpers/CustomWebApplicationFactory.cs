using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scraper;

namespace ComponentTests.TestHelpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
{
    private readonly Action<IServiceCollection> _servicesConfiguration;

    public CustomWebApplicationFactory(
        Action<IServiceCollection> servicesConfiguration)
    {
        _servicesConfiguration = servicesConfiguration;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        return base.CreateHost(builder);
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("appsettings.json");
                cb.AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<Startup>()
                    .UseTestServer()
                    .ConfigureTestServices(services =>
                    {
                        _servicesConfiguration(services);
                    });
            });

        return hostBuilder;
    }
}
