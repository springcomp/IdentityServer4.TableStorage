namespace SpringComp.IdentityServer.TableStorage.Options
{
    public class TableStorageConfigurationOptions : TableStorageOptions
    {
        public TableConfiguration ApiResources { get; set; } = new TableConfiguration("ApiResources");
        public TableConfiguration ApiResourceClaims { get; set; } = new TableConfiguration("ApiClaims");
        public TableConfiguration ApiScopes { get; set; } = new TableConfiguration("ApiScopes");
        public TableConfiguration ApiScopeClaims { get; set; } = new TableConfiguration("ApiScopeClaims");
        public TableConfiguration ApiSecrets { get; set; } = new TableConfiguration("ApiSecrets");
        public TableConfiguration ApiResourceNamesByScope { get; set; } = new TableConfiguration("ApiResourcesByScope");

        public TableConfiguration Clients { get; } = new TableConfiguration("Clients");
        public TableConfiguration ClientClaims { get; } = new TableConfiguration("ClientClaims");
        public TableConfiguration ClientCorsOrigins { get; } = new TableConfiguration("ClientCorsOrigins");
        public TableConfiguration ClientGrantTypes { get; } = new TableConfiguration("ClientGrantTypes");
        public TableConfiguration ClientIdPRestrictions { get; } = new TableConfiguration("ClientIdPRestrictions");
        public TableConfiguration ClientPostLogoutRedirectUris { get; } = new TableConfiguration("ClientPostLogoutRedirectUris");
        public TableConfiguration ClientRedirectUris { get; } = new TableConfiguration("ClientRedirectUris");
        public TableConfiguration ClientScopes { get; } = new TableConfiguration("ClientScopes");
        public TableConfiguration ClientSecrets { get; } = new TableConfiguration("ClientSecrets");

        public TableConfiguration IdentityResources { get; set; } = new TableConfiguration("IdentityResources");
        public TableConfiguration IdentityClaims { get; set; } = new TableConfiguration("IdentityClaims");
    }
}