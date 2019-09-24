using System.Collections.Generic;
using System.Threading.Tasks;
using SpringComp.IdentityServer.TableStorage.Entities;

namespace SpringComp.IdentityServer.TableStorage
{
    public interface IPersistedGrantStoreNotification
    {
        Task PersistedGrantsRemoveAsync(IEnumerable<PersistedGrant> persistedGrants);
    }
}