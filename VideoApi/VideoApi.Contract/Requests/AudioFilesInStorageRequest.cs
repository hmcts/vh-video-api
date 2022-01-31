namespace VideoApi.Contract.Requests
{
    public class AudioFilesInStorageRequest
    {
        public string FileNamePrefix { get; set; }

        public int FilesCount { get; set; }
    }
}
