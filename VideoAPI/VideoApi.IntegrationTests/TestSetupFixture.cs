using NUnit.Framework; 
using System;
using Testing.Common.Helper;

namespace VideoApi.IntegrationTests
{
    [SetUpFixture]
    public class TestSetupFixture
    {
        [OneTimeSetUp]
        public void StartZap()
        {
            Zap.StartZapDaemon();
        }

        [OneTimeTearDown]
        public void ZapReport()
        {
            var reportFileName = string.Format("VideoApi-Integration-Tests-Security-{0}", DateTime.Now.ToString("dd-MMM-yyyy-hh-mm-ss"));
            Zap.ReportAndShutDown(reportFileName);
        }
    }
}
