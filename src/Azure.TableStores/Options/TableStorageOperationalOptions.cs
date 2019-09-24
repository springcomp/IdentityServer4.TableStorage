namespace SpringComp.IdentityServer.TableStorage.Options
{
    public class TableStorageOperationalOptions : TableStorageOptions
    {
        public TableConfiguration PersistedGrants { get; set; } = new TableConfiguration("PersistedGrants");
    }
}
