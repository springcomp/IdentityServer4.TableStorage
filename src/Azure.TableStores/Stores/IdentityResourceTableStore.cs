using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpringComp.IdentityServer.TableStorage.Options;
using IdentityResource = IdentityServer4.Models.IdentityResource;

using IdentityResourceEntity = SpringComp.IdentityServer.TableStorage.Entities.IdentityResource;
using IdentityClaimEntity = SpringComp.IdentityServer.TableStorage.Entities.IdentityClaim;

namespace SpringComp.IdentityServer.TableStorage.Stores
{
    public sealed class IdentityResourceTableStore
    {
        private readonly ILogger<IdentityResourceTableStore> _logger;

        private readonly TableEntityStore<IdentityResourceEntity> _identityResources;
        private readonly TableEntityStore<IdentityClaimEntity> _identityClaims;

        public IdentityResourceTableStore(
            IOptions<TableStorageConfigurationOptions> tableStorageOptions,
            ILogger<IdentityResourceTableStore> logger)
        {
            var storage = tableStorageOptions.Value;
            var connectionString = storage.ConnectionString;

            _identityResources = new TableEntityStore<IdentityResourceEntity>(connectionString, storage.IdentityResources.TableName, logger);
            _identityClaims = new TableEntityStore<IdentityClaimEntity>(connectionString, storage.IdentityClaims.TableName, logger);

            _logger = logger;
        }

        public async Task StoreAsync(IdentityResource resource)
        {
            await _identityResources.InsertAsync(new IdentityResourceEntity(resource.Name) {
                Description = resource.Description,
                DisplayName = resource.DisplayName,
                Enabled = resource.Enabled,
                Emphasize = resource.Emphasize,
                Required = resource.Required,
                ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument,
            });

            foreach (var claim in resource.UserClaims)
                await _identityClaims.InsertAsync(new IdentityClaimEntity
                {
                    Name = resource.Name,
                    Claim = claim,
                });
        }

        /// <summary>
        /// Gets all identity resources
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<IdentityResource> GetAllResourcesAsync()
        {
            await foreach (var entity in _identityResources.EnumAsync(null))
                yield return await ConvertToModelAsync(entity);
        }

        private async Task<IdentityResource> ConvertToModelAsync(IdentityResourceEntity entity)
        {
            var resource = new IdentityResource
            {
                Enabled = entity.Enabled,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
            };

            await foreach (var claim in _identityClaims.EnumAsync(entity.Name))
                resource.UserClaims.Add(claim.Claim);

            var converted = resource;
            return converted;
        }
    }
}