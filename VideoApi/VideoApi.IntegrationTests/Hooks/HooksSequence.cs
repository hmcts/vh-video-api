namespace VideoApi.IntegrationTests.Hooks
{
    internal enum HooksSequence
    {
        ConfigHooks = 1,
        RemoveAudioFiles = 2,
        RemoveDataCreatedDuringTest = 3,
        RemoveConferences = 4,
        RemoveServer = 5
    }
}
