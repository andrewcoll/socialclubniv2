using System;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using SocialClubNI.Services;
using Blobr;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace SocialClubNI
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
            services.AddMemoryCache();

            services.AddMvc(options =>
                {
                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                });

            services.AddApplicationInsightsTelemetry(Configuration["AppInsightsKey"]);

            services.Configure<FormOptions>(opt =>
            {
                opt.MultipartBodyLengthLimit = 314572800;
            });

            services.AddTransient<StorageWrapper>(provider => 
            {
                var azureWrapper = new AzureStorageWrapper(Configuration["tscniBlobAccount"], Configuration["tscniBlobKey"], "webdata");
                return new StorageWrapper(azureWrapper);    
            });
            
            services.AddTransient<TelemetryClient>();
            services.AddTransient<LoginManager>();
            services.AddTransient<ClaimsManager>();
            services.AddTransient<MixCloudProvider>();
            services.AddTransient<SeasonProviderFactory>();
            services.AddTransient<MemoryCache>();
            services.AddTransient<PodcastFileProvider>();
            services.AddTransient<CloudBlobContainer>(provider => 
            {
                var storageAccount = new CloudStorageAccount(new StorageCredentials(Configuration["tscniBlobAccount"], Configuration["tscniBlobKey"]), true);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("podcasts");

                return container;
            });          

            services.AddAuthentication(
                o => {
                    o.DefaultChallengeScheme = "TscniCookieMiddlewareInstance";
                    o.DefaultAuthenticateScheme = "TscniCookieMiddlewareInstance";
                    o.DefaultScheme = "TscniCookieMiddlewareInstance";
                })
                .AddCookie("TscniCookieMiddlewareInstance", options =>
                {
                    options.LoginPath = new PathString("/login");
                    options.AccessDeniedPath = new PathString("/forbidden");
                    options.ExpireTimeSpan = new TimeSpan(14, 0, 0, 0);
                    options.SlidingExpiration = true;
                });

            services.AddAuthorization(options => 
            {
                options.AddPolicy("IsLoggedIn", policy => policy.RequireAuthenticatedUser());    
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // register Rss feed middleware
            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/rss"), builder => builder.UseRssFeed());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();          

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "rss",
                    template: "rss",
                    defaults: new { controller = "Rss", Action = "Index" }
                );

                routes.MapRoute(
                    name: "register",
                    template: "register",
                    defaults: new { controller = "Account", Action = "Register" }
                );

                routes.MapRoute(
                    name: "login",
                    template: "login",
                    defaults: new { controller = "Account", Action = "Login" }
                );

                routes.MapRoute(
                    name: "logout",
                    template: "logout",
                    defaults: new { controller = "Account", Action = "Logout" }
                );

                routes.MapRoute(
                    name: "seasons",
                    template: "seasons/{season?}",
                    defaults: new { controller = "Home", Action = "Seasons" }
                );

                routes.MapRoute(
                    name: "download",
                    template: "download/{filename}",
                    defaults: new { controller = "File", Action = "Download" }
                );

                routes.MapRoute(
                    name: "episode",
                    template: "episode/{season}/{stub}",
                    defaults: new { controller = "Home", Action = "Episode" }
                );

                routes.MapRoute(
                    name: "team",
                    template: "team",
                    defaults: new { controller = "Home", Action = "Team" }
                );

                routes.MapRoute(
                    name: "forbidden",
                    template: "forbidden",
                    defaults: new { controller = "Home", Action = "Forbidden" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}
