using FizzWare.NBuilder;

namespace VideoApi.IntegrationTests.Helper
{
    public class InvalidRequest
    {
        public string AString;

        public InvalidRequest()
        {
            AString = "ABC";
        }

        public InvalidRequest BuildRequest()
        {
            return Builder<InvalidRequest>.CreateNew()
                .With(x => x.AString = "ABC")          
                .Build();
        }
    }
}
