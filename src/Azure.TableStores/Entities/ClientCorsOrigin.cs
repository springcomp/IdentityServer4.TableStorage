using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ClientCorsOrigin : TableEntity
    {
        [IgnoreProperty]
        public string ClientId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        [IgnoreProperty]
        public string Origin
        {
            get => RowKey;
            set => RowKey = value;
        }
    }
}