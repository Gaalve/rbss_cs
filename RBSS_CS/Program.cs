using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Org.OpenAPITools.Client;
using Org.OpenAPIToolsServer;
using RBBS_CS.Controllers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RBBS_CS
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var yamlDeserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var settingsStr = System.IO.File.Exists("settings.yml") ? System.IO.File.ReadAllText("settings.yml") : null;
            var settings = settingsStr != null ? yamlDeserializer.Deserialize<ServerSettings>(settingsStr) : null;
            var hostB = CreateHostBuilder(args, settings);
            var host = hostB.Build();
            if (settings is not { TestingMode: true, TestingModeInitiator: true }) host.Run();
            else
            {
                host.RunAsync();

                Console.WriteLine("TestMode as Initiator");

                var syncAPI = host.Services.GetService(typeof(SyncApi));
                var debugAPI = host.Services.GetService(typeof(DebugApi));
                var modifyAPI = host.Services.GetService(typeof(ModifyApi));
                Thread.Sleep(1000);

                if (debugAPI != null && syncAPI != null && modifyAPI != null)
                {
                    var integrationTest = new IntegrationTest((DebugApi)debugAPI, (SyncApi)syncAPI, (ModifyApi)modifyAPI);
                    integrationTest.Run();
                }
                

            }

        }

        /// <summary>
        /// Create the host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args, ServerSettings? serverSettings) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    webBuilder.ConfigureServices((services) =>
                    {
                        services.AddSingleton<ServerSettings>(serverSettings ?? new ServerSettings());
                    });

                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:7042/");
                    webBuilder.UseUrls("http://0.0.0.0:80/");
                });
    }
}