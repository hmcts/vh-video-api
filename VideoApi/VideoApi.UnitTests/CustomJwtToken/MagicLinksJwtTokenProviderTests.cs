using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using VideoApi.Common.Configuration;
using VideoApi.Common.Security;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.CustomJwtToken
{
    public class MagicLinksJwtTokenProviderTests
    {
        private MagicLinksJwtTokenProvider _tokenProvider;
        private Mock<IOptions<MagicLinksConfiguration>> magicLinksConfiguration;

        [SetUp]
        public void SetUp()
        {
            magicLinksConfiguration = new Mock<IOptions<MagicLinksConfiguration>>();
            magicLinksConfiguration.SetupGet(x => x.Value).Returns(new MagicLinksConfiguration
            {
                Issuer = "Issuer",
                JwtProviderSecret = "WNi3oF3NfduzvwUiOPlnDdUUjIlMcv7fX28ms3udpPM="
            });
            _tokenProvider = new MagicLinksJwtTokenProvider(magicLinksConfiguration.Object);
        }

        [Test]
        public void GenerateToken_should_build_and_return_jwt_details()
        {
            //Arrange
            var expiryAfter = DateTime.UtcNow.AddMinutes(MagicLinksJwtTokenProvider.ExpiryTimeInMinutes);

            //Act
            var token = _tokenProvider.GenerateToken("name", "username", UserRole.Individual);

            //Assert
            Assert.Greater(token.Expiry, expiryAfter);
            Assert.IsNotNull(token.Token);
        }
    }
}
