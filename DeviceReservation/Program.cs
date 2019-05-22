﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DeviceReservation
{
    class Program
    {
        static void Main(string[] args) =>
            CreateWebHostBuilder(args).Build().Run();

        static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}