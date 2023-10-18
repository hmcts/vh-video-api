namespace VideoApi.Services
{
    public class FeatureTogglesStub : IFeatureToggles
    {
        public bool HrsIntegration { get; set; } = false;

        public bool HrsIntegrationEnabled()
        {
            return HrsIntegration;
        }
    }
}
