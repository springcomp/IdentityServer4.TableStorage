﻿using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ApiResourceClaim : TableEntity
    {
        [IgnoreProperty]
        public string Name
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