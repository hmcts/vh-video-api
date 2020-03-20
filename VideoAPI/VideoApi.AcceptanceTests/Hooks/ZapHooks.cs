using TechTalk.SpecFlow;
using Testing.Common.Helper;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public static class ZapHooks
    {
        [BeforeScenario(Order = (int) HooksSequence.ZapHooks)]
        public static void StartZap()
        {
            Zap.StartZapDaemon();
        }
    }
}
