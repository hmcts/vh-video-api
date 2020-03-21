namespace VideoApi.IntegrationTests.Hooks
{
    internal enum HooksSequence
    {
        ConfigHooks = 1,
        RemoveDataCreatedDuringTest = 2,
        RemoveConferences = 3,
        RemoveServer = 4
    }
}
