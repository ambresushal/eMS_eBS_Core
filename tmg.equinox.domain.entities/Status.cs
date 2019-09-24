namespace tmg.equinox.domain.entities
{
    public enum Status
    {
        InProgress = 1,
        Approved,
        Finalized
    }

    public enum MSMSyncStatus
    {
        ReadyForUpdate = 1,
        Inprogress = 2,
        Errored = 3,
        Completed = 4,
        InQueue = 5
    }
}
