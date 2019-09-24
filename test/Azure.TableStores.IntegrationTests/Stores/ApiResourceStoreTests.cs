using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using SpringComp.IdentityServer.TableStorage.Stores;
using Xunit;

namespace SpringComp.IdentityServer4.Azure.Tables.Stores
{
    public class ApiResourceStoreTests : ConfigurationStoreTests
    {
        private static ApiResourceTableStore CreateApiResourceStore()
        {
            var store = new ApiResourceTableStore(StoreOptions, new FakeLogger<ApiResourceTableStore>());
            return store;
        }
        private static ApiResource CreateApiResourceTestObject(string name, string scope)
        {
            var resource = new ApiResource
            {
                Name =name,
                DisplayName = $"Api Resource {name} ",
                UserClaims = { JwtClaimTypes.Name, },
                Scopes =
                {
                    new Scope(scope, new[] {JwtClaimTypes.Name, JwtClaimTypes.Role,}),
                },
                ApiSecrets = new List<Secret> { new Secret("secret".Sha256()), },
            };
            return resource;
        }

        [Fact]
        public async Task FindApiResourceAsync_WithNameReceived_ExpectedApiResourceReturned()
        {
            var store = CreateApiResourceStore();
            var resource = CreateApiResourceTestObject("api1", "scope");
            await store.StoreAsync(resource);

            var found = await store.FindResourceAsync("api1");

            Assert.Equal(resource.Name, found.Name);
            Assert.Equal(resource.DisplayName, found.DisplayName);
            Assert.Equal(resource.Description, found.Description);
            Assert.Single(resource.UserClaims);
            Assert.Equal("name", found.UserClaims.ToArray()[0]);

            var scope = found.Scopes.First(s => s.Name == "scope");
            Assert.Equal(2, scope.UserClaims.Count);
            Assert.Equal("name", scope.UserClaims.ToArray()[0]);
            Assert.Equal("role", scope.UserClaims.ToArray()[1]);
        }

        [Fact]
        public async Task FindApiResourcesByScopeAsync_WithScopeReceived_ExpectedApiResourcesReturned()
        {
            var store = CreateApiResourceStore();
            await store.StoreAsync(CreateApiResourceTestObject("api1", "scope"));
            await store.StoreAsync(CreateApiResourceTestObject("api2", "scope"));
            await store.StoreAsync(CreateApiResourceTestObject("api3", "not_in_scope"));

            var found = (await store.FindResourcesByScopeAsync(new[] {"scope",}))
                    .ToArray()
                ;

            Assert.Equal(2, found.Length);
            Assert.Equal("api1", found[0].Name);
            Assert.Equal("api2", found[1].Name);
        }

    }
}