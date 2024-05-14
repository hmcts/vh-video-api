using Microsoft.Extensions.Options;
using Moq;
using System;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.CustomJwtToken
{
    public class QuickLinksJwtTokenProviderTests
    {
        private QuickLinksJwtTokenProvider _tokenProvider;
        private Mock<IOptions<QuickLinksConfiguration>> quickLinksConfiguration;

        [SetUp]
        public void SetUp()
        {
            quickLinksConfiguration = new Mock<IOptions<QuickLinksConfiguration>>();
            quickLinksConfiguration.SetupGet(x => x.Value).Returns(new QuickLinksConfiguration
            {
                Issuer = "Issuer",
                RsaPrivateKey = "MIIEogIBAAKCAQEAnsbpTeAl1xUDMFzj3HK0gXyIgWhJFCjerkdCcTBOFW/uHxXDOnFb1aaozcOmV4j4ve4ypcUbpZJT4X6C0EXUrC2ZGLSiW0h6zxqNQPp3B1/8OAwgcuCCoDyVPuBvVJN5VDXdDoz0r9orjx2AUAAAMCMnv9cLd6cc4hm5f4iqSoPGX4XgTY2Y1WuFMgl3A+wvw91WfwJ4VYTBwlUiLvKz2Ca0iWx1vwxQQ/uprFv6pvLkaK0R9m2lw1XKbbAMPOYMiskON8pvoSnmXeiIDh6q4eVY19njp77x6nIXQqtLuL4wl7ro5/d594st5xzSmNc8cV8WjieO0wgPrAmQ9aLjXQIDAQABAoIBABwzSAzE2vOwj7hvRylIVjcJX1VcqF4kRCjAD1T7N2MnViu/OcWPjLgNub2SGmFBuqxBOk2nDJoDu1I5nhJP1M4f6ykwLAX6gZV3Kc94rNtqeOmbfcfpiEndeMSKKDvlVY9XYcPL1UnGfiYVH00WMvJZzB2OnELMM50U5JV6OISzSz6UeZEdntct6ElTfiVAX2OAVkKXS9K1Gtz8nRtRN0q0/nZINRhhei2XjafRQfpcCufoS//p7dgUIZGL7XxkhIAGprEUk1jlGzcfLhMI0CmHChpF7f55G9r1pheFaEvnKEphjyIh3y4e2qFfWTHm7DRhih9+ODYkehP4WlCcpQECgYEA0zs93fs2bJP9SH87CyjtC1Xn/20t1zztmjHnRHBLiQUnmpOWwmlsN+Z8FA3urmvrd55UfSxTvLMDrrovUbb/0aX1ve/bf7ApCZIAl2n3k1BZfZZ4wPjICg8Rib+30MaxvKu56nWaCQNdZpBmMR44MjF6PHX3PbiSn1STkXOWCeUCgYEAwG2oEmK4pbpgWWT0nO2A4z6im3KnckAoXNRa/njW94m1h9qXsY/lAmR/g1WcD0YoNfwRadqvg1LI1bnkVbMSuXlt/1oo5tkQm22L+4BIzZ7Cftr6jby1OhwXgzFpXg/LnOIcO2shEC8p5DBQWap5yAWHokwVwW+wYCUWUltdfBkCgYARnuCDUwBwQEwhrxNi0/AqQOlQgnAPg3KiqUUMjX2PmZF7MJKj9x1i8GQrXBosDK6g1OFha8A0fo5xFfD6lRR0Da8ZQBgEZ9KUjMxl1R4k+M/uTQ4XsuklBarF3etcjCapgX3J1DOcSt6Qvrz3nfi8Hyp+Pqz4RhxdFDYXLRg4DQKBgDvvq+MWJsdQFwShImPiwjuF3eYnPyqh+zB24GTUxLhTh1xqZMdkt4MYRHNm7fX/9zRhcBcVi4eO+IlBnnJnFZT50p4vuf+1aqvO4kQTABozsgcLM/GD7rrL1zMA5/3pU5VpBDwpsSG2Xwj5V32mUMpRlp6hrkETBc/cnWhHlwW5AoGAbNUYq84viSDOQUF0C8u8QIpLSlBN0e0fTrDKIBMAOWOuyy7unjxjWywRiNMef3E0vHEnPgRtUwZb8RMCKnCakU+xCyaau25vaCP2kSUvrRW0CzpnOQXkJvhnMJG7mXUwPwa1nfOaM3YNP/T2Ah1E2oEpIpmq+NCiMSERwp1YdpU=",
                ValidAudience = "vh-video-web"
            });
            _tokenProvider = new QuickLinksJwtTokenProvider(quickLinksConfiguration.Object);
        }

        [Test]
        public void GenerateToken_should_build_and_return_jwt_details()
        {
            //Arrange
            var expiryAfter = DateTime.UtcNow.AddMinutes(QuickLinksJwtTokenProvider.ExpiryTimeInMinutes);

            //Act
            var token = _tokenProvider.GenerateToken("name", "username", UserRole.Individual);

            //Assert
            ClassicAssert.Greater(token.Expiry, expiryAfter);
            ClassicAssert.IsNotNull(token.Token);
        }
    }
}
