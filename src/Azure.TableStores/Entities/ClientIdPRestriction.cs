using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ClientIdPRestriction: TableEntity
    {
        [IgnoreProperty]
        public string ClientId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        [IgnoreProperty]
        public string Provider
        {
            get => RowKey;
            set => RowKey = value;
        }
    }
}