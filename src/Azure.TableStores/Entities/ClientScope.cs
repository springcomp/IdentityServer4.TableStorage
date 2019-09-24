using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ClientScope :TableEntity
    {
        [IgnoreProperty]
        public string ClientId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        [IgnoreProperty]
        public string Scope
        {
            get => RowKey;
            set => RowKey = value;
        }
    }
}