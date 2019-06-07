using System;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Common.Security.CustomToken;
using VideoApi.Common.Security.HashGen;

namespace VideoApi.UnitTests.CustomJwtToken
{
    public class MessageEncryptorTest
    {
        private CustomJwtTokenSettings _customJwtTokenConfigSettings;
        [SetUp]
        public void SetUp()
        {
            _customJwtTokenConfigSettings = new CustomJwtTokenSettings
            {
                Secret = "W2gEmBn2H7b2FCMIQl6l9rggbJU1qR7luIeAf1uuaY+ik6TP5rN0NEsPVg0TGkroiel0SoCQT7w3cbk7hFrBtA=="
            };
        }

        [Test]
        public void should_encrypt()
        {
            
            var messageEncryptor = new MessageEncryptor(_customJwtTokenConfigSettings);
            var id = Guid.NewGuid().ToString("N");
            var request = $"https://poc.kinly.hmcts.net:{id}:test";
            var hashRequestTarget = messageEncryptor.HashRequestTarget(request);
            hashRequestTarget.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void should_fail_authentication()
        {
           
            var messageEncryptor = new MessageEncryptor(_customJwtTokenConfigSettings);
            var id = Guid.NewGuid().ToString("N");
            var request = $"https://poc.kinly.hmcts.net:{id}:test";
            var hashRequestTarget = messageEncryptor.HashRequestTarget(request);

            var request2 = $"https://poc.kinly.hmcts.net:{Guid.NewGuid():N}:test";
            var hashRequestTarget2 = messageEncryptor.HashRequestTarget(request2);
            hashRequestTarget2.Should().NotBe(hashRequestTarget);
        }
    }
}