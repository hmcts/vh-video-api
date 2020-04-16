namespace VideoApi.Contract.Responses
{
    public class AudioStreamServiceResponse
    {
        public AudioStreamServiceResponse(bool success, string message = null, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public bool Success { get; private set; }
        public string Message { get; private set; }
        public object Data { get; private set; }
    }
}
