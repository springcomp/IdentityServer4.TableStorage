using Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SpringComp.IdentityServer.TableStorage.Options;

namespace SpringComp.IdentityServer4.Azure.Tables.Options
{
    public class ConfigurationTestDatabaseOptions : IOptions<TableStorageConfigurationOptions>
    {
        private readonly IConfigurationRoot _configuration;

        public ConfigurationTestDatabaseOptions()
        {
            _configuration = new ConfigurationBuilder()
                .AddUserSecrets<Startup>()
                .Build();
        }

        public TableStorageConfigurationOptions Value => new TableStorageConfigurationOptions
        {
            ConnectionString = _configuration["IdentityServerTableStorageConnectionString"],
        };
    }
}