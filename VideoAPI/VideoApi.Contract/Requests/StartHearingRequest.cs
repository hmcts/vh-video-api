namespace VideoApi.Contract.Requests
{
    public enum HearingLayout
    {
        Dynamic,
        OnePlus7,
        TwoPlus21
    }
    public class StartHearingRequest
    {
        public StartHearingRequest()
        {
            Layout = HearingLayout.Dynamic;
        }
        public HearingLayout? Layout { get; set; }
    }
}
