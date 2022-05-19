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
using Parlivote.Web.Brokers.Authentications;
using Parlivote.Web.Brokers.LocalStorage;
using Parlivote.Web.Brokers.Logging;
using Parlivote.Web.Configurations;
using Parlivote.Web.Hubs;
using Parlivote.Web.Models.Views.Votes;
using Parlivote.Web.Services.Authentication;
using Parlivote.Web.Services.Foundations.Meetings;
using Parlivote.Web.Services.Foundations.Motions;
using Parlivote.Web.Services.Foundations.Users;
using Parlivote.Web.Services.Foundations.Votes;
using Parlivote.Web.Services.Views.Meetings;
using Parlivote.Web.Services.Views.Motions;
using Parlivote.Web.Services.Views.Votes;
using RESTFulSense.Clients;
using Syncfusion.Blazor;

namespace Parlivote.Web
{
    public class Startup
    {

        private readonly LocalConfigurations localConfigurations;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.localConfigurations = configuration.Get<LocalConfigurations>();
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

            services.AddHttpClient<IRESTFulApiFactoryClient, RESTFulApiFactoryClient>(
                client => { client.BaseAddress = new Uri(this.localConfigurations.ApiConfigurations.Url); });

            AddSyncfusionBlazor(services);

            AddServices(services);
        }

        private void AddSyncfusionBlazor(IServiceCollection services)
        {
            string apiKey = this.localConfigurations.SyncfusionApiKey;
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(apiKey);

            services.AddSyncfusionBlazor(options => options.IgnoreScriptIsolation = true);
        }

        private static void AddServices(IServiceCollection services)
        {
            //Brokers
            services.AddTransient<IApiBroker, ApiBroker>();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.AddTransient<IAuthenticationBroker, AuthenticationBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<ILocalStorageBroker, LocalStorageBroker>();

            //Foundation Services
            services.AddTransient<IMotionService, MotionService>();
            services.AddTransient<IMeetingService, MeetingService>();
            services.AddTransient<IVoteService, VoteService>();
            services.AddTransient<IUserService, UserService>();

            //View Services
            services.AddTransient<IMotionViewService, MotionViewService>();
            services.AddTransient<IMeetingViewService, MeetingViewService>();
            services.AddTransient<IVoteViewService, VoteViewService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            
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
