using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blobr;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

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

            services.AddTransient<CloudBlobContainer>(provider => 
            {
                var storageAccount = new CloudStorageAccount(new StorageCredentials(Configuration["tscniRealName"], Configuration["tscniRealKey"]), true);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("podcasts");

                return container;
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

            app.UseStaticFiles();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "seasons",
                    template: "seasons/{season?}",
                    defaults: new { controller = "Home", Action = "Seasons" }
                );

                routes.MapRoute(
                    name: "download",
                    template: "download/{filename}",
                    defaults: new { controller = "Download", Action = "Download" }
                );

                routes.MapRoute(
                    name: "episode",
                    template: "episode/{season}/{stub}",
                    defaults: new { controller = "Home", Action = "Episode" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{action=Index}/{id?}",
                    defaults: new { controller = "Home" });
            });
        }
    }
}
