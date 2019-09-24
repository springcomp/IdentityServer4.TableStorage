using System;

namespace SpringComp.IdentityServer.TableStorage.Options
{
    public class ConfigurationStoreOptions
    {
        internal Action<TableStorageConfigurationOptions> StorageTableOptionsAction { get; set; }
    }
}