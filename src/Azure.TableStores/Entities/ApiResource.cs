using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ApiResource : TableEntity
    {
        public ApiResource()
            :this("")
        { }

        public ApiResource(string rowKey)
            : base(Partition, rowKey)
        {
        }

        public static string Partition = "apiresource";

        [IgnoreProperty]
        public string Name
        {
            get => RowKey;
            set => RowKey = value;
        }

        public bool Enabled { get; set; } = true;
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
