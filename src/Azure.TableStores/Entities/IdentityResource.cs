// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.Azure.Cosmos.Table;

namespace SpringComp.IdentityServer.TableStorage.Entities
{
    public class IdentityResource :TableEntity
    {
        public IdentityResource()
            : this("")
        { }

        public IdentityResource(string rowKey)
            : base(Partition, rowKey)
        {
        }

        public static string Partition = "identityresource";

        [IgnoreProperty]
        public string Name
        {
            get => RowKey;
            set => RowKey = value;
        }

        public bool Enabled { get; set; } = true;
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
    }
}
