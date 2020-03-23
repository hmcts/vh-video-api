namespace VideoApi.AcceptanceTests.Hooks
{
    internal enum HooksSequence
    {
        ZapHooks = 1,
        ConfigHooks = 2,
        HealthCheckHooks = 3,
        RemoveConference = 4,
        RemoveConferences = 5,
        RemoveAllTodaysConferences = 6
    }
}
