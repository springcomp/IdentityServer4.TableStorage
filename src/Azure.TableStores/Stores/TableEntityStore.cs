using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace SpringComp.IdentityServer.TableStorage.Stores
{
    public class TableEntityStore<T> where T : TableEntity, new()
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private CloudTable _cloudTable;

        protected ILogger Logger { get; }

        public TableEntityStore(string connectionString, string tableName, ILogger logger)
        {
            _connectionString = connectionString;
            _tableName = tableName;

            Logger = logger;
        }

        protected internal async Task<CloudTable> InitTableAsync()
        {
            if (_cloudTable != null) return _cloudTable;

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _cloudTable = tableClient.GetTableReference(_tableName);
            await _cloudTable.CreateIfNotExistsAsync();

            return _cloudTable;
        }

        /// <summary>
        /// Retrieves a single entity matching the specified index
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task<T> FindAsync(string partitionKey, string rowKey)
        {
            await InitTableAsync();

            var query = new TableQuery<T>
            {
                FilterString = LogicalAnd(
                    WhereEqual("PartitionKey", partitionKey),
                    WhereEqual("RowKey", rowKey))
            };

            var continuationToken = new TableContinuationToken();
            var page = await _cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);

            return page?.Results.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all entities from the specified partition.
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<T> EnumAsync(string partitionKey)
        {
            await InitTableAsync();

            var query = new TableQuery<T>();

            if (!string.IsNullOrEmpty(partitionKey))
                query.FilterString = WhereEqual("PartitionKey", partitionKey);

            var continuation = new TableContinuationToken();

            do
            {
                var page = await _cloudTable.ExecuteQuerySegmentedAsync(query, continuation);

                foreach (var result in page.Results)
                    yield return result;

                continuation = page.ContinuationToken;

            } while (continuation != null);
        }

        public async Task InsertAsync(T entity)
        {
            await InitTableAsync();
            await _cloudTable.ExecuteAsync(TableOperation.InsertOrReplace(entity));
        }

        private static string WhereEqual(string property, string value)
        {
            return TableQuery.GenerateFilterCondition(property, QueryComparisons.Equal, value);
        }
        private static string LogicalAnd(string left, string right)
        {
            return TableQuery.CombineFilters(left, TableOperators.And, right);
        }
    }
}