using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using CardRegistration.Infrastructure.DI.Extensions;
using CardRegistration.Infrastructure.DbContexts;

namespace CardRegistration.Host.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMvc();
            services.AddOptions();
            services.AddMemoryCache();
            services.AddHealthChecks();

            services.AddDependencies();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RegistrationService", Version = "v1" });

                var xmlWebBffFile = Path.Combine(AppContext.BaseDirectory, $"Registration.WebApi.xml");
                if (File.Exists(xmlWebBffFile))
                    c.IncludeXmlComments(xmlWebBffFile);
            });

            services.AddDbContext<CustomerCardContext>(options => options.UseInMemoryDatabase("CustomerCards"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Registration.WebApi v1");
                c.DisplayRequestDuration();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
