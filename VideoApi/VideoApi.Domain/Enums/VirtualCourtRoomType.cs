namespace VideoApi.Domain.Enums
{
    public enum VirtualCourtRoomType
    {
        /// <summary>
        /// This is the room type of judge/joh consultation
        /// </summary>
        JudgeJOH = 1,
        /// <summary>
        /// This is the room type of participant consultation
        /// </summary>
        Participant = 2,
        WaitingRoom = 3,
        /// <summary>
        /// This is the room type of standard interpreter room
        /// </summary>
        Civilian = 4,
        /// <summary>
        /// This is the room type of witness interpreter room
        /// </summary>
        Witness = 5,
        /// <summary>
        /// This is the room type of judicial office holders
        /// </summary>
        JudicialShared = 6
    }
}
