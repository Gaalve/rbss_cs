using DAL1.RBSS_CS;
using RBSS_CS.Controllers;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RBSS_CS
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
            if (settingsStr == null) throw new NullReferenceException("settings file does not exist");
            var settings = yamlDeserializer.Deserialize<ServerSettings>(settingsStr);
            if (settings == null) throw new NullReferenceException("settings file could not be loaded");
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
                    var integrationTest = new IntegrationTest((DebugApi)debugAPI, (SyncApi)syncAPI, (ModifyApi)modifyAPI, 
                        host.Services.GetService<IPersistenceLayerSingleton>()!);
                    integrationTest.Run();
                }
                

            }
            
        }

        private static Type? GetByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tempType = assembly.GetType(name);
                if (tempType != null) return tempType;
            }

            return null;
        }

        /// <summary>
        /// Create the host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args, ServerSettings serverSettings) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((services) =>
                    {
                        var auxDsType = GetByName(serverSettings.AuxillaryDS);
                        if (auxDsType == null)
                            throw new TypeAccessException("Type not found: " + serverSettings.AuxillaryDS);

                        var genric = typeof(PersistenceLayer<>).MakeGenericType(auxDsType);
                        if (Activator.CreateInstance(genric, true) is not IPersistenceLayerSingleton instance) 
                            throw new TypeAccessException("Type is not assignable as auxillaryDS: " + serverSettings.AuxillaryDS);

                        services.AddSingleton<ServerSettings>(serverSettings ?? new ServerSettings());
                        services.AddSingleton<IPersistenceLayerSingleton>(instance);
                    });

                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:7042/");
                    webBuilder.UseUrls("http://0.0.0.0:80/");
                });
    }
}