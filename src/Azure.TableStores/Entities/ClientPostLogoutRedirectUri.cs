using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ClientPostLogoutRedirectUri :TableEntity
    {
        [IgnoreProperty]
        public string ClientId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        [IgnoreProperty]
        public string PostLogoutRedirectUri
        {
            get => RowKey;
            set => RowKey = value;
        }
    }
}