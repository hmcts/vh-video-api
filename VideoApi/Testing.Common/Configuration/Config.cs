using VideoApi.Common.Configuration;


namespace Testing.Common.Configuration
{
    public class Config
    {
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public ConnectionStringsConfig DbConnection { get; set; }
        public ServicesConfiguration VhServices { get; set; }
        public WowzaConfiguration Wowza { get; set; }
        public CvpConfiguration Cvp { get; set; }
        public AudioRecordingTestIdConfiguration AudioRecordingTestIds { get; set; }
    }
}
