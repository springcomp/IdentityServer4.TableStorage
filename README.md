[![Build status](https://ci.appveyor.com/api/projects/status/nu94fsfhnyli3838/branch/master?svg=true)](https://ci.appveyor.com/project/springcomp/identityserver4-tablestorage/branch/master)

# IdentityServer4.TableStorage

This projects provides a simple persistence layer for IdentityServer 4 configuration data that uses Azure Table Storage as its database abstraction.

It supports the client, resource and operational stores.

In order to run the tests, you will have to put an Azure Storage connection string in User Secrets, like this:

```
{
  "IdentityServerTableStorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=foo;AccountKey=TuF/ZNgQ==;EndpointSuffix=core.windows.net"
}
```

You can find a How-To here:
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets

# Usage

You enable support for Table Storage in your `Startup` class in `ConfigureServices`:

```
    var connectionString = "...";
    
    builder
        // configure azure table stores for IdentityResource, ApiResource and Client objects
        .AddConfigurationStore(connectionString)
        // configure azure table stores for persisted grants, consents and device flow codes
        .AddOperationalStore(connectionString)
        ;
```

The code shown above registers a custom configuration store that uses Azure Table Storage as a persistence mechanism. It also registers a custom implementation of `ICorsPolicyService` that is required to enable CORS for IdentityServer4's metadata endpoint at '/.well-known/openid-configuration'.

For better performances, the code shown above registers caching implementations of those stores.

To cleanup expired consents and device flow codes, it is necessary to enable the built-in `TokenCleanupService` like so:

```
    builder
        .AddOperationStore(connectionString, options => {
          options.EnableTokenCleanup = true;
          options.TokenCleanupInterval = 3600; // seconds
        })
        ;
```

You can also customize the Azure Storage Table names, using the following syntax:

```
    builder
        .AddConfigurationStore(connectionString, options => {
            options.ConfigureTableStorage(storage => {
                storage.Clients.TableName = "Clients";
                storage.ClientClaims.TableName = "ClientClaims";
                storage.ClientCorsOrigins.TableName = "ClientCorsOrigins";
                storage.ClientGrantTypes.TableName = "ClientGrantTypes";
                storage.ClientIdPRestrictions.TableName = "ClientIdPRestrictions";
                storage.ClientPostLogoutRedirectUris.TableName = "ClientPostLogoutRedirectUris";
                storage.ClientRedirectUris.TableName = "ClientRedirectUris";
                storage.ClientScopes.TableName = "ClientScopes";
                storage.ClientSecrets.TableName = "ClientSecrets";

                storage.IdentityResources.TableName = "IdentityResources";
                storage.IdentityClaims.TableName = "IdentityClaims";

                storage.ApiResources.TableName = "ApiResources";
                storage.ApiResourceClaims.TableName = "ApiClaims";
                storage.ApiScopes.TableName = "ApiScopes";
                storage.ApiScopeClaims.TableName = "ApiScopeClaims";
                storage.ApiSecrets.TableName = "ApiSecrets";

                storage.ApiResourceNamesByScope.TableName = "...";
            });
        });
        .AddOperationStore(connectionString, options => {
            options.ConfigureTableStorag(storage => {
                options.PersistedGrants.TableName = "PersistedGrants";
            });
        })
        ;
    
```
