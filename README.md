# Powel.AzureTableStorage.IdentityServer4

Powel.AzureTableStorage.IdentityServer4 is a persistence layer for IdentityServer 4 configuration data that uses Azure Table Storage as its database abstraction.

This only implements the persisted grant store, not the configuration stores.

In order to run the tests, you will have to put an Azure Storage connection string in User Secrets, like this:

```
{
  "IdentityServerTableStorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=foo;AccountKey=TuF/ZNgQ==;EndpointSuffix=core.windows.net"
}
```

You can find a How-To here:
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets