// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Host.Configuration;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpringComp.IdentityServer.TableStorage.Extensions;
using SpringComp.IdentityServer.TableStorage.Options;
using SpringComp.IdentityServer.TableStorage.Stores;

namespace Host
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfigurationRoot Configuration { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddUserSecrets<Startup>()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options => { options.EnableEndpointRouting = false; })
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0)
                ;

            var builder = services.AddIdentityServer();

            builder
                .AddSecretParser<JwtBearerClientAssertionSecretParser>()
                .AddSecretValidator<PrivateKeyJwtSecretValidator>()
                ;

            var connectionString = "DefaultEndpointsProtocol=https;AccountName=masked;AccountKey=JFXvVpoiQEExHltdNcO3LAHnD/6pnbsokpn5KIxERiesbICw99q2VFqrrzUXax6y9D0XM3inWdkeBJCZCUUfdw==;EndpointSuffix=core.windows.net;";

            builder
                .AddConfigurationStore(connectionString)
                .AddOperationalStore(connectionString, options =>
                {
                    // this enables automatic token cleanup. this is optional
                    options.EnableTokenCleanup = true;
                })
                .AddTestUsers(TestUsers.Users)

                .AddOperationalStoreNotification<Notification>()
                ;

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            Seed(app);


            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private static void Seed(IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<TableStorageConfigurationOptions>>();

            // seed API resources

            var apiResourceLogger = app.ApplicationServices.GetRequiredService<ILogger<ApiResourceTableStore>>();
            var apiResourceStore = new ApiResourceTableStore(options, apiResourceLogger);
            foreach (var apiResource in Host.Configuration.Resources.GetApiResources())
                apiResourceStore.StoreAsync(apiResource).GetAwaiter().GetResult();

            // seed Identity resources

            var identityResourceLogger = app.ApplicationServices.GetRequiredService<ILogger<IdentityResourceTableStore>>();
            var identityResourceStore = new IdentityResourceTableStore(options, identityResourceLogger);
            foreach (var identityResource in Host.Configuration.Resources.GetIdentityResources())
                identityResourceStore.StoreAsync(identityResource).GetAwaiter().GetResult();

            // seed Clients

            var clientLogger = app.ApplicationServices.GetRequiredService<ILogger<ClientStore>>();
            var clientStore = new ClientStore(options, clientLogger);
            foreach (var client in Clients.Get())
                clientStore.StoreAsync(client).GetAwaiter().GetResult();
        }
    }

    public class Notification : IOperationalStoreNotification
    {
        public Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants)
        {
            return Task.CompletedTask;
        }
    }
}