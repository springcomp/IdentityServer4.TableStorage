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
