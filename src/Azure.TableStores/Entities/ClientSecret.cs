﻿using System;
using IdentityServer4;
using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class ClientSecret : TableEntity
    {
        public string ClientId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }
        public int Sequence 
        {
            get => Int32.Parse(RowKey);
            set => RowKey = value.ToString("0000");
        }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = IdentityServerConstants.SecretTypes.SharedSecret;
    }
}