namespace SpringComp.IdentityServer.TableStorage.Options
{
    public class TableConfiguration
    {
        public TableConfiguration(string name)
        {
            TableName = name;
        }

        public string TableName { get; set; }
    }
}