using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
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
                JwtProviderSecret = "WNi3oF3NfduzvwUiOPlnDdUUjIlMcv7fX28ms3udpPM="
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
            Assert.Greater(token.Expiry, expiryAfter);
            Assert.IsNotNull(token.Token);
        }
    }
}
