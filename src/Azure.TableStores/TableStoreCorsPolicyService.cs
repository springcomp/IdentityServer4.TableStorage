using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using SpringComp.IdentityServer.TableStorage.Stores;

namespace SpringComp.IdentityServer.TableStorage
{
    public sealed class TableStoreCorsPolicyService : ICorsPolicyService
    {
        private readonly ClientStore store_;

        public TableStoreCorsPolicyService(ClientStore store)
        {
            store_ = store;
        }
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var clients = await store_.GetAllClientsAsync();
            var allowed = clients.SelectMany(c => c.AllowedCorsOrigins).Contains(origin);
            return allowed;
        }
    }
}