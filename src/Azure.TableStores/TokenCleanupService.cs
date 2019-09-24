// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework;
using IdentityServer4.Stores;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using SpringComp.IdentityServer.TableStorage.Mappers;
using SpringComp.IdentityServer.TableStorage.Stores;

using OperationalStoreOptions = SpringComp.IdentityServer.TableStorage.Options.OperationalStoreOptions;
using PersistedGrant = SpringComp.IdentityServer.TableStorage.Entities.PersistedGrant;

namespace SpringComp.IdentityServer.TableStorage
{
    /// <summary>
    /// Helper to cleanup stale persisted grants and device codes.
    /// </summary>
    public class TokenCleanupService
    {
        private readonly OperationalStoreOptions _options;
        private readonly PersistedGrantStore _persistedGrantStore;
        private readonly IPersistedGrantStoreNotification _operationalStoreNotification;
        private readonly ILogger<TokenCleanupService> _logger;

        /// <summary>
        /// Constructor for TokenCleanupService.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="persistedGrantStore"></param>
        /// <param name="operationalStoreNotification"></param>
        /// <param name="logger"></param>
        public TokenCleanupService(
            OperationalStoreOptions options,
            IPersistedGrantStore persistedGrantStore,
            ILogger<TokenCleanupService> logger,
            IPersistedGrantStoreNotification operationalStoreNotification = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.TokenCleanupBatchSize < 1) throw new ArgumentException("Token cleanup batch size interval must be at least 1");

            _persistedGrantStore = persistedGrantStore as PersistedGrantStore ?? throw new ArgumentNullException(nameof(persistedGrantStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _operationalStoreNotification = operationalStoreNotification;

        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                await RemoveDeviceCodesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveGrantsAsync()
        {
            try
            {
                var found = 0;

                do
                {
                    var collection = (await _persistedGrantStore .GetExpiredGrantsAsync(_options.TokenCleanupBatchSize))
                        .ToArray()
                        ;

                    found = collection.Length;
                    if (found > 0)
                    {
                        _logger.LogInformation("Removing {grantCount} grants", found);
                        if (_operationalStoreNotification != null)
                        {
                            await _operationalStoreNotification.PersistedGrantsRemoveAsync(collection);
                        }

                        foreach (var persistedGrant in collection)
                            await _persistedGrantStore.DeleteAsync(persistedGrant);
                    }
                } while (found > 0);

            }
            catch (Exception ex)
            {
                if (!HandleConcurrencyException(ex))
                    throw;
            }
        }

        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        /// <returns></returns>
        protected virtual Task RemoveDeviceCodesAsync()
        {
            //            var found = Int32.MaxValue;
            //
            //            while (found >= _options.TokenCleanupBatchSize)
            //            {
            //                //found = expiredCodes.Length;
            //                _logger.LogInformation("Removing {deviceCodeCount} device flow codes", found);
            //
            //                if (found > 0)
            //                {
            //                    try
            //                    {
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        if (!HandleConcurrencyException(ex))
            //                            throw;
            //                    }
            //                }
            //            }

            return Task.CompletedTask;
        }

        private bool HandleConcurrencyException(Exception exception)
        {
            if (!(exception is StorageException ex)) return false;

            var statusCode = ex.RequestInformation.HttpStatusCode;

            if (statusCode == (int)HttpStatusCode.Conflict || statusCode == (int)HttpStatusCode.PreconditionFailed)
            {
                // we get this if/when someone else already deleted the records
                // we want to essentially ignore this, and keep working
                _logger.LogDebug("Concurrency exception removing expired grants: {exception}", ex.Message);
                return true;
            }

            return false;
        }
    }
}