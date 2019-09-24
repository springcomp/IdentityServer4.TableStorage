using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using SpringComp.IdentityServer.TableStorage.Stores;
using Xunit;

namespace SpringComp.IdentityServer4.Azure.Tables.Stores
{
    public class IdentityResourceTests : ConfigurationStoreTests
    {
        private static IdentityResourceTableStore CreateIdentityResourceStore()
        {
            var store = new IdentityResourceTableStore(StoreOptions, new FakeLogger<IdentityResourceTableStore>());
            return store;
        }
        [Fact]
        public async Task FindIdentityResourcesByScopeAsync_WhenScopeReceived_ExpectIdentityResourceWithSameNameReturned()
        {
            var store = CreateIdentityResourceStore();
            var resourceStore = new ResourceStore(store, null);

            await store.StoreAsync(new IdentityResources.OpenId());
            await store.StoreAsync(new IdentityResources.Profile());

            var found = await resourceStore.FindIdentityResourcesByScopeAsync(new[] { "profile" });

            Assert.Single(found);
            Assert.Equal(new IdentityResources.Profile().Name, found.ToArray()[0].Name);
            Assert.Equal(new IdentityResources.Profile().DisplayName, found.ToArray()[0].DisplayName);
            Assert.Equal(new IdentityResources.Profile().Description, found.ToArray()[0].Description);
            Assert.Equal(new IdentityResources.Profile().ShowInDiscoveryDocument, found.ToArray()[0].ShowInDiscoveryDocument);
        }
    }
}