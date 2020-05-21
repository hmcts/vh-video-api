using TechTalk.SpecFlow;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Hooks
{
    [Binding]
    public static class RemoveAudioFileHooks
    {
        [AfterScenario(Order = (int)HooksSequence.RemoveAudioFiles)]
        public static void RemoveAudioFiles(TestContext context)
        {
            context.AzureStorage?.RemoveAudioFileFromStorage();
        }
    }
}
