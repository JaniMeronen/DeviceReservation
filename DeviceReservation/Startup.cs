using AspNet.Security.ApiKey.Providers;
using AspNet.Security.ApiKey.Providers.Events;
using AspNet.Security.ApiKey.Providers.Extensions;
using DeviceReservation.Models;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DeviceReservation
{
    public class Startup
    {
        readonly IConfiguration configuration;
        readonly IHostingEnvironment environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }

        public void Configure(IApplicationBuilder builder)
        {
            if (environment.IsDevelopment())
            {
                builder.UseDatabaseErrorPage();
                builder.UseDeveloperExceptionPage();
            }
            else
            {
                builder.UseHsts();
            }

            builder.UseAuthentication();

            builder.UseHttpsRedirection();
            builder.UseMvc(b =>
            {
                b.EnableDependencyInjection();
                b.Count().Expand().Filter().OrderBy().Select();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(o => o.DefaultScheme = ApiKeyDefaults.AuthenticationScheme)
                .AddApiKey(o =>
                {
                    o.Events.OnApiKeyValidated = OnApiKeyValidated;
                    o.Header = "X-API-Key";
                    o.HeaderKey = "";
                });

            services.AddDbContext<DataContext>(
                b => b.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddOData();
        }

        Task OnApiKeyValidated(ApiKeyValidatedContext context)
        {
            if (context.ApiKey == configuration.GetSection("ApiKeys")["DefaultKey"])
            {
                context.Success();
            }

            return Task.CompletedTask;
        }
    }
}