// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SpringComp.IdentityServer.TableStorage;
using SpringComp.IdentityServer.TableStorage.Options;
using SpringComp.IdentityServer.TableStorage.Stores;

namespace Microsoft.Extensions.DependencyInjection
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

            builder.Services.AddSingleton<IdentityResourceTableStore>();
            builder.Services.AddSingleton<ApiResourceTableStore>();

            // enable caching for better performances

            builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
            builder.Services.AddTransient(typeof(ICache<>), typeof(DefaultCache<>));

            builder.Services.AddSingleton<ClientStore>();
            builder.Services.AddTransient<IClientStore, CachingClientStore<ClientStore>>();

            builder.Services.AddTransient<ResourceStore>();
            builder.Services.AddTransient<IResourceStore, CachingResourceStore<ResourceStore>>();

            builder.Services.TryAddTransient<TableStoreCorsPolicyService>();
            builder.Services.AddTransient<ICorsPolicyService, CachingCorsPolicyService<TableStoreCorsPolicyService>>();

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

            builder.Services.AddSingleton<IPersistedGrantStore, PersistedGrantStore>();

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

        public static IIdentityServerBuilder AddPersistedGrantStoreNotification<T>(this IIdentityServerBuilder builder)
            where T : class, IPersistedGrantStoreNotification
        {
            builder.Services.AddTransient<IPersistedGrantStoreNotification, T>();
            return builder;
        }
    }
}