using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ApiScopeClaim : TableEntity
    {
        /// <summary>
        /// The API name|scope values
        /// </summary>
        [IgnoreProperty]
        public string Scope
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        [IgnoreProperty]
        public string Claim
        {
            get => RowKey;
            set => RowKey = value;
        }
    }
}