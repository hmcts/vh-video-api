using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public static class RemoveAudioFileHooks
    {
        [AfterScenario(Order = (int)HooksSequence.RemoveAudioFiles)]
        public static void RemoveAudioFiles(TestContext context)
        {
            context.Wowsa?.RemoveAudioFileFromStorage();
        }
    }
}
