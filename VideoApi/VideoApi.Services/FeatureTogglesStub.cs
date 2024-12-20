using System.Diagnostics.CodeAnalysis;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class FeatureTogglesStub : IFeatureToggles
    {
        public bool HrsIntegration { get; set; } = false;
        public bool Vodafone { get; set; } = false;
        public bool SendTransferRoles { get; set; } = false;
        
        public bool VodafoneIntegrationEnabled()
        {
            return Vodafone;
        }
        
        public bool SendTransferRolesEnabled()
        {
            return SendTransferRoles;
        }
        
        
        public bool HrsIntegrationEnabled()
        {
            return HrsIntegration;
        }
    }
}
