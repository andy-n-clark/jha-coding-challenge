using JHCC.Core.Infrastructure.InMemoryDatabase;
using JHCC.Core.Infrastructure.MassTransit.Consumers;
using JHCC.Core.Modules.Hashtags;
using JHCC.Core.Modules.Hashtags.Queries;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace JHCC.WebApi
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }        

        public void ConfigureServices(IServiceCollection services)
        {
            //Tweetinvi.Auth.SetUserCredentials(
            //    _configuration["Authentication:Twitter:ConsumerKey"],
            //    _configuration["Authentication:Twitter:ConsumerSecret"],
            //    _configuration["Authentication:Twitter:BearerToken"]);

            //services.AddSingleton<ITwitterClient, TwitterClient>();

            services.AddDbContext<DatabaseContext>(opt => opt.UseInMemoryDatabase("InMemoryDatabase"));

            services.AddMassTransit(busRegistrationConfigurator =>
            {
                busRegistrationConfigurator.AddConsumer<StartSampleStreamConsumer>();

                busRegistrationConfigurator.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IHashtagService, HashtagService>();

            services.AddMediatR(typeof(RetrieveAllHashtagsRequest).GetTypeInfo().Assembly);

            services.AddMemoryCache();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JHCC.WebApi", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JHCC.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
