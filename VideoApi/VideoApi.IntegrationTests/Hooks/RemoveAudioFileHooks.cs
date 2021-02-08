using System.Threading.Tasks;
using TechTalk.SpecFlow;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Hooks
{
    [Binding]
    public static class RemoveAudioFileHooks
    {
        [AfterScenario(Order = (int)HooksSequence.RemoveAudioFiles)]
        public static async Task RemoveAudioFiles(TestContext context)
        {
            context.AzureStorage?.RemoveAudioFileFromStorage();
            
            if (context.AzureStorage == null || context.Test == null || context.Test.CvpFileNamesOnStorage.Count == 0)
            {
                await Task.CompletedTask;
                return;
            }

            foreach (var cvpFilesOnStorage in context.Test?.CvpFileNamesOnStorage)
            {
                await foreach (var file in context.AzureStorage.GetAllBlobsAsync(cvpFilesOnStorage))
                {
                    await file.DeleteAsync();
                }
            }
        }
    }
}
