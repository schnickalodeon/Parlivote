using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parlivote.Web.Brokers.API;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Configurations;
using Parlivote.Web.Hubs;
using Parlivote.Web.Services.Authentication;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Motions;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Services.Views.Motions;
using RESTFulSense.Clients;
using Syncfusion.Blazor;

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

            services.AddRazorPages(options => options.RootDirectory = "/Views/Pages");
            services.AddServerSideBlazor();
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {"application/octet-stream"});
            });


            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            
            LocalConfigurations localConfigurations = 
                Configuration.Get<LocalConfigurations>();

            services.AddHttpClient<IRESTFulApiFactoryClient, RESTFulApiFactoryClient>(
                client =>
                {
                    client.BaseAddress = new Uri(localConfigurations.ApiConfigurations.Url);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "flonk");
                });

            AddSyncfusionBlazor(services);

          

            AddServices(services);
        }

        private void AddSyncfusionBlazor(IServiceCollection services)
        {

            LocalConfigurations localConfigurations =
                Configuration.Get<LocalConfigurations>();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(localConfigurations.SyncfusionApiKey);

            services.AddSyncfusionBlazor(options => options.IgnoreScriptIsolation = true);

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<MotionHub>("/motionhub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
