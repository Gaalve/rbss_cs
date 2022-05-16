using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Org.OpenAPIToolsServer;

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
            var host = CreateHostBuilder(args).Build();
            host.RunAsync();
            string? command;
            bool quitNow = false;
            while(!quitNow)
            {
                command = Console.ReadLine();
                var commands = command?.Split(' ');
                switch (commands?[0])
                {
                    case "/help":
                        Console.WriteLine("Use /quit to quit.");
                        break;

                    case "/quit": case "/q":
                        quitNow = true;
                        break;

                    default:
                        Console.WriteLine("Unknown Command " + commands?[0]);
                        break;
                }
            }
        }

        /// <summary>
        /// Create the host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>IHostBuilder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:7042/");
                    webBuilder.UseUrls("http://0.0.0.0:80/");
                });
    }
}