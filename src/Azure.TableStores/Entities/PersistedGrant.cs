using System;
using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class PersistedGrant : TableEntity
    {
        public string Key
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        public string SubjectId
        {
            get => RowKey;
            set => RowKey = value;
        }
        public string Type { get; set; }
        public string ClientId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? Expiration { get; set; }
        public string Data { get; set; }

        public override string ToString()
        {
            return $"{{PartitionKey (Key): '{PartitionKey}', RowKey (SubjectId): '{RowKey}', Type: '{Type}', ClientId: '{ClientId}', CreationTime: '{CreationTime}', Expiration: '{Expiration}', Data: '{Data}'}}";
        }
    }
}