using Host;
using Microsoft.Extensions.Configuration;

namespace SpringComp.IdentityServer4.Azure.Tables.Stores
{
    public class StoreTests
    { 
        protected static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build();

            return configuration["IdentityServerTableStorageConnectionString"];
        }
    }
}