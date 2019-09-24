using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpringComp.IdentityServer.TableStorage.Mappers;
using SpringComp.IdentityServer.TableStorage.Options;

using PersistedGrant = IdentityServer4.Models.PersistedGrant;
using PersistedGrantEntity = SpringComp.IdentityServer.TableStorage.Entities.PersistedGrant;

namespace SpringComp.IdentityServer.TableStorage.Stores
{
    public class PersistedGrantStore : TableEntityStore<PersistedGrantEntity>, IPersistedGrantStore
    {
        public PersistedGrantStore(
            IOptions<TableStorageOperationalOptions> tableStorageOptions,
            ILogger<PersistedGrantStore> logger
        )
            : base(
              tableStorageOptions.Value.ConnectionString
            , tableStorageOptions.Value.PersistedGrants.TableName
            , logger
            )
        {
        }

        public async Task StoreAsync(PersistedGrant token)
        {
            var persistedGrant = token.ToEntity();
            Logger.LogDebug("storing persisted grant: {persistedGrant}", persistedGrant);
            var table = await InitTableAsync();
            var operation = TableOperation.InsertOrReplace(persistedGrant);
            var result = await table.ExecuteAsync(operation);
            Logger.LogDebug("stored {persistedGrantKey} with result {result}", token.Key, result.HttpStatusCode);
        }

        private async Task<PersistedGrantEntity> GetEntityAsync(string key)
        {
            var table = await InitTableAsync();
            Entities.PersistedGrant model = null;
            var query = new TableQuery<Entities.PersistedGrant>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, WebUtility.UrlEncode(key)));
            TableContinuationToken continuationToken = null;

            do
            {
                var result = await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                if (result.Results.Count > 0)
                {
                    model = result.Results[0];
                    break;
                }

                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);

            Logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, model != null);
            return model;
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var entity = await GetEntityAsync(key);
            return entity?.ToModel();
        }

        private async Task<IEnumerable<PersistedGrantEntity>> GetAllEntitiesAsync(string subjectId)
        {
            var filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, subjectId);
            var persistedGrants = await GetFilteredEntitiesAsync(filter);
            var persistedGrantList = persistedGrants.ToList();

            Logger.LogDebug("{persistedGrantCount} persisted grants found for subjectId {subjectId}", persistedGrantList.Count, subjectId);
            return persistedGrantList;
        }

        private async Task<IEnumerable<PersistedGrantEntity>> GetAllEntitiesAsync(string subjectId, string clientId)
        {
            var subjectFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, subjectId);
            var clientFilter = TableQuery.GenerateFilterCondition("ClientId", QueryComparisons.Equal, clientId);
            var combinedFilter = TableQuery.CombineFilters(subjectFilter, TableOperators.And, clientFilter);

            var persistedGrants = await GetFilteredEntitiesAsync(combinedFilter);
            var persistedGrantList = persistedGrants.ToList();

            Logger.LogDebug("{persistedGrantCount} persisted grants found for subjectId {subjectId}, clientId {clientId}", persistedGrantList.Count, subjectId, clientId);
            return persistedGrantList;
        }

        private async Task<IEnumerable<PersistedGrantEntity>> GetAllEntitiesAsync(string subjectId, string clientId, string type)
        {
            var subjectFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, subjectId);
            var clientFilter = TableQuery.GenerateFilterCondition("ClientId", QueryComparisons.Equal, clientId);
            var typeFilter = TableQuery.GenerateFilterCondition("Type", QueryComparisons.Equal, type);
            var combinedFilter = TableQuery.CombineFilters(subjectFilter, TableOperators.And, TableQuery.CombineFilters(clientFilter, TableOperators.And, typeFilter));

            var persistedGrants = await GetFilteredEntitiesAsync(combinedFilter);
            var persistedGrantList = persistedGrants.ToList();

            Logger.LogDebug("{persistedGrantCount} persisted grants found for subjectId {subjectId}, clientId {type}, clientId {type}", persistedGrantList.Count, subjectId, clientId, type);
            return persistedGrantList;
        }

        private async Task<IEnumerable<PersistedGrantEntity>> GetFilteredEntitiesAsync(string filter)
        {
            var table = await InitTableAsync();
            var query = new TableQuery<Entities.PersistedGrant>().Where(filter);
            TableContinuationToken continuationToken = null;
            var persistedGrants = new List<Entities.PersistedGrant>();

            do
            {
                var result = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                persistedGrants.AddRange(result.Results);
                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);

            return persistedGrants;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var entities = await GetAllEntitiesAsync(subjectId);
            return entities.Select(e => e.ToModel());
        }

        public async Task RemoveAsync(string key)
        {
            var persistedGrant = await GetEntityAsync(key);
            if (persistedGrant == null)
            {
                Logger.LogDebug("no {persistedGrantKey} persisted grant found in database", key);
                return;
            }

            var table = await InitTableAsync();
            var operation = TableOperation.Delete(persistedGrant);
            var result = await table.ExecuteAsync(operation);
            Logger.LogDebug("removed {persistedGrantKey} from database with result {result}", key, result.HttpStatusCode);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var persistedGrants = await GetAllEntitiesAsync(subjectId, clientId);
            var persistedGrantList = persistedGrants.ToList();
            Logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}", persistedGrantList.Count, subjectId, clientId);

            await RemoveAllAsync(persistedGrantList);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var persistedGrants = await GetAllEntitiesAsync(subjectId, clientId, type);
            var persistedGrantList = persistedGrants.ToList();
            Logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}, grantType {persistedGrantType}", persistedGrantList.Count, subjectId, clientId, type);

            await RemoveAllAsync(persistedGrantList);
        }

        private async Task RemoveAllAsync(IEnumerable<PersistedGrantEntity> persistedGrants)
        {
            var table = await InitTableAsync();

            foreach (var persistedGrant in persistedGrants)
            {
                var operation = TableOperation.Delete(persistedGrant);
                var result = await table.ExecuteAsync(operation);
                Logger.LogDebug("removed {persistedGrantKey} from database with result {result}", persistedGrant.PartitionKey,
                    result.HttpStatusCode);
            }
        }

        internal async Task<IEnumerable<PersistedGrantEntity>> GetExpiredGrantsAsync(int batchSize)
        {
            await InitTableAsync();

            var condition = TableQuery.GenerateFilterConditionForDate(
                "Expiration", QueryComparisons.LessThanOrEqual, DateTime.UtcNow);

            var query = new TableQuery<PersistedGrantEntity>()
                .Where(condition)
                .Take(batchSize)
                ;

            var collection = new List<PersistedGrantEntity>();
            await foreach (var persistedGrant in EnumAsync(query))
                collection.Add(persistedGrant);

            return collection;
        }
    }
}