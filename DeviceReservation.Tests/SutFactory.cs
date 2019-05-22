using DeviceReservation.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DeviceReservation.Tests
{
    public class SutFactory : WebApplicationFactory<Startup>
    {
        string apiKey = "";

        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.ConfigureServices(s =>
            {
                var provider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                s.AddDbContext<DataContext>(b =>
                {
                    b.UseInMemoryDatabase("DeviceReservation");
                    b.UseInternalServiceProvider(provider);
                });

                using (var scope = s.BuildServiceProvider().CreateScope())
                {
                    apiKey = scope.ServiceProvider
                        .GetService<IConfiguration>()
                        .GetSection("ApiKeys")["DefaultKey"];
                }
            });

        protected override void ConfigureClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            base.ConfigureClient(client);
        }
    }
}