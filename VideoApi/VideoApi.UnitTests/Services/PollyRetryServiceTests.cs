using System;
using System.Threading.Tasks;
using VideoApi.Services;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Services
{
    public class PollyRetryServiceTests
    {
        private readonly IPollyRetryService _pollyRetryService;

        public PollyRetryServiceTests()
        {
            _pollyRetryService = new PollyRetryService();
        }

        [Test]
        public void WaitAndRetryAsync_Retries_On_Exception()
        {
            var retryInvoked = false;

            _pollyRetryService.WaitAndRetryAsync<Exception, object>
            (
                3, i => TimeSpan.FromMilliseconds(1), retryAttempt => retryInvoked = true,
                executeFunction: () => throw new Exception("What"));

            ClassicAssert.True(retryInvoked);
        }

        [Test]
        public async Task WaitAndRetryAsync_Does_Not_Retry()
        {
            var retryInvoked = false;

            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, object>
            (
                3, i => TimeSpan.FromMilliseconds(1), retryAttempt => retryInvoked = true,
                () => Task.FromResult<object>("returned")
            );

            ClassicAssert.False(retryInvoked);
            result.Should().Be("returned");
        }

        [Test]
        public void WaitAndRetryAsync_With_Result_Retries_On_Exception()
        {
            var retryInvoked = false;

            _pollyRetryService.WaitAndRetryAsync<Exception, TestResult>
            (
                3, i => TimeSpan.FromMilliseconds(1), retryAttempt => retryInvoked = true,
                x => !x.Success,
                () => throw new Exception("What")
            );

            ClassicAssert.True(retryInvoked);
        }

        [Test]
        public void WaitAndRetryAsync_With_Result_Retries_On_Failed_Result()
        {
            var retryInvoked = false;

            _pollyRetryService.WaitAndRetryAsync<Exception, TestResult>
            (
                3, i => TimeSpan.FromMilliseconds(1), retryAttempt => retryInvoked = true,
                x => !x.Success,
                () => Task.FromResult(new TestResult{Success = false})
            );

            ClassicAssert.True(retryInvoked);
        }

        [Test]
        public async Task WaitAndRetryAsync_With_Result_Does_Not_Retry()
        {
            var retryInvoked = false;

            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, TestResult>
            (
                3, i => TimeSpan.FromMilliseconds(1), retryAttempt => retryInvoked = true,
                x => !x.Success,
                () => Task.FromResult(new TestResult {Success = true})
            );

            ClassicAssert.False(retryInvoked);
            result.Success.Should().BeTrue();
        }

        private class TestResult
        {
            public bool Success { get; set; }
        }
    }
}
