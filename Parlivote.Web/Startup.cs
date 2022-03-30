using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Configurations;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Motions;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Services.Views.Motions;
using RESTFulSense.Clients;

namespace Parlivote.Web
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
            services.AddRazorPages();
            services.AddServerSideBlazor();

            LocalConfigurations localConfigurations = 
                Configuration.Get<LocalConfigurations>();

            services.AddHttpClient<IRESTFulApiFactoryClient, RESTFulApiFactoryClient>(
                client => client.BaseAddress = new Uri(localConfigurations.ApiConfigurations.Url));

            AddServices(services);
        }

        private static void AddServices(IServiceCollection services)
        {
            //Brokers
            services.AddTransient<IApiBroker, ApiBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();

            //View Services
            services.AddTransient<IMotionViewService, MotionViewService>();
            services.AddTransient<IMeetingViewService, MeetingViewService>();

            //Foundation Services
            services.AddTransient<IMotionService, MotionService>();
            services.AddTransient<IMeetingService, MeetingService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
