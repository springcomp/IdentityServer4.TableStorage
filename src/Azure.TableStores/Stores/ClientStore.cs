using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpringComp.IdentityServer.TableStorage.Options;

using Client = IdentityServer4.Models.Client;
using Secret = IdentityServer4.Models.Secret;

using ClientEntity = SpringComp.IdentityServer.TableStorage.Entities.Client;
using ClientClaimEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientClaim;
using ClientCorsOriginEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientCorsOrigin;
using ClientGrantTypeEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientGrantType;
using ClientIdPRestrictionEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientIdPRestriction;
using ClientPostLogoutRedirectUriEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientPostLogoutRedirectUri;
using ClientRedirectUriEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientRedirectUri;
using ClientSecretEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientSecret;
using ClientScopeEntity = SpringComp.IdentityServer.TableStorage.Entities.ClientScope;

namespace SpringComp.IdentityServer.TableStorage.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly TableEntityStore<ClientEntity> _clients;
        private readonly TableEntityStore<ClientClaimEntity> _claims;
        private readonly TableEntityStore<ClientCorsOriginEntity> _corsOrigins;
        private readonly TableEntityStore<ClientGrantTypeEntity> _grantTypes;
        private readonly TableEntityStore<ClientIdPRestrictionEntity> _idPRestrictions;
        private readonly TableEntityStore<ClientPostLogoutRedirectUriEntity> _postLogoutRedirectUris;
        private readonly TableEntityStore<ClientRedirectUriEntity> _redirectUris;
        private readonly TableEntityStore<ClientScopeEntity> _scopes;
        private readonly TableEntityStore<ClientSecretEntity> _secrets;

        private readonly ILogger<ClientStore> _logger;

        public ClientStore(
            IOptions<TableStorageConfigurationOptions> tableStorageOptions,
            ILogger<ClientStore> logger)
        {
            var options = tableStorageOptions.Value;
            var connectionString = options.ConnectionString;

            _clients = new TableEntityStore<ClientEntity>(connectionString, tableStorageOptions.Value.Clients.TableName, logger);
            _claims = new TableEntityStore<ClientClaimEntity>(connectionString, tableStorageOptions.Value.ClientClaims.TableName, logger);
            _corsOrigins = new TableEntityStore<ClientCorsOriginEntity>(connectionString, tableStorageOptions.Value.ClientCorsOrigins.TableName, logger);
            _grantTypes = new TableEntityStore<ClientGrantTypeEntity>(connectionString, tableStorageOptions.Value.ClientGrantTypes.TableName, logger);
            _idPRestrictions = new TableEntityStore<ClientIdPRestrictionEntity>(connectionString, tableStorageOptions.Value.ClientIdPRestrictions.TableName, logger);
            _postLogoutRedirectUris = new TableEntityStore<ClientPostLogoutRedirectUriEntity>(connectionString, tableStorageOptions.Value.ClientPostLogoutRedirectUris.TableName, logger);
            _redirectUris = new TableEntityStore<ClientRedirectUriEntity>(connectionString, tableStorageOptions.Value.ClientRedirectUris.TableName, logger);
            _scopes = new TableEntityStore<ClientScopeEntity>(connectionString, tableStorageOptions.Value.ClientScopes.TableName, logger);
            _secrets = new TableEntityStore<ClientSecretEntity>(connectionString, tableStorageOptions.Value.ClientSecrets.TableName, logger);

            _logger = logger;
        }

        public async Task StoreAsync(Client client)
        {
            await _clients.InsertAsync(new ClientEntity
            {
                ClientId = client.ClientId,
                Enabled = client.Enabled,
                ProtocolType = client.ProtocolType,
                RequireClientSecret = client.RequireClientSecret,
                ClientName = client.ClientName,
                Description = client.Description,
                ClientUri = client.ClientUri,
                LogoUri = client.LogoUri,
                RequireConsent = client.RequireConsent,
                AllowRememberConsent = client.AllowRememberConsent,
                RequirePkce = client.RequirePkce,
                AllowPlainTextPkce = client.AllowPlainTextPkce,
                AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser,
                FrontChannelLogoutUri = client.FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired,
                BackChannelLogoutUri = client.BackChannelLogoutUri,
                BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired,
                AllowOfflineAccess = client.AllowOfflineAccess,
                AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken,
                IdentityTokenLifetime = client.IdentityTokenLifetime,
                AccessTokenLifetime = client.AccessTokenLifetime,
                AuthorizationCodeLifetime = client.AuthorizationCodeLifetime,
                AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
                SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
                ConsentLifetime = client.ConsentLifetime,
                RefreshTokenUsage = client.RefreshTokenUsage,
                UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
                RefreshTokenExpiration = client.RefreshTokenExpiration,
                AccessTokenType = client.AccessTokenType,
                EnableLocalLogin = client.EnableLocalLogin,
                IncludeJwtId = client.IncludeJwtId,
                AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                ClientClaimsPrefix = client.ClientClaimsPrefix,
                PairWiseSubjectSalt = client.PairWiseSubjectSalt,
                UserSsoLifetime = client.UserSsoLifetime,
                UserCodeType = client.UserCodeType,
                DeviceCodeLifetime = client.DeviceCodeLifetime,
            });

            foreach (var claim in client.Claims)
                await _claims.InsertAsync(new ClientClaimEntity
                {
                    ClientId = client.ClientId,
                    Type = claim.Type,
                    Value = claim.Value,
                });

            foreach (var corsOrigin in client.AllowedCorsOrigins)
                await _corsOrigins.InsertAsync(new ClientCorsOriginEntity
                {
                    ClientId = client.ClientId,
                    Origin = WebUtility.UrlEncode(corsOrigin),
                });

            foreach (var grantType in client.AllowedGrantTypes)
                await _grantTypes.InsertAsync(new ClientGrantTypeEntity
                {
                    ClientId = client.ClientId,
                    GrantType = grantType,
                });

            foreach (var restriction in client.IdentityProviderRestrictions)
                await _idPRestrictions.InsertAsync(new ClientIdPRestrictionEntity
                {
                    ClientId = client.ClientId,
                    Provider = restriction,
                });

            foreach (var redirectUri in client.PostLogoutRedirectUris)
                await _postLogoutRedirectUris.InsertAsync(new ClientPostLogoutRedirectUriEntity
                {
                    ClientId = client.ClientId,
                    PostLogoutRedirectUri = WebUtility.UrlEncode(redirectUri),
                });

            foreach (var redirectUri in client.RedirectUris)
                await _redirectUris.InsertAsync(new ClientRedirectUriEntity
                {
                    ClientId = client.ClientId,
                    RedirectUri = WebUtility.UrlEncode(redirectUri),
                });

            var secrets = client.ClientSecrets.ToArray();
            for (var index = 0; index < client.ClientSecrets.Count; index++)
            {
                var secret = secrets[index];
                await _secrets.InsertAsync(new ClientSecretEntity
                {
                    ClientId = client.ClientId,
                    Sequence = index,
                    Description = secret.Description,
                    Expiration = secret.Expiration,
                    Type = secret.Type,
                    Value = secret.Value,
                });
            }

            foreach (var scope in client.AllowedScopes)
                await _scopes.InsertAsync(new ClientScopeEntity
                {
                    ClientId = client.ClientId,
                    Scope = scope,
                });
        }

        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns></returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var entity = await _clients.FindAsync(ClientEntity.Partition, clientId);
            if (entity == null)
                return null;
            return await ConvertToModelAsync(entity);
        }

        private async Task<Client> ConvertToModelAsync(ClientEntity entity)
        {
            var client = new Client
            {
                ClientId = entity.ClientId,
                Enabled = entity.Enabled,
                ProtocolType = entity.ProtocolType,
                RequireClientSecret = entity.RequireClientSecret,
                ClientName = entity.ClientName,
                Description = entity.Description,
                ClientUri = entity.ClientUri,
                LogoUri = entity.LogoUri,
                RequireConsent = entity.RequireConsent,
                AllowRememberConsent = entity.AllowRememberConsent,
                RequirePkce = entity.RequirePkce,
                AllowPlainTextPkce = entity.AllowPlainTextPkce,
                AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser,
                FrontChannelLogoutUri = entity.FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = entity.FrontChannelLogoutSessionRequired,
                BackChannelLogoutUri = entity.BackChannelLogoutUri,
                BackChannelLogoutSessionRequired = entity.BackChannelLogoutSessionRequired,
                AllowOfflineAccess = entity.AllowOfflineAccess,
                AlwaysIncludeUserClaimsInIdToken = entity.AlwaysIncludeUserClaimsInIdToken,
                IdentityTokenLifetime = entity.IdentityTokenLifetime,
                AccessTokenLifetime = entity.AccessTokenLifetime,
                AuthorizationCodeLifetime = entity.AuthorizationCodeLifetime,
                AbsoluteRefreshTokenLifetime = entity.AbsoluteRefreshTokenLifetime,
                SlidingRefreshTokenLifetime = entity.SlidingRefreshTokenLifetime,
                ConsentLifetime = entity.ConsentLifetime,
                RefreshTokenUsage = entity.RefreshTokenUsage,
                UpdateAccessTokenClaimsOnRefresh = entity.UpdateAccessTokenClaimsOnRefresh,
                RefreshTokenExpiration = entity.RefreshTokenExpiration,
                AccessTokenType = entity.AccessTokenType,
                EnableLocalLogin = entity.EnableLocalLogin,
                IncludeJwtId = entity.IncludeJwtId,
                AlwaysSendClientClaims = entity.AlwaysSendClientClaims,
                ClientClaimsPrefix = entity.ClientClaimsPrefix,
                PairWiseSubjectSalt = entity.PairWiseSubjectSalt,
                UserSsoLifetime = entity.UserSsoLifetime,
                UserCodeType = entity.UserCodeType,
                DeviceCodeLifetime = entity.DeviceCodeLifetime,
            };

            await foreach (var claim in _claims.EnumAsync(entity.ClientId))
                client.Claims.Add(new Claim(claim.Type, claim.Value));

            await foreach (var corsOrigin in _corsOrigins.EnumAsync(client.ClientId))
                client.AllowedCorsOrigins.Add(WebUtility.UrlDecode(corsOrigin.Origin));

            await foreach (var grantType in _grantTypes.EnumAsync(entity.ClientId))
                client.AllowedGrantTypes.Add(grantType.GrantType);

            await foreach (var restriction in _idPRestrictions.EnumAsync(entity.ClientId))
                client.IdentityProviderRestrictions.Add(restriction.Provider);

            await foreach (var redirectUri in _postLogoutRedirectUris.EnumAsync(entity.ClientId))
                client.PostLogoutRedirectUris.Add(WebUtility.UrlDecode(redirectUri.PostLogoutRedirectUri));

            await foreach (var redirectUri in _redirectUris.EnumAsync(entity.ClientId))
                client.RedirectUris.Add(WebUtility.UrlDecode(redirectUri.RedirectUri));

            await foreach (var secret in _secrets.EnumAsync(entity.ClientId))
                client.ClientSecrets.Add(new Secret
                {
                    Description = secret.Description,
                    Expiration = secret.Expiration,
                    Type = secret.Type,
                    Value = secret.Value,
                });

            await foreach (var scope in _scopes.EnumAsync(entity.ClientId))
                client.AllowedScopes.Add(scope.Scope);

            return client;
        }
    }
}