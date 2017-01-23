using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blobr;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Authentication.Cookies;
using SocialClubNI.Services;

namespace SocialClubNI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddTransient<StorageWrapper>(provider => 
            {
                var azureWrapper = new AzureStorageWrapper(Configuration["tscniBlobAccount"], Configuration["tscniBlobKey"], "testingcontainer");
                return new StorageWrapper(azureWrapper);    
            });

            services.AddTransient<LoginManager>();
            services.AddTransient<ClaimsManager>();

            services.AddTransient<CloudBlobContainer>(provider => 
            {
                var storageAccount = new CloudStorageAccount(new StorageCredentials(Configuration["tscniRealName"], Configuration["tscniRealKey"]), true);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("podcasts");

                return container;
            });

            services.AddAuthorization(options => 
            {
                options.AddPolicy("LoggedIn", policy => policy.RequireClaim(ClaimTypes.NameIdentifier));    
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // setup authorisation
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "TscniCookieMiddlewareInstance",
                LoginPath = new PathString("/login"),
                AccessDeniedPath = new PathString("/forbidden"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                ExpireTimeSpan = new TimeSpan(14, 0, 0, 0),
                SlidingExpiration = true
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
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
