namespace VideoApi.AcceptanceTests.Hooks
{
    internal enum HooksSequence
    {
        ZapHooks = 1,
        ConfigHooks = 2,
        HealthCheckHooks = 3,
        RemoveAudioFiles = 4,
        RemoveConference = 5,
        RemoveConferences = 6,
        RemoveAllTodaysConferences = 7
    }
}
