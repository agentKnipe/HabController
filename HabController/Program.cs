using HabController.Models.GPS;
using HabController.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Ports;
using System.Threading;

namespace HabController
{
    internal class Program
    {
        static bool _continue;
        static SerialPort _serialPort;

        private static SatallitesInView _satallitesInView;
        private static Position _currentPosition;
        private static SystemFix _systemFix;


        private static IConfiguration _configuration {  get; set; }

        static void Main(string[] args)
        {
            var services = ConfigureServices();

            var serviceProviders = services.BuildServiceProvider();

            serviceProviders.GetService<HabControllerHub>().Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            var config = LoadConfiguration();
            _configuration = config;

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(provider => config);
            services.AddSingleton<GpsService>();


            services.AddSingleton<HabControllerHub>();

            return services;
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
