﻿using System;
using System.IO;
using System.Linq;

using FubarDev.WebDavServer.AspNetCore;
using FubarDev.WebDavServer.FileSystem;
using FubarDev.WebDavServer.Sample.AspNetCore.Support;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FubarDev.WebDavServer.Sample.AspNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IFileSystemFactory>(new TestFileSystemFactory(GetHomePath()))
                .AddTransient(sp =>
                {
                    var factory = sp.GetRequiredService<IFileSystemFactory>();
                    var context = sp.GetRequiredService<IHttpContextAccessor>();
                    return factory.CreateFileSystem(context.HttpContext.User.Identity);
                })
                .AddMvcCore()
                .AddAuthorization()
                .AddWebDav();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private static string GetHomePath()
        {
            var homeEnvVars = new[] {"HOME", "USERPROFILE", "PUBLIC"};
            var home = homeEnvVars.Select(Environment.GetEnvironmentVariable).First(x => !string.IsNullOrEmpty(x));
            return Path.GetDirectoryName(home);
        }
    }
}
