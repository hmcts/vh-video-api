using NUnit.Framework; 
using System;
using Testing.Common.Helper;

namespace VideoApi.AcceptanceTests
{
    [SetUpFixture]
    public class TestSetupFixture
    { 
        [OneTimeTearDown]
        public void ZapReport()
        {
            var reportFileName = string.Format("VideoApi-Acceptance-Tests-Security-{0}", DateTime.Now.ToString("dd-MMM-yyyy-hh-mm-ss"));
            Zap.ReportAndShutDown(reportFileName); 
        }
    }
}
