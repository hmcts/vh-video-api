
using System.Linq;
using VideoApi.Common.Security.Supplier.Vodafone;

namespace VideoApi.UnitTests.CustomJwtToken
{
    public class CustomJwtTokenProviderTests
    {
        private VodafoneJwtProvider _customJwtTokenProvider;
        private VodafoneJwtHandler _customJwtTokenHandler;
        private const string Issuer = "hmcts.video.service";
        private const string Audience = "test.video.supplier.endpoint";

        [SetUp]
        public void Setup()
        {
            var secretKey = "6t8obpbl0iHOSvDpnUImSdZjYEpTcaEWC8SzLO4/X6iOHArHEm/3Ja6NnzaKS6JwE3U/Bjy1LE/bARMqNCN98w==";
            var customTokenSettings = new VodafoneConfiguration{ ApiSecret = secretKey, Audience = Audience, Issuer = Issuer};
            _customJwtTokenProvider = new VodafoneJwtProvider(customTokenSettings);
            _customJwtTokenHandler = new VodafoneJwtHandler(customTokenSettings);
        }

        [Test]
        public void Should_generate_jwt_token_when_generate_token_is_called()
        {
            var generateToken = _customJwtTokenProvider.GenerateApiToken("Test User", 30);
            generateToken.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Should_be_valid_token_when_generated_by_custom_token_provider()
        {
            var token = _customJwtTokenProvider.GenerateApiToken("Test User", 30);

            var isValidToken = _customJwtTokenHandler.IsValidToken(token);
            isValidToken.Should().BeTrue();
        }

        [Test]
        public void Should_be_invalid_token_when_token_is_random_string()
        {
            var token = "ey1221213121";
            var isValidToken = _customJwtTokenHandler.IsValidToken(token);

            isValidToken.Should().BeFalse();
        }

        [Test]
        public void Should_get_principal_when_get_principal_is_called_with_valid_token()
        {
            var testUser = "Test User";
            var token = _customJwtTokenProvider.GenerateApiToken(testUser, 30);

            var claimsPrincipal = _customJwtTokenHandler.GetPrincipal(token);
            claimsPrincipal.Claims.First().Value.Should().Be(testUser);
        }

        [Test]
        public void Should_not_be_able_to_get_principal_when_get_principal_is_called_with_invalid_token()
        {
            var token = "ey1221213121";

            var claimsPrincipal = _customJwtTokenHandler.GetPrincipal(token);
            claimsPrincipal.Should().BeNull();
        }

        [Test]
        public void Should_be_invalid_token_when_token_generated_with_different_secret()
        {
            var secretKey = "F8pf/zwOgm/kASEFs+BKRDdyq+RhHCQ9i9tPjeaPjUebm6HvzXKIsr/nX28wpwAZoWRG0FQK9LVf6nrkW/vg4w==";
            var customTokenSettings = new VodafoneConfiguration { ApiSecret = secretKey, Audience = Audience, Issuer = Issuer};
            _customJwtTokenProvider = new VodafoneJwtProvider(customTokenSettings);

            var token = _customJwtTokenProvider.GenerateApiToken("Test User", 1);

            var claimsPrincipal = _customJwtTokenHandler.IsValidToken(token);
            claimsPrincipal.Should().BeFalse();
        }
    }
}
