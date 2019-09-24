using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ApiResourceNameByScope : TableEntity
    {
        [IgnoreProperty]
        public string Scope
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        [IgnoreProperty]
        public string Name
        {
            get => RowKey;
            set => RowKey = value;
        }
    }
}