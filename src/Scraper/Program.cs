using Scraper;
using Serilog;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo
            .Console()
            .CreateLogger();
        IDisposable? metricsCollector = null;
        try
        {
            Log.Logger.Debug("init main");
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Stopped program because of exception");
            return 1;
        }
        finally
        {
            // Ensure to flush
            Log.CloseAndFlush();
            metricsCollector?.Dispose();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                    .UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10); });
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });
    }
}