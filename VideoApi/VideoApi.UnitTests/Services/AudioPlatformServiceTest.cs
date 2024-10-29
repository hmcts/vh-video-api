using System;
using VideoApi.Common.Configuration;
using VideoApi.Services;

namespace VideoApi.UnitTests.Services
{
    [TestFixture]
    public class AudioPlatformServiceTest
    {
        private readonly WowzaConfiguration _wowzaConfiguration;
        private readonly AudioPlatformService _audioPlatformService;
        
        public AudioPlatformServiceTest()
        {
            _wowzaConfiguration = new WowzaConfiguration {StreamingEndpoint = "http://streamIt.com/", ApplicationName = "vh-recording-app"};
            _audioPlatformService = new AudioPlatformService( _wowzaConfiguration);
        }
        
        [Test]
        public void GetAudioIngestUrl_overload_returns_expected_url()
        {
            var serviceId = "ServiceId";
            var caseNumber = "CaseNumber";
            var hearingId = Guid.NewGuid().ToString();
            var url = _audioPlatformService.GetAudioIngestUrl(serviceId, caseNumber, hearingId);
            url.Should().Contain($"{serviceId}-{caseNumber}-{hearingId}");
            url.Should().Contain(_wowzaConfiguration.ApplicationName);
            url.Should().Contain(_wowzaConfiguration.StreamingEndpoint);
        }
        
        [TestCase(" Name with spaces", "Namewithspaces")]
        [TestCase("Name!@#$%^&*()_+=[{]}|;:'\\\",<>/~`?", "Name")]
        [TestCase("-Name-", "Name")]
        public void GetAudioIngestUrl_overload_strips_out_special_characters(string suppliedString, string expectedString)
        {
            var hearingId = Guid.NewGuid().ToString();
            var url = _audioPlatformService.GetAudioIngestUrl(suppliedString, suppliedString, hearingId);
            url.Should().Contain($"{expectedString}-{expectedString}-{hearingId}");
            url.Should().Contain(_wowzaConfiguration.ApplicationName);
            url.Should().Contain(_wowzaConfiguration.StreamingEndpoint);
        }
    }
}
