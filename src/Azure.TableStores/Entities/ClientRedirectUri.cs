using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ClientRedirectUri : TableEntity
    {
        [IgnoreProperty]
        public string ClientId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        [IgnoreProperty]
        public string RedirectUri
        {
            get => RowKey;
            set => RowKey = value;
        }
    }
}