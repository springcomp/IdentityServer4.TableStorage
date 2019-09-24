using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ApiScope : TableEntity
    {
        [IgnoreProperty]
        public string Name
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
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
    }
}