using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpringComp.IdentityServer.TableStorage.Entities;
using SpringComp.IdentityServer.TableStorage.Options;
using ApiResource = IdentityServer4.Models.ApiResource;
using ApiResourceClaimEntity = SpringComp.IdentityServer.TableStorage.Entities.ApiResourceClaim;
using ApiResourceEntity = SpringComp.IdentityServer.TableStorage.Entities.ApiResource;
using ApiScopeClaimEntity = SpringComp.IdentityServer.TableStorage.Entities.ApiScopeClaim;
using ApiScopeEntity = SpringComp.IdentityServer.TableStorage.Entities.ApiScope;
using ApiSecretEntity = SpringComp.IdentityServer.TableStorage.Entities.ApiSecret;

namespace SpringComp.IdentityServer.TableStorage.Stores
{
    public sealed class ApiResourceTableStore
    {
        private readonly ILogger<ApiResourceTableStore> _logger;

        private readonly TableEntityStore<ApiResourceEntity> _apiResources;
        private readonly TableEntityStore<ApiResourceClaimEntity> _apiResourceClaims;
        private readonly TableEntityStore<ApiScopeEntity> _apiScopes;
        private readonly TableEntityStore<ApiScopeClaimEntity> _apiScopeClaims;
        private readonly TableEntityStore<ApiSecretEntity> _apiSecrets;

        private readonly TableEntityStore<ApiResourceNameByScope> _byScopes;
        public ApiResourceTableStore(
            IOptions<TableStorageConfigurationOptions> tableStorageOptions,
            ILogger<ApiResourceTableStore> logger
        )
        {
            var storage = tableStorageOptions.Value;
            var connectionString = storage.ConnectionString;

            _apiResources = new TableEntityStore<ApiResourceEntity>(connectionString, storage.ApiResources.TableName, logger);
            _apiResourceClaims = new TableEntityStore<ApiResourceClaimEntity>(connectionString, storage.ApiResourceClaims.TableName, logger);
            _apiScopes = new TableEntityStore<ApiScopeEntity>(connectionString, storage.ApiScopes.TableName, logger);
            _apiScopeClaims = new TableEntityStore<ApiScopeClaimEntity>(connectionString, storage.ApiScopeClaims.TableName, logger);
            _apiSecrets = new TableEntityStore<ApiSecretEntity>(connectionString, storage.ApiSecrets.TableName, logger);

            _byScopes = new TableEntityStore<ApiResourceNameByScope>(connectionString, storage.ApiResourceNamesByScope.TableName, logger);


            _logger = logger;
        }

        /// <summary>
        /// Stores a new API resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public async Task StoreAsync(ApiResource resource)
        {
            await _apiResources.InsertAsync(new ApiResourceEntity(resource.Name)
            {
                Description = resource.Description,
                DisplayName = resource.DisplayName,
                Enabled = resource.Enabled,
            });

            foreach (var claim in resource.UserClaims)
                await _apiResourceClaims.InsertAsync(new ApiResourceClaimEntity
                {
                    Name = resource.Name,
                    Claim = claim,
                });

            var secrets = resource.ApiSecrets.ToArray();
            for (var index = 0; index < resource.ApiSecrets.Count; index++)
            {
                var secret = secrets[index];
                await _apiSecrets.InsertAsync(new ApiSecretEntity
                {
                    Name = resource.Name,
                    Sequence = index,
                    Description = secret.Description,
                    Expiration = secret.Expiration,
                    Value = secret.Value,
                    Type = secret.Type,
                });
            }

            foreach (var scope in resource.Scopes)
            {
                await _byScopes.InsertAsync(new ApiResourceNameByScope { Scope = scope.Name, Name = resource.Name, });

                await _apiScopes.InsertAsync(new ApiScopeEntity
                {
                    Name = resource.Name,
                    Scope = scope.Name,
                    Description = scope.Description,
                    DisplayName = scope.DisplayName,
                    Required = scope.Required,
                    ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument,
                });

                foreach (var claim in scope.UserClaims)
                    await _apiScopeClaims.InsertAsync(new ApiScopeClaimEntity
                    {
                        Scope = $"{resource.Name}|{scope.Name}",
                        Claim = claim,
                    });
            }
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<ApiResource> FindResourceAsync(string name)
        {
            var entity = await _apiResources.FindAsync(ApiResourceEntity.Partition, name);
            return await ConvertToModelAsync(entity);
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var dictionary = new Dictionary<string, ApiResource>();

            foreach (var scopeName in scopeNames)
            {
                await foreach (var byScope in _byScopes.EnumAsync(scopeName))
                {
                    var apiName = byScope.Name;
                    if (!dictionary.ContainsKey(apiName))
                        dictionary.Add(apiName, await FindResourceAsync(apiName));
                }
            }

            return dictionary.Values;
        }

        /// <summary>
        /// Gets all API resources.
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<ApiResource> GetAllResourcesAsync()
        {
            await foreach (var api in _apiResources.EnumAsync(null))
                yield return await ConvertToModelAsync(api);
        }

        private async Task<ApiResource> ConvertToModelAsync(ApiResourceEntity entity)
        {
            var resource = new ApiResource
            {
                Enabled = entity.Enabled,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
            };

            await foreach (var claim in _apiResourceClaims.EnumAsync(entity.Name))
                resource.UserClaims.Add(claim.Claim);

            await foreach (var secret in _apiSecrets.EnumAsync(entity.Name))
                resource.ApiSecrets.Add(new Secret
                {
                    Description = secret.Description,
                    Expiration = secret.Expiration,
                    Type = secret.Type,
                    Value = secret.Value,
                });

            await foreach (var scope in _apiScopes.EnumAsync(entity.Name))
            {
                var apiScope = new Scope
                {
                    Name = scope.Scope,
                    DisplayName = scope.DisplayName,
                    Emphasize = scope.Emphasize,
                    Required = scope.Required,
                    ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument,
                };

                await foreach (var claim in _apiScopeClaims.EnumAsync($"{entity.Name}|{apiScope.Name}"))
                    apiScope.UserClaims.Add(claim.Claim);

                resource.Scopes.Add(apiScope);
            }

            return resource;
        }
    }
}