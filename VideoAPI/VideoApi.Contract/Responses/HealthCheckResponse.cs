using System.Collections;

namespace VideoApi.Contract.Responses
{
    public class HealthCheckResponse
    {
        public HealthCheckResponse()
        {
            DatabaseHealth = new HealthCheck();
            KinlySelfTestHealth = new HealthCheck();
            KinlyApiHealth = new HealthCheck();
            AppVersion = new ApplicationVersion();
            WowzaHealth = new HealthCheck();
        }
        public HealthCheck DatabaseHealth { get; set; }
        public HealthCheck KinlySelfTestHealth { get; set; }
        public HealthCheck KinlyApiHealth { get; set; }
        public ApplicationVersion AppVersion { get; set; }
        public HealthCheck WowzaHealth { get; set; }
    }

    public class HealthCheck
    {
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }
        public IDictionary Data { get; set; }
    }

    public class ApplicationVersion
    {
        public string FileVersion { get; set; }
        public string InformationVersion { get; set; }
    }
}
