using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace SpringComp.IdentityServer.TableStorage.Stores
{
    public sealed class ResourceStore : IResourceStore
    {
        private readonly IdentityResourceTableStore _identityResources;
        private readonly ApiResourceTableStore _apiResources;

        public ResourceStore(
            IdentityResourceTableStore identityResources,
            ApiResourceTableStore apiResources
            )
        {
            _identityResources = identityResources;
            _apiResources = apiResources;
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();
            var collection = new List<IdentityResource>();

            await foreach (var resource in _identityResources.GetAllResourcesAsync())
                if (scopes.Contains(resource.Name))
                    collection.Add(resource);

            return collection;
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return _apiResources.FindResourcesByScopeAsync(scopeNames);
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return _apiResources.FindResourceAsync(name);
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public async Task<Resources> GetAllResourcesAsync()
        {
            var identityResources = new List<IdentityResource>();

            var apiResources = new List<ApiResource>();
            await foreach (var apiResource in _apiResources.GetAllResourcesAsync())
                apiResources.Add(apiResource);

            return new Resources(
                identityResources,
                apiResources
            );
        }
    }
}