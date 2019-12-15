using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using LamarCodeGeneration.Model;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;

namespace WebShop.Api
{
    public class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production}.json", 
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elk:9200")))
                .WriteTo.File(new RenderedCompactJsonFormatter(),@"C:\webshop-logs.txt", 
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Getting the motors running...");

                var host = CreateHostBuilder(args).Build();
                Log.Information("Env vars: {@EnvVars}",
                    Environment.GetEnvironmentVariables());
                await host.RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseLamar()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    /*
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLamar()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureAppConfiguration((webHostBuilder, configurationBuilder) =>
                        {
                            configurationBuilder.AddConfiguration(new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile($"appsettings.Development.json").Build());
                        })
                        .UseStartup<Startup>();
                });
    }*/
}