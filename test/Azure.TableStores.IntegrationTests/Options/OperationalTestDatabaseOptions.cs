using Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SpringComp.IdentityServer.TableStorage.Options;

namespace SpringComp.IdentityServer4.Azure.Tables.Options
{
    public class OperationalTestDatabaseOptions : IOptions<TableStorageOperationalOptions>
    {
        private readonly IConfigurationRoot _configuration;

        public OperationalTestDatabaseOptions()
        {
            _configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build();
        }

        public TableStorageOperationalOptions Value => new TableStorageOperationalOptions
        {
            ConnectionString = _configuration["IdentityServerTableStorageConnectionString"],
        };
    }
}
