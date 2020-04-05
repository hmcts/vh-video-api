using VideoApi.Common.Configuration;


namespace Testing.Common.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public ConnectionStringsConfig DbConnection { get; set; }
        public ServicesConfiguration VhServices { get; set; }
    }
}
