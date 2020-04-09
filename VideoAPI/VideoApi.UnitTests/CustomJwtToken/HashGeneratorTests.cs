using System;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Common.Security.HashGen;
using VideoApi.Common.Security.Kinly;

namespace VideoApi.UnitTests.CustomJwtToken
{
    public class HashGeneratorTests
    {
        private KinlyConfiguration _kinlyConfiguration;

        [SetUp]
        public void SetUp()
        {
            _kinlyConfiguration = new KinlyConfiguration
            {
                ApiSecret = "W2gEmBn2H7b2FCMIQl6l9rggbJU1qR7luIeAf1uuaY+ik6TP5rN0NEsPVg0TGkroiel0SoCQT7w3cbk7hFrBtA=="
            };
        }

        [Test]
        public void Should_encrypt()
        {
            var hashGenerator = new HashGenerator(_kinlyConfiguration);
            var id = Guid.NewGuid().ToString();
            var computedHash = hashGenerator.GenerateHash(DateTime.UtcNow.AddMinutes(20), id);
            computedHash.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Should_fail_authentication()
        {
            var hashGenerator = new HashGenerator(_kinlyConfiguration);
            var id = Guid.NewGuid().ToString();
            var computedHash = hashGenerator.GenerateHash(DateTime.UtcNow.AddMinutes(20), id);

            var id2 = Guid.NewGuid().ToString();
            var reComputedHash = hashGenerator.GenerateHash(DateTime.UtcNow.AddMinutes(-20), id2);
            reComputedHash.Should().NotBe(computedHash);
        }
    }
}