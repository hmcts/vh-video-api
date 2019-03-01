namespace Testing.Common.Helper
{
    public class ApiUriFactory
    {
        public CallbackEndpoints CallbackEndpoints { get; }
        
        public ApiUriFactory()
        {
            CallbackEndpoints = new CallbackEndpoints();
        }
    }
    
    public class CallbackEndpoints
    {
        private string ApiRoot => "/callback";
        public string Event => $"{ApiRoot}/event";
    }
}