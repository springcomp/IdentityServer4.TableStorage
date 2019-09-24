// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpringComp.IdentityServer.TableStorage.Options;
using SpringComp.IdentityServer.TableStorage.Stores;

namespace SpringComp.IdentityServer.TableStorage.Extensions
{
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder,
            string connectionString,
            Action<ConfigurationStoreOptions> storeOptionsAction = null
            )
        {
            var storeOptions = new ConfigurationStoreOptions();
            storeOptionsAction?.Invoke(storeOptions);

            builder.Services.Configure<TableStorageConfigurationOptions>(storageOptions =>
                {
                    storageOptions.ConnectionString = connectionString;
                    storeOptions.StorageTableOptionsAction?.Invoke(storageOptions);
                }
            );

            builder.Services.AddTransient<IdentityResourceTableStore>();
            builder.Services.AddTransient<ApiResourceTableStore>();

            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<IClientStore, ClientStore>();

            return builder;
        }

        public static ConfigurationStoreOptions ConfigureTableStorage(
            this ConfigurationStoreOptions storeOptions,
            Action<TableStorageConfigurationOptions> storageOptionsAction = null)
        {
            if (storeOptions == null)
            {
                throw new ArgumentNullException(nameof(storeOptions));
            }

            storeOptions.StorageTableOptionsAction = storageOptionsAction;
            return storeOptions;
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            string connectionString,
            Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            var storeOptions = new OperationalStoreOptions();
            storeOptionsAction?.Invoke(storeOptions);

            builder.Services.Configure<TableStorageOperationalOptions>(storageOptions =>
                {
                    storageOptions.ConnectionString = connectionString;
                    storeOptions.StorageTableOptionsAction?.Invoke(storageOptions);
                });

            builder.Services.AddSingleton(storeOptions);

            builder.Services.AddTransient<TokenCleanupService>();
            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            return builder;
        }

        public static OperationalStoreOptions ConfigureTableStorage(
            this OperationalStoreOptions storeOptions,
            Action<TableStorageOperationalOptions> storageOptionsAction = null)
        {
            if (storeOptions == null)
            {
                throw new ArgumentNullException(nameof(storeOptions));
            }

            storeOptions.StorageTableOptionsAction = storageOptionsAction;
            return storeOptions;
        }
    }
}