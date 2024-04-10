using System.Diagnostics.CodeAnalysis;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class FeatureTogglesStub : IFeatureToggles
    {
        public bool HrsIntegration { get; set; } = false;
        public bool Vodafone { get; set; } = false;

        public bool HrsIntegrationEnabled()
        {
            return HrsIntegration;
        }
        public bool VodafoneIntegrationEnabled()
        {
            return Vodafone;
        }
    }
}
